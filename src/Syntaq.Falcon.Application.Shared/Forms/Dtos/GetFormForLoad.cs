using System;

namespace Syntaq.Falcon.Forms.Dtos
{
	public class GetFormForLoad
	{
		public string AuthToken { get; set; }
		public string AnonAuthToken { get; set; }
		public string TenantName { get; set; }
		public int? TenantId { get; set; }
		public Guid FormId { get; set; }
		public string FormData { get; set; }
		public string FormName { get; set; }
		public Guid? RecordId { get; set; }
		public Guid? RecordMatterId { get; set; }
		public Guid SubmissionId { get; set; }

        public Guid? ReleaseId { get; set; }
        public string ProjectName { get;set; }
        public string Schema { get; set; }

    }
}
