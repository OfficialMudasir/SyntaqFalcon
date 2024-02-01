
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.AccessControlList.Dtos
{
    public class CreateOrEditACLDto : EntityDto<int?>
    {
		 public long? UserId { get; set; }
		 
		 public long? OrganizationUnitId { get; set; } 
    }
}