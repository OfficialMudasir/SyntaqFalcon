using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Dto;
using System.Collections.Generic;
using Syntaq.Falcon.Users;

namespace Syntaq.Falcon.Users
{
    public interface IUserAcceptanceTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserAcceptanceTypeForViewDto>> GetAll(GetAllUserAcceptanceTypesInput input);

		Task<GetUserAcceptanceTypeForEditOutput> GetUserAcceptanceTypeForEdit(EntityDto<Guid> input);

		Task<GetUserAcceptanceTypeForEditOutput> GetUserAcceptanceTypeForView(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditUserAcceptanceTypeDto input);

		Task Delete(EntityDto<Guid> input);

		Task<FileDto> GetUserAcceptanceTypesToExcel(GetAllUserAcceptanceTypesForExcelInput input);

		Task<PagedResultDto<UserAcceptanceTypeTemplateLookupTableDto>> GetAllTemplateForLookupTable(GetAllForLookupTableInput input);

		Task<List<UserAcceptanceTypeDto>> GetAllActiveUserAcceptanceTypesForView();

	}
}