using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInput
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}