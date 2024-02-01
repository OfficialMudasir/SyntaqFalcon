using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using JsonDiffer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Documents.Models;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static Syntaq.Falcon.Records.RecordMatterConsts;

namespace Syntaq.Falcon.Records
{
	public class RecordManager : FalconDomainServiceBase
   {
		public IAbpSession _abpSession { get; set; }
		
		private readonly ACLManager _ACLManager;
		private readonly EntityManager _entityManager;
		private readonly FolderManager _folderManager;
		private readonly SubmissionManager _submissionManager;
		private readonly IRepository<ACL> _aclRepository;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IRepository<Form, Guid> _formRepository;
		private readonly IRepository<Record, Guid> _recordRepository;
		private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
		private readonly IRepository<RecordMatterAudit, Guid> _recordMatterAuditRepository;
		private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
		private readonly IRepository<RecordMatterItemHistory, Guid> _recordMatterItemHistoryRepository;
		
		private readonly IRepository<Project, Guid> _projectRepository;

		public RecordManager(
			ACLManager aclManager, 
			EntityManager entityManager, 
			FolderManager folderManager, 
			SubmissionManager submissionManager, 
			IRepository<ACL> aclRepository, 
			IUnitOfWorkManager unitOfWorkManager, 
			IRepository<Form, Guid> formRepository, 
			IRepository<Record, Guid> recordRepository, 
			IRepository<RecordMatter, Guid> recordMatterRepository,
			IRepository<RecordMatterAudit, Guid> recordMatterAuditRepository,
			IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
			IRepository<RecordMatterItemHistory, Guid> recordMatterItemHistoryRepository,
			IRepository<Project, Guid> projectRepository
			)
		{
			_ACLManager = aclManager;
			_entityManager = entityManager;
			_folderManager = folderManager;
			_submissionManager = submissionManager;
			_aclRepository = aclRepository;
			_unitOfWorkManager = unitOfWorkManager;
			_formRepository = formRepository;
			_recordRepository = recordRepository;
			_recordMatterRepository = recordMatterRepository;
			_recordMatterAuditRepository = recordMatterAuditRepository;
			_recordMatterItemRepository = recordMatterItemRepository;
			_recordMatterItemHistoryRepository = recordMatterItemHistoryRepository;
			_projectRepository = projectRepository;
		}

		public async Task<bool> IsLocked(Guid rId)
		{
			var result = await _recordRepository.GetAll().AnyAsync(r => r.Id == rId && r.Locked > DateTime.UtcNow.AddMinutes(-4));
			return result;
		}

		//public async Task Lock(Guid rId)
  //      {


  //          var record = await _recordRepository.GetAll().FirstOrDefaultAsync(r => r.Id == rId);
  //          if (record != null)
  //          {
  //              record.Locked = DateTime.Now;
  //          }

  //          // save changes to the database


  //      }

        //public async Task UnLock(Guid rId)
        //{

        //    var record = await _recordRepository.GetAll().FirstOrDefaultAsync(r => r.Id == rId);
        //    if (record != null)
        //    {
        //        record.Locked = null;
        //    }

        //}

