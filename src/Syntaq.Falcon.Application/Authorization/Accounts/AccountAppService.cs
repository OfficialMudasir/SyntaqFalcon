using System;
using System.Threading.Tasks;
using System.Web;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Syntaq.Falcon.Authorization.Accounts.Dto;
using Syntaq.Falcon.Authorization.Impersonation;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Configuration;
using Syntaq.Falcon.Debugging;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Security.Recaptcha;
using Syntaq.Falcon.Url;
using Syntaq.Falcon.Authorization.Delegation;
using Abp.Domain.Repositories;

// STQ MODIFIED START
using System.Linq;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Folders;
using Abp.Domain.Uow;
using Syntaq.Falcon.Common.Dto;

namespace Syntaq.Falcon.Authorization.Accounts
{
    public class AccountAppService : FalconAppServiceBase, IAccountAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        public IRecaptchaValidator RecaptchaValidator { get; set; }

        private readonly IUserEmailer _userEmailer;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IUserLinkManager _userLinkManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IWebUrlService _webUrlService;
        private readonly IUserDelegationManager _userDelegationManager;

		// STQ MODIFIED START
		private readonly IRepository<UserPasswordHistory, Guid> _userPasswordHistoryRepository;
        private readonly FolderManager _folderManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly TenantManager _tenantManager;
        // STQ MODIFIED END

