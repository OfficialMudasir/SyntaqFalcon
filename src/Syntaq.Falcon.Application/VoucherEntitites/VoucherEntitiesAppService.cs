using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.VoucherEntitites.Dtos;
using Syntaq.Falcon.VoucherEntitites.Exporting;
using Syntaq.Falcon.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.VoucherEntitites
{

#if STQ_PRODUCTION

    [AbpAuthorize(AppPermissions.Pages_VoucherEntities)]
    public class VoucherEntitiesAppService : FalconAppServiceBase, IVoucherEntitiesAppService
    {
        private readonly IRepository<VoucherEntity, Guid> _voucherEntityRepository;
        private readonly IVoucherEntitiesExcelExporter _voucherEntitiesExcelExporter;
        private readonly IRepository<Voucher, Guid> _lookup_voucherRepository;


        public VoucherEntitiesAppService(IRepository<VoucherEntity, Guid> voucherEntityRepository, IVoucherEntitiesExcelExporter voucherEntitiesExcelExporter, IRepository<Voucher, Guid> lookup_voucherRepository)
        {
            _voucherEntityRepository = voucherEntityRepository;
            _voucherEntitiesExcelExporter = voucherEntitiesExcelExporter;
            _lookup_voucherRepository = lookup_voucherRepository;

        }

        public async Task<PagedResultDto<GetVoucherEntityForViewDto>> GetAll(GetAllVoucherEntitiesInput input)
        {

            var filteredVoucherEntities = _voucherEntityRepository.GetAll()
                        .Include(e => e.VoucherFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.EntityType.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EntityKeyFilter.ToString()), e => e.EntityKey.ToString() == input.EntityKeyFilter.ToString().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EntityTypeFilter), e => e.EntityType.ToLower() == input.EntityTypeFilter.ToLower().Trim());
            //.WhereIf(!string.IsNullOrWhiteSpace(input.VoucherTenantIdFilter), e => e.VoucherFk != null && e.VoucherFk.TenantId.ToLower() == input.VoucherTenantIdFilter.ToLower().Trim());

            var pagedAndFilteredVoucherEntities = filteredVoucherEntities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var voucherEntities = from o in pagedAndFilteredVoucherEntities
                                  join o1 in _lookup_voucherRepository.GetAll() on o.VoucherId equals o1.Id into j1
                                  from s1 in j1.DefaultIfEmpty()

                                  select new GetVoucherEntityForViewDto()
                                  {
                                      VoucherEntity = new VoucherEntityDto
                                      {
                                          EntityKey = o.EntityKey,
                                          EntityType = o.EntityType,
                                          Id = o.Id
                                      },
                                      VoucherTenantId = s1 == null ? "" : s1.TenantId.ToString()
                                  };

            var totalCount = await filteredVoucherEntities.CountAsync();

            return new PagedResultDto<GetVoucherEntityForViewDto>(
                totalCount,
                await voucherEntities.ToListAsync()
            );
        }

        public async Task<GetVoucherEntityForViewDto> GetVoucherEntityForView(Guid id)
        {
            var voucherEntity = await _voucherEntityRepository.GetAsync(id);

            var output = new GetVoucherEntityForViewDto { VoucherEntity = ObjectMapper.Map<VoucherEntityDto>(voucherEntity) };

            if (output.VoucherEntity.VoucherId != null)
            {
                var _lookupVoucher = await _lookup_voucherRepository.FirstOrDefaultAsync((Guid)output.VoucherEntity.VoucherId);
                output.VoucherTenantId = _lookupVoucher.TenantId.ToString();
            }

            return output;
        }

        // Get Voucher Entity  ByFormId
        public async Task<GetVoucherEntityForViewDto> GetVoucherEntityByFormId(Guid FormId)
        {
            var voucherEntity = _voucherEntityRepository.GetAll().FirstOrDefault(x => x.EntityKey == FormId);
            return new GetVoucherEntityForViewDto { VoucherEntity = ObjectMapper.Map<VoucherEntityDto>(voucherEntity) };
        }

        [AbpAuthorize(AppPermissions.Pages_VoucherEntities_Edit)]
        public async Task<GetVoucherEntityForEditOutput> GetVoucherEntityForEdit(EntityDto<Guid> input)
        {
            var voucherEntity = await _voucherEntityRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetVoucherEntityForEditOutput { VoucherEntity = ObjectMapper.Map<CreateOrEditVoucherEntityDto>(voucherEntity) };

            if (output.VoucherEntity.VoucherId != null)
            {
                var _lookupVoucher = await _lookup_voucherRepository.FirstOrDefaultAsync((Guid)output.VoucherEntity.VoucherId);
                output.VoucherTenantId = _lookupVoucher.TenantId.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditVoucherEntityDto input)
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

        [AbpAuthorize(AppPermissions.Pages_VoucherEntities_Create)]
        private async Task Create(CreateOrEditVoucherEntityDto input) //
        {
            var voucherEntity = ObjectMapper.Map<VoucherEntity>(input);


            if (AbpSession.TenantId != null)
            {
                voucherEntity.TenantId = (int?)AbpSession.TenantId;
            }


            await _voucherEntityRepository.InsertAsync(voucherEntity);
        }

        [AbpAuthorize(AppPermissions.Pages_VoucherEntities_Edit)]
        private async Task Update(CreateOrEditVoucherEntityDto input)
        {
            var voucherEntity = await _voucherEntityRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, voucherEntity);
        }

        [AbpAuthorize(AppPermissions.Pages_VoucherEntities_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _voucherEntityRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetVoucherEntitiesToExcel(GetAllVoucherEntitiesForExcelInput input)
        {

            var filteredVoucherEntities = _voucherEntityRepository.GetAll()
                        .Include(e => e.VoucherFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.EntityType.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EntityKeyFilter.ToString()), e => e.EntityKey.ToString() == input.EntityKeyFilter.ToString().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EntityTypeFilter), e => e.EntityType.ToLower() == input.EntityTypeFilter.ToLower().Trim());
            //.WhereIf(!string.IsNullOrWhiteSpace(input.VoucherTenantIdFilter), e => e.VoucherFk != null && e.VoucherFk.TenantId == input.VoucherTenantIdFilter);

            var query = (from o in filteredVoucherEntities
                         join o1 in _lookup_voucherRepository.GetAll() on o.VoucherId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetVoucherEntityForViewDto()
                         {
                             VoucherEntity = new VoucherEntityDto
                             {
                                 EntityKey = o.EntityKey,
                                 EntityType = o.EntityType,
                                 Id = o.Id
                             },
                             VoucherTenantId = s1 == null ? "" : s1.TenantId.ToString()
                         });


            var voucherEntityListDtos = await query.ToListAsync();

            return _voucherEntitiesExcelExporter.ExportToFile(voucherEntityListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_VoucherEntities)]
        public async Task<PagedResultDto<VoucherEntityVoucherLookupTableDto>> GetAllVoucherForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_voucherRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.TenantId.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var voucherList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<VoucherEntityVoucherLookupTableDto>();
            foreach (var voucher in voucherList)
            {
                lookupTableDtoList.Add(new VoucherEntityVoucherLookupTableDto
                {
                    Id = voucher.Id.ToString(),
                    DisplayName = voucher.TenantId?.ToString()
                });
            }

            return new PagedResultDto<VoucherEntityVoucherLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }

#endif

}