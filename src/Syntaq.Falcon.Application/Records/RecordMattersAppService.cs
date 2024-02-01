using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Records.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using GetAllForLookupTableInput = Syntaq.Falcon.Records.Dtos.GetAllForLookupTableInput;
using Syntaq.Falcon.Utility;
using Syntaq.Falcon.Submissions;
using Abp.UI;
using Syntaq.Falcon.Projects;
using Abp.Organizations;
using Syntaq.Falcon.Forms;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.Extensions.Options;
using Syntaq.Falcon.Web;

namespace Syntaq.Falcon.Records
{

	[EnableCors("AllowAll")]
	// [AbpAuthorize(AppPermissions.Pages_RecordMatters)] all roles can edit recordmatters 
	// access is controlled at the ACL check level
	public class RecordMattersAppService : FalconAppServiceBase, IRecordMattersAppService
	{
		private readonly ACLManager _ACLManager;
		private readonly IRepository<ACL> _aclRepository;
		private readonly RecordManager _recordManager;
		private readonly IRepository<Form, Guid> _formRepository;
		private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
		private readonly IRepository<Record, Guid> _recordRepository;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly UserManager _userManager;
		private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
		private readonly IRepository<Submission, Guid> _submissionRepository;
		private readonly IRepository<Project, Guid> _projectRepository;

        private readonly IConfiguration _configuration;
        private readonly int _jwtExpiry = 365;
        private readonly IOptions<JSONWebToken> _JSONWebToken;

        public RecordMattersAppService(
			ACLManager aclManager,
			RecordManager recordManager,
			IRepository<ACL> aclRepository,
			IRepository<Form, Guid> formRepository,
			IRepository<RecordMatter, Guid> recordMatterRepository,
			IRepository<Record, Guid> recordRepository,
			UserManager userManager,
			IUnitOfWorkManager unitOfWorkManager,
			IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
			IRepository<Submission, Guid> submissionRepository,
			IRepository<Project, Guid> projectRepository,
			IOptions<JSONWebToken> JSONWebToken,
            IConfiguration configuration
		)
		{
			_ACLManager = aclManager;
			_recordManager = recordManager;
			_aclRepository = aclRepository;
			_formRepository = formRepository;
			_recordMatterRepository = recordMatterRepository;
			_recordRepository = recordRepository;
			_unitOfWorkManager = unitOfWorkManager;
			_userManager = userManager;

			_recordMatterItemRepository = recordMatterItemRepository;
			_submissionRepository = submissionRepository;
			_projectRepository = projectRepository;

            _configuration = configuration;
            //_jwtExpiry = _configuration.GetValue<int>("JSONWebToken:Expiry", 365);
            _jwtExpiry = JSONWebToken.Value.Expiry;
            _JSONWebToken = JSONWebToken;
        }

		public PagedResultDto<GetRecordMatterForView> GetAllByRecord(GetAllRecordMattersInput input)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
			{
				Action = "View",
				EntityId = input.Id,
				UserId = AbpSession.UserId,
				AccessToken = string.Empty,
				TenantId = AbpSession.TenantId
			});

