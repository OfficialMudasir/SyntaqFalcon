using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Records;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.Records.Exporting;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using static Syntaq.Falcon.Records.RecordMatterConsts;

///
/// No Public Interface

//namespace Syntaq.Falcon.Records
//{
//    [AbpAuthorize(AppPermissions.Pages_RecordMatterAudits)]
//    public class RecordMatterAuditsAppService : FalconAppServiceBase, IRecordMatterAuditsAppService
//    {
//        private readonly IRepository<RecordMatterAudit, Guid> _recordMatterAuditRepository;
//        private readonly IRecordMatterAuditsExcelExporter _recordMatterAuditsExcelExporter;
//        private readonly IRepository<User, long> _lookup_userRepository;
//        private readonly IRepository<RecordMatter, Guid> _lookup_recordMatterRepository;

//        public RecordMatterAuditsAppService(IRepository<RecordMatterAudit, Guid> recordMatterAuditRepository, IRecordMatterAuditsExcelExporter recordMatterAuditsExcelExporter, IRepository<User, long> lookup_userRepository, IRepository<RecordMatter, Guid> lookup_recordMatterRepository)
//        {
//            _recordMatterAuditRepository = recordMatterAuditRepository;
//            _recordMatterAuditsExcelExporter = recordMatterAuditsExcelExporter;
//            _lookup_userRepository = lookup_userRepository;
//            _lookup_recordMatterRepository = lookup_recordMatterRepository;

//        }

//        public async Task<PagedResultDto<GetRecordMatterAuditForViewDto>> GetAll(GetAllRecordMatterAuditsInput input)
//        {
//            var statusFilter = input.StatusFilter.HasValue
//                        ? (RecordMatterStatus)input.StatusFilter
//                        : default;

//            var filteredRecordMatterAudits = _recordMatterAuditRepository.GetAll()
//                        .Include(e => e.UserFk)
//                        .Include(e => e.RecordMatterFk)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Data.Contains(input.Filter))
//                        .WhereIf(input.StatusFilter.HasValue && input.StatusFilter > -1, e => e.Status == statusFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DataFilter), e => e.Data == input.DataFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.RecordMatterRecordMatterNameFilter), e => e.RecordMatterFk != null && e.RecordMatterFk.RecordMatterName == input.RecordMatterRecordMatterNameFilter);

//            var pagedAndFilteredRecordMatterAudits = filteredRecordMatterAudits
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var recordMatterAudits = from o in pagedAndFilteredRecordMatterAudits
//                                     join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
//                                     from s1 in j1.DefaultIfEmpty()

//                                     join o2 in _lookup_recordMatterRepository.GetAll() on o.RecordMatterId equals o2.Id into j2
//                                     from s2 in j2.DefaultIfEmpty()

//                                     select new GetRecordMatterAuditForViewDto()
//                                     {
//                                         RecordMatterAudit = new RecordMatterAuditDto
//                                         {
//                                             Status = o.Status,
//                                             Data = o.Data,
//                                             Id = o.Id
//                                         },
//                                         UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
//                                         RecordMatterRecordMatterName = s2 == null || s2.RecordMatterName == null ? "" : s2.RecordMatterName.ToString()
//                                     };

//            var totalCount = await filteredRecordMatterAudits.CountAsync();

//            return new PagedResultDto<GetRecordMatterAuditForViewDto>(
//                totalCount,
//                await recordMatterAudits.ToListAsync()
//            );
//        }

//        public async Task<GetRecordMatterAuditForViewDto> GetRecordMatterAuditForView(Guid id)
//        {
//            var recordMatterAudit = await _recordMatterAuditRepository.GetAsync(id);

//            var output = new GetRecordMatterAuditForViewDto { RecordMatterAudit = ObjectMapper.Map<RecordMatterAuditDto>(recordMatterAudit) };

//            if (output.RecordMatterAudit.UserId != null)
//            {
//                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.RecordMatterAudit.UserId);
//                output.UserName = _lookupUser?.Name?.ToString();
//            }

//            if (output.RecordMatterAudit.RecordMatterId != null)
//            {
//                var _lookupRecordMatter = await _lookup_recordMatterRepository.FirstOrDefaultAsync((Guid)output.RecordMatterAudit.RecordMatterId);
//                output.RecordMatterRecordMatterName = _lookupRecordMatter?.RecordMatterName?.ToString();
//            }

//            return output;
//        }

//        [AbpAuthorize(AppPermissions.Pages_RecordMatterAudits_Edit)]
//        public async Task<GetRecordMatterAuditForEditOutput> GetRecordMatterAuditForEdit(EntityDto<Guid> input)
//        {
//            var recordMatterAudit = await _recordMatterAuditRepository.FirstOrDefaultAsync(input.Id);

//            var output = new GetRecordMatterAuditForEditOutput { RecordMatterAudit = ObjectMapper.Map<CreateOrEditRecordMatterAuditDto>(recordMatterAudit) };

//            if (output.RecordMatterAudit.UserId != null)
//            {
//                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.RecordMatterAudit.UserId);
//                output.UserName = _lookupUser?.Name?.ToString();
//            }

//            if (output.RecordMatterAudit.RecordMatterId != null)
//            {
//                var _lookupRecordMatter = await _lookup_recordMatterRepository.FirstOrDefaultAsync((Guid)output.RecordMatterAudit.RecordMatterId);
//                output.RecordMatterRecordMatterName = _lookupRecordMatter?.RecordMatterName?.ToString();
//            }