        public virtual async Task<(RecordSet, bool, string)> SaveRecordSet(RecordSet recordSet, bool saverecordmatteritem = true)
        {

            var _UserId = string.IsNullOrEmpty(recordSet.UserId.ToString()) ? null : recordSet.UserId;
            var _OrganizationUnitId = string.IsNullOrEmpty(recordSet.OrganizationId.ToString()) ? null : recordSet.OrganizationId;
            var _Role = recordSet.ACLPermission;
            bool HasACLAccess = true;
            string AccessError = null;
            FolderDto newFolder = null;
            Record NewRecord = null;
            RecordMatter newRecordMatter = null;
            RecordMatterItem NewRecordMatterItem = null;
            Submission NewSubmission = null;

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                ACL aCL = new ACL()
                {
                    UserId = _UserId,
                    OrganizationUnitId = _OrganizationUnitId,
                    Role = _Role,
                    AccessToken = recordSet.AnonAuthToken
                };
                if (!(aCL.UserId == null && aCL.OrganizationUnitId == null))
                {
                    if (!string.IsNullOrEmpty(recordSet.FolderId.ToString()))
                    {
                        try
                        {
                            newFolder = await _folderManager.CreateAndOrFetchFolder(aCL, new Folder()
                            {
                                Id = recordSet.FolderId,
                                TenantId = recordSet.TenantId,
                                Name = !string.IsNullOrEmpty(recordSet.FolderName) ? recordSet.FolderName : "New Folder",
                                Type = recordSet.FolderType
                            }, (long)recordSet.UserId);
                        }
                        catch (Exception)
                        {
                            HasACLAccess = false;
                            AccessError += "You don't have the correct Folder ACL Authorization - Assembly Halted,";
                        }
                    }

                    recordSet.SubmissionData = ContentCleaner.RemoveHtmlJavaScriptCss(recordSet.SubmissionData);

                    JObject data = null;
                    // DO NOT USE JObject.Parse this will create relative UTC Dates
                    // JObject.Parse("{Data: " + recordSet.SubmissionData + "}");				
                    using (JsonReader reader = new JsonTextReader(new System.IO.StringReader("{Data: " + recordSet.SubmissionData + "}")))
                    {
                        reader.DateParseHandling = DateParseHandling.None;
                        data = JObject.Load(reader);
                    }

                    // Remove Time ZONE INFORMATION
                    NormaliseDates(data);

                    recordSet.SubmissionData = Convert.ToString(data["Data"]);

                    var projectstatus = Convert.ToString(data["Data"]["ProjectStatus"]);
                    var recordstatus = data["Data"]["RecordStatus"];
                    var recordmatterstatus = data["Data"]["RecordMatterStatus"];
                    var recordname = ContentCleaner.RemoveHtmlJavaScriptCss(data["Data"]["RecordName"] != null ? Convert.ToString(data["Data"]["RecordName"]) : Convert.ToString(recordSet.RecordName));
                    var recordmattername = ContentCleaner.RemoveHtmlJavaScriptCss(data["Data"]["RecordMatterName"] != null ? Convert.ToString(data["Data"]["RecordMatterName"]) : Convert.ToString(recordSet.RecordMatterName));

                    var dataA = string.Empty;
                    if (_recordMatterRepository.GetAll().Any(rm => rm.Id == (Guid)recordSet.RecordMatterId))
                    {
                        dataA = _recordMatterRepository.FirstOrDefault(recordSet.RecordMatterId).Data;
                    }


                    if (!string.IsNullOrEmpty(recordSet.RecordId.ToString()))
                    {
                        try
                        {
                            ACL RACL = new ACL()
                            {
                                UserId = _UserId,
                                OrganizationUnitId = _OrganizationUnitId,
                                Role = _Role,
                                AccessToken = recordSet.AnonAuthToken
                            };

                            NewRecord = await CreateAndOrFetchRecord(RACL, new Record()
                            {
                                Id = recordSet.RecordId,
                                TenantId = recordSet.TenantId,
                                //AccessToken = recordSet.AnonAuthToken,
                                FolderId = !string.IsNullOrEmpty(newFolder.Id.ToString()) ? newFolder.Id : !string.IsNullOrEmpty(recordSet.FolderId.ToString()) ? recordSet.FolderId : Guid.Parse("00000000-0000-0000-0000-000000000000")
                            });

                            NewRecord.RecordName = recordname;
                            NewRecord.Data = recordSet.SubmissionData;
                            NewRecord.AccessToken = recordSet.AnonAuthToken;
                            await CreateOrEditRecord(aCL, NewRecord);


                            Project project = _projectRepository.GetAll().FirstOrDefault(e => e.RecordId == NewRecord.Id);
                            if (project != null)
                            {

                                var recordmatterList = _recordMatterRepository.GetAll().Where(rm => rm.RecordId == NewRecord.Id).ToList();

                                // if all are new then new
                                // Remove Steps where validation rules fail
                                List<Guid> invalidsteps = new List<Guid>();
                                foreach (RecordMatter recordmatter in recordmatterList)
                                {

                                    if (!ValidateRecordMatterStep(recordmatter))
                                    {
                                        invalidsteps.Add(recordmatter.Id);
                                    }
                                }

                                foreach (Guid invalidstep in invalidsteps)
                                {
                                    recordmatterList.Remove(recordmatterList.FirstOrDefault(e => e.Id == invalidstep));
                                }

                                var final = !recordmatterList.Any(rm => rm.Status != RecordMatterConsts.RecordMatterStatus.Final);
                                //var final = ! _recordRepository.GetAllIncluding(e => e.RecordMatters).Where(r => r.Id == NewRecord.Id).Any(e => e.RecordMatters.Any(rm => rm.Status != RecordMatterConsts.RecordMatterStatus.Final));

                                int i = 0;
                                bool result = int.TryParse(projectstatus, out i); //i now = 108  
                                ProjectConsts.ProjectStatus status = (ProjectConsts.ProjectStatus)Enum.ToObject(typeof(ProjectConsts.ProjectStatus), i);
                                project.Status = final ? ProjectConsts.ProjectStatus.Completed : ProjectConsts.ProjectStatus.InProgress;
                            }
                        }
                        catch (Exception)
                        {
                            HasACLAccess = false;
                            AccessError += "You don't have the correct Record ACL Authorization - Assembly Halted,";
                        }
                    }

                    if (!string.IsNullOrEmpty(recordSet.RecordMatterId.ToString()))
                    {
                        try
                        {
                            ACL RMACL = new ACL()
                            {
                                UserId = _UserId,
                                OrganizationUnitId = _OrganizationUnitId,
                                Role = _Role
                            };

                            newRecordMatter = await CreateAndOrFetchRecordMatter(RMACL, new RecordMatter()
                            {
                                Id = recordSet.RecordMatterId,
                                FormId = recordSet.FormId,
                                TenantId = recordSet.TenantId,
                                HasFiles = recordSet.HasFiles,
                                AccessToken = recordSet.AnonAuthToken,
                                RecordId = !string.IsNullOrEmpty(NewRecord.Id.ToString()) ? NewRecord.Id : !string.IsNullOrEmpty(recordSet.RecordId.ToString()) ? recordSet.RecordId : Guid.Parse("00000000-0000-0000-0000-000000000000")
                            });

                            newRecordMatter.HasFiles = recordSet.HasFiles;
                            newRecordMatter.Status = newRecordMatter.Status == RecordMatterConsts.RecordMatterStatus.New ? RecordMatterConsts.RecordMatterStatus.Draft : newRecordMatter.Status;
                            newRecordMatter.Data = recordSet.SubmissionData;

                            // Update All RecordMatters if this is a Project
                            if (_projectRepository.GetAll().Any(p => p.RecordId == newRecordMatter.RecordId))
                            {
                                var record = _recordRepository.GetAll().First(r => r.Id == newRecordMatter.RecordId);
                                if (record != null)
                                {
                                    record.Data = recordSet.SubmissionData;
                                }
                            }
                            else
                            {
                                // Only update the record matter name if this is not part of a project
                                newRecordMatter.Status = RecordMatterStatus.Final;
                                newRecordMatter.RecordMatterName = string.IsNullOrEmpty(recordSet.RecordMatterName) || recordSet.RecordMatterName == "Default" ? recordmattername : recordSet.RecordMatterName;
                            }

                            // RecordMatterKey
                            var recordmatterkey = Convert.ToString(data["Data"]["RecordKey"]);
                            newRecordMatter.Key = recordmatterkey;

                            newRecordMatter.AccessToken = recordSet.AnonAuthToken;
                            await CreateOrEditRecordMatter(aCL, newRecordMatter);

                        }
                        catch (Exception)
                        {
                            HasACLAccess = false;
                            AccessError += "You don't have the correct Record Matter ACL Authorization - Assembly Halted,";
                        }
                    }

                    if (!string.IsNullOrEmpty(recordSet.SubmissionId.ToString()))
                    {
                        try
                        {
                            FormDto _form = new FormDto();
                            bool HasForm = false;
                            if (recordSet.FormId != null/* || recordSet.FormId.ToString() != "00000000-0000-0000-0000-000000000000"*/)
                            {
                                _form = _formRepository
									.GetAll()
                                    .Where(f => f.Id == (Guid)recordSet.FormId)
                                    .Select(f => new FormDto
                                    {
                                        PaymentEnabled = f.PaymentEnabled
                                        // initialize other properties as needed
                                    })
                                    .FirstOrDefault();

                                HasForm = _form != null ? true : false;
                            }
                            Guid SubmissionId = recordSet.SubmissionId.ToString() == "00000000-0000-0000-0000-000000000000" ? Guid.NewGuid() : (Guid)recordSet.SubmissionId;
                            NewSubmission = await _submissionManager.CreateAndOrFetchSubmission(new CreateOrEditSubmissionDto()
                            {
                                Id = recordSet.SubmissionId,
                                TenantId = recordSet.TenantId,
                                AccessToken = null,
                                RequiresPayment = HasForm != false ? _form.PaymentEnabled : false,
                                PaymentStatus = null,
                                PaymentAmount = HasForm != false ? _form.PaymentAmount : 0.00m,
                                VoucherAmount = DynamicUtility.IsPropertyExist(data["Data"], "VoucherAmount") ? Convert.ToDecimal(data["Data"]["VoucherAmount"]) : 0,
                                AmountPaid = null,
                                ChargeId = null,
                                SubmissionStatus = "Started",
                                Type = null,
                                RecordId = !string.IsNullOrEmpty(NewRecord.Id.ToString()) ? NewRecord.Id : !string.IsNullOrEmpty(recordSet.RecordId.ToString()) ? recordSet.RecordId : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                                RecordMatterId = recordSet.RecordMatterId,
                                UserId = recordSet.UserId,
                                AppJobId = null,
                                AppId = recordSet.AppId != null || Convert.ToString(recordSet.AppId) != "00000000-0000-0000-0000-000000000000" ? recordSet.AppId : (Guid?)null,
                                FormId = recordSet.FormId != null || Convert.ToString(recordSet.FormId) != "00000000-0000-0000-0000-000000000000" ? recordSet.FormId : (Guid?)null
                            });
                            CurrentUnitOfWork.SaveChanges();
                        }
                        catch (Exception)
                        {

                        }
                    }

                    Guid RmdId = recordSet.RecordMatterItemId.ToString() == "00000000-0000-0000-0000-000000000000" ? Guid.NewGuid() : recordSet.RecordMatterItemId;
                    if (!string.IsNullOrEmpty(recordSet.RecordMatterItemId.ToString()) && saverecordmatteritem)
                    {
                        try
                        {

                            NewRecordMatterItem = await CreateAndOrFetchRecordMatterItem(new RecordMatterItem()
                            {
                                Id = RmdId,
                                TenantId = recordSet.TenantId,
                                RecordMatterId = !string.IsNullOrEmpty(newRecordMatter.Id.ToString()) ? newRecordMatter.Id : !string.IsNullOrEmpty(recordSet.RecordMatterId.ToString()) ? recordSet.RecordMatterId : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                                FormId = recordSet.FormId,
                                GroupId = RmdId,
                                DocumentName = ContentCleaner.RemoveHtmlJavaScriptCss(recordSet.DocumentName),
                                SubmissionId = NewSubmission.Id,
                                LockOnBuild = recordSet.LockOnBuild,
                                Status = recordmatterstatus == null ? string.Empty : recordmatterstatus.ToString()
                            });

                        }
                        catch (Exception e)
                        {
                        }

                        NewRecordMatterItem.LockOnBuild = recordSet.LockOnBuild;
                    }

                    recordSet.FolderId = newFolder.Id;
                    recordSet.ParentFolderId = newFolder.ParentId == null ? null : (Guid)newFolder.ParentId;
                    recordSet.RecordId = NewRecord.Id;
                    recordSet.RecordMatterId = newRecordMatter.Id;
                    recordSet.RecordMatterItemId = RmdId;
                    recordSet.SubmissionId = NewSubmission.Id;
                    recordSet.LockOnBuild = recordSet.LockOnBuild;

                    var jDiff = GetDataDiff(dataA, newRecordMatter.Data);
                    // Create Audit record for RecordmatterItem
                    var rma = new RecordMatterAudit()
                    {
                        Data = jDiff == null ? String.Empty : jDiff.ToString(),
                        RecordMatterId = newRecordMatter.Id,
                        Status = newRecordMatter.Status == null ? RecordMatterStatus.New : (RecordMatterStatus)newRecordMatter.Status,
                        UserId = _abpSession.UserId,
                        TenantId = newRecordMatter.TenantId
                    };

                    _recordMatterAuditRepository.InsertAsync(rma);

                }
                else
                {
                    return (new RecordSet(), false, "Assembly RecordMatter must have UserID - Assembly Halted");
                }
                unitOfWork.Complete();
            }
            return (recordSet, HasACLAccess, AccessError);
        }

