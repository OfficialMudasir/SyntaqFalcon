using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Authorization.Users.Dto
{
    //Mapped to/from User in CustomDtoMapper
    public class UserEditDto : IPassivable
    {
		/// <summary>
		/// Set null to create a new user. Set user's Id to update a user
		/// </summary>
		public long? Id { get; set; }
		public string Title { get; set; }
		[Required]
		[StringLength(AbpUserBase.MaxNameLength)]
		public string Name { get; set; }
		[Required]
		[StringLength(AbpUserBase.MaxSurnameLength)]
		public string Surname { get; set; }
		[Required]
		[StringLength(AbpUserBase.MaxUserNameLength)]
		public string UserName { get; set; }
		public string JobTitle { get; set; }
		public string WebsiteURL { get; set; }
		[Required]
		[EmailAddress]
		[StringLength(AbpUserBase.MaxEmailAddressLength)]
		public string EmailAddress { get; set; }

		private string _EmailAddressWorkmail;
		//[EmailAddress]
		[StringLength(AbpUserBase.MaxEmailAddressLength)]
		public string EmailAddressWork { get { return _EmailAddressWorkmail; } set { _EmailAddressWorkmail = string.IsNullOrWhiteSpace(value) ? null : value; } }

		public string Fax { get; set; }
		[StringLength(UserConsts.MaxPhoneNumberLength)]
		public string PhoneNumber { get; set; }
		[StringLength(UserConsts.MaxPhoneNumberLength)]
		public string PhoneNumberMobile { get; set; }
		[StringLength(UserConsts.MaxPhoneNumberLength)]
		public string PhoneNumberWork { get; set; }
		// Not used "Required" attribute since empty value is used to 'not change password'
		[StringLength(AbpUserBase.MaxPlainPasswordLength)]
		[DisableAuditing]
		public string Password { get; set; }

		public bool HasPaymentConfigured { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentProvider { get; set; }

		public string AddressCO { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string AddressPostCode { get; set; }
		public string AddressState { get; set; }
		public string AddressSub { get; set; }
		public string AddressCountry { get; set; }
		public string Entity { get; set; }

		//[RegularExpression(@"([0-9]+)")]
		public string ABN { get; set; }
		public string EntityAddressCO { get; set; }
		public string EntityAddressLine1 { get; set; }
		public string EntityAddressLine2 { get; set; }
		public string EntityAddressPostCode { get; set; }
		public string EntityAddressState { get; set; }
		public string EntityAddressSub { get; set; }
		public string EntityAddressCountry { get; set; }
		public bool IsActive { get; set; }
		public bool ShouldChangePasswordOnNextLogin { get; set; }
		public virtual bool IsTwoFactorEnabled { get; set; }
		public virtual bool IsLockoutEnabled { get; set; }

	}
}