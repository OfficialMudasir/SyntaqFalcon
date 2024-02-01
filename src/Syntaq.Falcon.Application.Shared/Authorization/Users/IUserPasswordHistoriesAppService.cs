using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization.Users.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Authorization.Users
{
    public interface IUserPasswordHistoriesAppService : IApplicationService
    {
        Task<PagedResultDto<GetUserPasswordHistoryForViewDto>> GetAll(GetAllUserPasswordHistoriesInput input);

        Task<GetUserPasswordHistoryForEditOutput> GetUserPasswordHistoryForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditUserPasswordHistoryDto input);

        Task Delete(EntityDto<Guid> input);

        Task<PagedResultDto<UserPasswordHistoryUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);

    }
}