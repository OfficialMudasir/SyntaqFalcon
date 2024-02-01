using Syntaq.Falcon.ASIC.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Asic
{
    public class AsicViewModel : GetAsicForViewDto
    {
       public string FilterText { get; set; }
    }
}