using Abp.Domain.Entities;

namespace Syntaq.Falcon.Documents
{
    public class Document : Entity
    {
		public int? TenantId { get; set; }

		public virtual string Xyz { get; set; }
    }
}