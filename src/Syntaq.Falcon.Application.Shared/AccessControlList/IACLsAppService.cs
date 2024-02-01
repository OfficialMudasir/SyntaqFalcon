using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.AccessControlList
{
    public interface IACLsAppService : IApplicationService 
    {
        //Task<PagedResultDto<GetACLForView>> GetAll(GetAllACLsInput input);

		ListResultDto<GetACLForEditOutput> GetACLForEdit(ACLCheckDto aCLCheckDto);

		//Task CreateOrEdit(CreateOrEditACLDto input);

		//Task Delete(EntityDto input);

        //Task GrantACL(ACLInput input);

        Task GrantACL(dynamic grantACLDto);

        Task UpdateACL(ACLInput input);

        Task RevokeACL(ACLInput input);


        //Task<PagedResultDto<UserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);

        //Task<PagedResultDto<OrganizationUnitLookupTableDto>> GetAllOrganizationUnitForLookupTable(GetAllForLookupTableInput input);

    }
}