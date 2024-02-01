
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Users.Dtos
{
    public class CreateOrEditUserAcceptanceDto : EntityDto<Guid?>
    {

		public int AcceptedDocTemplateVersion { get; set; }

		public Guid? UserAcceptanceTypeId { get; set; }
		 
		 		 public long? UserId { get; set; }
		 
		 		 public Guid? RecordMatterContributorId { get; set; }
		 
		 
    }
}