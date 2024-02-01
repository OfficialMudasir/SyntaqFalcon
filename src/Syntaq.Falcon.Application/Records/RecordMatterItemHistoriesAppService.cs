using Syntaq.Falcon.Records;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Submissions;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Syntaq.Falcon.Storage;
using System.IO;
using Syntaq.Falcon.Utility;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.AccessControlList;

namespace Syntaq.Falcon.Records
{
    [AbpAuthorize(AppPermissions.Pages_RecordMatterItemHistories)]
    public class RecordMatterItemHistoriesAppService : FalconAppServiceBase, IRecordMatterItemHistoriesAppService
    {
        
        private readonly IRepository<RecordMatter, Guid> _lookup_recordMatterRepository;
        private readonly IRepository<RecordMatterItemHistory, Guid> _recordMatterItemHistoryRepository;
        private readonly IRepository<RecordMatterItem, Guid> _lookup_recordMatterItemRepository;
        private readonly IRepository<Form, Guid> _lookup_formRepository;
        private readonly IRepository<Submission, Guid> _lookup_submissionRepository;
        private readonly ACLManager _ACLManager;

        public RecordMatterItemHistoriesAppService(
            IRepository<RecordMatterItemHistory, Guid> recordMatterItemHistoryRepository, 
            IRepository<RecordMatterItem, Guid> lookup_recordMatterItemRepository,
            IRepository<RecordMatter, Guid> lookup_recordMatterRepository,
            IRepository<Form, Guid> lookup_formRepository, 
            IRepository<Submission, Guid> lookup_submissionRepository, 
            ACLManager aclManager)
        {
            _recordMatterItemHistoryRepository = recordMatterItemHistoryRepository;
            _lookup_recordMatterItemRepository = lookup_recordMatterItemRepository;
            _lookup_recordMatterRepository = lookup_recordMatterRepository;
            _lookup_formRepository = lookup_formRepository;
            _lookup_submissionRepository = lookup_submissionRepository;
            _ACLManager = aclManager;

        }

