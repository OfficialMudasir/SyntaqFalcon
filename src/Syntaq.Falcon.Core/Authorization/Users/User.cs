using System;
using System.Collections.Generic;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Timing;

namespace Syntaq.Falcon.Authorization.Users
{
    /// <summary>
    /// Represents a user in the system.
    /// Refactor For Merge
    /// </summary>
    public class User : AbpUser<User>
    {
        public virtual Guid? ProfilePictureId { get; set; }
        
        //STQ MODIFIED
        public virtual Guid? LogoPictureId { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        public DateTime? SignInTokenExpireTimeUtc { get; set; }

        public string SignInToken { get; set; }

        public string GoogleAuthenticatorKey { get; set; }

        public List<UserOrganizationUnit> OrganizationUnits { get; set; }

        //Can add application specific user properties here
        //STQ MODIFIED

        public virtual Guid? ProfileBackgroundPictureId { get; set; }

        public string Title { get; set; }
        public string JobTitle { get; set; }
        public string WebsiteURL { get; set; }

        public string EmailAddressWork { get; set; }
        public string Fax { get; set; }

        public string PhoneNumberWork { get; set; }
        public string PhoneNumberMobile { get; set; }

        public string AddressCO { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressPostCode { get; set; }
        public string AddressState { get; set; }
        public string AddressSub { get; set; }
        public string AddressCountry { get; set; }

        public string ABN { get; set; }
        public string LegalABN { get; set; }

        public string Entity { get; set; }
		public string EntityAddressCO { get; set; }
		public string EntityAddressLine1 { get; set; }
		public string EntityAddressLine2 { get; set; }
		public string EntityAddressSub { get; set; }
		public string EntityAddressPostCode { get; set; }
		public string EntityAddressState { get; set; }
		public string EntityAddressCountry { get; set; }

		public string BillingName { get; set; }
		public string BillingAddressLine1 { get; set; }
		public string BillingAddressLine2 { get; set; }
		public string BillingAddressPostCode { get; set; }
		public string BillingAddressState { get; set; }
		public string BillingAddressCountry { get; set; }

		public byte[] Logo { get; set; }
		public bool? PayOnAccount { get; set; }

		public bool HasPaymentConfigured { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentProvider { get; set; }
		public string PaymentAccessToken { get; set; }
		public string PaymentRefreshToken { get; set; }
		public string PaymentPublishableToken { get; set; }

		public string FLT { get; set; } // Federated Logon tag

		public User()
		{
			IsLockoutEnabled = true;
			IsTwoFactorEnabled = true;
		}

        /// <summary>
        /// Creates admin <see cref="User"/> for a tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="emailAddress">Email address</param>
        /// <param name="name">Name of admin user</param>
        /// <param name="surname">Surname of admin user</param>
        /// <returns>Created <see cref="User"/> object</returns>
        public static User CreateTenantAdminUser(int tenantId, string emailAddress, string adminname = "admin", string name = null, string surname = null)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = adminname, // string.IsNullOrWhiteSpace(name) ? AdminUserName : name,
                Name = string.IsNullOrWhiteSpace(name) ? adminname : name,
                Surname = string.IsNullOrWhiteSpace(surname) ? adminname : surname,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>(),
                OrganizationUnits = new List<UserOrganizationUnit>()
            };

            user.SetNormalizedNames();

            return user;
        }

        public override void SetNewPasswordResetCode()
        {
            /* This reset code is intentionally kept short.
             * It should be short and easy to enter in a mobile application, where user can not click a link.
             */
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();
        }

        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }

        public void SetSignInToken()
        {
            SignInToken = Guid.NewGuid().ToString();
            SignInTokenExpireTimeUtc = Clock.Now.AddMinutes(1).ToUniversalTime();
        }
    }
}