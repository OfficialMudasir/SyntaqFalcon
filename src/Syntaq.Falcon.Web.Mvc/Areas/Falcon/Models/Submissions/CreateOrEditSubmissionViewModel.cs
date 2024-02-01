using Syntaq.Falcon.Submissions.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Submissions
{
    public class CreateOrEditSubmissionModalViewModel
    {
       public CreateOrEditSubmissionDto Submission { get; set; }

	   		public string RecordRecordName { get; set;}

		public string RecordMatterRecordMatterName { get; set;}

		public string UserName { get; set;}

		public string AppJobName { get; set;}


	   public bool IsEditMode => Submission.Id.HasValue;
    }
}