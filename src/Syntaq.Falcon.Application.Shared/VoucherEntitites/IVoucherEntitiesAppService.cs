using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.VoucherEntitites.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.VoucherEntitites
{
    public interface IVoucherEntitiesAppService : IApplicationService
    {
        Task<PagedResultDto<GetVoucherEntityForViewDto>> GetAll(GetAllVoucherEntitiesInput input);

        Task<GetVoucherEntityForViewDto> GetVoucherEntityForView(Guid id);

        Task<GetVoucherEntityForEditOutput> GetVoucherEntityForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditVoucherEntityDto input);

        Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetVoucherEntitiesToExcel(GetAllVoucherEntitiesForExcelInput input);

        Task<GetVoucherEntityForViewDto> GetVoucherEntityByFormId(Guid FormId);

        Task<PagedResultDto<VoucherEntityVoucherLookupTableDto>> GetAllVoucherForLookupTable(GetAllForLookupTableInput input);

    }
}