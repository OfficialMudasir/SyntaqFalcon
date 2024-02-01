using System.Threading.Tasks;
using Abp.Application.Services;
using Syntaq.Falcon.Sessions.Dto;

namespace Syntaq.Falcon.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}
