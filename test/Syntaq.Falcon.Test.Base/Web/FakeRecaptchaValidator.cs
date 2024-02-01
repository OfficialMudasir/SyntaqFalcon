using System.Threading.Tasks;
using Syntaq.Falcon.Security.Recaptcha;

namespace Syntaq.Falcon.Test.Base.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}
