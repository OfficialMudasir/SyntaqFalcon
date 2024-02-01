using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Syntaq.Falcon.Projects.ProjectConsts;
using static Syntaq.Falcon.Records.RecordMatterContributorConsts;

namespace Syntaq.Falcon.Records
{
    [EnableCors("AllowAll")]
    public class RecordMatterItemsAppService : FalconAppServiceBase, IRecordMatterItemsAppService
    {
        private readonly RecordManager _recordManager;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
        private readonly IRepository<ACL> _aclRepository;

        private readonly IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;
        private readonly IRepository<Record, Guid> _recordRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Submission, Guid> _submissionRepository;
        IRepository<Project, Guid> _projectRepository;
        private readonly ACLManager _ACLManager;

        public RecordMatterItemsAppService(
            ACLManager aclManager,
            RecordManager recordManager,
            IRepository<ACL> aclRepository,
            IRepository<Project, Guid> projectRepository,
            IRepository<Record, Guid> recordRepository,
            IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
            IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository,
            IRepository<RecordMatter, Guid> recordMatterRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Submission, Guid> submissionRepository
        )
        {
            _recordManager = recordManager;
            _recordRepository = recordRepository;
            _aclRepository = aclRepository;
            _projectRepository = projectRepository;
            _recordMatterItemRepository = recordMatterItemRepository;
            _recordMatterContributorRepository = recordMatterContributorRepository;
            _recordMatterRepository = recordMatterRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _submissionRepository = submissionRepository;
            _ACLManager = aclManager;
        }

        public PagedResultDto<GetRecordMatterItemForView> GetAllByRecordMatter(GetAllRecordMatterItemsInput input)
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
                var query = (from o in _recordMatterItemRepository.GetAll().Where(j => j.RecordMatterId == input.Id)
                             join o1 in _recordMatterRepository.GetAll() on o.RecordMatterId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()
                             select new GetRecordMatterItemForView()
                             {
                                 RecordMatterItem = new RecordMatterItemDto()
                                 {
                                     AllowedFormats = o.AllowedFormats,
                                     AllowHTML = o.AllowedFormats.Contains("H"),
                                     AllowPdf = o.AllowedFormats.Contains("P"),
                                     AllowWord = o.AllowedFormats.Contains("W"),
                                     CreationTime = o.CreationTime,
                                     DocumentName = o.DocumentName,
                                     FormId = o.FormId,
                                     GroupId = o.GroupId,
                                     // HasDocument = true,
                                     Id = o.Id,
                                     //LastModified = (DateTime)o.LastModificationTime,
                                     LastModified = o.LastModificationTime == null ? o.CreationTime : (DateTime)o.LastModificationTime,
                                     LockOnBuild = o.LockOnBuild,
                                     RecordMatterId = o.RecordMatterId,
                                     Status = o.Status

                                 }
                             }); ;
                return new PagedResultDto<GetRecordMatterItemForView>(
                    query.Count(),
                    query.ToList()
                );
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<GetRecordMatterItemForEditOutput> GetRecordMatterItemForEdit(EntityDto<Guid> input)
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

                var recordMatterItem = await _recordMatterItemRepository.FirstOrDefaultAsync(input.Id);
                var output = new GetRecordMatterItemForEditOutput { RecordMatterItem = ObjectMapper.Map<CreateOrEditRecordMatterItemDto>(recordMatterItem) };
                if (output.RecordMatterItem.RecordMatterId != null)
                {
                    var recordMatter = await _recordMatterRepository.FirstOrDefaultAsync((Guid)output.RecordMatterItem.RecordMatterId);
                    output.RecordMatterTenantId = recordMatter.TenantId.ToString();
                }
                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task CreateOrEdit(CreateOrEditRecordMatterItemDto input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
            {
                Action = "Edit",
                EntityId = (Guid)input.Id,
                UserId = AbpSession.UserId,
                AccessToken = string.Empty,
                TenantId = AbpSession.TenantId
            });

