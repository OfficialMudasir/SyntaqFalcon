using Abp.Domain.Services;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Folders.Dtos;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Folders
{
    public interface IFolderManager : IDomainService
    {
        Task CreateOrEditFolder(ACL ACL, Folder Folder);
        Task<FolderDto> CreateAndOrFetchFolder(ACL ACL, Folder Folder, long userid);
        Task<bool> Move(MoveFolderDto moveFolderDto);
    }
}
