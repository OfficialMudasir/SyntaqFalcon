using Abp.AutoMapper;
using Syntaq.Falcon.Localization.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Languages
{
    [AutoMapFrom(typeof(GetLanguageForEditOutput))]
    public class CreateOrEditLanguageModalViewModel : GetLanguageForEditOutput
    {
        public bool IsEditMode => Language.Id.HasValue;
    }
}