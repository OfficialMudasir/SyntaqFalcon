using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.AccessControlList.Dtos
{
	public class GetACLForView
	{
		public Guid EntityId { get; set; }

		public string EntityName { get; set; }

		public string EntityType { get; set; }

		public ListResultDto<GetACLForEditOutput> EntityACL { get; set; }
	}
}