using System.Threading.Tasks;

namespace Syntaq.Falcon.Security.Recaptcha
{
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}