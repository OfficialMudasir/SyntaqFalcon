using System.Threading.Tasks;
using Abp.Domain.Policies;

namespace Syntaq.Falcon.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(int tenantId);
    }
}
