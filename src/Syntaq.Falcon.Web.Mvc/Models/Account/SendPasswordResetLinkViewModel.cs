using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Web.Models.Account
{
    public class SendPasswordResetLinkViewModel
    {
        [Required]
        public string EmailAddress { get; set; }

        public string PasswordResetReturnUrl { get; set; }

    }
}