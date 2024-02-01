
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Apps.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Records
{
    public class NewAppForRecordModal
    {
        public string RecordID { get; set; }
        public string RecordMatterID { get; set; }
        public string Data { get; set; }
        public List<AppDto> AppsList { get; set; }

    }
}
