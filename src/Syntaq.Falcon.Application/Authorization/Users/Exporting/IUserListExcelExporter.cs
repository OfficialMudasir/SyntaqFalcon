using System.Collections.Generic;
using Syntaq.Falcon.Authorization.Users.Dto;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}