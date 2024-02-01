using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Teams.Dtos;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Teams
{
    public interface ITeamsAppService : IApplicationService
    {
        Task<PagedResultDto<GetTeamForView>> GetAll(GetAllTeamsInput input);

        //Task<GetTeamForEditOutput> GetTeamForEdit(EntityDto<Guid> input);

        //Task CreateOrEdit(CreateOrEditTeamDto input);

        //Task Delete(EntityDto<Guid> input);

        Task<ListResultDto<GetTeamForView>> GetAllList();
    }
}
