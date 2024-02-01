using Syntaq.Falcon.Records.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterContributors
{
    public class CreateOrEditRecordMatterContributorModalViewModel
    {
       public CreateOrEditRecordMatterContributorDto RecordMatterContributor { get; set; }

		public string RecordMatterRecordMatterName { get; set;}

		public string UserName { get; set;}

		public string FormName { get; set;}
        public int Role { get; set; }

        public bool IsEditMode => RecordMatterContributor.Id.HasValue;


    }
}