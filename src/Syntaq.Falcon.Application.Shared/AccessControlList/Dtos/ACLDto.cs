using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.AccessControlList.Dtos
{
	public class ACLDto : EntityDto
	{
		 public long? UserId { get; set; }

		 public long? OrganizationUnitId { get; set; }

        public System.Guid? EntityID { get; set; }

    }
}