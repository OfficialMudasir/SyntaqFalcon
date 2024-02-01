using Syntaq.Falcon.RecordPolicyActions.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.RecordPolicyActions
{
    public class CreateOrEditRecordPolicyActionModalViewModel
    {
        public CreateOrEditRecordPolicyActionDto RecordPolicyAction { get; set; }

        public string RecordPolicyName { get; set; }

        public bool IsEditMode => RecordPolicyAction.Id.HasValue;
    }
}