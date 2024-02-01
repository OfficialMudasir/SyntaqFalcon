using Abp.Domain.Repositories;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Payments.Dto;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Payments
{
    public class PaymentManager : FalconDomainServiceBase
    {
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;

        public PaymentManager(IRepository<Form, Guid> formRepository, IRepository<User, long> userRepository, UserManager userManager, TenantManager tenantManager)
        {
            _formRepository = formRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
        }

        public async Task<bool> SetStripeTokensAsync(StripeTokensDto StripeTokens)
        {
            bool HasUpdated = false;
            //{\"Entity\":\"Form\",\"Key\":\"005da596-5ff1-46e1-95e3-de483ec3ed4d\",\"UserId\":\"1\",\"TenantId\":null}
            switch (StripeTokens.StripeResponse.Entity)
            {
                case "User":
                    User _User = _userRepository.FirstOrDefault(long.Parse(StripeTokens.StripeResponse.Key));
                    if (_User != null)
                    {
                        _User.PaymentAccessToken = StripeTokens.AccessToken;
                        _User.PaymentRefreshToken = StripeTokens.RefreshToken;
                        _User.PaymentPublishableToken = StripeTokens.PublishableKey;
                        await _userManager.UpdateAsync(_User);
                        HasUpdated = true;
                        return HasUpdated;
                    }
                    break;
                case "Form":
                    Form form = _formRepository.FirstOrDefault(Guid.Parse(StripeTokens.StripeResponse.Key));
                    if (form != null)
                    {
                        form.PaymentAccessToken = StripeTokens.AccessToken;
                        form.PaymentRefreshToken = StripeTokens.RefreshToken;
                        form.PaymentPublishableToken = StripeTokens.PublishableKey;
                        _formRepository.Update(form);
                        HasUpdated = true;
                        return HasUpdated;
                    }
                    break;
                case "Tenant":
                    HasUpdated = true;
                    //return HasUpdated;
                    break;
            }
            return HasUpdated;
        }
    }
}




