using Syntaq.Falcon.RecordPolicies.Dtos;
using System.Collections.Generic;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.RecordPolicies
{
    public class CreateOrEditRecordPolicyModalViewModel
    {
        public CreateOrEditRecordPolicyDto RecordPolicy { get; set; }
        public List<TenantIdNameListDto> TenantList { get; set; }

        public bool IsEditMode => RecordPolicy.Id.HasValue;
    }
}