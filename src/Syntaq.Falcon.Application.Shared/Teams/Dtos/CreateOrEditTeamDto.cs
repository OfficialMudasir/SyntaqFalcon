
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class CreateOrEditTeamDto : AuditedEntityDto<Guid?>
    {

		[Required]
		[StringLength(TeamConsts.MaxTeamNameLength, MinimumLength = TeamConsts.MinTeamNameLength)]
		public string TeamName { get; set; }
		
		

    }
}