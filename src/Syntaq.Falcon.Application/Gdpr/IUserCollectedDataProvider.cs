using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
