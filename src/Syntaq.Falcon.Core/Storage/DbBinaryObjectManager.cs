using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.UI;
using Syntaq.Falcon.Files;

namespace Syntaq.Falcon.Storage
{
    public class DbBinaryObjectManager : IBinaryObjectManager, ITransientDependency
    {
        private readonly IRepository<BinaryObject, Guid> _binaryObjectRepository;

        // STQ MODIFIED
        private readonly FilesManager _filesManager;

        public DbBinaryObjectManager(
            IRepository<BinaryObject, Guid> binaryObjectRepository,
            FilesManager filesManager // STQ Modified
            )
        {
            _binaryObjectRepository = binaryObjectRepository;

            // STQ Modified
            _filesManager = filesManager;
        }

        public Task<BinaryObject> GetOrNullAsync(Guid id)
        {
            return _binaryObjectRepository.FirstOrDefaultAsync(id);
        }

        public Task SaveAsync(BinaryObject file)
        {

            // STQ MODIFIED AV MALWARE SCANNING
            if (_filesManager.ValidateFile(file.Bytes, Convert.ToString( file.Id)).Result)
            {
                return _binaryObjectRepository.InsertAsync(file);
            }
            else
            {
                throw new UserFriendlyException("Malware scan failed. ");
            }


        }

        public Task DeleteAsync(Guid id)
        {
            return _binaryObjectRepository.DeleteAsync(id);
        }
    }
}