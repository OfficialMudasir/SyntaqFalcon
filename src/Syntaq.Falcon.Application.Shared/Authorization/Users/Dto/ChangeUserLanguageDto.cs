using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
