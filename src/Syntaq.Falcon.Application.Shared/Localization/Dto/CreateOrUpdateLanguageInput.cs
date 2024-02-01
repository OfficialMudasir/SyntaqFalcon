using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}