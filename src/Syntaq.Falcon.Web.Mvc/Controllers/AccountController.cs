using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Caching;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Web.Models;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Syntaq.Falcon.Authentication.TwoFactor.Google;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Authorization.Accounts;
using Syntaq.Falcon.Authorization.Accounts.Dto;
using Syntaq.Falcon.Authorization.Delegation;
using Syntaq.Falcon.Authorization.Impersonation;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Configuration;
using Syntaq.Falcon.Debugging;
using Syntaq.Falcon.Identity;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Net.Sms;
using Syntaq.Falcon.Notifications;
using Syntaq.Falcon.Web.Models.Account;
using Syntaq.Falcon.Security;
using Syntaq.Falcon.Security.Recaptcha;
using Syntaq.Falcon.Sessions;
using Syntaq.Falcon.Url;
using Syntaq.Falcon.Web.Authentication.External;
using Syntaq.Falcon.Web.Security.Recaptcha;
using Syntaq.Falcon.Web.Session;
using Syntaq.Falcon.Web.Views.Shared.Components.TenantChange;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Folders;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using System.Security.Authentication;
using ITfoxtec.Identity.Saml2.Schemas;
using Abp.Domain.Repositories;
using System.Text.Json;
using System.ServiceModel.Security;
using System.Security.Cryptography.X509Certificates;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Schemas.Conditions;
using ITfoxtec.Identity.Saml2.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using Syntaq.Falcon.Web.Startup;
using ComponentSpace.Saml2;
using Abp.CachedUniqueKeys;
using Abp.AspNetCore.Mvc.Caching;


// REFACTOR FOR MERGE
namespace Syntaq.Falcon.Web.Controllers
{

	//STQ MODIFIED
	public class STQSaml2Configuration : Saml2Configuration
	{
		public Boolean Enabled { get; set; }
		public String LoggedOutPage { get; set; }
	}

	[AbpAllowAnonymous]
	//[Route("Auth")]
	public class AccountController : FalconControllerBase
	{
		private readonly UserManager _userManager;
		private readonly TenantManager _tenantManager;
		private readonly IMultiTenancyConfig _multiTenancyConfig;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IWebUrlService _webUrlService;
		private readonly IAppUrlService _appUrlService;
		private readonly IAppNotifier _appNotifier;
		private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
		private readonly IUserLinkManager _userLinkManager;
		private readonly LogInManager _logInManager;
		private readonly SignInManager _signInManager;
		private readonly IRecaptchaValidator _recaptchaValidator;
		private readonly IPerRequestSessionCache _sessionCache;
		private readonly ITenantCache _tenantCache;
		private readonly IAccountAppService _accountAppService;
		private readonly UserRegistrationManager _userRegistrationManager;
		private readonly IImpersonationManager _impersonationManager;
		private readonly ISmsSender _smsSender;
		private readonly IEmailSender _emailSender;
		private readonly IPasswordComplexitySettingStore _passwordComplexitySettingStore;
		private readonly IdentityOptions _identityOptions;
		private readonly ISessionAppService _sessionAppService;
		private readonly ExternalLoginInfoManagerFactory _externalLoginInfoManagerFactory;
		private readonly ISettingManager _settingManager;
		private readonly IUserDelegationManager _userDelegationManager;
		private readonly ICachedUniqueKeyPerUser _cachedUniqueKeyPerUser;
		private readonly IGetScriptsResponsePerUserConfiguration _getScriptsResponsePerUserConfiguration;

		// STQ MODIFIED
		private readonly FolderManager _folderManager;
 
		private readonly IConfigurationRoot _appConfiguration;
		private STQSaml2Configuration _saml2Configuration;

		private readonly ISamlServiceProvider _samlServiceProvider;

		const string relayStateReturnUrl = "ReturnUrl";

		private IWebHostEnvironment _hostEnvironment;
 
		public AccountController(
			UserManager userManager,
			IMultiTenancyConfig multiTenancyConfig,
			TenantManager tenantManager,
			IUnitOfWorkManager unitOfWorkManager,
			IAppNotifier appNotifier,
			IWebUrlService webUrlService,
			AbpLoginResultTypeHelper abpLoginResultTypeHelper,
			IUserLinkManager userLinkManager,
			LogInManager logInManager,
			SignInManager signInManager,
			IRecaptchaValidator recaptchaValidator,
			ITenantCache tenantCache,
			IAccountAppService accountAppService,
			UserRegistrationManager userRegistrationManager,
			IImpersonationManager impersonationManager,
			IAppUrlService appUrlService,
			IPerRequestSessionCache sessionCache,
			IEmailSender emailSender,
			ISmsSender smsSender,
			IPasswordComplexitySettingStore passwordComplexitySettingStore,
			IOptions<IdentityOptions> identityOptions,
			ISessionAppService sessionAppService,
			ExternalLoginInfoManagerFactory externalLoginInfoManagerFactory,
			ISettingManager settingManager,
			IUserDelegationManager userDelegationManager,
			ICachedUniqueKeyPerUser cachedUniqueKeyPerUser,
			IGetScriptsResponsePerUserConfiguration getScriptsResponsePerUserConfiguration,

			//STQ MODIFIED
			FolderManager folderManager,			
			IWebHostEnvironment environment,
			IWebHostEnvironment env, 
			IConfiguration configuration,
			IOptions<STQSaml2Configuration> saml2Configuration,
			ISamlServiceProvider samlServiceProvider

		)
		{
			_userManager = userManager;
			_multiTenancyConfig = multiTenancyConfig;
			_tenantManager = tenantManager;
			_unitOfWorkManager = unitOfWorkManager;
			_webUrlService = webUrlService;
			_appNotifier = appNotifier;
			_abpLoginResultTypeHelper = abpLoginResultTypeHelper;
			_userLinkManager = userLinkManager;
			_logInManager = logInManager;
			_signInManager = signInManager;
			_recaptchaValidator = recaptchaValidator;
			_tenantCache = tenantCache;
			_accountAppService = accountAppService;
			_userRegistrationManager = userRegistrationManager;
			_impersonationManager = impersonationManager;
			_appUrlService = appUrlService;
			_sessionCache = sessionCache;
			_emailSender = emailSender;
			_smsSender = smsSender;
			_passwordComplexitySettingStore = passwordComplexitySettingStore;
			_identityOptions = identityOptions.Value;
			_sessionAppService = sessionAppService;
			_externalLoginInfoManagerFactory = externalLoginInfoManagerFactory;
			_settingManager = settingManager;
			_userDelegationManager = userDelegationManager;
			_cachedUniqueKeyPerUser = cachedUniqueKeyPerUser;
			_getScriptsResponsePerUserConfiguration = getScriptsResponsePerUserConfiguration;

			// STQ MODIFIED
			_folderManager = folderManager;
			_hostEnvironment = environment;
 
			_appConfiguration = env.GetAppConfiguration();
			_saml2Configuration = saml2Configuration.Value;

			_samlServiceProvider = samlServiceProvider;

		}