        public async Task<PagedResultDto<GetRecordMatterItemHistoryForViewDto>> GetAll(GetAllRecordMatterItemHistoriesInput input)
        {
            input.Filter = input.Filter?.Trim();

            var filteredRecordMatterItemHistories = _recordMatterItemHistoryRepository.GetAll()
                        .Include(e => e.RecordMatterItemFk)
                        .Include(e => e.FormFk)
                        .Include(e => e.SubmissionFk)
                        .WhereIf(input.RecordMatterId != null , e => e.RecordMatterItemId == input.RecordMatterId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DocumentName.Contains(input.Filter) || e.AllowedFormats.Contains(input.Filter) || e.Status.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DocumentNameFilter), e => e.DocumentName == input.DocumentNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AllowedFormatsFilter), e => e.AllowedFormats == input.AllowedFormatsFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter), e => e.Status == input.StatusFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RecordMatterItemDocumentNameFilter), e => e.RecordMatterItemFk != null && e.RecordMatterItemFk.DocumentName == input.RecordMatterItemDocumentNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.FormNameFilter), e => e.FormFk != null && e.FormFk.Name == input.FormNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SubmissionSubmissionStatusFilter), e => e.SubmissionFk != null && e.SubmissionFk.SubmissionStatus == input.SubmissionSubmissionStatusFilter);

            var pagedAndFilteredRecordMatterItemHistories = filteredRecordMatterItemHistories
                .OrderBy(input.Sorting ?? "CreationTime desc")
                .PageBy(input);

            var recordMatterItemHistories = from o in pagedAndFilteredRecordMatterItemHistories
                                            join o1 in _lookup_recordMatterItemRepository.GetAll() on o.RecordMatterItemId equals o1.Id into j1
                                            from s1 in j1.DefaultIfEmpty()

                                            join o2 in _lookup_formRepository.GetAll() on o.FormId equals o2.Id into j2
                                            from s2 in j2.DefaultIfEmpty()

                                            join o3 in _lookup_submissionRepository.GetAll() on o.SubmissionId equals o3.Id into j3
                                            from s3 in j3.DefaultIfEmpty()

                                            select new
                                            {
                                                o.CreationTime,
                                                o.DocumentName,
                                                o.Name,
                                                o.AllowedFormats,
                                                o.Status,
                                                Id = o.Id,
                                                RecordMatterItemDocumentName = s1 == null || s1.DocumentName == null ? "" : s1.DocumentName.ToString(),
                                                FormName = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                                                SubmissionSubmissionStatus = s3 == null || s3.SubmissionStatus == null ? "" : s3.SubmissionStatus.ToString()
                                            };

            var totalCount = await filteredRecordMatterItemHistories.CountAsync();

            var dbList = await recordMatterItemHistories.ToListAsync();
            var results = new List<GetRecordMatterItemHistoryForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetRecordMatterItemHistoryForViewDto()
                {
                    RecordMatterItemHistory = new RecordMatterItemHistoryDto
                    {

                        DocumentName = o.DocumentName,
                        Name = o.Name,
                        AllowedFormats = o.AllowedFormats,
                        Status = o.Status,
                        Id = o.Id,
                        CreatedDate = o.CreationTime
                    },
                    RecordMatterItemDocumentName = o.RecordMatterItemDocumentName,
                    FormName = o.FormName,
                    SubmissionSubmissionStatus = o.SubmissionSubmissionStatus
                };

                results.Add(res);
            }

            return new PagedResultDto<GetRecordMatterItemHistoryForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetRecordMatterItemHistoryForViewDto> GetRecordMatterItemHistoryForView(Guid id)
        {
            var recordMatterItemHistory = await _recordMatterItemHistoryRepository.GetAsync(id);

            var output = new GetRecordMatterItemHistoryForViewDto { RecordMatterItemHistory = ObjectMapper.Map<RecordMatterItemHistoryDto>(recordMatterItemHistory) };

            if (output.RecordMatterItemHistory.RecordMatterItemId != null)
            {
                var _lookupRecordMatterItem = await _lookup_recordMatterItemRepository.FirstOrDefaultAsync((Guid)output.RecordMatterItemHistory.RecordMatterItemId);
                output.RecordMatterItemDocumentName = _lookupRecordMatterItem?.DocumentName?.ToString();
            }

            if (output.RecordMatterItemHistory.FormId != null)
            {
                var _lookupForm = await _lookup_formRepository.FirstOrDefaultAsync((Guid)output.RecordMatterItemHistory.FormId);
                output.FormName = _lookupForm?.Name?.ToString();
            }

            if (output.RecordMatterItemHistory.SubmissionId != null)
            {
                var _lookupSubmission = await _lookup_submissionRepository.FirstOrDefaultAsync((Guid)output.RecordMatterItemHistory.SubmissionId);
                output.SubmissionSubmissionStatus = _lookupSubmission?.SubmissionStatus?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItemHistories_Edit)]
        public async Task<GetRecordMatterItemHistoryForEditOutput> GetRecordMatterItemHistoryForEdit(EntityDto<Guid> input)
        {
            var recordMatterItemHistory = await _recordMatterItemHistoryRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetRecordMatterItemHistoryForEditOutput { RecordMatterItemHistory = ObjectMapper.Map<CreateOrEditRecordMatterItemHistoryDto>(recordMatterItemHistory) };

            if (output.RecordMatterItemHistory.RecordMatterItemId != null)
            {
                var _lookupRecordMatterItem = await _lookup_recordMatterItemRepository.FirstOrDefaultAsync((Guid)output.RecordMatterItemHistory.RecordMatterItemId);
                output.RecordMatterItemDocumentName = _lookupRecordMatterItem?.DocumentName?.ToString();
            }

            if (output.RecordMatterItemHistory.FormId != null)
            {
                var _lookupForm = await _lookup_formRepository.FirstOrDefaultAsync((Guid)output.RecordMatterItemHistory.FormId);
                output.FormName = _lookupForm?.Name?.ToString();
            }

            if (output.RecordMatterItemHistory.SubmissionId != null)
            {
                var _lookupSubmission = await _lookup_submissionRepository.FirstOrDefaultAsync((Guid)output.RecordMatterItemHistory.SubmissionId);
                output.SubmissionSubmissionStatus = _lookupSubmission?.SubmissionStatus?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditRecordMatterItemHistoryDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItemHistories_Create)]
        protected virtual async Task Create(CreateOrEditRecordMatterItemHistoryDto input)
        {
            var recordMatterItemHistory = ObjectMapper.Map<RecordMatterItemHistory>(input);

            if (AbpSession.TenantId != null)
            {
                recordMatterItemHistory.TenantId = (int?)AbpSession.TenantId;
            }

            await _recordMatterItemHistoryRepository.InsertAsync(recordMatterItemHistory);

        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItemHistories_Edit)]
        protected virtual async Task Update(CreateOrEditRecordMatterItemHistoryDto input)
        {
            var recordMatterItemHistory = await _recordMatterItemHistoryRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, recordMatterItemHistory);

        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItemHistories_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _recordMatterItemHistoryRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItemHistories)]
        public async Task<PagedResultDto<RecordMatterItemHistoryRecordMatterItemLookupTableDto>> GetAllRecordMatterItemForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_recordMatterItemRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.DocumentName != null && e.DocumentName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var recordMatterItemList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<RecordMatterItemHistoryRecordMatterItemLookupTableDto>();
            foreach (var recordMatterItem in recordMatterItemList)
            {
                lookupTableDtoList.Add(new RecordMatterItemHistoryRecordMatterItemLookupTableDto
                {
                    Id = recordMatterItem.Id.ToString(),
                    DisplayName = recordMatterItem.DocumentName?.ToString()
                });
            }

            return new PagedResultDto<RecordMatterItemHistoryRecordMatterItemLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItemHistories)]
        public async Task<PagedResultDto<RecordMatterItemHistoryFormLookupTableDto>> GetAllFormForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_formRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var formList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<RecordMatterItemHistoryFormLookupTableDto>();
            foreach (var form in formList)
            {
                lookupTableDtoList.Add(new RecordMatterItemHistoryFormLookupTableDto
                {
                    Id = form.Id.ToString(),
                    DisplayName = form.Name?.ToString()
                });
            }

            return new PagedResultDto<RecordMatterItemHistoryFormLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterItemHistories)]
        public async Task<PagedResultDto<RecordMatterItemHistorySubmissionLookupTableDto>> GetAllSubmissionForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_submissionRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.SubmissionStatus != null && e.SubmissionStatus.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var submissionList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<RecordMatterItemHistorySubmissionLookupTableDto>();
            foreach (var submission in submissionList)
            {
                lookupTableDtoList.Add(new RecordMatterItemHistorySubmissionLookupTableDto
                {
                    Id = submission.Id.ToString(),
                    DisplayName = submission.SubmissionStatus?.ToString()
                });
            }

            return new PagedResultDto<RecordMatterItemHistorySubmissionLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public GetRecordMatterItemForDownload GetDocumentForDownload(EntityDto<Guid> input, string format)
        {

            var recordmatteritemhistory = _recordMatterItemHistoryRepository.GetAll().Include(i => i.RecordMatterItemFk).Where(i => i.Id == input.Id).FirstOrDefault();

            if (recordmatteritemhistory != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "View",
                    EntityId = (Guid) recordmatteritemhistory.RecordMatterItemId,
                    UserId = AbpSession.UserId,

                });

                if (ACLResult.IsAuthed)
                {

                    Byte[] bydoc = recordmatteritemhistory.Document;
                    var AllowedFormats = recordmatteritemhistory.AllowedFormats;
                    RecordMatterItemForDownloadType type = RecordMatterItemForDownloadType.PDF;

                    if (bydoc != null)
                    {
                        bool draft = true;
                        var recordMatter = _lookup_recordMatterRepository.FirstOrDefault(e => e.Id == recordmatteritemhistory.RecordMatterItemFk.RecordMatterId);                      
                        if(recordMatter != null){
                            draft = recordMatter.Status == RecordMatterConsts.RecordMatterStatus.Final ? false : true;
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
                    }

                    var filename = Path.GetFileNameWithoutExtension(recordmatteritemhistory.DocumentName);
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
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

    }
}