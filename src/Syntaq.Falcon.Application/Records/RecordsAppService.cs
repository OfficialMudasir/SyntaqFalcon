using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.EntityFrameworkCore.Repositories;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Teams;
using Syntaq.Falcon.Utility;
using Syntaq.Falcon.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Records
{
    [EnableCors("AllowAll")]
    [AbpAuthorize(AppPermissions.Pages_Records)]
    public class RecordsAppService : FalconAppServiceBase, IRecordsAppService
    {
        private readonly ACLManager _ACLManager;
        private readonly TeamManager _teamManager;
        private readonly RecordManager _recordManager;
        private readonly FolderManager _folderManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly ICustomRecordsRepository _customrecordRepository;

        private readonly IRepository<ACL> _aclRepository;
        private readonly IRepository<Record, Guid> _recordRepository;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
        private readonly IRepository<RecordMatterAudit, Guid> _recordMatterAuditRepository;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IRepository<Folder, Guid> _folderRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        private readonly IConfiguration _configuration;
        private readonly int _jwtExpiry = 365;
        private readonly IOptions<JSONWebToken> _JSONWebToken;

        public RecordsAppService(
            ACLManager aclManager,
            TeamManager teamManager,
            RecordManager recordManager,
            FolderManager folderManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<ACL> aclRepository,
            IRepository<Record, Guid> recordRepository,
            ICustomRecordsRepository customrecordRepository,
            IRepository<RecordMatter, Guid> recordMatterRepository,
            IRepository<RecordMatterAudit, Guid> recordMatterAuditRepository,
            IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
            IRepository<Folder, Guid> folderRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOptions<JSONWebToken> JSONWebToken,
            IConfiguration configuration
        )
        {
            _ACLManager = aclManager;
            _teamManager = teamManager;
            _aclRepository = aclRepository;
            _recordManager = recordManager;
            _folderManager = folderManager;
            _unitOfWorkManager = unitOfWorkManager;
            _recordRepository = recordRepository;

            _customrecordRepository = customrecordRepository;

            _recordMatterRepository = recordMatterRepository;
            _recordMatterAuditRepository = recordMatterAuditRepository;
            _recordMatterItemRepository = recordMatterItemRepository;
            _folderRepository = folderRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;

            _configuration = configuration;
            //_jwtExpiry = _configuration.GetValue<int>("JSONWebToken:Expiry", 365);
            _jwtExpiry = JSONWebToken.Value.Expiry;
            _JSONWebToken = JSONWebToken;
        }

        public async Task<PagedResultDto<ResultsWithMattersDto>> GetAll(GetAllRecordsInput input)
        {
            Guid userRootFolderId = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, "R");
            Guid? parentFolderId = (string.IsNullOrEmpty(input.Id) || input.Id == "00000000-0000-0000-0000-000000000000") ? userRootFolderId : new Guid(input.Id);
            input.Id = input.Id == "00000000-0000-0000-0000-000000000000" || input.Id == null ? userRootFolderId.ToString() : input.Id;

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = Guid.Parse(input.Id), UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                input.Filter = input.Filter?.Trim();

                List<ResultsWithMattersDto> dtoResult = new List<ResultsWithMattersDto>();

                List<Guid> recordids = new List<Guid>();
                List<Guid> recordmatterids = new List<Guid>();
                List<Guid> recordmatteritemids = new List<Guid>();

                if (!string.IsNullOrEmpty(input.Filter))
                {

                    recordids = (
                        from o in _recordRepository.GetAll()
                        where o.RecordName.ToLower().Contains(input.Filter) && o.IsArchived == input.IsArchived
                        join a in _aclRepository.GetAll() on o.Id equals a.EntityID
                        where a.UserId == AbpSession.UserId
                        select o.Id
                    ).ToList();

                    recordmatterids = (
                        from o in _recordMatterRepository.GetAll()
                        where o.RecordMatterName.ToLower().Contains(input.Filter)
                        join a in _aclRepository.GetAll() on o.Id equals a.EntityID
                        where a.UserId == AbpSession.UserId
                        select o.RecordId
                    ).ToList();

                    recordmatteritemids = (
                        from o in _recordMatterItemRepository.GetAll()
                        where o.DocumentName.ToLower().Contains(input.Filter)
                        join a in _aclRepository.GetAll() on o.RecordMatterId equals a.EntityID
                        where a.UserId == AbpSession.UserId
                        select o.RecordMatter.RecordId
                    ).ToList();

                    recordids = recordids.Concat(recordmatterids).Concat(recordmatteritemids).ToList();

                }

                var recordcount = recordids.Count() + recordmatterids.Count() + recordmatteritemids.Count();

                // IMPT BUG in Entity framework?
                // Very slow method, replaced with GetRecords by Raw SQL

                //var records = _customrecordRepository.GetAllWithRecords()
                //    .Where(e => recordcount > 0 ? 
                //        recordids.Contains(e.Id) || recordmatterids.Contains(e.Id) || recordmatteritemids.Contains(e.Id) : 
                //        string.IsNullOrEmpty(input.Filter) ? e.FolderId == parentFolderId : false
                //        );

                var records = _customrecordRepository.GetAllWithRecordsRaw(parentFolderId, input.Filter, recordids, input.IsArchived);

                var folders = (

                        from o in _folderRepository.GetAll()
                        where string.IsNullOrWhiteSpace(input.Filter) ? o.ParentId == parentFolderId && o.Type == "R" : o.Name.ToLower().Contains(input.Filter)
                        && o.ParentId != new Guid("00000000-0000-0000-0000-000000000000")

                        join a in _aclRepository.GetAll() on o.Id equals a.EntityID
                        where a.UserId == AbpSession.UserId
                        select o
                    ).Distinct().ToList();

                if (parentFolderId == userRootFolderId)
                {
                    // Already captured above using ACL join
                    var sharedRecords = _customrecordRepository
                        .GetAllWithRecordsSharedUser(AbpSession.UserId)
                        .Where(r => r.FolderId != null) // Project Records
                        .Where(e => string.IsNullOrEmpty(input.Filter) ?
                            true :
                            Convert.ToString(e.RecordName).ToLower().Contains(input.Filter.ToLower()) || e.RecordMatters.Any(n => Convert.ToString(n.RecordMatterName).ToLower().Contains(input.Filter.ToLower())) || e.RecordMatters.Any(i => i.RecordMatterItems.Any(o => string.IsNullOrEmpty(o.DocumentName) ? false : Convert.ToString(o.DocumentName).ToLower().Contains(input.Filter.ToLower())))
                            ).Distinct();

                    records = records.Concat(sharedRecords).Distinct();

                    sharedRecords = _customrecordRepository
                    .GetAllWithRecordsSharedOrg(AbpSession.UserId)
                    .Where(e => string.IsNullOrEmpty(input.Filter) ? true : Convert.ToString(e.RecordName).ToLower().Contains(input.Filter.ToLower()) || e.RecordMatters.Any(n => Convert.ToString(n.RecordMatterName).ToLower().Contains(input.Filter.ToLower())) || e.RecordMatters.Any(i => i.RecordMatterItems.Any(o => string.IsNullOrEmpty(o.DocumentName) ? false : Convert.ToString(o.DocumentName).ToLower().Contains(input.Filter.ToLower()))));

                    records = records.Concat(sharedRecords).Distinct();

                    var sharedFolders = (from folder in _folderRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter))
                                         join acl in _aclRepository.GetAll() on folder.Id equals acl.EntityID
                                         where acl.CreatorUserId != AbpSession.UserId && acl.UserId == AbpSession.UserId && folder.Type == "R"
                                         select folder).ToList();

                    sharedFolders = (sharedFolders.Concat(from folder in _folderRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter))
                                                          join acl in _aclRepository.GetAll() on folder.Id equals acl.EntityID
                                                          join ut in _userOrganizationUnitRepository.GetAll() on acl.OrganizationUnitId equals ut.OrganizationUnitId
                                                          where ut.UserId == AbpSession.UserId && folder.Type == "R"
                                                          select folder)).ToList();

                    folders = folders.Concat(sharedFolders).Distinct().ToList();
                }

                var recordcnt = records.Count();
                var foldercnt = folders.Where(i => i.Name != "Your Records").Count();

                folders.Where(i => i.Name != "Your Records")
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList()
                    .ForEach(i =>
                    {
                        var Result = new ResultsWithMattersDto()
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.Description,
                            LastModified = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime,
                            Type = "Folder",
                            RecordMatters = new List<RecordMatterDto>(),
                            UserACLPermission = i.ParentId == parentFolderId ? ACLResult.Permission : _ACLManager.FetchRole(new ACLCheckDto() { EntityId = i.Id, UserId = AbpSession.UserId })
                            //UserACLPermission = i.ParentId == parentFolderId ? ACLResult.Permission : _ACLManager.FetchRole(new ACLCheckDto() { EntityId = i.Id, UserId = AbpSession.UserId, OrgId = i.OrganizationUnitId != null ? i.OrganizationUnitId : null })
                        };
                        dtoResult.Add(Result);
                    });


                var recordsToSkip = input.SkipCount == 0 ? input.SkipCount : (input.SkipCount - dtoResult.Count() - foldercnt) + dtoResult.Count();
                var recordsToTake = input.MaxResultCount - dtoResult.Count;

                records.OrderByDescending(i => i.CreationTime).Skip(recordsToSkip).Take(recordsToTake).ToList().ForEach(i =>
                {
                    var Result = new ResultsWithMattersDto()
                    {
                        Id = i.Id,
                        Name = i.RecordName,
                        isArchived = i.IsArchived,
                        Description = "",
                        LastModified = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime,
                        Type = "Record",
                        UserACLPermission = i.FolderId == parentFolderId ? ACLResult.Permission : _ACLManager.FetchRole(new ACLCheckDto() { EntityId = i.Id, UserId = AbpSession.UserId, OrgId = i.OrganizationUnitId != null ? i.OrganizationUnitId : null }),
                        RecordMatters = i.RecordMatters.Select(x => new RecordMatterDto
                        {
                            Id = x.Id,
                            RecordId = x.RecordId,
                            RecordMatterName = x.RecordMatterName,
                            CreationTime = x.CreationTime,
                            Comments = x.Comments,
                            FormId = x.FormId,
                            HasFiles = x.HasFiles,
                            LastModified = x.LastModificationTime == null ? x.CreationTime : (DateTime)x.LastModificationTime,
                            RecordMatterItems = x.RecordMatterItems.OrderBy(rmi => rmi.Order).Select(y => new RecordMatterItemDto
                            {
                                Id = y.Id,
                                RecordMatterId = y.RecordMatterId,
                                GroupId = y.GroupId,
                                FormId = y.FormId,
                                DocumentName = y.DocumentName,
                                Document = y.HasDocument,
                                CreationTime = y.CreationTime,
                                LastModified = y.LastModificationTime == null ? y.CreationTime : (DateTime)y.LastModificationTime,

                                //AllowWord =  y.AllowedFormats.Contains("W") ? true : false,
                                //AllowPdf = y.AllowedFormats.Contains("P") ? true : false,
                                //AllowHTML = y.AllowedFormats.Contains("H") ? true : false

                                // Support old basic format allowed also or else formats will appear 
                                // for all users 
                                AllowWord = FileFormatAllowed(y.AllowWordAssignees, y.AllowedFormats, "W").Result,
                                AllowPdf = FileFormatAllowed(y.AllowPdfAssignees, y.AllowedFormats, "P").Result,
                                AllowHTML = FileFormatAllowed(y.AllowHtmlAssignees, y.AllowedFormats, "H").Result,

                                LockOnBuild = y.LockOnBuild

                            }).ToList()
                        }).ToList(),
                        Comments = i.Comments
                    };
                    dtoResult.Add(Result);
                });

                int totalCount = recordcnt + foldercnt;

                IQueryable<ResultsWithMattersDto> iQformFolders;
                iQformFolders = dtoResult.AsQueryable();
                iQformFolders = iQformFolders.OrderBy("type asc").ThenBy(input.Sorting ?? "type asc");//.PageBy(input);
                dtoResult = iQformFolders.ToList();

				return new PagedResultDto<ResultsWithMattersDto>(totalCount, dtoResult);
			}
			else
			{
				return new PagedResultDto<ResultsWithMattersDto>(0, new List<ResultsWithMattersDto>());
			}
		}
	
		// Support old basic format allowed also or else formats will appear // for all users
		private async Task<bool> FileFormatAllowed(string assignees, string allowedformats = "P", string type = "P")
        {

            // default none set then true
            bool result = true;

            if (string.IsNullOrEmpty(assignees) || assignees == "[]" || assignees == "null")
            {
                result = allowedformats.Contains(type) ? true : false;
            }
            else
            {
                List<GrantACLDto> obj = new List<GrantACLDto>();
                result = false;

                obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GrantACLDto>>(assignees);

                if (obj != null)
                {
                    var user = await UserManager.GetUserByIdAsync((long)AbpSession.UserId);

                    if (user != null)
                    {
                        if (obj.Any(i => i.value == user.UserName)) return true;

                        _userOrganizationUnitRepository.GetAll().Where(i => i.UserId == user.Id).ToList().ForEach(i => {
                            if (obj.Any(n => n.Type == "Team" && n.Id == i.OrganizationUnitId))
                            {
                                result = true;
                            }
                        });
                    }
                }
            }

            return result;
        }

        [Authorize(Policy = "ViewById")]
        public async Task<string> GetRecordJSONData(Guid id)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
            {
                Action = "View",
                EntityId = id,
                UserId = AbpSession.UserId,
                AccessToken = string.Empty,
                TenantId = AbpSession.TenantId
            });

            if (ACLResult.IsAuthed)
            {


                if (_recordRepository.GetAll().Any(rm => rm.Id == id))
                {
                    var Record = await _recordRepository.GetAsync(id);
                    return Record.Data ?? "{}";
                }

                if (_recordMatterRepository.GetAll().Any(rm => rm.Id == id))
                {
                    var rm = await _recordMatterRepository.GetAsync(id);
                    return rm.Data ?? "{}";
                }

                if (_recordMatterAuditRepository.GetAll().Any(rma => rma.Id == id))
                {
                    var rma = await _recordMatterAuditRepository.GetAsync(id);
                    return rma.Data ?? "{}";
                }

                return "{}";

            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }


        [AbpAuthorize(AppPermissions.Pages_Records_Create, AppPermissions.Pages_Records_Edit)]
        [Authorize(Policy = "EditById")]
        public async Task CreateOrEdit(CreateOrEditRecordDto input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
            {
                Action = "Edit",
                EntityId = input.Id == null ? Guid.Parse("00000000-0000-0000-0000-000000000000") : (Guid)input.Id,
                UserId = AbpSession.UserId,
                AccessToken = string.Empty,
                TenantId = AbpSession.TenantId
            });

            if (ACLResult.IsAuthed)
            {
                Record record = _recordRepository.GetAll().FirstOrDefault(e => e.Id == input.Id);
                if (record == null)
                {
                    record = new Record()
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                        RecordName = input.RecordName.Truncate(RecordConsts.MaxRecordNameLength),
                        FolderId = input.FolderId,
                        Data = input.Data
                    };
                }
                else
                {
                    record.RecordName = input.RecordName.Truncate(RecordConsts.MaxRecordNameLength);
                    record.FolderId = input.FolderId;
                }

                ACL aCL = new ACL()
                {
                    UserId = AbpSession.UserId
                };

                await _recordManager.CreateOrEditRecord(aCL, record);
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<GetRecordForEditOutput> GetRecordForEdit(EntityDto<Guid> input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
            {
                Action = "Edit",
                EntityId = input.Id,
                UserId = AbpSession.UserId,
                AccessToken = string.Empty,
                TenantId = AbpSession.TenantId
            });

            if (ACLResult.IsAuthed)
            {

                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                var record = await _recordRepository.FirstOrDefaultAsync(input.Id);

                record = record ?? new Record()
                {
                    Id = input.Id,
                    AccessToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
                    Data = "{}"
                };

                GetRecordForEditOutput output = new GetRecordForEditOutput { Record = ObjectMapper.Map<CreateOrEditRecordDto>(record) };
                //output.Record.Data = buildRecordMatter(recordMatter);

                output.Record.Data = output.Record.Data.Replace("\"IsPaid\":true", "\"IsPaid\":false");

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

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

                    var rec = _recordRepository.FirstOrDefault(input.Id);

                    var rm = _recordMatterRepository.GetAll().Where(rm => rm.RecordId == input.Id);
                    _recordMatterRepository.GetDbContext().RemoveRange(rm);

                    var rmi = _recordMatterItemRepository.GetAll().Where(rmi => rmi.RecordMatter.RecordId == input.Id);
                    _recordMatterItemRepository.GetDbContext().RemoveRange(rmi);

                    await _recordRepository.DeleteAsync(input.Id);
					await _ACLManager.RemoveACL(new ACL() { EntityID = input.Id, UserId = AbpSession.UserId });
					unitOfWork.Complete();
				}
				return new MessageOutput()
				{
					Message = "Record Removed",
					Success = true
				};
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}


        }

        public async Task<MessageOutput> Archive(EntityDto<Guid> input)
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

                    var rec = _recordRepository.FirstOrDefault(input.Id);
                    rec.IsArchived = true;
                    unitOfWork.Complete();
                }
                return new MessageOutput()
                {
                    Message =  "Record Archived" ,
                    Success = true
                };
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }


        public async Task<MessageOutput> UnArchive(EntityDto<Guid> input)
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

                    var rec = _recordRepository.FirstOrDefault(input.Id);
                    rec.IsArchived = false;
                    unitOfWork.Complete();
                }
                return new MessageOutput()
                {
                    Message = "Record Un-Archived",
                    Success = true
                };
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<bool> Move(MoveFolderDto moveFolderDto)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
            {
                Action = "Edit",
                EntityId = moveFolderDto.Id,
                UserId = AbpSession.UserId,
                AccessToken = string.Empty,
                TenantId = AbpSession.TenantId
            });

            if (ACLResult.IsAuthed)
            {
                moveFolderDto.UserId = AbpSession.UserId;
                var IsMoved = await _folderManager.Move(moveFolderDto);
                return IsMoved;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }
    }
}