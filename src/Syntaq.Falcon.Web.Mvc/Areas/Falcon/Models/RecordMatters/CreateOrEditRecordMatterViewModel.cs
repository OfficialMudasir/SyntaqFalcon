using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterItems;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatters
{
    public class CreateOrEditRecordMatterModalViewModel
    {
       public CreateOrEditRecordMatterDto RecordMatter { get; set; }

        //public string RecordRecordName { get; set;}

        //public System.Collections.Generic.List<RecordMatterItems.RecordMatterLookupTableViewModel> RecordMatters { get; set; }

        //public RecordMatterLookupTableViewModel RecordMatters { get; set; }
        
       public bool IsEditMode => RecordMatter.Id.HasValue;
    }
}