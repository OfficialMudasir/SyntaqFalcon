using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.AccessControlList.Dtos
{
	public class GetAllACLsInput //: PagedAndSortedResultRequestDto
	{
		public long UserId { get; set; }
		public long OrgantizationId { get; set; }
		public string RoleFilter { get; set; }
		public string EntityFilter { get; set; }
	}
}