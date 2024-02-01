using Syntaq.Falcon.Records.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Submissions.Dtos
{
	public class GetSubmissionForViewOutput
	{
		public CreateOrEditSubmissionDto Submission { get; set; }
		public string RecordName { get; set; }
		public string RecordMatterName { get; set; }
		public List<RecordMatterItemDto> RecordMatterItems { get; set; }
		public string UserName { get; set; }
		public string UserEmail { get; set; }
		public virtual bool? HasFiles { get; set; } = false;
	}
}