//            return output;
//        }

//        public async Task CreateOrEdit(CreateOrEditRecordMatterAuditDto input)
//        {
//            if (input.Id == null)
//            {
//                await Create(input);
//            }
//            else
//            {
//                await Update(input);
//            }
//        }

//        [AbpAuthorize(AppPermissions.Pages_RecordMatterAudits_Create)]
//        protected virtual async Task Create(CreateOrEditRecordMatterAuditDto input)
//        {
//            var recordMatterAudit = ObjectMapper.Map<RecordMatterAudit>(input);

//            if (AbpSession.TenantId != null)
//            {
//                recordMatterAudit.TenantId = (int?)AbpSession.TenantId;
//            }

//            await _recordMatterAuditRepository.InsertAsync(recordMatterAudit);
//        }

//        [AbpAuthorize(AppPermissions.Pages_RecordMatterAudits_Edit)]
//        protected virtual async Task Update(CreateOrEditRecordMatterAuditDto input)
//        {
//            var recordMatterAudit = await _recordMatterAuditRepository.FirstOrDefaultAsync((Guid)input.Id);
//            ObjectMapper.Map(input, recordMatterAudit);
//        }

//        [AbpAuthorize(AppPermissions.Pages_RecordMatterAudits_Delete)]
//        public async Task Delete(EntityDto<Guid> input)
//        {
//            await _recordMatterAuditRepository.DeleteAsync(input.Id);
//        }

//        public async Task<FileDto> GetRecordMatterAuditsToExcel(GetAllRecordMatterAuditsForExcelInput input)
//        {
//            var statusFilter = input.StatusFilter.HasValue
//                        ? (RecordMatterStatus)input.StatusFilter
//                        : default;

//            var filteredRecordMatterAudits = _recordMatterAuditRepository.GetAll()
//                        .Include(e => e.UserFk)
//                        .Include(e => e.RecordMatterFk)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Data.Contains(input.Filter))
//                        .WhereIf(input.StatusFilter.HasValue && input.StatusFilter > -1, e => e.Status == statusFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DataFilter), e => e.Data == input.DataFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.RecordMatterRecordMatterNameFilter), e => e.RecordMatterFk != null && e.RecordMatterFk.RecordMatterName == input.RecordMatterRecordMatterNameFilter);

//            var query = (from o in filteredRecordMatterAudits
//                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
//                         from s1 in j1.DefaultIfEmpty()

//                         join o2 in _lookup_recordMatterRepository.GetAll() on o.RecordMatterId equals o2.Id into j2
//                         from s2 in j2.DefaultIfEmpty()

//                         select new GetRecordMatterAuditForViewDto()
//                         {
//                             RecordMatterAudit = new RecordMatterAuditDto
//                             {
//                                 Status = o.Status,
//                                 Data = o.Data,
//                                 Id = o.Id
//                             },
//                             UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
//                             RecordMatterRecordMatterName = s2 == null || s2.RecordMatterName == null ? "" : s2.RecordMatterName.ToString()
//                         });

//            var recordMatterAuditListDtos = await query.ToListAsync();

//            return _recordMatterAuditsExcelExporter.ExportToFile(recordMatterAuditListDtos);
//        }

//        [AbpAuthorize(AppPermissions.Pages_RecordMatterAudits)]
//        public async Task<PagedResultDto<RecordMatterAuditUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
//        {
//            var query = _lookup_userRepository.GetAll().WhereIf(
//                   !string.IsNullOrWhiteSpace(input.Filter),
//                  e => e.Name != null && e.Name.Contains(input.Filter)
//               );

//            var totalCount = await query.CountAsync();

//            var userList = await query
//                .PageBy(input)
//                .ToListAsync();

//            var lookupTableDtoList = new List<RecordMatterAuditUserLookupTableDto>();
//            foreach (var user in userList)
//            {
//                lookupTableDtoList.Add(new RecordMatterAuditUserLookupTableDto
//                {
//                    Id = user.Id,
//                    DisplayName = user.Name?.ToString()
//                });
//            }

//            return new PagedResultDto<RecordMatterAuditUserLookupTableDto>(
//                totalCount,
//                lookupTableDtoList
//            );
//        }

//        [AbpAuthorize(AppPermissions.Pages_RecordMatterAudits)]
//        public async Task<PagedResultDto<RecordMatterAuditRecordMatterLookupTableDto>> GetAllRecordMatterForLookupTable(GetAllForLookupTableInput input)
//        {
//            var query = _lookup_recordMatterRepository.GetAll().WhereIf(
//                   !string.IsNullOrWhiteSpace(input.Filter),
//                  e => e.RecordMatterName != null && e.RecordMatterName.Contains(input.Filter)
//               );

//            var totalCount = await query.CountAsync();

//            var recordMatterList = await query
//                .PageBy(input)
//                .ToListAsync();

//            var lookupTableDtoList = new List<RecordMatterAuditRecordMatterLookupTableDto>();
//            foreach (var recordMatter in recordMatterList)
//            {
//                lookupTableDtoList.Add(new RecordMatterAuditRecordMatterLookupTableDto
//                {
//                    Id = recordMatter.Id.ToString(),
//                    DisplayName = recordMatter.RecordMatterName?.ToString()
//                });
//            }

//            return new PagedResultDto<RecordMatterAuditRecordMatterLookupTableDto>(
//                totalCount,
//                lookupTableDtoList
//            );
//        }
//    }
//}