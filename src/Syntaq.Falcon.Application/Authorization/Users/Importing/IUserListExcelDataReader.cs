using System.Collections.Generic;
using Syntaq.Falcon.Authorization.Users.Importing.Dto;
using Abp.Dependency;

namespace Syntaq.Falcon.Authorization.Users.Importing
{
    public interface IUserListExcelDataReader: ITransientDependency
    {
        List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes);
    }
}
