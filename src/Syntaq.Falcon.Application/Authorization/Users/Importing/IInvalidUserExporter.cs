using System.Collections.Generic;
using Syntaq.Falcon.Authorization.Users.Importing.Dto;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Authorization.Users.Importing
{
    public interface IInvalidUserExporter
    {
        FileDto ExportToFile(List<ImportUserDto> userListDtos);
    }
}
