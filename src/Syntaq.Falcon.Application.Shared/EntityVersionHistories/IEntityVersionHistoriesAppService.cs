using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.EntityVersionHistories.Dtos;
using Syntaq.Falcon.Dto;
using System.Collections.Generic;

namespace Syntaq.Falcon.EntityVersionHistories
{
    public interface IEntityVersionHistoriesAppService : IApplicationService
    {
        Task<PagedResultDto<GetEntityVersionHistoryForViewDto>> GetAll(GetAllEntityVersionHistoriesInput input);

        Task<GetEntityVersionHistoryForViewDto> GetEntityVersionHistoryForView(Guid id);

        //Task<GetEntityVersionHistoryForEditOutput> GetEntityVersionHistoryForEdit(EntityDto<Guid> input);

        //Task CreateOrEdit(CreateOrEditEntityVersionHistoryDto input);

        //Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetEntityVersionHistoriesToExcel(GetAllEntityVersionHistoriesForExcelInput input);

        Task<List<EntityVersionHistoryUserLookupTableDto>> GetAllUserForTableDropdown();

    }
}