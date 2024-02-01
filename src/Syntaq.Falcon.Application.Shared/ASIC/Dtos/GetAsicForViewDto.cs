using System;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class GetAsicForViewDto
    {
        public AsicDto Asic { get; set; }
        public string TenantName { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
    }
    public class Submit201Output
    {
        public int requestId { get; set; }
        public Guid RecordId { get; set; }
        public Guid RecordMatterId { get; set; }
        public Guid RecordMatterItemId { get; set; }
        public Guid SubmissionId { get; set; }

    }
}