        public AccountAppService(
			IUnitOfWorkManager unitOfWorkManager,
			IUserEmailer userEmailer,
			UserRegistrationManager userRegistrationManager,
			TenantManager tenantManager,
			IImpersonationManager impersonationManager,
			IUserLinkManager userLinkManager,
			IPasswordHasher<User> passwordHasher,
			IWebUrlService webUrlService,
            FolderManager folderManager,
			IRepository<UserPasswordHistory, Guid> userPasswordHistoryRepository
		)
		{
			_unitOfWorkManager = unitOfWorkManager;
			_userEmailer = userEmailer;
			_userRegistrationManager = userRegistrationManager;
			_tenantManager = tenantManager;
			_impersonationManager = impersonationManager;
			_userLinkManager = userLinkManager;
			_passwordHasher = passwordHasher;
			_webUrlService = webUrlService;
            _folderManager = folderManager;

            AppUrlService = NullAppUrlService.Instance;
			RecaptchaValidator = NullRecaptchaValidator.Instance;

			// STQ MODIFIED
			_userPasswordHistoryRepository = userPasswordHistoryRepository;
		}

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id, _webUrlService.GetServerRootAddress(input.TenancyName));
        }

        public Task<int?> ResolveTenantId(ResolveTenantIdInput input)
        {
            if (string.IsNullOrEmpty(input.c))
            {
                return Task.FromResult(AbpSession.TenantId);
            }

            var parameters = SimpleStringCipher.Instance.Decrypt(input.c);
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["tenantId"] == null)
            {
                return Task.FromResult<int?>(null);
            }

            var tenantId = Convert.ToInt32(query["tenantId"]) as int?;
            return Task.FromResult(tenantId);
        }


        public Task<int?> GetTenantIdByName(GetTenantIdByNameInput input)
        {
            if (string.IsNullOrEmpty(input.TenantName))
            {
                return null;
            }

            var tenantId = _tenantManager.GetTenantId(input.TenantName);
            return tenantId;
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
		{
			if (UseCaptchaOnRegistration())
			{
				await RecaptchaValidator.ValidateAsync(input.CaptchaResponse);
			}

            // Check for teanant ID
            //AbpSession.TenantId = 1;
            // Tenant May be passed in rather than found in the session object (embedded registration)
            var tenantId = AbpSession.TenantId;
            if (!string.IsNullOrEmpty(input.TenantName))
            {
                Tenant tenant = null;
                tenant = await _tenantManager.FindByTenancyNameAsync(input.TenantName);
                if (tenant == null)
                {
                    throw new InvalidOperationException("Tenant not found");
                }
                else
                {
                    tenantId = tenant.Id;
                }
            }

            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                false,
                AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId, input.EmailActivationReturnUrl)
            );

            var isNewRegisteredUserActiveByDefault = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault);
            user.IsActive = isNewRegisteredUserActiveByDefault;

            //STQ MODIFIED
            //DefaultFolders
            ACL aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
            //RecordsFolder
            Folder Rfolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Records", Description = "", ParentId = null, Type = "R" };
            await _folderManager.CreateOrEditFolder(aCL, Rfolder);
            await CurrentUnitOfWork.SaveChangesAsync();
            //TemplatesFolder
            aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
            Folder Tfolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Templates", Description = "", ParentId = null, Type = "T" };
            await _folderManager.CreateOrEditFolder(aCL, Tfolder);
            await CurrentUnitOfWork.SaveChangesAsync();
            //FormsFolder
            aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
            Folder Ffolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Forms", Description = "", ParentId = null, Type = "F" };
            await _folderManager.CreateOrEditFolder(aCL, Ffolder);
            await CurrentUnitOfWork.SaveChangesAsync();


            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

		public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
		{

			// STQ MODIFED
			// Do not throw error if user not found
			//var user = await GetUserByChecking(input.EmailAddress);
			var user = await UserManager.FindByEmailAsync(input.EmailAddress);

			if (user != null)
            {

				var url = AppUrlService.CreatePasswordResetUrlFormat(AbpSession.TenantId);

				if (!string.IsNullOrEmpty(input.PasswordResetReturnUrl))
				{
					Uri uri = new Uri(url);
					var query = uri.Query.Replace("%7B", "{").Replace("%7D", "}");
					url = input.PasswordResetReturnUrl + query; // + "?userId={userId}&resetCode={resetCode}";
				}

				user.SetNewPasswordResetCode();
				await _userEmailer.SendPasswordResetLinkAsync(
					user,
					url
					);
            }

		}

        // STQ MODIFIED
		public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
		{
           // using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
           // {

                var user = await UserManager.GetUserByIdAsync(input.UserId);
			    if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
			    {

				    throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
			    }

			    await UserManager.InitializeOptionsAsync(AbpSession.TenantId);

                /// Removed Replaced with ASPNET NATIVE FUNATIONALITY
				///// STQ MODIFIED START				
				//var passAlreadyExist = _userPasswordHistoryRepository.GetAll()
				//	.Where(h => h.UserId == user.Id)
				//	.ToList()
				//	.Any(hash => {
				//		var res = _passwordHasher.VerifyHashedPassword(user, hash.PasswordHash, input.Password);
				//		return res == PasswordVerificationResult.Success;
				//	});

				//if (passAlreadyExist)
				//	throw new UserFriendlyException(L("PasswordUsed"));

				CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));

				//var hashedpassword = _passwordHasher.HashPassword(user, input.Password);
				//UserPasswordHistory userPasswordHistory = new UserPasswordHistory()
				//{
				//	Id = Guid.NewGuid(),
				//	PasswordHash = hashedpassword,
				//	UserId = user.Id,
				//	TenantId = AbpSession.TenantId
				//};

				//await _userPasswordHistoryRepository.InsertAsync(userPasswordHistory);
				//CurrentUnitOfWork.SaveChanges();
				/// STQ MODIFIED END
			
			    user.PasswordResetCode = null;
			    user.IsEmailConfirmed = true;
			    user.ShouldChangePasswordOnNextLogin = false;

			    await UserManager.UpdateAsync(user);

			    return new ResetPasswordOutput
			    {
				    CanLogin = user.IsActive,
				    UserName = user.UserName
			    };
           // }

		}

        public async Task SendEmailActivationLink(SendEmailActivationLinkInput input)
        {
            var user = await UserManager.FindByEmailAsync(input.EmailAddress);
            if (user == null)
            {
                return;
            }

            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(
                user,
                AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId, string.Empty) // STQ MODIFIED
            );
        }

       
        public async Task<MessageOutput> ActivateEmail(ActivateEmailInput input)
        {
            //Disable Tenant checking as request may come from embedded host sites
            User user;
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                user = await UserManager.GetUserByIdAsync(input.UserId);


                if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() || user.EmailConfirmationCode != input.ConfirmationCode)
                {
                    throw new UserFriendlyException(L("InvalidEmailConfirmationCode"), L("InvalidEmailConfirmationCode_Detail"));
                }

                user.IsEmailConfirmed = true;
                user.EmailConfirmationCode = null;

                await UserManager.UpdateAsync(user);
            }

            // STQ MODIFIED
            return new MessageOutput()
            {
                Message = "Account activated",
                Success = true,
                Title = "Success"
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Impersonation)]
        public virtual async Task<ImpersonateOutput> ImpersonateUser(ImpersonateUserInput input)
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationToken(input.UserId, AbpSession.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
            };
        }
        
        [AbpAuthorize(AppPermissions.Pages_Tenants_Impersonation)]
        public virtual async Task<ImpersonateOutput> ImpersonateTenant(ImpersonateTenantInput input)
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationToken(input.UserId, input.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
            };
        }

        public virtual async Task<ImpersonateOutput> DelegatedImpersonate(DelegatedImpersonateInput input)
        {
            var userDelegation = await _userDelegationManager.GetAsync(input.UserDelegationId);
            if (userDelegation.TargetUserId != AbpSession.GetUserId())
            {
                throw new UserFriendlyException("User delegation error.");
            }

            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationToken(userDelegation.SourceUserId, userDelegation.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(userDelegation.TenantId)
            };
        }

        public virtual async Task<ImpersonateOutput> BackToImpersonator()
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetBackToImpersonatorToken(),
                TenancyName = await GetTenancyNameOrNullAsync(AbpSession.ImpersonatorTenantId)
            };
        }

        public virtual async Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input)
        {
            if (!await _userLinkManager.AreUsersLinked(AbpSession.ToUserIdentifier(), input.ToUserIdentifier()))
            {
                throw new Exception(L("This account is not linked to your account"));
            }

            return new SwitchToLinkedAccountOutput
            {
                SwitchAccountToken = await _userLinkManager.GetAccountSwitchToken(input.TargetUserId, input.TargetTenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TargetTenantId)
            };
        }

        private bool UseCaptchaOnRegistration()
        {
            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await TenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        private async Task<string> GetTenancyNameOrNullAsync(int? tenantId)
        {
            return tenantId.HasValue ? (await GetActiveTenantAsync(tenantId.Value)).TenancyName : null;
        }
    }
}
