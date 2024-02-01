using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.VoucherEntitites;
using Syntaq.Falcon.VoucherEntitites.Dtos;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.Vouchers.Exporting;
using Syntaq.Falcon.VoucherUsages;
using Syntaq.Falcon.VoucherUsages.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using GetAllForLookupTableInput = Syntaq.Falcon.Vouchers.Dtos.GetAllForLookupTableInput;

namespace Syntaq.Falcon.Vouchers
{


    public class VouchersAppService : FalconAppServiceBase, IVouchersAppService
    {
        private readonly IRepository<Voucher, Guid> _voucherRepository;
        private readonly IRepository<VoucherEntity, Guid> _voucherEntityRepository;
        private readonly IRepository<VoucherUsage, Guid> _voucherUsageRepository;
        private readonly IVouchersExcelExporter _vouchersExcelExporter;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Form, Guid> _formRepository;

        public VouchersAppService(IRepository<Voucher,
                                    Guid> voucherRepository,
                                    IRepository<VoucherEntity, Guid> voucherEntityRepository,
                                    IRepository<VoucherUsage, Guid> voucherUsageRepository,
                                    IVouchersExcelExporter vouchersExcelExporter,
                                    IUnitOfWorkManager unitOfWorkManager,
                                    IRepository<Form, Guid> formRepository)
        {
            _voucherRepository = voucherRepository;
            _voucherEntityRepository = voucherEntityRepository;
            _voucherUsageRepository = voucherUsageRepository;
            _vouchersExcelExporter = vouchersExcelExporter;
            _unitOfWorkManager = unitOfWorkManager;
            _formRepository = formRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Vouchers)]
        public async Task<PagedResultDto<GetVoucherForViewDto>> GetAll(GetAllVouchersInput input)
        {
            var filteredVouchers = _voucherRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Key.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.DiscountType.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.KeyFilter), e => e.Key.ToLower().Contains( input.KeyFilter.ToLower().Trim()))
                        .WhereIf(input.MinExpiryFilter != null, e => e.Expiry >= input.MinExpiryFilter)
                        .WhereIf(input.MaxExpiryFilter != null, e => e.Expiry <= input.MaxExpiryFilter)
                        .WhereIf(input.MinNoOfUsesFilter != null, e => e.NoOfUses >= input.MinNoOfUsesFilter)
                        .WhereIf(input.MaxNoOfUsesFilter != null, e => e.NoOfUses <= input.MaxNoOfUsesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DiscountTypeFilter), e => e.DiscountType.ToLower() == input.DiscountTypeFilter.ToLower().Trim());

            var pagedAndFilteredVouchers = filteredVouchers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            try
            {
                if (input.KeyFilter != null)
                {

                    var ddd_ = pagedAndFilteredVouchers.Where(d => d.Expiry > Convert.ToDateTime(input.MinExpiryFilter)
                    && d.Expiry < Convert.ToDateTime(input.MaxExpiryFilter)).ToList();

                    var ddd = pagedAndFilteredVouchers.Where(d => d.Expiry > Convert.ToDateTime(input.KeyFilter).AddDays(-1)
                    && d.Expiry < Convert.ToDateTime(input.KeyFilter).AddDays(1)).ToList();
                }
            }
            catch (Exception ex)
            {

            }

            var vouchers = from o in pagedAndFilteredVouchers
                           select new GetVoucherForViewDto()
                           {
                               Voucher = new VoucherDto
                               {
                                   Key = o.Key,
                                   Value = o.Value,
                                   Expiry = o.Expiry,
                                   NoOfUses = o.NoOfUses,
                                   Description = o.Description,
                                   DiscountType = o.DiscountType,
                                   Id = o.Id
                               }
                           };

            var totalCount = await filteredVouchers.CountAsync();

            return new PagedResultDto<GetVoucherForViewDto>(
                totalCount,
                await vouchers.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Vouchers)]
        public async Task<GetVoucherForViewDto> GetVoucherForView(Guid id)
        {
            var voucher = await _voucherRepository.GetAsync(id);

            var output = new GetVoucherForViewDto { Voucher = ObjectMapper.Map<VoucherDto>(voucher) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Vouchers_Edit)]
        public async Task<GetVoucherForEditOutput> GetVoucherForEdit(EntityDto<Guid> input)
        {
            //get the voucher from the DB that has the passed id from the input
            Voucher voucher = await _voucherRepository.FirstOrDefaultAsync(input.Id);

            //get all voucher entities from the DB that match the id, 
            // and map them into the VoucherEntities collection property of the voucher

            //problem here
            voucher.VoucherEntities = ObjectMapper.Map<List<CreateOrEditVoucherEntityDto>>(_voucherEntityRepository.GetAll().Where(i => i.VoucherId == voucher.Id));

            //if the collection is NOT empty...
            if (voucher.VoucherEntities != null)
            {
                // go through each object in collection and get the entity from the DB, 
                // put the name of the entity into the voucherEntity
                foreach (CreateOrEditVoucherEntityDto vEntity in voucher.VoucherEntities)
                {
                    var entity = await _formRepository.FirstOrDefaultAsync(vEntity.EntityKey);
                    vEntity.EntityName = entity.Name;
                }
            }

            GetVoucherForEditOutput output = new GetVoucherForEditOutput
            {
                Voucher = ObjectMapper.Map<CreateOrEditVoucherDto>(voucher)
            };
            // send the voucher data (including the voucher entities) to the view
            return output;
        }

        public async Task CreateOrEdit(CreateOrEditVoucherDto input)
        {
            var v = await _voucherRepository.FirstOrDefaultAsync((Guid)input.Id);
            // use repository objects ( get object from DB )
            // check DB for match on ID? if no match, 
            // id doesn't exist, so do create
            if (v == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Vouchers_Create)]
        private async Task Create(CreateOrEditVoucherDto input)
        {

            var voucher = ObjectMapper.Map<Voucher>(input);
            var voucherEntitys = ObjectMapper.Map<List<VoucherEntity>>(input.VoucherEntities);

            if (AbpSession.TenantId != null)
            {
                voucher.TenantId = (int?)AbpSession.TenantId;
            }

            foreach (VoucherEntity vEntity in voucherEntitys)
            {
                vEntity.TenantId = voucher.TenantId;
                await _voucherEntityRepository.InsertAsync(vEntity);
            }
            await _voucherRepository.InsertAsync(voucher);
        }

        [AbpAuthorize(AppPermissions.Pages_Vouchers_Edit)]
        private async Task Update(CreateOrEditVoucherDto input)
        {
            //DB LIST
            var dbVoucher = await _voucherRepository.FirstOrDefaultAsync((Guid)input.Id);

            // Don't forget to update the voucher itself.....
            ObjectMapper.Map(input, dbVoucher);

            var dbVoucherEntities = ObjectMapper.Map<List<VoucherEntity>>(_voucherEntityRepository.GetAll().Where(i => i.VoucherId == dbVoucher.Id));
            //UPDATE LIST
            List<VoucherEntity> vEntities = ObjectMapper.Map<List<VoucherEntity>>(input.VoucherEntities);

            // FIRST check if db list is empty
            if (dbVoucherEntities.Count == 0)
            {
                //if empty, insert the new update list into DB!!
                var voucher = ObjectMapper.Map<Voucher>(input);
                foreach (VoucherEntity vE in vEntities)
                {
                    vE.VoucherId = (Guid)input.Id;
                    vE.TenantId = voucher.TenantId;
                    await _voucherEntityRepository.InsertAsync(vE);
                }
            }
            else //if not empty...
            {
                //create a safe bubble to work in...
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    // check each object in update list for 'blank' id's
                    vEntities.ForEach(async i =>
                    {   // if blank, insert into DB (new guid will be given when inserted)
                        if (i.Id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                        {
                            //i.Id = Guid.NewGuid();
                            await _voucherEntityRepository.InsertOrUpdateAsync(i);
                        }
                    });

                    dbVoucherEntities.ForEach(async j =>
                    {   //then check through each object in DB list against the update list...
                        if (vEntities.Any(i => i.Id == j.Id))
                        {   // if the DB object id matches with any in the update list, update it.
                            await _voucherEntityRepository.UpdateAsync(j);
                        }
                        else
                        {   //otherwise, delete it because it doesnt match
                            await _voucherEntityRepository.DeleteAsync(j.Id);
                        }
                    });
                    unitOfWork.Complete();
                }  // now map the input objects to the DB objects (implicit update)
                ObjectMapper.Map(input, dbVoucher);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Vouchers_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _voucherRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetVouchersToExcel(GetAllVouchersForExcelInput input)
        {

            var filteredVouchers = _voucherRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Key.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.DiscountType.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.KeyFilter), e => e.Key.ToLower() == input.KeyFilter.ToLower().Trim())
                        .WhereIf(input.MinExpiryFilter != null, e => e.Expiry >= input.MinExpiryFilter)
                        .WhereIf(input.MaxExpiryFilter != null, e => e.Expiry <= input.MaxExpiryFilter)
                        .WhereIf(input.MinNoOfUsesFilter != null, e => e.NoOfUses >= input.MinNoOfUsesFilter)
                        .WhereIf(input.MaxNoOfUsesFilter != null, e => e.NoOfUses <= input.MaxNoOfUsesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DiscountTypeFilter), e => e.DiscountType.ToLower() == input.DiscountTypeFilter.ToLower().Trim());

            var query = (from o in filteredVouchers
                         select new GetVoucherForViewDto()
                         {
                             Voucher = new VoucherDto
                             {
                                 Key = o.Key,
                                 Value = o.Value,
                                 Expiry = o.Expiry,
                                 NoOfUses = o.NoOfUses,
                                 Description = o.Description,
                                 DiscountType = o.DiscountType,
                                 Id = o.Id
                             }
                         });


            var voucherListDtos = await query.ToListAsync();

            return _vouchersExcelExporter.ExportToFile(voucherListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Vouchers)]
        public async Task<PagedResultDto<EntityLookupTableDto>> GetAllEntitysForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _formRepository.GetAll()
                .Where(e => e.CreatorUserId == AbpSession.UserId)
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  f => f.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var entityList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<EntityLookupTableDto>();

            foreach (var entity in entityList)
            {
                lookupTableDtoList.Add(new EntityLookupTableDto
                {
                    Id = entity.Id,
                    DisplayName = entity.Name.ToString()
                });
            }

            return new PagedResultDto<EntityLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Vouchers)]
        public async Task<PagedResultDto<VoucherUsageLookupDto>> GetVoucherUsagesForLookupTable(GetAllForLookupTableInput input)
        {
            var voucherUsages = _voucherUsageRepository.GetAll()
                .Include(i => i.UserFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), i => i.VoucherFk.Id.ToString().Contains(input.Filter));

            var totalCount = await voucherUsages.CountAsync();

            var vUsageList = await voucherUsages
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<VoucherUsageLookupDto>();

            foreach (var vUsage in vUsageList)
            {
                var entity = _formRepository.FirstOrDefault(Guid.Parse(vUsage.EntityKey));

                lookupTableDtoList.Add(new VoucherUsageLookupDto
                {
                    Id = vUsage.Id,
                    EntityId = Guid.Parse(vUsage.EntityKey),
                    UserID = vUsage.UserId,
                    DateRedeemed = vUsage.DateRedeemed,
                    EntityName = entity.Name,
                    EntityType = vUsage.EntityType,
                    UserName = vUsage.UserFk.UserName,          // USerFK returned null for now
                    UserEmail = vUsage.UserFk.EmailAddress
                });
            }
            return new PagedResultDto<VoucherUsageLookupDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        //GetVoucherIdByKey ???
        // Needs to Anon
        //[AbpAuthorize(AppPermissions.Pages_Vouchers)]
        public async Task<VoucherValidityDto> GetVoucherDetailsByKey(GetVoucherDetailsByKeyInput input)
        {   //checking voucher validity for usage
            // this is only checking form entities currently, but will adapt to include switch case for document entities

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                // Encoding fix
                input.VoucherKey = input.VoucherKey.Replace(" ", "+");

                Voucher voucher = await _voucherRepository.GetAll().FirstOrDefaultAsync(i => i.Key == input.VoucherKey);
                if (voucher != null)
                {
                    //check balance is not lower than voucher value!!

                    //var balance = Convert.ToDecimal (input.Balance);

                    //if (balance >= voucher.Value)
                    //{
                    if (voucher.Expiry > DateTime.Now)
                    { // voucher has not expired, valid
                        if (voucher.NoOfUses == null) //unlimited usage avail
                        {
                            return CheckVoucherValidityOnEntities((Guid)input.EntityId, voucher);
                        }
                        else /*if (voucher.NoOfUses != null)*/
                        {  //return count of vUsages with that VoucherId
                            var vUsages = _voucherUsageRepository.GetAll().Where(i => i.VoucherFk.Id == voucher.Id);
                            var usageCount = await vUsages.CountAsync();

                            if (usageCount < voucher.NoOfUses)
                            {
                                input.EntityId = input.EntityId.HasValue ? input.EntityId : Guid.Empty;
                                return CheckVoucherValidityOnEntities((Guid)input.EntityId, voucher);
                            }
                            else /*if (usageCount >= voucher.NoOfUses)*/
                            { //usage limit has already been reached 
                                var message = "The voucher is not valid for use; no usage available";
                                return InvalidVoucherResponse(message);
                            };
                        };
                    }
                    else
                    {
                        var message = "Expired voucher";
                        return InvalidVoucherResponse(message);
                    };
                    //}
                    //else
                    //{
                    //    var message = "Voucher value cannot be greater than the payment value";
                    //    return InvalidVoucherResponse(message);
                    //}
                }
                else
                {
                    var message = "There is no voucher for that key";
                    return InvalidVoucherResponse(message);
                };
            }


        }

        private VoucherValidityDto CheckVoucherValidityOnEntities(Guid? entityId, Voucher voucher)
        {

            //var vEntityList = voucher.VoucherEntities;
            //if (_voucherEntityRepository.GetAll().Any(v => v.VoucherId == voucher.Id && (v.EntityKey == entityId || ! voucher.VoucherEntities.Any() )))
            //{
            //        var message = "Success. Voucher has been applied";
            //        return ValidVoucherResponse(voucher, message);
            //}
            //else
            //{
            //    var message = "Invalid, for this entity";
            //    return InvalidVoucherResponse(message);
            //}

            var entityKeys = _voucherEntityRepository.GetAll().Where(v => v.VoucherId == voucher.Id).Select(v => v.EntityKey);

            if (entityKeys.Any())
            {
                if (!entityKeys.Any(e => e == entityId))
                {
                    var message = "Invalid, for this entity";
                    return InvalidVoucherResponse(message);
                }
            }

            return ValidVoucherResponse(voucher, "Success. Voucher has been applied");
        }

        private static VoucherValidityDto InvalidVoucherResponse(string message)
        {
            return new VoucherValidityDto()
            {
                VoucherId = null,
                VoucherValid = false,
                ValidityMessage = message,
                NoOfUses = null,
                VoucherValue = null,
                DiscountType = ""
            };
        }

        private static VoucherValidityDto ValidVoucherResponse(Voucher voucher, string message)
        {
            return new VoucherValidityDto()
            {
                VoucherId = voucher.Id,
                VoucherValid = true,
                ValidityMessage = message,
                NoOfUses = voucher.NoOfUses,
                VoucherValue = voucher.Value,
                DiscountType = voucher.DiscountType
            };
        }

        public Task<bool> CheckVoucherKeyNotExisting(string vKey)
        {
            var v = _voucherRepository.GetAll().Where(i => i.Key == vKey);
            if (v.Count() != 0)
            {
                return Task.FromResult(false);
            }
            else
            {
                return Task.FromResult(true);
            }
        }

        public async Task InsertVoucherUsage(CreateOrEditVoucherUsageDto input)
        {
            //input.VoucherId
            var voucherUsage = ObjectMapper.Map<VoucherUsage>(input);
            voucherUsage.DateRedeemed = DateTime.Now;
            //voucherUsage.VoucherId = 

            if (AbpSession.TenantId != null)
            {
                voucherUsage.TenantId = AbpSession.TenantId;
            }

            if (AbpSession.UserId != null)
            {
                voucherUsage.UserId = (long)AbpSession.UserId;
            }

            await _voucherUsageRepository.InsertAsync(voucherUsage);
        }
    }


}