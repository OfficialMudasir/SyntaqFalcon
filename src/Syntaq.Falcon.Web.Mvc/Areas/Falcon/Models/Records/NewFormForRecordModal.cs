
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Forms.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Records
{
    public class NewFormForRecordModal
    {
        public string RecordID { get; set; }
        public string RecordMatterID { get; set; }
        public List<FormListDto> FormsList { get; set; }
    }
}