        // DATETIMES must be stored without timezone information so that they remain absolute
        private void NormaliseDates(JObject input)
		{
            foreach (JToken token in input.Descendants())
            {
				WalkNode(token, null, prop =>
				{
					DateTime temp;
					if (DateTime.TryParse(Convert.ToString(prop.Value), out temp))
					{
						var vals = prop.Value.ToString().Split("+");
						prop.Value = vals.Count() > 0 ? vals[0] : prop.Value;						
					}
				});
			}
        }

		private void WalkNode(JToken node, Action<JObject> objectAction = null, Action<JProperty> propertyAction = null)
		{
			if (node.Type == JTokenType.Object)
			{
				if (objectAction != null) objectAction((JObject)node);

				foreach (JProperty child in node.Children<JProperty>())
				{
					if (propertyAction != null) propertyAction(child);
					WalkNode(child.Value, objectAction, propertyAction);
				}
			}
			else if (node.Type == JTokenType.Array)
			{
				foreach (JToken child in node.Children())
				{
					WalkNode(child, objectAction, propertyAction);
				}
			}
		}

		private Boolean ValidateRecordMatterStep(RecordMatter recordmatter)
		{
			var result = true;
			var rm = _recordMatterRepository.GetAll()
				.Select(rm => new { rm.Id, rm.Filter})
				.FirstOrDefault(r => r.Id == recordmatter.Id);

			if (!string.IsNullOrEmpty(rm.Filter))
			{
				var data = _recordRepository.FirstOrDefault(recordata => recordata.Id == recordmatter.RecordId).Data;

				if (string.IsNullOrEmpty(data))
				{
					data = "{}";
				}

				try
				{
					XmlNode node = null;
					try
					{
						XmlDocument doc = new XmlDocument();
						doc.LoadXml(JsonConvert.DeserializeXNode(data, "Data").ToString());
						node = doc.DocumentElement.SelectSingleNode(rm.Filter);

						if (node == null) result = false;

					}
					catch (Exception) { result = false; }


				}
				catch
				{
					result = true;
				}
			}
			return result;
		}


