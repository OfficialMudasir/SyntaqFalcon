using static Syntaq.Falcon.Projects.ProjectConsts;
using static Syntaq.Falcon.Records.RecordMatterConsts;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetRecordMatterContributorForViewDto
    {
		public RecordMatterContributorDto RecordMatterContributor { get; set; }

		public string RecordMatterRecordMatterName { get; set;}

		public string UserName { get; set;}

		public string FormName { get; set;}

		public string Message { get; set; }

		public RecordMatterStatus? Status { get; set; }

		public bool ActionEnabled { get; set; }

		public bool RequireReview { get; set; }
		public bool RequireApproval { get; set; }

	}
}