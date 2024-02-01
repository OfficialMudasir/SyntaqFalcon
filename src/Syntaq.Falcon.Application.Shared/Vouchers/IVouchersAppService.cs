using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.VoucherUsages.Dtos;

namespace Syntaq.Falcon.Vouchers
{
    public interface IVouchersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetVoucherForViewDto>> GetAll(GetAllVouchersInput input);

        Task<GetVoucherForViewDto> GetVoucherForView(Guid id);

		Task<GetVoucherForEditOutput> GetVoucherForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditVoucherDto input);

		Task Delete(EntityDto<Guid> input);

		Task<FileDto> GetVouchersToExcel(GetAllVouchersForExcelInput input);

        Task<bool> CheckVoucherKeyNotExisting(string key);

        Task InsertVoucherUsage(CreateOrEditVoucherUsageDto input);
    }
}