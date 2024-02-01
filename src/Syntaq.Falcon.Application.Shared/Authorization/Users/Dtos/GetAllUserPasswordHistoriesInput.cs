using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Authorization.Users.Dtos
{
    public class GetAllUserPasswordHistoriesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string UserNameFilter { get; set; }

    }
}