            if (ACLResult.IsAuthed)
            {

                //await _recordManager.CreateOrEditRecordMatterItem(RecordMatterItem);
                var rmi = _recordMatterItemRepository.GetAll().FirstOrDefault(i => i.Id == input.Id);

                if (input.Id == null || rmi == null)
                {
                    RecordMatterItem RecordMatterItem = ObjectMapper.Map<RecordMatterItem>(input);
                    await _recordMatterItemRepository.InsertAsync(RecordMatterItem);
                }
                else
                {
                    ObjectMapper.Map(input, rmi);
                    await _recordMatterItemRepository.UpdateAsync(rmi);
                }

            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }


        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItems_Create)]
        private async Task CreateRecordMatterItem(RecordMatterItem RecordMatterItem)
        {
            await _recordMatterItemRepository.InsertAsync(RecordMatterItem);
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItems_Edit)]
        private async Task UpdateRecordMatterItem(RecordMatterItem RecordMatterItem)
        {
            await _recordMatterItemRepository.UpdateAsync(RecordMatterItem);
        }

        public async Task Delete(EntityDto<Guid> input)
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
                await _recordMatterItemRepository.DeleteAsync(input.Id);
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }


        [AbpAuthorize(AppPermissions.Pages_RecordMatterItems_Edit)]
        public async Task<PagedResultDto<RecordMatterLookupTableDto>> GetAllRecordMatterForLookupTable(GetAllForLookupTableInput input)
        {
            Guid userRootFolderId = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, "R");
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = userRootFolderId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                input.Filter = input.Filter?.Trim();

                var recordListId = (
                                from o in _recordRepository.GetAll()
                                where o.FolderId != null
                                join a in _aclRepository.GetAll() on o.Id equals a.EntityID
                                where a.UserId == AbpSession.UserId
                                select o.Id
                            );

                var query = (
                           from o in _recordMatterRepository.GetAll()
                           .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), o => o.RecordMatterName.ToLower().Contains(input.Filter))
                           where recordListId.Contains(o.RecordId)
                           join a in _aclRepository.GetAll() on o.Id equals a.EntityID
                           where a.UserId == AbpSession.UserId
                           select o
                       );


                var totalCount = await query.CountAsync();

                var recordMatterList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<RecordMatterLookupTableDto>();
                foreach (var recordMatter in recordMatterList)
                {
                    lookupTableDtoList.Add(new RecordMatterLookupTableDto
                    {
                        Id = recordMatter.Id.ToString(),
                        DisplayName = string.IsNullOrEmpty(recordMatter.RecordMatterName) ? "" : recordMatter.RecordMatterName.ToString()
                    });
                }

                return new PagedResultDto<RecordMatterLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }


        public async Task<GetRecordMatterItemForDownload> GetDocumentForDownload(EntityDto<Guid> input, int version, string format, string AccessToken)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "View",
                    EntityId = input.Id,
                    UserId = AbpSession.UserId,
                    AccessToken = AccessToken,
                    TenantId = AbpSession.TenantId
                });

                if (ACLResult.IsAuthed)
                {

                    var recordmatteritem = await _recordMatterItemRepository.GetAll().Where(i => i.Id == input.Id).FirstOrDefaultAsync();
                    Byte[] bydoc = recordmatteritem.Document;
                    var AllowedFormats = recordmatteritem.AllowedFormats;
                    RecordMatterItemForDownloadType type = RecordMatterItemForDownloadType.PDF;

                    if (bydoc != null)
                    {
                        bool draft = false;

                        var recordMatter = _recordMatterRepository.GetAll().FirstOrDefault(e => e.Id == recordmatteritem.RecordMatterId);
                        draft = recordMatter.Status == RecordMatterConsts.RecordMatterStatus.Final ? false : true;

                        if (!draft)
                        {
                            if (recordmatteritem.SubmissionId.HasValue)
                            {
                                var submission = _submissionRepository.Get((Guid)recordmatteritem.SubmissionId);
                                draft = submission.RequiresPayment && submission.PaymentStatus != "Paid" ? true : false;
                            }
                        }

                        if (AllowedFormats.Contains("W") && (format == "docx" || format == "word" || format == "doc"))
                        {
                            bydoc = AsposeUtility.BytesToWord(bydoc, draft);
                            type = RecordMatterItemForDownloadType.Word;
                        }
                        else if (AllowedFormats.Contains("P") && format == "pdf")
                        {
                            bydoc = AsposeUtility.BytesToPdf(bydoc, draft);
                            type = RecordMatterItemForDownloadType.PDF;
                        }
                        else if (AllowedFormats.Contains("H") && format == "html")
                        {
                            bydoc = AsposeUtility.BytesToHTML(bydoc);
                            type = RecordMatterItemForDownloadType.HTML;
                        }
                        else
                        {
                            type = RecordMatterItemForDownloadType.Disallow;
                        }
                    }

                    var filename = Path.GetFileNameWithoutExtension(recordmatteritem.DocumentName);
                    var output = new GetRecordMatterItemForDownload
                    {
                        RecordMatterItem = new RecordMatterItemForDownloadDto
                        {
                            Document = type != RecordMatterItemForDownloadType.Disallow ? bydoc : null,
                            DocumentName = type != RecordMatterItemForDownloadType.Disallow ? filename + "." + format : null,
                            Type = type
                        }
                    };
                    return output;

                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }
            }
        }
    }
}