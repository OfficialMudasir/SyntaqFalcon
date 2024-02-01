using Syntaq.Falcon.ASIC.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Asic
{
    public class CreateOrEditAsicModalViewModel
    {
        public CreateOrEditAsicDto Asic { get; set; }

        public bool IsEditMode => Asic.Id.HasValue;
    }
}