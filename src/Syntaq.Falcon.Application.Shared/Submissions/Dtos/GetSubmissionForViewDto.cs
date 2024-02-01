using Syntaq.Falcon.Records.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Submissions.Dtos
{
	public class GetSubmissionForViewDto
	{
		public SubmissionDto Submission { get; set; }
		public string FormName { get; set; }
		public string RecordName { get; set;}
		public string RecordMatterName { get; set;}
		public List<RecordMatterItemDto> RecordMatterItems { get; set; }
		public int RecordMatterItemCount { get; set; }
		public string UserName { get; set;}
		public string UserEmail { get; set; }
		public string AppJobName { get; set;}
		public string AppName { get; set;}
	}
}