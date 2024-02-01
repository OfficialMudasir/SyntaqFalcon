using System.Collections.Generic;
using Syntaq.Falcon.DynamicEntityProperties.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.DynamicProperty
{
    public class CreateOrEditDynamicPropertyViewModel
    {
        public DynamicPropertyDto DynamicPropertyDto { get; set; }

        public List<string> AllowedInputTypes { get; set; }
    }
}
