using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Syntaq.Falcon.Teams.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Teams
{
    public interface ITeamUserRolesAppService : IApplicationService 
    {
        //Task<bool> CheckTeamPermissions(string permissionName, int TeamId);

        Task<PagedResultDto<GetTeamUserRoleForView>> GetAll(GetAllTeamUserRolesInput input);

        //Task<GetTeamUserRoleForEditOutput> GetTeamUserRoleForEdit(EntityDto<long> input);
        List<Permission> GetTeamUserPermissions(int TeamId);

        Task<GetTeamUserRoleForEditOutput> GetTeamUserRoleForEdit(TeamUserRoleDto input);

        //Task CreateOrEdit(CreateOrEditTeamUserRoleDto input);

        //Task Delete(EntityDto<long> input);

        //Task AssignOrUpdateRoles(CreateOrUpdateTeamUserRoleInput input);

        //Task<PagedResultDto<OrganizationUnitLookupTableDto>> GetAllOrganizationUnitForLookupTable(Forms.Dtos.GetAllForLookupTableInput input);
		
		//Task<PagedResultDto<UserLookupTableDto>> GetAllUserForLookupTable(Forms.Dtos.GetAllForLookupTableInput input);
		
    }
}