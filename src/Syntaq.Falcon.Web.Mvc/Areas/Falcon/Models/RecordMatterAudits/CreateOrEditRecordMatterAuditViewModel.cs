using Syntaq.Falcon.Records.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterAudits
{
    public class CreateOrEditRecordMatterAuditViewModel
    {
        public CreateOrEditRecordMatterAuditDto RecordMatterAudit { get; set; }

        public string UserName { get; set; }

        public string RecordMatterRecordMatterName { get; set; }

        public bool IsEditMode => RecordMatterAudit.Id.HasValue;
    }
}