		public virtual async Task CreateOrEditRecord(ACL ACL, Record Record)
		{

            if (Record.Id.ToString() == "00000000-0000-0000-0000-000000000000" || !_recordRepository.GetAll().Any(i => i.Id == Record.Id))
			{
				ACL.Role = "O";
				await CreateRecord(ACL, Record);
			}
			else
			{
				await UpdateRecord(Record);
			}
		}
		
		public virtual async Task<Record> CreateAndOrFetchRecord(ACL ACL, Record Record)
		{

            try
            {
				if (_entityManager.IsSoftDelete(Record.Id, null, EntityManager.SoftDeleteEntities.Record))
				{
					using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
					{
						Record _record = _recordRepository.FirstOrDefault(Record.Id);
						_record.IsDeleted = false;
						_record.DeleterUserId = null;
						_record.DeletionTime = null;
						_recordRepository.Update(_record);
						CurrentUnitOfWork.SaveChanges();
                        _recordRepository.GetDbContext().Entry(_record).State = EntityState.Detached;
                    }
				}
            }
            catch (Exception ex)
            {
				var err = ex;
            }



			if (Record.Id.ToString() == "00000000-0000-0000-0000-000000000000" || !_recordRepository.GetAll().Any(i => i.Id == Record.Id))
			{
				ACL.Role = "O";
				await CreateRecord(ACL, Record);
				CurrentUnitOfWork.SaveChanges();
			}
			ACLCheckDto aCLCheckDto = new ACLCheckDto()
			{
				AccessToken = ACL.AccessToken,
				Action = "Edit",
				EntityId = Record.Id,
				UserId = ACL.UserId // _abpSession.UserId				
			};

			if (!_ACLManager.CheckAccess(aCLCheckDto).IsAuthed)
			{
				// Check to see if the access tokens correct
				throw new Exception();
			}

			Record record = _recordRepository.Get(Record.Id);

			return record;
		}

