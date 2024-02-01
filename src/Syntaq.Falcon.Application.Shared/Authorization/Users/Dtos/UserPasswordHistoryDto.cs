using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Authorization.Users.Dtos
{
    public class UserPasswordHistoryDto : EntityDto<Guid>
    {

        public long UserId { get; set; }

    }
}