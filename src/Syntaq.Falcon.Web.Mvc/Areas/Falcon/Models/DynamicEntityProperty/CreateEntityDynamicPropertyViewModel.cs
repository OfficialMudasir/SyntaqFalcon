using System.Collections.Generic;
using Syntaq.Falcon.DynamicEntityProperties.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.DynamicEntityProperty
{
    public class CreateEntityDynamicPropertyViewModel
    {
        public string EntityFullName { get; set; }

        public List<string> AllEntities { get; set; }

        public List<DynamicPropertyDto> DynamicProperties { get; set; }
    }
}
