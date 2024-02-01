using System;
using System.Web;
using Abp.Runtime.Security;
using Abp.Runtime.Validation;
using Syntaq.Falcon.Authorization.Accounts.Dto;
using Syntaq.Falcon.Security;

namespace Syntaq.Falcon.Web.Models.Account
{
    public class ResetPasswordViewModel : ResetPasswordInput
    {
        public int? TenantId { get; set; }



        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

        protected override void ResolveParameters()
        {
            base.ResolveParameters();

            if (!string.IsNullOrEmpty(c))
            {
                c = c.Replace(" ", "+");

                var parameters = SimpleStringCipher.Instance.Decrypt(c);
                var query = HttpUtility.ParseQueryString(parameters);

                if (query["tenantId"] != null)
                {
                    TenantId = Convert.ToInt32(query["tenantId"]);
                }
            }
        }
    }
}