        #region Login / Logout

		// STQ MODIFIED
		//[Route("Login")]
        public async Task<IActionResult> Login(string userNameOrEmailAddress = "", string returnUrl = "", string OriginalId = "", string RecordMatterId = "", string RecordMatterItemId = "", string version = "", string successMessage = "", string ss = "", string view = "/Views/Account/Login.cshtml", string disablesso = "false", string tenant = "")
        {

            if (string.IsNullOrEmpty(tenant) & !string.IsNullOrEmpty(returnUrl))
            {

				returnUrl = System.Web.HttpUtility.HtmlDecode(returnUrl);

				Uri myUri = new Uri( new Uri( this.HttpContext.Request.Scheme + "://" + this.HttpContext.Request.Host), returnUrl  );
				tenant = System.Web.HttpUtility.ParseQueryString(myUri.Query).Get("tenant");

			}
 
			int? tenantId = null;
			if (!string.IsNullOrEmpty(tenant))
            { 				
				tenantId = await _tenantManager.GetTenantId(tenant);
                if (AbpSession.TenantId != tenantId)
                {
					AbpSession.Use(tenantId.Value, null);

                    if (string.IsNullOrEmpty(returnUrl))
                    {
						returnUrl = "/account/login?tenant=" + tenant;
					}
                    else
                    {

						if( System.Web.HttpUtility.ParseQueryString(returnUrl)["tenant"] == null)
                        {
							returnUrl = returnUrl + "&tenant=" + tenant;
                        }

					}

                    Response.Cookies.Append("TenantId", tenantId.ToString());

                    //return RedirectToAction("SetTenant", new SetTenantViewModel() { TenantId = tenantId , ReturnUrl = returnUrl   });	
                }
				else
				{
					tenantId = AbpSession.TenantId;
				}
			}
            else
            {

				if(AbpSession.TenantId == null)
                {
					returnUrl = NormalizeReturnUrl("/Falcon/HostDashboard");
				}
                else
                {
					returnUrl = NormalizeReturnUrl("/Falcon/Dashboard");
                }

				
				tenantId = AbpSession.TenantId;
			}

			// If this is the host then redirect to normal login
			var samlenabled = Convert.ToBoolean(_appConfiguration["Saml2:Enabled"]);			
			if ( ! samlenabled || disablesso.ToLower() == "true" )
            {

				if (!string.IsNullOrWhiteSpace(returnUrl))
				{
					returnUrl = NormalizeReturnUrl(returnUrl);

					if (!string.IsNullOrEmpty(RecordMatterId))
					{
						returnUrl += "&RecordMatterId=" + RecordMatterId;
					}
					if (!string.IsNullOrEmpty(RecordMatterItemId))
					{
						returnUrl += "&RecordMatterItemId=" + RecordMatterItemId;
					}
					if (!string.IsNullOrEmpty(version))
					{
						returnUrl += "&version=" + version;
					}
				}


				if (!string.IsNullOrEmpty(ss) && ss.Equals("true", StringComparison.OrdinalIgnoreCase) && AbpSession.UserId > 0)
				{
					var updateUserSignInTokenOutput =  _sessionAppService.UpdateUserSignInToken().Result;
					returnUrl = AddSingleSignInParametersToReturnUrl(returnUrl, updateUserSignInTokenOutput.SignInToken, AbpSession.UserId.Value, AbpSession.TenantId);
					return Redirect(returnUrl);
				}


				ViewBag.disablesso = disablesso ;
				ViewBag.ReturnUrl = returnUrl;

				ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;
				ViewBag.SingleSignIn = ss;
				ViewBag.UseCaptcha = UseCaptchaOnLogin();

				return View(
					view,
					new LoginFormViewModel
					{
						IsSelfRegistrationEnabled = IsSelfRegistrationEnabled(),
						IsTenantSelfRegistrationEnabled = IsTenantSelfRegistrationEnabled(),
						SuccessMessage = successMessage,
						UserNameOrEmailAddress = userNameOrEmailAddress
					});

			}
            else
            {
				var partnerName = _appConfiguration["PartnerName"];

                // To login automatically at the service provider, 
                // initiate single sign-on to the identity provider (SP-initiated SSO).            
                // The return URL is remembered as SAML relay state.

                if (AbpSession.TenantId == null)
                {
					await _samlServiceProvider.InitiateSsoAsync(partnerName, "&returnUrl=" + returnUrl);
				}
                else
                {
					await _samlServiceProvider.InitiateSsoAsync(partnerName, "tenantId=" + Convert.ToString(tenantId) + "&returnUrl=" + returnUrl );
				}
				
				return new EmptyResult();

			}

        }


		[Route("AssertionConsumerService")]
		[HttpPost]
        public async Task<IActionResult> AssertionConsumerService()
		{
			var returnUrl = NormalizeReturnUrl("/Falcon/Dashboard");

			var binding = new Saml2PostBinding();
			Saml2Configuration samlconfig = GetSAMLDecryptConfig(); 
			var saml2AuthnResponse = new Saml2AuthnResponse(samlconfig);	
			binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);

			int? tenantId = null;
			if (binding.RelayState != "null" && !string.IsNullOrEmpty(binding.RelayState))
            {
				var bindingitems = System.Web.HttpUtility.ParseQueryString(binding.RelayState);
				if (bindingitems["tenantId"] != null){
					tenantId = Convert.ToInt16(bindingitems["tenantId"]);
					AbpSession.Use(tenantId, null);
					_unitOfWorkManager.Current.SetTenantId(tenantId);
				}

				if (bindingitems["returnUrl"] != null)
				{
					returnUrl = Convert.ToString(bindingitems["returnUrl"]);
                    if (string.IsNullOrEmpty(returnUrl)) { returnUrl = NormalizeReturnUrl("/Falcon/Dashboard"); }
				}
			}

			if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
			{
				throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
			}

			//binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);
			var claimsprincipal = await saml2AuthnResponse.CreateSession(HttpContext, claimsTransform: (claimsPrincipal) => ClaimsTransform.Transform(claimsPrincipal));

