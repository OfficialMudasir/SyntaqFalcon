using System.Threading.Tasks;
using Syntaq.Falcon.Sessions.Dto;

namespace Syntaq.Falcon.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