			if (ACLResult.IsAuthed)
			{

				var query = (from o in _recordMatterRepository.GetAll().Where(j => j.RecordId == input.Id)
							 join o1 in _recordRepository.GetAll() on o.RecordId equals o1.Id into j1
							 from s1 in j1.DefaultIfEmpty()

							 select new GetRecordMatterForView()
							 {
								 RecordMatter = ObjectMapper.Map<RecordMatterDto>(o),
								 Record = ObjectMapper.Map<RecordDto>(s1)
								 //RecordRecordName = s1 == null ? "" : s1.RecordName.ToString()
							 });

				var totalCount = query.Count();

				IReadOnlyList<GetRecordMatterForView> recordMatters = query.ToList();

				return new PagedResultDto<GetRecordMatterForView>(
					totalCount,
					recordMatters
				);

			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		public async Task<PagedResultDto<GetRecordMatterForView>> GetAll(GetAllRecordMattersInput input)
		{
			List<ACL> UserACRs = _ACLManager.FetchAllUserACLs(new GetAllACLsInput() { UserId = (long)AbpSession.UserId });

			List<RecordMatter> RecordMatterACRs = new List<RecordMatter>();

			UserACRs.ForEach(i =>
			{
				var FoundRecordMatter = _recordMatterRepository.GetAll().Where(j => j.Id == i.EntityID && j.RecordId == input.Id);
				FoundRecordMatter.ToList().ForEach(j => RecordMatterACRs.Add(j));
			});


			if (RecordMatterACRs.Count() > 0)
			{
				var query = (from o in RecordMatterACRs
							 join o1 in _recordRepository.GetAll() on o.RecordId equals o1.Id into j1
							 from s1 in j1.DefaultIfEmpty()

							 select new GetRecordMatterForView()
							 {
								 RecordMatter = ObjectMapper.Map<RecordMatterDto>(o),
								 Record = ObjectMapper.Map<RecordDto>(s1)
							 });

				var totalCount = query.Count();

				IReadOnlyList<GetRecordMatterForView> recordMatters = query.ToList();

				return new PagedResultDto<GetRecordMatterForView>(
					totalCount,
					recordMatters
				);
			}
			else
			{
				return new PagedResultDto<GetRecordMatterForView>();
			}
		}


		public async Task<PagedResultDto<RecordLookupTableDto>> GetAllRecordForLookupTable(GetAllForLookupTableInput input)
		{
			Guid userRootFolderId = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, "R");
			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = userRootFolderId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				input.Filter = input.Filter?.Trim();

				var query = (
							from o in _recordRepository.GetAll()
							.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), o => o.RecordName.ToLower().Contains(input.Filter))
							where o.FolderId != null
							join a in _aclRepository.GetAll() on o.Id equals a.EntityID
							where a.UserId == AbpSession.UserId
							select o
						);

				var totalCount = query.Count();

				var recordList = await query
					.PageBy(input)
					.ToListAsync();

				var lookupTableDtoList = new List<RecordLookupTableDto>();
				foreach (var record in recordList)
				{
					lookupTableDtoList.Add(new RecordLookupTableDto
					{
						Id = record.Id.ToString(),
						DisplayName = record.RecordName.ToString()
					});
				}

				return new PagedResultDto<RecordLookupTableDto>(
					totalCount,
					lookupTableDtoList
				);
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}
		}

 
		public async Task<GetRecordMatterForEditOutput> GetRecordMatterForEdit(Guid Id, Guid RecordMatterItemId, Guid? formId, string accessToken)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
			{
				Action = "Edit",
				EntityId = Id,
				UserId = AbpSession.UserId,
				AccessToken = accessToken,
				TenantId = AbpSession.TenantId
			});

			if (ACLResult.IsAuthed)
			{
				_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

				RecordMatter recordMatter = _recordMatterRepository.FirstOrDefault(Id);
				var data = "{}";
				recordMatter = recordMatter ?? new RecordMatter()
				{
					Id = Id,
					AccessToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
					Data = data
				};

				// Check RecordMatterItem submission for paid
				if (_recordMatterItemRepository.GetAll().Any(e => e.Id == RecordMatterItemId))
				{
					Guid? submissionId = _recordMatterItemRepository.FirstOrDefault(RecordMatterItemId)?.SubmissionId;
					if (submissionId != null)
					{
						var submission = _submissionRepository.FirstOrDefault(e => e.Id == (Guid)submissionId);
						if (submission != null)
						{
							if (submission.RequiresPayment && submission.PaymentStatus != "Paid")
							{
								data.Replace("\"IsPaid\":true", "\"IsPaid\":false");
							}
						}
					}
				}

				GetRecordMatterForEditOutput output = new GetRecordMatterForEditOutput { RecordMatter = ObjectMapper.Map<CreateOrEditRecordMatterDto>(recordMatter) };
				// If null data take copy from master Record (Projects)
                if (string.IsNullOrEmpty(output.RecordMatter.Data))
                {
					var record = _recordRepository.GetAll().First(r => r.Id == recordMatter.RecordId);
					if (record != null)
					{
						recordMatter.Data = record.Data;
					}
				}

				output.RecordMatter.Data = BuildRecordMatter(recordMatter, formId);
				return output;
			}
			else
			{
				//throw new UserFriendlyException(L("NotAuthorized"));
				throw new UserFriendlyException($"{L("NotAuthorized_P1").Trim()}\n{L("NotAuthorized_P2", L("NotAuthorizedLink")).Trim()}");
			}
		}

		public async Task CreateOrEdit(CreateOrEditRecordMatterDto input)
		{
			

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
			{
				Action = "Edit",
				EntityId = (Guid)input.Id,
				UserId = AbpSession.UserId,
				AccessToken = string.Empty,
				TenantId = AbpSession.TenantId
			});

			RecordMatter recordMatter = _recordMatterRepository.GetAll().FirstOrDefault(e => e.Id == input.Id);

			if (ACLResult.IsAuthed || recordMatter == null)
			{
				if (recordMatter == null)
				{

					Guid? rmid = input.Id;
					if (rmid == null) rmid = Guid.NewGuid();

					recordMatter = new RecordMatter()
					{
						Id = (Guid)rmid,
						RecordMatterName = input.RecordMatterName.Truncate(RecordConsts.MaxRecordNameLength),
						RecordId = input.RecordId,
						Comments = input.Comments,
						Data = input.Data
					};
				}
				else
				{
					recordMatter.RecordMatterName = input.RecordMatterName.Truncate(RecordConsts.MaxRecordNameLength);
					recordMatter.Data = input.Data;
					recordMatter.Comments = input.Comments;
				}

				//RecordMatter recordMatter = ObjectMapper.Map<RecordMatter>(input);
				ACL aCL = new ACL()
				{
					UserId = AbpSession.UserId

				};
				await _recordManager.CreateOrEditRecordMatter(aCL, recordMatter);

			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}


		}

		[AbpAuthorize(AppPermissions.Pages_RecordMatters_Delete)]
		public async Task<MessageOutput> Delete(EntityDto<Guid> input)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
			{
				Action = "Delete",
				EntityId = input.Id,
				UserId = AbpSession.UserId,
				AccessToken = string.Empty,
				TenantId = AbpSession.TenantId
			});

			if (ACLResult.IsAuthed)
			{
				using (var unitOfWork = _unitOfWorkManager.Begin())
				{
					await _recordMatterRepository.DeleteAsync(input.Id);
					await _ACLManager.RemoveACL(new ACL() { EntityID = input.Id, UserId = AbpSession.UserId });
					unitOfWork.Complete();
				}
				return new MessageOutput()
				{
					Message = "Template Removed",
					Success = true
				};
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		public async Task<string> GetRecordMatterJsonData(EntityDto<Guid> input)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
			{
				Action = "View",
				EntityId = input.Id,
				UserId = AbpSession.UserId,
				AccessToken = string.Empty,
				TenantId = AbpSession.TenantId
			});

			if (ACLResult.IsAuthed)
			{

				var result = "{}";
				var recordMatter = await _recordMatterRepository.FirstOrDefaultAsync(input.Id);
				recordMatter = recordMatter ?? new RecordMatter()
				{
					Id = input.Id,
					AccessToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
					Data = "{}"
				};
				result = BuildRecordMatter(recordMatter, null);
				return result;

			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		public string GetRecordMatterByNameJsonData(string input)
		{
			var result = "{}";
			var userId = AbpSession.UserId;
			var recordmatter = _recordMatterRepository.GetAll().FirstOrDefault(i => i.CreatorUserId == userId && i.RecordMatterName == input);

			//WHATS THIS DOING?
			if (recordmatter != null)
			{
				result = recordmatter.Data;
				result = result.TrimStart('"');
				result = result.TrimEnd('"');
				result = result.Replace("\\r", "");
				result = result.Replace("\\n", "");
				result = result.Replace("\\", "");
			}
			result = BuildRecordMatter(recordmatter, null);
			return result;
		}

		private string BuildRecordMatter(RecordMatter recordMatter, Guid? formId)
		{
			var result = recordMatter.Data ?? "{}";

			//{ DateParseHandling = DateParseHandling.None }

			JObject jData = null;
			using (Newtonsoft.Json.JsonReader reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(result)))
			{
				reader.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;
				jData = JObject.Load(reader);
			}

			// Add myUserData
			// may be Anon user via an access token
			if (AbpSession.UserId != null)
			{
				User user = _userManager.GetUser(new Abp.UserIdentifier(AbpSession.TenantId, (long)AbpSession.UserId));
				if (user != null)
				{
					jData["AccessToken"] = recordMatter.AccessToken;
					jData["MyUserDataId"] = user.Id;
					jData["MyUserDataABN"] = user.ABN;
					jData["MyUserDataAddressCO"] = user.AddressCO;
					jData["MyUserDataAddressCountry"] = user.AddressCountry;
					jData["MyUserDataAddressLine1"] = user.AddressLine1;
					jData["MyUserDataAddressLine2"] = user.AddressLine2;
					jData["MyUserDataAddressPostCode"] = user.AddressPostCode;
					jData["MyUserDataAddressState"] = user.AddressState;
					jData["MyUserDataAddressSub"] = user.AddressSub;
					jData["MyUserDataBillingAddressCountry"] = user.BillingAddressCountry;
					jData["MyUserDataBillingAddressLine1"] = user.BillingAddressLine1;
					jData["MyUserDataBillingAddressLine2"] = user.BillingAddressLine2;
					jData["MyUserDataBillingAddressPostCode"] = user.BillingAddressPostCode;
					jData["MyUserDataBillingAddressState"] = user.BillingAddressState;
					jData["MyUserDataBillingName"] = user.BillingName;
					jData["MyUserDataEmailAddress"] = user.EmailAddress;
					jData["MyUserDataEmailAddressWork"] = user.EmailAddressWork;
					jData["MyUserDataEntityAddressCO"] = user.EntityAddressCO;
					jData["MyUserDataEntityAddressCountry"] = user.EntityAddressCountry;
					jData["MyUserDataEntityAddressLine1"] = user.EntityAddressLine1;
					jData["MyUserDataEntityAddressLine2"] = user.EntityAddressLine2;
					jData["MyUserDataEntityAddressPostCode"] = user.EntityAddressPostCode;
					jData["MyUserDataEntityAddressState"] = user.EntityAddressState;
					jData["MyUserDataEntityAddressSub"] = user.EntityAddressSub;
					jData["MyUserDataFax"] = user.Fax;
					jData["MyUserDataFullName"] = user.FullName;
					jData["MyUserDataJobTitle"] = user.JobTitle;
					jData["MyUserDataLegalABN"] = user.LegalABN;
					jData["MyUserDataName"] = user.Name;
					jData["MyUserDataNormalizedEmailAddress"] = user.NormalizedEmailAddress;
					jData["MyUserDataNormalizedUserName"] = user.NormalizedUserName;
					jData["MyUserDataPhoneNumber"] = user.PhoneNumber;
					jData["MyUserDataPhoneNumberMobile"] = user.PhoneNumberMobile;
					jData["MyUserDataPhoneNumberWork"] = user.PhoneNumberWork;
					jData["MyUserDataSurname"] = user.Surname;
					jData["MyUserDataTitle"] = user.Title;
					jData["MyUserDataUserName"] = user.UserName;
					jData["MyUserDataWebsiteURL"] = user.WebsiteURL;

				}
			}

			// ProjectTemplateId does not require authentication 
			var projecttemplate = _projectRepository.GetAll().FirstOrDefault(p => p.RecordId == recordMatter.RecordId);
			if (projecttemplate != null)
			{
				jData["ProjectTemplateId"] = projecttemplate.ProjectTemplateId;
			}

			var tenantId = AbpSession.TenantId;

			var project = _projectRepository.FirstOrDefault(p=> p.RecordId == recordMatter.RecordId);
			var recordMatters = _recordMatterRepository.GetAll().Where(rm => rm.RecordId == recordMatter.RecordId).OrderBy("Order asc").ToList();
			if (project != null )
			{

				tenantId = project.TenantId;

				jData["ProjectTemplateId"] = project.ProjectTemplateId;
				//project steps

				jData["CurrentStep"] = recordMatter.Id;
				JObject projectSteps = JObject.Parse("{}");
				foreach (RecordMatter rm in recordMatters)
				{
					JObject step = new JObject();
					step["stepId"] = rm.Id;
					step["name"] = rm.RecordMatterName;
					var status = (RecordMatterConsts.RecordMatterStatus)rm.Status;
					step["status"] = status.ToString();
					var order = rm.Order.ToString();
					projectSteps[order] = step;
				}
				jData["ProjectSteps"] = projectSteps;
			}

			// Build and Load External and Internal DataSources
			jData = BuildRecordMatterDataSources(jData, "TenantData");

			// Get Default Values from Form and RecordData
			var form = _formRepository.GetAll().FirstOrDefault(f => f.Id == formId);
			if (form != null)
            {
				JObject jObj = JObject.Parse(form.Schema);
				WalkFormDataSources(jData, jObj);
            }


			// Look in Form data for futher datatypes

			jData["DataSources"] = _jsonArrayDataSources;

			result = jData.ToString();
			return result;
		}

		JObject _jsonArrayDataSources = new JObject();

		private JObject BuildRecordMatterDataSources(JObject jData, string primaryKey)
        {
			var tenantId = AbpSession.TenantId;

			// Build Tenant Specific Data			
			try
			{

				// Clean The Data  // Default Data
				var rms = _recordMatterRepository.GetAll().Where(rm => rm.Key.ToLower() == primaryKey.ToLower() && rm.TenantId == tenantId);

                JArray jtKey = JArray.Parse("[]");
				
                foreach (RecordMatter rm in rms)
				{
                    JObject joTenantData = JObject.Parse(rm.Data);
					if (joTenantData != null)
					{
						JToken jtTenantData = joTenantData["Data"];
						if (jtTenantData != null)
						{
							if (jtTenantData.Count() > 0)
                                jtKey.Add(jtTenantData[0]);
						}
					}
                }

				// Get User Team Membership

				if (AbpSession.UserId != null)
				{
					var user = _userManager.GetUserById((long)AbpSession.UserId);
					var ous = _userManager.GetOrganizationUnits(user);

					foreach (OrganizationUnit ou in ous)
					{
						var key = primaryKey + ":" + ou.DisplayName;
						rms = _recordMatterRepository.GetAll().Where(rm => rm.Key.ToLower() == key.ToLower() && rm.TenantId == tenantId);
						foreach (RecordMatter rm in rms)
						{
							JObject joTenantData = JObject.Parse(rm.Data);
							if (joTenantData != null)
							{
								JToken jtTenantData = joTenantData["Data"];
								if (jtTenantData != null)
								{
									if (jtTenantData.Count() > 0)
										jtKey.Add(jtTenantData[0]);
								}
							}
						};
					}
				}

				if(jtKey.Count() > 0){
					string key = Regex.Replace(primaryKey, "datasource:", "", RegexOptions.IgnoreCase);
                    _jsonArrayDataSources[key] = jtKey;
                }
                
            }
			catch { 
			}

			return jData;

		}

		private void WalkFormDataSources(JObject jData, JToken node, Action<JObject> objectAction = null, Action<JProperty> propertyAction = null)
		{
			if (node.Type == JTokenType.Object)
			{
				if (objectAction != null) objectAction((JObject)node);

				foreach (JProperty child in node.Children<JProperty>())
				{
					if (propertyAction != null) propertyAction(child);

					if(child.Name == "defaultValue")
                    {
						if(child.Value.ToString().ToLower().StartsWith("datasource"))
                        {
							var vals = child.Value.ToString().Split(':');
                            if (vals.Length > 1)
                            {
								string key = "Datasource:" + vals[1]; string team = string.Empty;
                                if (vals.Length > 2)
									team = vals[2];
								key = string.IsNullOrEmpty(team) ? key : $"{key}:{team}";
								jData = BuildRecordMatterDataSources(jData, key);
							}
						}
                    }
					WalkFormDataSources(jData, child.Value, objectAction, propertyAction);
				}
			}
			else if (node.Type == JTokenType.Array)
			{
				foreach (JToken child in node.Children())
				{
					WalkFormDataSources(jData, child, objectAction, propertyAction);
				}
			}
		}

	}
}