		private async Task CreateRecord(ACL ACL, Record Record)
		{
			if (_abpSession.TenantId != null)
			{
				Record.TenantId = _abpSession.TenantId;
				ACL.TenantId = _abpSession.TenantId;
			}
			using (var unitOfWork = _unitOfWorkManager.Begin())
			{
				await _recordRepository.InsertAsync(Record);
				ACL.EntityID = Record.Id;
				ACL.Type = "Record";
				await _ACLManager.AddACL(ACL);

				CurrentUnitOfWork.SaveChanges();
                _recordRepository.GetDbContext().Entry(Record).State = EntityState.Detached;

                unitOfWork.Complete();
			}
		}

		[AbpAuthorize(AppPermissions.Pages_Records_Edit)]
		private async Task UpdateRecord(Record Record)
		{

			if (_abpSession.TenantId != null)
			{
				Record.TenantId = _abpSession.TenantId;
			}
			await _recordRepository.UpdateAsync(Record);          
		}

		public virtual async Task CreateOrEditRecordMatter(ACL ACL, RecordMatter RecordMatter)
		{
			if (RecordMatter.Id == null || !_recordMatterRepository.GetAll().Any(i => i.Id == RecordMatter.Id))
			{
				ACL.Role = "O";
				await CreateRecordMatter(ACL, RecordMatter);
			}
			else
			{
				await UpdateRecordMatter(RecordMatter);
			}
		}

