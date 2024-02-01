
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Records.Dtos
{
	public class RecordMatterItemDto : EntityDto<Guid>
	{
		public Guid GroupId { get; set; }
		public Guid? FormId { get; set; }
		public bool Document { get; set; }
        public bool LockOnBuild { get; set; } 
        public string DocumentName { get; set; }
		public Guid RecordMatterId { get; set; }
		public DateTime CreationTime { get; set; }
		public bool AllowWord { get; set; }
		public bool AllowPdf { get; set; }
		public bool AllowHTML { get; set; }
		public string ErrorDetails { get; set; }
		public DateTime? LastModified { get; set; }
 
        public string AllowedFormats { get; set; }

        public string AllowWordAssignees { get; set; }
        public string AllowPdfAssignees { get; set; }
        public string AllowHtmlAssignees { get; set; }

        public DateTime LastModificationTime { get; set; } // Need to remove this or the LAstModifed Property - redundant

        public string Status { get; set; }

		public bool HasDocument { get; set; }
		public Guid? SubmissionId { get; set; }
		public virtual string SubmissionStatus { get; set; }
	}

	public enum RecordMatterItemForDownloadType
	{
		PDF, Word, HTML, Disallow
	}

	public class RecordMatterItemForDownloadDto : EntityDto<Guid>
	{
		public byte[] Document { get; set; }
		public string DocumentName { get; set; }
		public Guid RecordMatterId { get; set; }
		public RecordMatterItemForDownloadType Type { get; set; } = RecordMatterItemForDownloadType.PDF;
	}
}