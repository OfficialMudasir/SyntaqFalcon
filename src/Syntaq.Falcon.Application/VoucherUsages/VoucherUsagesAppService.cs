using Syntaq.Falcon.Authorization.Users;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.VoucherUsages.Exporting;
using Syntaq.Falcon.VoucherUsages.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Syntaq.Falcon.VoucherUsages
{
#if STQ_PRODUCTION

	[AbpAuthorize(AppPermissions.Pages_VoucherUsages)]
	public class VoucherUsagesAppService : FalconAppServiceBase, IVoucherUsagesAppService
	{
		 private readonly IRepository<VoucherUsage, Guid> _voucherUsageRepository;
		 private readonly IVoucherUsagesExcelExporter _voucherUsagesExcelExporter;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 

		  public VoucherUsagesAppService(IRepository<VoucherUsage, Guid> voucherUsageRepository, IVoucherUsagesExcelExporter voucherUsagesExcelExporter , IRepository<User, long> lookup_userRepository) 
		  {
			_voucherUsageRepository = voucherUsageRepository;
			_voucherUsagesExcelExporter = voucherUsagesExcelExporter;
			_lookup_userRepository = lookup_userRepository;
		
		  }

		 public async Task<PagedResultDto<GetVoucherUsageForViewDto>> GetAll(GetAllVoucherUsagesInput input)
		 {
			
			var filteredVoucherUsages = _voucherUsageRepository.GetAll()
						.Include( e => e.UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.EntityKey.Contains(input.Filter) || e.EntityType.Contains(input.Filter))
						.WhereIf(input.MinDateRedeemedFilter != null, e => e.DateRedeemed >= input.MinDateRedeemedFilter)
						.WhereIf(input.MaxDateRedeemedFilter != null, e => e.DateRedeemed <= input.MaxDateRedeemedFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EntityKeyFilter),  e => e.EntityKey.ToLower() == input.EntityKeyFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EntityTypeFilter),  e => e.EntityType.ToLower() == input.EntityTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim());

			var pagedAndFilteredVoucherUsages = filteredVoucherUsages
				.OrderBy(input.Sorting ?? "id asc")
				.PageBy(input);

			var voucherUsages = from o in pagedAndFilteredVoucherUsages
						 join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
						 from s1 in j1.DefaultIfEmpty()
						 
						 select new GetVoucherUsageForViewDto() {
							VoucherUsage = new VoucherUsageDto
							{
								DateRedeemed = o.DateRedeemed,
								EntityKey = o.EntityKey,
								EntityType = o.EntityType,
								Id = o.Id
							},
							UserName = s1 == null ? "" : s1.Name.ToString()
						};

			var totalCount = await filteredVoucherUsages.CountAsync();

			return new PagedResultDto<GetVoucherUsageForViewDto>(
				totalCount,
				await voucherUsages.ToListAsync()
			);
		 }
		 
		 public async Task<GetVoucherUsageForViewDto> GetVoucherUsageForView(Guid id)
		 {
			var voucherUsage = await _voucherUsageRepository.GetAsync(id);

			var output = new GetVoucherUsageForViewDto { VoucherUsage = ObjectMapper.Map<VoucherUsageDto>(voucherUsage) };

			if (output.VoucherUsage.UserId != null)
			{
				var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.VoucherUsage.UserId);
				output.UserName = _lookupUser.Name.ToString();
			}
			
			return output;
		 }
		 
		 [AbpAuthorize(AppPermissions.Pages_VoucherUsages_Edit)]
		 public async Task<GetVoucherUsageForEditOutput> GetVoucherUsageForEdit(EntityDto<Guid> input)
		 {
			var voucherUsage = await _voucherUsageRepository.FirstOrDefaultAsync(input.Id);
		   
			var output = new GetVoucherUsageForEditOutput {VoucherUsage = ObjectMapper.Map<CreateOrEditVoucherUsageDto>(voucherUsage)};

			//if (output.VoucherUsage.UserId != null)
	  //      {
	  //          var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.VoucherUsage.UserId);
	  //          output.UserName = _lookupUser.Name.ToString();
	  //      }
			var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)AbpSession.UserId);
			output.UserName = _lookupUser.Name.ToString();

			return output;
		 }

		 public async Task CreateOrEdit(CreateOrEditVoucherUsageDto input)
		 {
			if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
		 }

		 [AbpAuthorize(AppPermissions.Pages_VoucherUsages_Create)]
		 private async Task Create(CreateOrEditVoucherUsageDto input)
		 {
			var voucherUsage = ObjectMapper.Map<VoucherUsage>(input);

			
			if (AbpSession.TenantId != null)
			{
				voucherUsage.TenantId = (int?) AbpSession.TenantId;
			}
		

			await _voucherUsageRepository.InsertAsync(voucherUsage);
		 }

		 [AbpAuthorize(AppPermissions.Pages_VoucherUsages_Edit)]
		 private async Task Update(CreateOrEditVoucherUsageDto input)
		 {
			var voucherUsage = await _voucherUsageRepository.FirstOrDefaultAsync((Guid)input.Id);
			 ObjectMapper.Map(input, voucherUsage);
		 }

		 [AbpAuthorize(AppPermissions.Pages_VoucherUsages_Delete)]
		 public async Task Delete(EntityDto<Guid> input)
		 {
			await _voucherUsageRepository.DeleteAsync(input.Id);
		 } 

		public async Task<FileDto> GetVoucherUsagesToExcel(GetAllVoucherUsagesForExcelInput input)
		 {
			
			var filteredVoucherUsages = _voucherUsageRepository.GetAll()
						.Include( e => e.UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.EntityKey.Contains(input.Filter) || e.EntityType.Contains(input.Filter))
						.WhereIf(input.MinDateRedeemedFilter != null, e => e.DateRedeemed >= input.MinDateRedeemedFilter)
						.WhereIf(input.MaxDateRedeemedFilter != null, e => e.DateRedeemed <= input.MaxDateRedeemedFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EntityKeyFilter),  e => e.EntityKey.ToLower() == input.EntityKeyFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EntityTypeFilter),  e => e.EntityType.ToLower() == input.EntityTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim());

			var query = (from o in filteredVoucherUsages
						 join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
						 from s1 in j1.DefaultIfEmpty()
						 
						 select new GetVoucherUsageForViewDto() { 
							VoucherUsage = new VoucherUsageDto
							{
								DateRedeemed = o.DateRedeemed,
								EntityKey = o.EntityKey,
								EntityType = o.EntityType,
								Id = o.Id
							},
							UserName = s1 == null ? "" : s1.Name.ToString()
						 });


			var voucherUsageListDtos = await query.ToListAsync();

			return _voucherUsagesExcelExporter.ExportToFile(voucherUsageListDtos);
		 }



		[AbpAuthorize(AppPermissions.Pages_VoucherUsages)]
		 public async Task<PagedResultDto<VoucherUsageUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
		 {
			 var query = _lookup_userRepository.GetAll().WhereIf(
					!string.IsNullOrWhiteSpace(input.Filter),
				   e=> e.Name.ToString().Contains(input.Filter)
				);

			var totalCount = await query.CountAsync();

			var userList = await query
				.PageBy(input)
				.ToListAsync();

			var lookupTableDtoList = new List<VoucherUsageUserLookupTableDto>();
			foreach(var user in userList){
				lookupTableDtoList.Add(new VoucherUsageUserLookupTableDto
				{
					Id = user.Id,
					DisplayName = user.Name?.ToString()
				});
			}

			return new PagedResultDto<VoucherUsageUserLookupTableDto>(
				totalCount,
				lookupTableDtoList
			);
		 }
	}

#endif

}