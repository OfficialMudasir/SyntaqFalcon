using Syntaq.Falcon.Users.Dtos;
using Abp.Extensions;
using System;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.UserAcceptances
{
    public class UserAcceptancesAcceptViewModel
	{
		public List<UserAcceptanceTypeDto> ActiveUserAcceptanceTypesList { get; set; }

		public long? UserId { get; set; }

		public Guid? RecordMatterContributorId { get; set; }

	}
}