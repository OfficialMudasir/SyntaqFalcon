using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Teams
{
	//[Table("SfaTeams")]
    public class Team : AuditedEntity<Guid> , IMayHaveTenant
    {
		public int? TenantId { get; set; }


		[Required]
		[StringLength(TeamConsts.MaxTeamNameLength, MinimumLength = TeamConsts.MinTeamNameLength)]
		public virtual string TeamName { get; set; }
		

    }
}