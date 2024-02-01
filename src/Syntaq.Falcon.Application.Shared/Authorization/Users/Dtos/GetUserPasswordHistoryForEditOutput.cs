using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Authorization.Users.Dtos
{
    public class GetUserPasswordHistoryForEditOutput
    {
        public CreateOrEditUserPasswordHistoryDto UserPasswordHistory { get; set; }

        public string UserName { get; set; }

    }
}