			// Need the Email here
			var fltClaim = claimsprincipal.Claims.FirstOrDefault(e => e.Type.ToLower().Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
			var flt = fltClaim.Value;

			ExternalLoginInfo externalLoginInfo = new ExternalLoginInfo(claimsprincipal, "realme", flt, "realme");
			var externalLoginInfo2 = await _signInManager.GetExternalLoginInfoAsync();
			UserLoginInfo userLoginInfo = new UserLoginInfo("realme", flt, "realme");

			var tenancyName = GetTenancyNameOrNull();
			var loginResult = await _logInManager.LoginAsync(userLoginInfo, tenancyName);

			switch (loginResult.Result)
			{
				case AbpLoginResultType.Success:
					{

						await _signInManager.SignInAsync(loginResult.Identity, false);

						if (loginResult.Result == AbpLoginResultType.Success)
						{					 
							loginResult.User.SetSignInToken();
							returnUrl = AddSingleSignInParametersToReturnUrl(returnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);							
						}


						return RedirectToAction("SetTenant", new SetTenantViewModel() { TenantId = tenantId, ReturnUrl = returnUrl });
						//return Redirect(returnUrl);
					}
				case AbpLoginResultType.UnknownExternalLogin:

					RegisterViewModel registerviewmodel = new RegisterViewModel()
					{
						IsExternalLogin=true,
						RegisterSAMLLogin = true,
						ExternalLoginAuthSchema = "realme",
						FLT = flt,
						TenantId = tenantId
					};

					ExternalLoginInfo externalregistermodel = new ExternalLoginInfo(claimsprincipal, "realme", flt, "realme");
					return RegisterView(registerviewmodel);

				default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? externalLoginInfo.ProviderKey,
                        tenancyName
                    );
            }
		}

		public async Task<IActionResult> SingleLogoutService()
		{
			// Receive the single logout request or response.
			// If a request is received then single logout is being initiated by the identity provider.
			// If a response is received then this is in response to single logout having been initiated by the service provider.
			var sloResult = await _samlServiceProvider.ReceiveSloAsync();

			if (sloResult.IsResponse)
			{
				// SP-initiated SLO has completed.
				if (!string.IsNullOrEmpty(sloResult.RelayState))
				{
					return LocalRedirect(sloResult.RelayState);
				}

				return RedirectToPage("/Index");
			}
			else
			{
				// Logout locally.
				await _signInManager.SignOutAsync();

				// Respond to the IdP-initiated SLO request indicating successful logout.
				await _samlServiceProvider.SendSloAsync();
			}

			return new EmptyResult();
		}

		public async Task<IActionResult> ArtifactResolutionService()
		{
			// Resolve the HTTP artifact.
			// This is only required if supporting the HTTP-Artifact binding.
			await _samlServiceProvider.ResolveArtifactAsync();

			return new EmptyResult();
		}

		private Saml2Configuration GetSAMLConfig()
		{
			var config = new Saml2Configuration();

			config.Issuer = "DOCS_SIT";
			config.SingleSignOnDestination = new Uri(_appConfiguration["Saml2:SingleSignOnDestination"]);
			config.AudienceRestricted = false;

			config.CertificateValidationMode = X509CertificateValidationMode.None;
			config.SignatureAlgorithm = _appConfiguration["Saml2:SignatureAlgorithm"];

			var fname = _hostEnvironment.MapToPhysicalFilePath(_appConfiguration["Saml2:SigningCertificateFile"]);
			config.SigningCertificate = CertificateUtil.Load(fname, "npdocs");
			//config.CertificateValidationMode = X509CertificateValidationMode.None;

			config.SignAuthnRequest = false;


			return config;
		}

		private Saml2Configuration GetSAMLDecryptConfig()
		{
			var config = new Saml2Configuration();
			config.DecryptionCertificate = CertificateUtil.Load(_appConfiguration["Saml2:SigningCertificateFile"], _appConfiguration["Saml2:SigningCertificateFilePassword"]);
			config.AudienceRestricted = false;
			return config;
		}

		private async Task<ActionResult> RegisterForSamlLogin(ExternalLoginInfo externalLoginInfo)
        {
				var emailClaim = externalLoginInfo.Principal.Claims.FirstOrDefault(e => e.Type.ToLower().Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
				var email = emailClaim.Value;

				var username = email; // model.Name + model.Surname; 
				int cnt = 1;
				while (_userManager.Users.Any(e => e.UserName.ToLower() == username.ToLower()))
				{
					username = username + cnt;
				}

				RegisterViewModel model = new RegisterViewModel();

				model.UserName = username;
				model.EmailAddress = email;
				model.Password = await _userManager.CreateRandomPassword();

				var user = await _userRegistrationManager.RegisterAsync(
					model.Name = model.Name == null? string.Empty : model.Name,
					model.Surname = model.Surname == null ? string.Empty : model.Surname,
					model.EmailAddress,
					model.UserName,
					model.Password,
					true, 
					_appUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId, string.Empty),
					string.Empty
				);

				// CREATE STANDARD FOLDERS
				ACL aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
				//RecordsFolder
				Folder Rfolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Records", Description = "", ParentId = new Guid("00000000-0000-0000-0000-000000000000"), Type = "R" };
				await _folderManager.CreateOrEditFolder(aCL, Rfolder);
				await CurrentUnitOfWork.SaveChangesAsync();
				//TemplatesFolder
				aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
				Folder Tfolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Templates", Description = "", ParentId = new Guid("00000000-0000-0000-0000-000000000000"), Type = "T" };
				await _folderManager.CreateOrEditFolder(aCL, Tfolder);
				await CurrentUnitOfWork.SaveChangesAsync();
				//FormsFolder
				aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
				Folder Ffolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Forms", Description = "", ParentId = new Guid("00000000-0000-0000-0000-000000000000"), Type = "F" };
				await _folderManager.CreateOrEditFolder(aCL, Ffolder);
				await CurrentUnitOfWork.SaveChangesAsync();

				//Getting tenant-specific settings
				var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);
				if (model.IsExternalLogin)
				{
					Debug.Assert(externalLoginInfo != null);

					if (string.Equals(externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email), model.EmailAddress, StringComparison.OrdinalIgnoreCase))
					{
						user.IsEmailConfirmed = true;
					}