		public virtual async Task<RecordMatter> CreateAndOrFetchRecordMatter(ACL ACL, RecordMatter RecordMatter)
		{
			if (_entityManager.IsSoftDelete(RecordMatter.Id, null, EntityManager.SoftDeleteEntities.RecordMatter))
			{
				using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
				{
					RecordMatter _recordMatter = _recordMatterRepository.FirstOrDefault(RecordMatter.Id);
					_recordMatter.IsDeleted = false;
					_recordMatter.DeleterUserId = null;
					_recordMatter.DeletionTime = null;
					_recordMatterRepository.Update(_recordMatter);
					CurrentUnitOfWork.SaveChanges();

                    _recordMatterRepository.GetDbContext().Entry(_recordMatter).State = EntityState.Detached;

                }
            }

            var recordmatterexists = _recordMatterRepository.GetAll().Any(i => i.Id == RecordMatter.Id);
            if (RecordMatter.Id.ToString() == "00000000-0000-0000-0000-000000000000" || !recordmatterexists)
			{
				ACL.Role = "O";
				await CreateRecordMatter(ACL, RecordMatter);
				CurrentUnitOfWork.SaveChanges();
			}

			ACLCheckDto aCLCheckDto = new ACLCheckDto()
			{
				AccessToken = RecordMatter.AccessToken,
				Action = "Edit",
				EntityId = RecordMatter.Id,
				UserId = ACL.UserId
			};
 
			if (!_ACLManager.CheckAccess(aCLCheckDto).IsAuthed)
			{
				throw new Exception();
			}
 
			return _recordMatterRepository.Get(RecordMatter.Id);
		}

		[AbpAuthorize(AppPermissions.Pages_RecordMatters_Create)]
		private async Task CreateRecordMatter(ACL ACL, RecordMatter RecordMatter)
		{
			if (_abpSession.TenantId != null)
			{
				RecordMatter.TenantId = _abpSession.TenantId;
				ACL.TenantId = _abpSession.TenantId;
			}
			using (var unitOfWork = _unitOfWorkManager.Begin())
			{
				await _recordMatterRepository.InsertAsync(RecordMatter);
				ACL.EntityID = RecordMatter.Id;
				ACL.Type = "RecordMatter";
				await _ACLManager.AddACL(ACL);
                unitOfWork.Complete();
            }

			// Create Audit record for RecordmatterItem
			var rma = new RecordMatterAudit()
			{
				Data = RecordMatter.Data,
				RecordMatterId = RecordMatter.Id,
				Status = RecordMatter.Status == null ?  RecordMatterStatus.Draft : (RecordMatterStatus)RecordMatter.Status,
				UserId = ACL.UserId,
				TenantId = RecordMatter.TenantId
			};

			await _recordMatterAuditRepository.InsertAsync(rma);

		}

		[AbpAuthorize(AppPermissions.Pages_RecordMatters_Edit)]
		public async Task UpdateRecordMatter(RecordMatter RecordMatter)
		{

			if (_abpSession.TenantId != null)
			{
				RecordMatter.TenantId = _abpSession.TenantId;
			}
			await _recordMatterRepository.UpdateAsync(RecordMatter);
			CurrentUnitOfWork.SaveChanges();
            _recordMatterRepository.GetDbContext().Entry(RecordMatter).State = EntityState.Detached;
        }

