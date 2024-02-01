using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Authorization.Users.Dtos
{
    public class CreateOrEditUserPasswordHistoryDto : EntityDto<Guid?>
    {

        public long UserId { get; set; }

    }
}