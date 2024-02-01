using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class CheckStatusDto
    {
        public JToken response { get; set; }
        public List<object> error { get; set; }
      
    }


   public class Response
    {
        public string status { get; set; }
        public string document_number { get; set; }
        public bool info { get; set; }

        public int? request_id { get; set; }
        [CanBeNull] public Communication communication { get; set; }
        [CanBeNull]  public string created_at { get; set; }
    }

    
}
