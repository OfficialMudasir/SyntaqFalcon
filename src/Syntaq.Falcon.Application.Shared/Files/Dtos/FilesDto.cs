using System;

namespace Syntaq.Falcon.Files.Dtos
{
	public class FilesDto
	{
		public Guid FileUploadId { get; set; } //Can Remove?

		public string FileUploads { get; set; } //Can Remove?

		public string FileName { get; set; }

		public string Type { get; set; }

		public string Size { get; set; }

		public string RecordId { get; set; }

		public string RecordMatterId { get; set; }

        public string RecordMatterItemGroupId { get; set; }

        public string SubmissionId { get; set; }

        public string AccessToken { get; set; }

    }
}