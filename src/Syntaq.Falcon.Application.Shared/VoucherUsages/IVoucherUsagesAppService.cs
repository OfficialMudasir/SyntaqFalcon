using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.VoucherUsages.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.VoucherUsages
{
    public interface IVoucherUsagesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetVoucherUsageForViewDto>> GetAll(GetAllVoucherUsagesInput input);

        Task<GetVoucherUsageForViewDto> GetVoucherUsageForView(Guid id);

		Task<GetVoucherUsageForEditOutput> GetVoucherUsageForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditVoucherUsageDto input);

		Task Delete(EntityDto<Guid> input);

		Task<FileDto> GetVoucherUsagesToExcel(GetAllVoucherUsagesForExcelInput input);

		
		Task<PagedResultDto<VoucherUsageUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
    }
}