		public JToken GetDataDiff(string dataA, string dataB)
        {
			JToken result = JToken.Parse("{}");
            try
            {

				dataA = string.IsNullOrEmpty(dataA) ? "{}" : dataA;
				dataB = string.IsNullOrEmpty(dataB) ? "{}" : dataB;
 
				var j1 = JToken.Parse(dataA);
				var j2 = JToken.Parse(dataB);

				result = JsonDifferentiator.Differentiate(j2, j1);
  
				return result;
            }
            catch
            {
				return result;
			}

		}

		public virtual async Task CreateOrEditRecordMatterItem(RecordMatterItem RecordMatterItem)
		{
			if (RecordMatterItem.Id == null || !_recordMatterItemRepository.GetAll().Any(i => i.Id == RecordMatterItem.Id))
			{
				await CreateRecordMatterItem(RecordMatterItem);
			}
			else
			{
				await UpdateRecordMatterItem(RecordMatterItem);
			}
		}

		public virtual async Task<RecordMatterItem> CreateAndOrFetchRecordMatterItem(RecordMatterItem RecordMatterItem)
		{
			
			using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
			{
				if (RecordMatterItem.Id.ToString() == "00000000-0000-0000-0000-000000000000" || !_recordMatterItemRepository.GetAll().Any(i => i.Id == RecordMatterItem.Id ))
				{
					Guid RMIID = RecordMatterItem.Id.ToString() != "00000000-0000-0000-0000-000000000000" ? RecordMatterItem.Id : Guid.NewGuid();
					RecordMatterItem.Id = RMIID;
					RecordMatterItem.GroupId = RecordMatterItem.GroupId == RMIID ? RMIID : RecordMatterItem.GroupId.ToString() != "00000000-0000-0000-0000-000000000000" ? RecordMatterItem.GroupId : Guid.NewGuid();
					RecordMatterItem.Status = string.IsNullOrEmpty(RecordMatterItem.Status) ?  "Created" : RecordMatterItem.Status;

					if (string.IsNullOrEmpty(RecordMatterItem.DocumentName ))
                    {
						// Set the FormName
						var frm = _formRepository.GetAll().First(f => f.Id == RecordMatterItem.FormId);
						if(frm != null)
						{
							RecordMatterItem.DocumentName = ContentCleaner.RemoveHtmlJavaScriptCss(frm.Name);
						}
                    }
					await CreateRecordMatterItem(RecordMatterItem);
					CurrentUnitOfWork.SaveChanges();

				}
 
			}

			return RecordMatterItem;
		}

		[AbpAuthorize(AppPermissions.Pages_RecordMatterItems_Create)]
		private async Task CreateRecordMatterItem(RecordMatterItem RecordMatterItem)
		{
			await _recordMatterItemRepository.InsertAsync(RecordMatterItem);
            CurrentUnitOfWork.SaveChanges();
            _recordMatterItemRepository.GetDbContext().Entry(RecordMatterItem).State = EntityState.Detached;
        }

		[AbpAuthorize(AppPermissions.Pages_RecordMatterItems_Edit)]
		private async Task UpdateRecordMatterItem(RecordMatterItem RecordMatterItem)
		{
			await _recordMatterItemRepository.UpdateAsync(RecordMatterItem);
            CurrentUnitOfWork.SaveChanges();
            _recordMatterItemRepository.GetDbContext().Entry(RecordMatterItem).State = EntityState.Detached;
        }


	}

    /// <summary>
    /// Cleans HTML, JavaScript and CSS from a string
    /// </summary>
    public class ContentCleaner
    {
        public static string RemoveHtmlJavaScriptCss(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Remove HTML tags
            string noHtml = Regex.Replace(input, "<.*?>", String.Empty);

            // Remove JavaScript
            string noJavaScript = Regex.Replace(noHtml, "<script.*?</script>", String.Empty, RegexOptions.Singleline);

            // Remove CSS
            string noCss = Regex.Replace(noJavaScript, "<style.*?</style>", String.Empty, RegexOptions.Singleline);

            // Return the cleaned string
            return noCss;
        }
    }
}
