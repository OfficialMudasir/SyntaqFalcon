using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class GetRequestIdDTO
    {
        public int? response { get; set; }
        public JArray error { get; set; }
    }

    public class GetResponseDto
    { 
        public string response { get; set; }
        public JArray error { get; set; }
    }

    

    //class GetRequestIdDTO
    //{
    //    public int response { get; set; }
    //    public List<object> error { get; set; }
    //}



}
