using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Dto;
using System.Collections.Generic;

namespace Syntaq.Falcon.Users
{
    public interface IUserAcceptancesAppService : IApplicationService 
    {

		Task<GetUserAcceptanceForEditOutput> GetUserAcceptanceForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditUserAcceptanceDto input);

		Task Delete(EntityDto<Guid> input);
		Task DeleteSelected(List<Guid> input);

		Task<FileDto> GetUserAcceptancesToExcel(GetAllUserAcceptancesForExcelInput input);

		
		Task<PagedResultDto<UserAcceptanceUserAcceptanceTypeLookupTableDto>> GetAllUserAcceptanceTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<UserAcceptanceUserLookupTableDto>> GetAllAcceptedUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<UserAcceptanceRecordMatterContributorLookupTableDto>> GetAllRecordMatterContributorForLookupTable(GetAllForLookupTableInput input);

		// TODO REFACTOR
		string GetUserAcceptanceForRecordMatterContributor(Guid? recordMatterContributorId, long? userId);
		string CheckIfUserOrContributorAccepted(UserAcceptanceDto userAcceptanceDto);

		Task<PagedResultDto<GetUserAcceptanceForViewDto>> GetRequiredAcceptancesForUser(GetRequiredUserAcceptancesInput input);
		
		Task<bool> Accept(List<AcceptInput> input);
	}
}