					user.Logins = new List<UserLogin>
					{
						new UserLogin
						{
							LoginProvider = externalLoginInfo.LoginProvider,
							ProviderKey = externalLoginInfo.ProviderKey,
							TenantId = user.TenantId
						}
					};
				}

				await _unitOfWorkManager.Current.SaveChangesAsync();
				var tenant = await _tenantManager.GetByIdAsync(user.TenantId.Value);

				//Directly login if possible
				if (user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin))
				{
					AbpLoginResult<Tenant, User> loginResult;
					if (externalLoginInfo != null)
					{
						loginResult = await _logInManager.LoginAsync(externalLoginInfo, tenant.TenancyName);
					}
					else
					{
						loginResult = await GetLoginResultAsync(user.UserName, model.Password, tenant.TenancyName);
					}

					if (loginResult.Result == AbpLoginResultType.Success)
					{
						await _signInManager.SignInAsync(loginResult.Identity, false);
						if (!string.IsNullOrEmpty(model.SingleSignIn) && model.SingleSignIn.Equals("true", StringComparison.OrdinalIgnoreCase) && loginResult.Result == AbpLoginResultType.Success)
						{
							var returnUrl = NormalizeReturnUrl(model.ReturnUrl);
							loginResult.User.SetSignInToken();
							returnUrl = AddSingleSignInParametersToReturnUrl(returnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
							return Redirect(returnUrl);
						}

						return Redirect(GetAppHomeUrl());
					}

					Logger.Warn("New registered user could not be login. This should not be normally. login result: " + loginResult.Result);
				}

				return View("RegisterResult", new RegisterResultViewModel
				{
					TenancyName = tenant.TenancyName,
					NameAndSurname = user.Name + " " + user.Surname,
					UserName = user.UserName,
					EmailAddress = user.EmailAddress,
					IsActive = user.IsActive,
					IsEmailConfirmationRequired = isEmailConfirmationRequiredForLogin
				});
			}
 
        private void CheckSamlLoginForSyntaqLogin(ExternalLoginInfo SamlLoginInfo)
		{

			UserLogin userlogin = new UserLogin();

			// STQ MODIFIED ////////
			var claim = SamlLoginInfo.Principal.Claims.FirstOrDefault(e => e.Type.ToLower().Contains("email") || e.Type.ToLower() == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
			var user = _userManager.Users.FirstOrDefault(e => e.EmailAddress.ToLower() == claim.Value.ToLower());
            // STQ MODIFIED ///////

            if (user == null)
            {
				// Create User and userLogin
				RegisterForSamlLogin(SamlLoginInfo);
			}

			if (user != null)
			{
				if (user.Logins == null)
				{
					user.Logins = new List<UserLogin>
					{
						new UserLogin
						{
							LoginProvider = SamlLoginInfo.LoginProvider,
							ProviderKey = SamlLoginInfo.ProviderKey,
							TenantId = user.TenantId
						}
					};

					_unitOfWorkManager.Current.SaveChanges();
					return;
				}

				if (!user.Logins.Any(e => e.ProviderKey == SamlLoginInfo.ProviderKey))
				{
					user.Logins.Add(new UserLogin()
					{
						LoginProvider = SamlLoginInfo.LoginProvider,
						ProviderKey = SamlLoginInfo.ProviderKey,
						TenantId = user.TenantId
					});
					_unitOfWorkManager.Current.SaveChanges();
				}
			}
            else
            {

            }
		}

		[HttpPost]
		[UnitOfWork]
		public virtual async Task<JsonResult> Login(LoginViewModel loginModel, string returnUrl = "", string returnUrlHash = "", string ss = "", string disablesso = "false")
		{
			returnUrl = NormalizeReturnUrl(returnUrl);
			if (!string.IsNullOrWhiteSpace(returnUrlHash))
			{
				returnUrl = returnUrl + returnUrlHash;
			}

			if (UseCaptchaOnLogin())
			{
				await _recaptchaValidator.ValidateAsync(HttpContext.Request.Form[RecaptchaValidator.RecaptchaResponseKey]);
			}

			var loginResult = await GetLoginResultAsync(loginModel.UsernameOrEmailAddress, loginModel.Password, GetTenancyNameOrNull());


			if (!string.IsNullOrEmpty(ss) && ss.Equals("true", StringComparison.OrdinalIgnoreCase) && loginResult.Result == AbpLoginResultType.Success)
			{
				loginResult.User.SetSignInToken();
				returnUrl = AddSingleSignInParametersToReturnUrl(returnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
			}

			if (_settingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser))
			{
				await _userManager.UpdateSecurityStampAsync(loginResult.User);
			}

			if (loginResult.User.ShouldChangePasswordOnNextLogin)
			{
				loginResult.User.SetNewPasswordResetCode();

				if (!string.IsNullOrEmpty(ss) && ss.Equals("true", StringComparison.OrdinalIgnoreCase) &&
					loginResult.Result == AbpLoginResultType.Success)
				{
					loginResult.User.SetSignInToken();
					returnUrl = AddSingleSignInParametersToReturnUrl(returnUrl, loginResult.User.SignInToken,
						loginResult.User.Id, loginResult.User.TenantId);
				}
			}

			var signInResult = await _signInManager.SignInOrTwoFactorAsync(loginResult, loginModel.RememberMe);
				if (signInResult.RequiresTwoFactor)
				{
					return Json(new AjaxResponse
					{
						TargetUrl = Url.Action(
							"SendSecurityCode",
							new
							{
								returnUrl,
								rememberMe = loginModel.RememberMe
							})
					});
				}

				Debug.Assert(signInResult.Succeeded);

				await UnitOfWorkManager.Current.SaveChangesAsync();

				return Json(new AjaxResponse { TargetUrl = returnUrl });

			
		}

		// Min view must return standard JSON
		[HttpPost]
		[UnitOfWork]
		public virtual async Task<JsonResult> Login_min(LoginViewModel loginModel, string returnUrl = "", string returnUrlHash = "", string ss = "")
		{

			if (!string.IsNullOrWhiteSpace(returnUrl))
			{
				returnUrl = NormalizeReturnUrl(returnUrl);
				if (!string.IsNullOrWhiteSpace(returnUrlHash))
				{
					returnUrl = returnUrl + returnUrlHash;
				}
			}

			var loginResult = await GetLoginResultAsync(loginModel.UsernameOrEmailAddress, loginModel.Password, GetTenancyNameOrNull());

			if (!string.IsNullOrEmpty(ss) && ss.Equals("true", StringComparison.OrdinalIgnoreCase) && loginResult.Result == AbpLoginResultType.Success)
			{
				loginResult.User.SetSignInToken();
				returnUrl = AddSingleSignInParametersToReturnUrl(returnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
			}

            if (loginResult.User.ShouldChangePasswordOnNextLogin)
            {
                loginResult.User.SetNewPasswordResetCode();

                return Json(new AjaxResponse
                {
                    TargetUrl = Url.Action(
                        "ResetPassword",
                        new ResetPasswordViewModel
                        {
                            TenantId = AbpSession.TenantId,
                            UserId = loginResult.User.Id,
                            ResetCode = loginResult.User.PasswordResetCode,
                            ReturnUrl = returnUrl,
                            SingleSignIn = ss
                        })
                });
            }

			var signInResult = await _signInManager.SignInOrTwoFactorAsync(loginResult, loginModel.RememberMe);
			if (signInResult.RequiresTwoFactor)
			{
				return Json(new AjaxResponse
				{
					TargetUrl = Url.Action(
						"SendSecurityCode",
						new
						{
							returnUrl,
							rememberMe = loginModel.RememberMe
						})
				});
			}
			Debug.Assert(signInResult.Succeeded);

			await UnitOfWorkManager.Current.SaveChangesAsync();
			return Json(new { TargetUrl = returnUrl, success = signInResult.Succeeded });

		}
		public async Task<ActionResult> Logout(string returnUrl = "")
		{
			await _signInManager.SignOutAsync();
			var userIdentifier = AbpSession.ToUserIdentifier();

			if (userIdentifier != null && _settingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser))
			{
				var user = await _userManager.GetUserAsync(userIdentifier);
				await _userManager.UpdateSecurityStampAsync(user);
			}

            if (!string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = NormalizeReturnUrl(returnUrl);
                return Redirect(returnUrl);
            }

			// STQ MODIFIED
            if (Convert.ToBoolean(_appConfiguration["Saml2:Enabled"]))
            {
				return RedirectToAction("LoggedOut");
			}
            else
            {
				return RedirectToAction("Login");
            }

			
		}

		// STQ MODIFIED
		public async Task<ActionResult> LoggedOut()
		{
			return View();
		}
		
		public async Task<ActionResult> SetTenant(SetTenantViewModel input)
		{
			return View(input);
		}

		private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
		{
			var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result,
                        usernameOrEmailAddress, tenancyName);
            }
        }

        private string AddSingleSignInParametersToReturnUrl(string returnUrl, string signInToken, long userId,
            int? tenantId)
        {
            returnUrl += (returnUrl.Contains("?") ? "&" : "?") +
                         "accessToken=" + signInToken +
                         "&userId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));
            if (tenantId.HasValue)
            {
                returnUrl += "&tenantId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(tenantId.Value.ToString()));
            }

            return returnUrl;
        }

        public ActionResult SessionLockScreen()
        {
            ViewBag.UseCaptcha = UseCaptchaOnLogin();
            return View();
        }

        #endregion

        #region Two Factor Auth

        public async Task<ActionResult> SendSecurityCode(string returnUrl, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            CheckCurrentTenant(await _signInManager.GetVerifiedTenantIdAsync());

            var userProviders = await _userManager.GetValidTwoFactorProvidersAsync(user);

            var factorOptions = userProviders.Select(
                userProvider =>
                    new SelectListItem
                    {
                        Text = userProvider,
                        Value = userProvider
                    }).ToList();

            return View(
                new SendSecurityCodeViewModel
                {
                    Providers = factorOptions,
                    ReturnUrl = returnUrl,
                    RememberMe = rememberMe
                }
            );
        }

        [HttpPost]
        public async Task<ActionResult> SendSecurityCode(SendSecurityCodeViewModel model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            CheckCurrentTenant(await _signInManager.GetVerifiedTenantIdAsync());

            if (model.SelectedProvider != GoogleAuthenticatorProvider.Name)
            {
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
                var message = L("EmailSecurityCodeBody", code);

                if (model.SelectedProvider == "Email")
                {
                    await _emailSender.SendAsync(await _userManager.GetEmailAsync(user), L("EmailSecurityCodeSubject"),
                        message);
                }
                else if (model.SelectedProvider == "Phone")
                {
                    await _smsSender.SendAsync(await _userManager.GetPhoneNumberAsync(user), message);
                }
            }

            return RedirectToAction(
                "VerifySecurityCode",
                new
                {
                    provider = model.SelectedProvider,
                    returnUrl = model.ReturnUrl,
                    rememberMe = model.RememberMe
                }
            );
        }

        public async Task<ActionResult> VerifySecurityCode(string provider, string returnUrl, bool rememberMe)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new UserFriendlyException(L("VerifySecurityCodeNotLoggedInErrorMessage"));
            }

            CheckCurrentTenant(await _signInManager.GetVerifiedTenantIdAsync());

            var isRememberBrowserEnabled = await IsRememberBrowserEnabledAsync();

            return View(
                new VerifySecurityCodeViewModel
                {
                    Provider = provider,
                    ReturnUrl = returnUrl,
                    RememberMe = rememberMe,
                    IsRememberBrowserEnabled = isRememberBrowserEnabled
                }
            );
        }

        [HttpPost]
        public async Task<JsonResult> VerifySecurityCode(VerifySecurityCodeViewModel model)
        {
            model.ReturnUrl = NormalizeReturnUrl(model.ReturnUrl);

            CheckCurrentTenant(await _signInManager.GetVerifiedTenantIdAsync());

            var result = await _signInManager.TwoFactorSignInAsync(
                model.Provider,
                model.Code,
                model.RememberMe,
                await IsRememberBrowserEnabledAsync() && model.RememberBrowser
            );

            if (result.Succeeded)
            {
                return Json(new AjaxResponse { TargetUrl = model.ReturnUrl });
            }

            if (result.IsLockedOut)
            {
                throw new UserFriendlyException(L("UserLockedOutMessage"));
            }

            throw new UserFriendlyException(L("InvalidSecurityCode"));
        }

        private Task<bool> IsRememberBrowserEnabledAsync()
        {
            return SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin
                .IsRememberBrowserEnabled);
        }

        #endregion

        #region Register

		public async Task<ActionResult> Register(string returnUrl = "", string ss = "", string view = "/Views/Account/Register.cshtml")
		{
			return RegisterView(new RegisterViewModel
			{
				PasswordComplexitySetting = await _passwordComplexitySettingStore.GetSettingsAsync(),
				ReturnUrl = returnUrl,
				SingleSignIn = ss
			}, view); //STQ MODIFIED
		}

		private ActionResult RegisterView(RegisterViewModel model, string view = "/Views/Account/Register.cshtml")
		{
			// Registration allowed if SAML (RealMe) etc
			if(! model.IsExternalLogin == true)
            {
				CheckSelfRegistrationIsEnabled();
			}
			
			ViewBag.UseCaptcha = !model.IsExternalLogin && UseCaptchaOnRegistration();
			return View(view, model); //STQ MODIFIED
		}

        [HttpPost]
        [UnitOfWork(IsolationLevel.ReadUncommitted)]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!model.IsExternalLogin && UseCaptchaOnRegistration())
                {
                    await _recaptchaValidator.ValidateAsync(
                        HttpContext.Request.Form[RecaptchaValidator.RecaptchaResponseKey]);
                }

				ExternalLoginInfo externalLoginInfo = null;
				if (model.IsExternalLogin)
				{

					// STQ MODIFIED
					if (model.RegisterSAMLLogin)
					{

						// If there is an esiting one with this email then create external UserLogin and redirect back to login
						var existinguser = _userManager.Users.FirstOrDefault(e => e.EmailAddress.ToLower() == model.EmailAddress.ToLower());

						if (existinguser != null)
						{
							existinguser.Logins = new List<UserLogin>
							{
								new UserLogin
								{
									LoginProvider = externalLoginInfo == null? model.ExternalLoginAuthSchema : externalLoginInfo.LoginProvider,
									ProviderKey = externalLoginInfo == null ? model.FLT : externalLoginInfo.ProviderKey,
									TenantId = existinguser.TenantId
								}
							};

							await _unitOfWorkManager.Current.SaveChangesAsync();

							// Go back and login again
							return RedirectToAction(nameof(Login));
						}


						var username = model.Name + model.Surname; int cnt = 1;
						while (_userManager.Users.Any(e => e.UserName.ToLower() == username.ToLower()))
						{
							username = model.Name + model.Surname + cnt;
							cnt++;
						}
                        model.Password = await _userManager.CreateRandomPassword();
                        model.UserName = username;
					}
					else
					{
						externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
						if (externalLoginInfo == null)
						{
							throw new Exception("Can not external login!");
						}

						using (var providerManager =
							   _externalLoginInfoManagerFactory.GetExternalLoginInfoManager(externalLoginInfo
								   .LoginProvider))
						{
							model.UserName =
								providerManager.Object.GetUserNameFromClaims(externalLoginInfo.Principal.Claims.ToList());
						}

						model.Password = await _userManager.CreateRandomPassword();
					}
                }
                else
                {
                    if (model.UserName.IsNullOrEmpty() || model.Password.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(L("FormIsNotValidMessage"));
                    }
                }

				

				var user = await _userRegistrationManager.RegisterAsync(
					model.Name,
					model.Surname,
					model.EmailAddress,
					model.UserName,
					model.Password,
					false,
					_appUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId, string.Empty),
					string.Empty
				);
 
				// STQ MODIFIED
				// CREATE STANDARD FOLDERS
				ACL aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
				//RecordsFolder
				Folder Rfolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Records", Description = "", ParentId = new Guid("00000000-0000-0000-0000-000000000000"), Type = "R" };
				await _folderManager.CreateOrEditFolder(aCL, Rfolder);
				await CurrentUnitOfWork.SaveChangesAsync();
				//TemplatesFolder
				aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
				Folder Tfolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Templates", Description = "", ParentId = new Guid("00000000-0000-0000-0000-000000000000"), Type = "T" };
				await _folderManager.CreateOrEditFolder(aCL, Tfolder);
				await CurrentUnitOfWork.SaveChangesAsync();
				//FormsFolder
				aCL = new ACL() { UserId = user.Id, TenantId = AbpSession.TenantId };
				Folder Ffolder = new Folder() { TenantId = AbpSession.TenantId, Name = "Your Forms", Description = "", ParentId = new Guid("00000000-0000-0000-0000-000000000000"), Type = "F" };
				await _folderManager.CreateOrEditFolder(aCL, Ffolder);
				await CurrentUnitOfWork.SaveChangesAsync();
 
				//Getting tenant-specific settings
				var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

				// STQ MODIFIED
				if (model.IsExternalLogin)
				{			
                    if ( model.RegisterSAMLLogin)
                    {
						user.IsEmailConfirmed = true;
						Logger.Info("Register ExternalLogin Email Confirmed");
                    }
                    else
                    {
						if (string.Equals(externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email), model.EmailAddress, StringComparison.OrdinalIgnoreCase))
						{
							user.IsEmailConfirmed = true;
						}
					}

					user.Logins = new List<UserLogin>
					{
						new UserLogin
						{
							LoginProvider = externalLoginInfo == null? model.ExternalLoginAuthSchema : externalLoginInfo.LoginProvider,
							ProviderKey = externalLoginInfo == null ? model.FLT : externalLoginInfo.ProviderKey,
							TenantId = user.TenantId
						}
					};
				}

                await _unitOfWorkManager.Current.SaveChangesAsync();

                Debug.Assert(user.TenantId != null);

                var tenant = await _tenantManager.GetByIdAsync(user.TenantId.Value);

                //Directly login if possible
                if (user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin))
                {
                    AbpLoginResult<Tenant, User> loginResult;
                    if (externalLoginInfo != null)
                    {
                        loginResult = await _logInManager.LoginAsync(externalLoginInfo, tenant.TenancyName);
                    }
                    else
                    {
                        loginResult = await GetLoginResultAsync(user.UserName, model.Password, tenant.TenancyName);
                    }

                    if (loginResult.Result == AbpLoginResultType.Success)
                    {
                        await _signInManager.SignInAsync(loginResult.Identity, false);
                        if (!string.IsNullOrEmpty(model.SingleSignIn) &&
                            model.SingleSignIn.Equals("true", StringComparison.OrdinalIgnoreCase) &&
                            loginResult.Result == AbpLoginResultType.Success)
                        {
                            var returnUrl = NormalizeReturnUrl(model.ReturnUrl);
                            loginResult.User.SetSignInToken();
                            returnUrl = AddSingleSignInParametersToReturnUrl(returnUrl, loginResult.User.SignInToken,
                                loginResult.User.Id, loginResult.User.TenantId);
                            return Redirect(returnUrl);
                        }

                        return Redirect(GetAppHomeUrl());
                    }

                    Logger.Warn("New registered user could not be login. This should not be normally. login result: " +
                                loginResult.Result);
                }

                return View("RegisterResult", new RegisterResultViewModel
                {
                    TenancyName = tenant.TenancyName,
                    NameAndSurname = user.Name + " " + user.Surname,
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                    IsActive = user.IsActive,
                    IsEmailConfirmationRequired = isEmailConfirmationRequiredForLogin
                });
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.UseCaptcha = !model.IsExternalLogin && UseCaptchaOnRegistration();
                ViewBag.ErrorMessage = ex.Message;

				model.PasswordComplexitySetting = await _passwordComplexitySettingStore.GetSettingsAsync();

				return View("Register", model);
			}
		}

        private bool UseCaptchaOnRegistration()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                //Host users can not register
                throw new InvalidOperationException();
            }

            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration);
        }

        private bool UseCaptchaOnLogin()
        {
            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnLogin);
        }

        private void CheckSelfRegistrationIsEnabled()
        {
            if (!IsSelfRegistrationEnabled())
            {
                throw new UserFriendlyException(L("SelfUserRegistrationIsDisabledMessage_Detail"));
            }
        }

        private bool IsSelfRegistrationEnabled()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return false; //No registration enabled for host users!
            }

            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowSelfRegistration);
        }

        private bool IsTenantSelfRegistrationEnabled()
        {
            if (AbpSession.TenantId.HasValue)
            {
                return false;
            }

            return SettingManager.GetSettingValue<bool>(AppSettings.TenantManagement.AllowSelfRegistration);
        }

        #endregion

        #region ForgotPassword / ResetPassword

        public ActionResult ForgotPassword()
        {
            return View();
        }

		/// <summary>
        /// STQ MODIFIED
		/// Embedded view of password reset
		/// No Form
		/// </summary>
		/// <returns></returns>
		public ActionResult ForgotPasswordMin()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> SendPasswordResetLink(SendPasswordResetLinkViewModel model)
		{
			await _accountAppService.SendPasswordResetCode(
				new SendPasswordResetCodeInput
				{
					EmailAddress = model.EmailAddress,
					PasswordResetReturnUrl = model.PasswordResetReturnUrl
				});

            return Json(new AjaxResponse());
        }

        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            await SwitchToTenantIfNeeded(model.TenantId);

            var user = await _userManager.GetUserByIdAsync(model.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != model.ResetCode)
            {
                throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
            }

            model.PasswordComplexitySetting = await _passwordComplexitySettingStore.GetSettingsAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordInput input)
        {
			// STQ MODIFIED
            await SwitchToTenantIfNeeded(input.TenantId);

            var output = await _accountAppService.ResetPassword(input);

            if (output.CanLogin)
            {
                var user = await _userManager.GetUserByIdAsync(input.UserId);
                await _signInManager.SignInAsync(user, false);

                if (!string.IsNullOrEmpty(input.SingleSignIn) &&
                    input.SingleSignIn.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    user.SetSignInToken();
                    var returnUrl =
                        AddSingleSignInParametersToReturnUrl(input.ReturnUrl, user.SignInToken, user.Id, user.TenantId);
                    return Redirect(returnUrl);
                }
            }

            return Redirect(NormalizeReturnUrl(input.ReturnUrl));
        }

        #endregion

        #region Email activation / confirmation

        public ActionResult EmailActivation()
        {
            return View();
        }

        [HttpPost]
        public virtual async Task<JsonResult> SendEmailActivationLink(SendEmailActivationLinkInput model)
        {
            await _accountAppService.SendEmailActivationLink(model);
            return Json(new AjaxResponse());
        }

        public virtual async Task<ActionResult> EmailConfirmation(EmailConfirmationViewModel input)
        {
            await SwitchToTenantIfNeeded(input.TenantId);
            await _accountAppService.ActivateEmail(input);
            return RedirectToAction(
                "Login",
                new
                {
                    successMessage = L("YourEmailIsConfirmedMessage"),
                    userNameOrEmailAddress = (await _userManager.GetUserByIdAsync(input.UserId)).UserName
                });
        }

        #endregion

        #region External Login

        [HttpPost]
        public ActionResult ExternalLogin(string provider, string returnUrl, string ss = "")
        {
            var redirectUrl = Url.Action(
                "ExternalLoginCallback",
                "Account",
                new
                {
                    ReturnUrl = returnUrl,
                    authSchema = provider,
                    ss = ss
                });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl, string remoteError = null,
            string ss = "")
        {
            returnUrl = NormalizeReturnUrl(returnUrl);

            if (remoteError != null)
            {
                Logger.Error("Remote Error in ExternalLoginCallback: " + remoteError);
                throw new UserFriendlyException(L("CouldNotCompleteLoginOperation"));
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                Logger.Warn("Could not get information from external login.");
                return RedirectToAction(nameof(Login));
            }

            var tenancyName = GetTenancyNameOrNull();

			//STQ MODIFIED
			// Check if Account existing with Email address already and does not have an AbpUserLogin row
			// Connects existing Syntaq Logins to external logins automatically
			CheckExternalLoginForSyntaqLogin(externalLoginInfo);

			var loginResult = await _logInManager.LoginAsync(externalLoginInfo, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                {
                    await _signInManager.SignInAsync(loginResult.Identity, false);

                    if (!string.IsNullOrEmpty(ss) && ss.Equals("true", StringComparison.OrdinalIgnoreCase) &&
                        loginResult.Result == AbpLoginResultType.Success)
                    {
                        loginResult.User.SetSignInToken();
                        returnUrl = AddSingleSignInParametersToReturnUrl(returnUrl, loginResult.User.SignInToken,
                            loginResult.User.Id, loginResult.User.TenantId);
                    }

						return Redirect(returnUrl);
					}
				case AbpLoginResultType.UnknownExternalLogin:

					// If can match up user then login as this user instead
					//var password = await _userManager.CreateRandomPassword();

					return await RegisterForExternalLogin(externalLoginInfo);
				default:
					throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
						loginResult.Result,
						externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? externalLoginInfo.ProviderKey,
						tenancyName
					);
			}
		}

		private void CheckExternalLoginForSyntaqLogin(ExternalLoginInfo externalLoginInfo)
		{

			UserLogin userlogin = new UserLogin();

			// USE the FLT
			(string name, string surname) nameInfo;
			using (var providerManager = _externalLoginInfoManagerFactory.GetExternalLoginInfoManager(externalLoginInfo.LoginProvider))
			{
				nameInfo = providerManager.Object.GetNameAndSurnameFromClaims(externalLoginInfo.Principal.Claims.ToList(), _identityOptions);
			}

			// STQ MODIFIED ////////
			var FLTClaim = externalLoginInfo.Principal.Claims.FirstOrDefault(e => e.Type.ToLower().Contains("email") || e.Type.ToLower() == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
			var user = _userManager.Users.FirstOrDefault(e => e.FLT == FLTClaim.Value.ToLower());
			// STQ MODIFIED ///////
			
			if (user != null)
			{
				if (user.Logins == null)
				{
					user.Logins = new List<UserLogin>
					{
						new UserLogin
						{
							LoginProvider = externalLoginInfo.LoginProvider,
							ProviderKey = externalLoginInfo.ProviderKey,
							TenantId = user.TenantId
						}
					};

					_unitOfWorkManager.Current.SaveChanges();
					return;
				}

				if (!user.Logins.Any(e => e.ProviderKey == externalLoginInfo.ProviderKey))
				{
					user.Logins.Add(new UserLogin()
					{
						LoginProvider = externalLoginInfo.LoginProvider,
						ProviderKey = externalLoginInfo.ProviderKey,
						TenantId = user.TenantId
					});
					_unitOfWorkManager.Current.SaveChanges();
				}
			}


		}

		private async Task<ActionResult> RegisterForExternalLogin(ExternalLoginInfo externalLoginInfo)
		{
			var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            (string name, string surname) nameInfo;
            using (var providerManager =
                   _externalLoginInfoManagerFactory.GetExternalLoginInfoManager(externalLoginInfo.LoginProvider))
            {
                nameInfo = providerManager.Object.GetNameAndSurnameFromClaims(
                    externalLoginInfo.Principal.Claims.ToList(), _identityOptions);
            }

            var viewModel = new RegisterViewModel
            {
                EmailAddress = email,
                Name = nameInfo.name,
                Surname = nameInfo.surname,
                IsExternalLogin = true,
                ExternalLoginAuthSchema = externalLoginInfo.LoginProvider
            };

            if (nameInfo.name != null &&
                nameInfo.surname != null &&
                email != null)
            {
                return await Register(viewModel);
            }

            return RegisterView(viewModel);
        }

        #endregion

        #region Impersonation

        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users_Impersonation)]
        public virtual async Task<JsonResult> ImpersonateUser([FromBody] ImpersonateUserInput input)
        {
            var output = await _accountAppService.ImpersonateUser(input);

            await _signInManager.SignOutAsync();

            return Json(new AjaxResponse
            {
                TargetUrl = _webUrlService.GetSiteRootAddress(output.TenancyName) +
                            "Account/ImpersonateSignIn?tokenId=" + output.ImpersonationToken
            });
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenants_Impersonation)]
        public virtual async Task<JsonResult> ImpersonateTenant([FromBody] ImpersonateTenantInput input)
        {
            var output = await _accountAppService.ImpersonateTenant(input);

            await _signInManager.SignOutAsync();

            return Json(new AjaxResponse
            {
                TargetUrl = _webUrlService.GetSiteRootAddress(output.TenancyName) +
                            "Account/ImpersonateSignIn?tokenId=" + output.ImpersonationToken
            });
        }

        public virtual async Task<ActionResult> ImpersonateSignIn(string tokenId)
        {
            await ClearGetScriptsResponsePerUserCache();

            var result = await _impersonationManager.GetImpersonatedUserAndIdentity(tokenId);
            await _signInManager.SignInAsync(result.Identity, false);
            return RedirectToAppHome();
        }

        [AbpMvcAuthorize]
        public virtual async Task<JsonResult> DelegatedImpersonate([FromBody] DelegatedImpersonateInput input)
        {
            var output = await _accountAppService.DelegatedImpersonate(new DelegatedImpersonateInput
            {
                UserDelegationId = input.UserDelegationId
            });

            await _signInManager.SignOutAsync();

            return Json(new AjaxResponse
            {
                TargetUrl = _webUrlService.GetSiteRootAddress(output.TenancyName) +
                            "Account/DelegatedImpersonateSignIn?userDelegationId=" + input.UserDelegationId +
                            "&tokenId=" + output.ImpersonationToken
            });
        }

        public virtual async Task<ActionResult> DelegatedImpersonateSignIn(long userDelegationId, string tokenId)
        {
            await ClearGetScriptsResponsePerUserCache();

            var userDelegation = await _userDelegationManager.GetAsync(userDelegationId);
            var result = await _impersonationManager.GetImpersonatedUserAndIdentity(tokenId);

            if (userDelegation.SourceUserId != result.User.Id)
            {
                throw new UserFriendlyException("User delegation error...");
            }

            await _signInManager.SignInWithClaimsAsync(result.User, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = userDelegation.EndTime.ToUniversalTime()
            }, result.Identity.Claims);

            return RedirectToAppHome();
        }

        public virtual JsonResult IsImpersonatedLogin()
        {
            return Json(new AjaxResponse { Result = AbpSession.ImpersonatorUserId.HasValue });
        }

        public virtual async Task<JsonResult> BackToImpersonator()
        {
            var output = await _accountAppService.BackToImpersonator();

            await _signInManager.SignOutAsync();

            return Json(new AjaxResponse
            {
                TargetUrl = _webUrlService.GetSiteRootAddress(output.TenancyName) +
                            "Account/ImpersonateSignIn?tokenId=" + output.ImpersonationToken
            });
        }

        private async Task ClearGetScriptsResponsePerUserCache()
        {
            if (!_getScriptsResponsePerUserConfiguration.IsEnabled)
            {
                return;
            }

            await _cachedUniqueKeyPerUser.RemoveKeyAsync(GetScriptsResponsePerUserCache.CacheName);
        }

        #endregion

        #region Linked Account

        [AbpMvcAuthorize]
        public virtual async Task<JsonResult> SwitchToLinkedAccount([FromBody] SwitchToLinkedAccountInput model)
        {
            var output = await _accountAppService.SwitchToLinkedAccount(model);

            await _signInManager.SignOutAsync();

            return Json(new AjaxResponse
            {
                TargetUrl = _webUrlService.GetSiteRootAddress(output.TenancyName) +
                            "Account/SwitchToLinkedAccountSignIn?tokenId=" + output.SwitchAccountToken
            });
        }

        public virtual async Task<ActionResult> SwitchToLinkedAccountSignIn(string tokenId)
        {
            var result = await _userLinkManager.GetSwitchedUserAndIdentity(tokenId);

            await _signInManager.SignInAsync(result.Identity, false);
            return RedirectToAppHome();
        }

        #endregion

        #region Change Tenant

        public async Task<ActionResult> TenantChangeModal()
        {
            var loginInfo = await _sessionCache.GetCurrentLoginInformationsAsync();
            return View("/Views/Shared/Components/TenantChange/_ChangeModal.cshtml", new ChangeModalViewModel
            {
                TenancyName = loginInfo.Tenant?.TenancyName
            });
        }

        #endregion

        #region Common

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private void CheckCurrentTenant(int? tenantId)
        {
            if (AbpSession.TenantId != tenantId)
            {
                throw new Exception(
                    $"Current tenant is different than given tenant. AbpSession.TenantId: {AbpSession.TenantId}, given tenantId: {tenantId}");
            }
        }

        private async Task SwitchToTenantIfNeeded(int? tenantId)
        {
            if (tenantId != AbpSession.TenantId)
            {
                if (_webUrlService.SupportsTenancyNameInUrl)
                {
                    throw new InvalidOperationException($"Given tenantid ({tenantId}) does not match to tenant's URL!");
                }

                SetTenantIdCookie(tenantId);
                CurrentUnitOfWork.SetTenantId(tenantId);
                await _signInManager.SignOutAsync();
            }
        }

        #endregion

        #region Helpers

        public ActionResult RedirectToAppHome()
        {
            return RedirectToAction("Index", "Home", new { area = "Falcon" });
        }

        public string GetAppHomeUrl()
        {
            return Url.Action("Index", "Home", new { area = "Falcon" });
        }

        private string NormalizeReturnUrl(string returnUrl, Func<string> defaultValueBuilder = null)
        {
            if (defaultValueBuilder == null)
            {
                defaultValueBuilder = GetAppHomeUrl;
            }

            if (returnUrl.IsNullOrEmpty())
            {
                return defaultValueBuilder();
            }

			// STQ TODO This was added in 11.2
			// Strips out return URLS for logged in users
			// Support query lodged with ASPNETZERO 
			//if (AbpSession.UserId.HasValue)
			//{
			//	return defaultValueBuilder();
			//}

			if (returnUrl.ToLower().Contains("legalconsolidated.com.au") || returnUrl.ToLower().Contains("demo.syntaq.com"))
			{
				return returnUrl;
			}

			if (Url.IsLocalUrl(returnUrl) || _webUrlService.GetRedirectAllowedExternalWebSites().Any(returnUrl.Contains))
            {
                return returnUrl;
            }

            return defaultValueBuilder();
        }

        #endregion

        #region Etc

        [AbpMvcAuthorize]
        public async Task<ActionResult> TestNotification(string message = "", string severity = "info")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            await _appNotifier.SendMessageAsync(
                AbpSession.ToUserIdentifier(),
                message,
                severity.ToPascalCase().ToEnum<NotificationSeverity>()
            );

            return Content("Sent notification: " + message);
        }

        #endregion
    }

	// STQ ADDED
	public class STQSaml2
	{
		public string Issuer { get; set; }
		public string SingleSignOnDestination { get; set; }
		public string AssertionConsumerServiceUrl { get; set; }
		public string SignatureAlgorithm { get; set; }
		public string SigningCertificateFile { get; set; }
		public string AllowedAudienceUris { get; set; }
		public string SPNameQualifier { get; set; }
	}

}
