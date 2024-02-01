using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Syntaq.Falcon.Authorization.Roles;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Editions;
using Syntaq.Falcon.MultiTenancy.Demo;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Syntaq.Falcon.Notifications;
using System;
using System.Diagnostics;
using Abp.BackgroundJobs;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using Syntaq.Falcon.MultiTenancy.Payments;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Folders;
using Abp.Localization;

namespace Syntaq.Falcon.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// </summary>
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IUserEmailer _userEmailer;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<SubscribableEdition> _subscribableEditionRepository;
        protected readonly IBackgroundJobManager _backgroundJobManager;

        //STQ MODIFIED
        private readonly FolderManager _folderManager;
        public TenantManager(

            //STQ MODIFIED
            FolderManager folderManager,

            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            UserManager userManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IAbpZeroFeatureValueStore featureValueStore,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IPasswordHasher<User> passwordHasher,
            IRepository<SubscribableEdition> subscribableEditionRepository,
            IBackgroundJobManager backgroundJobManager) : base(
            tenantRepository,
            tenantFeatureRepository,
            editionManager,
            featureValueStore
        )
        {
            //STQ MODIFIED
            _folderManager = folderManager;

            AbpSession = NullAbpSession.Instance;

            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userManager = userManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _passwordHasher = passwordHasher;
            _subscribableEditionRepository = subscribableEditionRepository;
            _backgroundJobManager = backgroundJobManager;
        }

        public async Task<int> CreateWithAdminUserAsync(
            string tenancyName,
            string name,
            string adminPassword,
            string adminEmailAddress,
            string connectionString,
            bool isActive,
            int? editionId,
            bool shouldChangePasswordOnNextLogin,
            bool sendActivationEmail,
            DateTime? subscriptionEndDate,
            bool isInTrialPeriod,
            string emailActivationLink,
            string adminName = null,
            string adminSurname = null
            )
        {
            int newTenantId;
            long newAdminId;

            await CheckEditionAsync(editionId, isInTrialPeriod);

            if (isInTrialPeriod && !subscriptionEndDate.HasValue)
            {
                throw new UserFriendlyException(LocalizationManager.GetString(
                    FalconConsts.LocalizationSourceName, "TrialWithoutEndDateErrorMessage"));
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //Create tenant
                var tenant = new Tenant(tenancyName, name)
                {
                    IsActive = isActive,
                    EditionId = editionId,
                    SubscriptionEndDateUtc = subscriptionEndDate?.ToUniversalTime(),
                    IsInTrialPeriod = isInTrialPeriod,
                    ConnectionString = connectionString.IsNullOrWhiteSpace()
                        ? null
                        : SimpleStringCipher.Instance.Encrypt(connectionString)
                };

                await CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get new tenant's id.

                //Create tenant database
                _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

                //We are working entities of new tenant, so changing tenant filter
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    //Create static roles for new tenant

                    // STQ MODIFIED ERROR 
                    try {
                        CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                    }
                    catch (Exception ex){
                        var temp = ex;
                    }
                    
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    await _roleManager.GrantAllPermissionsAsync(adminRole);

                    //User role should be default
                    // STQ MODIFIED AUTHOR ROLE IS THE DEFAULT ONE 
                    var userRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Author);
                    userRole.IsDefault = true;
                    CheckErrors(await _roleManager.UpdateAsync(userRole));

                    //Create admin user for the tenant
                    var adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress, adminName, name, adminSurname);
                    adminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                    adminUser.IsActive = true;

                    if (adminPassword.IsNullOrEmpty())
                    {
                        adminPassword = await _userManager.CreateRandomPassword();
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
                        foreach (var validator in _userManager.PasswordValidators)
                        {
                            CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                        }
                    }

                    adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);

                    CheckErrors(await _userManager.CreateAsync(adminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                    //Assign admin user to admin role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                    //STQ MODIFIED
					//DefaultFolders
					//RecordsFolder
						ACL aCL = new ACL() { UserId = adminUser.Id, TenantId = tenant.Id };
						Folder Rfolder = new Folder() { TenantId = tenant.Id, Name = "Your Records", Description = "", ParentId = null, Type = "R" };
						await _folderManager.CreateOrEditFolder(aCL, Rfolder);
						await _unitOfWorkManager.Current.SaveChangesAsync();
					//TemplatesFolder
						aCL = new ACL() { UserId = adminUser.Id, TenantId = tenant.Id };
						Folder Tfolder = new Folder() { TenantId = tenant.Id, Name = "Your Templates", Description = "", ParentId = null, Type = "T" };
						await _folderManager.CreateOrEditFolder(aCL, Tfolder);
						await _unitOfWorkManager.Current.SaveChangesAsync();
					//FormsFolder
						aCL = new ACL() { UserId = adminUser.Id, TenantId = tenant.Id };
						Folder Ffolder = new Folder() { TenantId = tenant.Id, Name = "Your Forms", Description = "", ParentId = null, Type = "F" };
						await _folderManager.CreateOrEditFolder(aCL, Ffolder);
						await _unitOfWorkManager.Current.SaveChangesAsync();

					//Notifications
					await _appNotifier.WelcomeToTheApplicationAsync(adminUser);

                    //Send activation email
                    if (sendActivationEmail)
                    {
                        adminUser.SetNewEmailConfirmationCode();
                        await _userEmailer.SendEmailActivationLinkAsync(adminUser, emailActivationLink, adminPassword);
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    await _backgroundJobManager.EnqueueAsync<TenantDemoDataBuilderJob, int>(tenant.Id);

                    newTenantId = tenant.Id;
                    newAdminId = adminUser.Id;
                }

                await uow.CompleteAsync();
            }

            //Used a second UOW since UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(newTenantId))
                {
                    await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(
                        new UserIdentifier(newTenantId, newAdminId));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }

            return newTenantId;
        }

        public async Task CheckEditionAsync(int? editionId, bool isInTrialPeriod)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (!editionId.HasValue || !isInTrialPeriod)
                {
                    return;
                }

                var edition = await _subscribableEditionRepository.GetAsync(editionId.Value);
                if (!edition.IsFree)
                {
                    return;
                }

                var error = LocalizationManager.GetSource(FalconConsts.LocalizationSourceName)
                    .GetString("FreeEditionsCannotHaveTrialVersions");
                throw new UserFriendlyException(error);
            });
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public decimal GetUpgradePrice(SubscribableEdition currentEdition, SubscribableEdition targetEdition,
            int totalRemainingHourCount, PaymentPeriodType paymentPeriodType)
        {
            int numberOfHoursPerDay = 24;

            var totalRemainingDayCount = totalRemainingHourCount / numberOfHoursPerDay;
            var unusedPeriodCount = totalRemainingDayCount / (int) paymentPeriodType;
            var unusedHoursCount = totalRemainingHourCount % ((int) paymentPeriodType * numberOfHoursPerDay);

            decimal currentEditionPriceForUnusedPeriod = 0;
            decimal targetEditionPriceForUnusedPeriod = 0;

            var currentEditionPrice = currentEdition.GetPaymentAmount(paymentPeriodType);
            var targetEditionPrice = targetEdition.GetPaymentAmount(paymentPeriodType);

            if (currentEditionPrice > 0)
            {
                currentEditionPriceForUnusedPeriod = currentEditionPrice * unusedPeriodCount;
                currentEditionPriceForUnusedPeriod += (currentEditionPrice / (int) paymentPeriodType) /
                    numberOfHoursPerDay * unusedHoursCount;
            }

            if (targetEditionPrice > 0)
            {
                targetEditionPriceForUnusedPeriod = targetEditionPrice * unusedPeriodCount;
                targetEditionPriceForUnusedPeriod += (targetEditionPrice / (int) paymentPeriodType) /
                    numberOfHoursPerDay * unusedHoursCount;
            }

            return targetEditionPriceForUnusedPeriod - currentEditionPriceForUnusedPeriod;
        }

        public async Task<Tenant> UpdateTenantAsync(int tenantId, bool isActive, bool? isInTrialPeriod,
            PaymentPeriodType? paymentPeriodType, int editionId, EditionPaymentType editionPaymentType)
        {
            var tenant = await FindByIdAsync(tenantId);

            tenant.IsActive = isActive;

            if (isInTrialPeriod.HasValue)
            {
                tenant.IsInTrialPeriod = isInTrialPeriod.Value;
            }

            tenant.EditionId = editionId;

            if (paymentPeriodType.HasValue)
            {
                tenant.UpdateSubscriptionDateForPayment(paymentPeriodType.Value, editionPaymentType);
            }

            return tenant;
        }

        public async Task<EndSubscriptionResult> EndSubscriptionAsync(Tenant tenant, SubscribableEdition edition,
            DateTime nowUtc)
        {
            if (tenant.EditionId == null || tenant.HasUnlimitedTimeSubscription())
            {
                throw new Exception(
                    $"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} tenant has unlimited time subscription!");
            }

            Debug.Assert(tenant.SubscriptionEndDateUtc != null, "tenant.SubscriptionEndDateUtc != null");

            var subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value;
            if (!tenant.IsInTrialPeriod)
            {
                subscriptionEndDateUtc =
                    tenant.SubscriptionEndDateUtc.Value.AddDays(edition.WaitingDayAfterExpire ?? 0);
            }

            if (subscriptionEndDateUtc >= nowUtc)
            {
                throw new Exception(
                    $"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} since subscription has not expired yet!");
            }

            if (!tenant.IsInTrialPeriod && edition.ExpiringEditionId.HasValue)
            {
                tenant.EditionId = edition.ExpiringEditionId.Value;
                tenant.SubscriptionEndDateUtc = null;

                await UpdateAsync(tenant);

                return EndSubscriptionResult.AssignedToAnotherEdition;
            }

            tenant.IsActive = false;
            tenant.IsInTrialPeriod = false;

            await UpdateAsync(tenant);

            return EndSubscriptionResult.TenantSetInActive;
        }

        public async Task<int?> GetTenantId(string tenantname)
        {
            var tenant = await  FindByTenancyNameAsync (tenantname);
            return tenant?.Id ;
        }

        private static decimal GetMonthlyCalculatedPrice(SubscribableEdition currentEdition, SubscribableEdition upgradeEdition, int remainingDaysCount)
		{
			decimal currentUnusedPrice = 0;
			decimal upgradeUnusedPrice = 0;

			if (currentEdition.MonthlyPrice.HasValue)
			{
				currentUnusedPrice = (currentEdition.MonthlyPrice.Value / (int)PaymentPeriodType.Monthly) * remainingDaysCount;
			}

			if (upgradeEdition.MonthlyPrice.HasValue)
			{
				upgradeUnusedPrice = (upgradeEdition.MonthlyPrice.Value / (int)PaymentPeriodType.Monthly) * remainingDaysCount;
			}

			var additionalPrice = upgradeUnusedPrice - currentUnusedPrice;
			return additionalPrice;
		}

		private static decimal GetAnnualCalculatedPrice(SubscribableEdition currentEdition, SubscribableEdition upgradeEdition, int remainingYearsCount)
		{
			var currentUnusedPrice = (currentEdition.AnnualPrice ?? 0) * remainingYearsCount;
			var upgradeUnusedPrice = (upgradeEdition.AnnualPrice ?? 0) * remainingYearsCount;

			var additionalPrice = upgradeUnusedPrice - currentUnusedPrice;
			return additionalPrice;
		}

		public override Task UpdateAsync(Tenant tenant)
		{
			if (tenant.IsInTrialPeriod && !tenant.SubscriptionEndDateUtc.HasValue)
			{
				throw new UserFriendlyException(LocalizationManager.GetString(FalconConsts.LocalizationSourceName, "TrialWithoutEndDateErrorMessage"));
			}

			return base.UpdateAsync(tenant);
		}

        //STQ MODIFIED
        public async Task<string> GetTenantName(int tenantId)
        {
            var tenant = await FindByIdAsync(tenantId);
            return tenant.Name ?? "Host";
        }
    }
}
