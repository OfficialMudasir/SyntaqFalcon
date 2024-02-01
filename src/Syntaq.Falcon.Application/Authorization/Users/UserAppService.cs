using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Syntaq.Falcon.Authorization.Permissions;
using Syntaq.Falcon.Authorization.Permissions.Dto;
using Syntaq.Falcon.Authorization.Roles;
using Syntaq.Falcon.Authorization.Users.Dto;
using Syntaq.Falcon.Authorization.Users.Exporting;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Net.Emailing;
using Syntaq.Falcon.Notifications;
using Syntaq.Falcon.Url;
using Syntaq.Falcon.Organizations.Dto;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.AccessControlList;
using Abp.Domain.Uow;

namespace Syntaq.Falcon.Authorization.Users
{

    public class UserAppService : FalconAppServiceBase, IUserAppService
    {
        public IAppUrlService AppUrlService { get; set; }

		private readonly RoleManager _roleManager;
		private readonly FolderManager _folderManager;
		private readonly ACLManager _ACLManager;
		private readonly IUserEmailer _userEmailer;
		private readonly IUserListExcelExporter _userListExcelExporter;
		private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
		private readonly IAppNotifier _appNotifier;
		private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
		private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;
		private readonly IRepository<UserRole, long> _userRoleRepository;
		private readonly IRepository<Role> _roleRepository;
		private readonly IUserPolicy _userPolicy;
		private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
		private readonly IPasswordHasher<User> _passwordHasher;
		private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
		private readonly IRoleManagementConfig _roleManagementConfig;
		private readonly UserManager _userManager;
		private readonly IUnitOfWorkManager _unitOfWorkManager;

		private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
		private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;

		//STQ MODIFIED
		private readonly IRepository<UserPasswordHistory, Guid> _userPasswordHistoryRepository;
        private readonly IOptions<UserOptions> _userOptions;
        private readonly IEmailSettingsChecker _emailSettingsChecker;

        public UserAppService(
			RoleManager roleManager,
			FolderManager folderManager,
			ACLManager aclManager,
			IUserEmailer userEmailer,
			IUserListExcelExporter userListExcelExporter,
			INotificationSubscriptionManager notificationSubscriptionManager,
			IAppNotifier appNotifier,
			IRepository<RolePermissionSetting, long> rolePermissionRepository,
			IRepository<UserPermissionSetting, long> userPermissionRepository,
			IRepository<UserRole, long> userRoleRepository,
			IRepository<Role> roleRepository,
			IUserPolicy userPolicy,
			IEnumerable<IPasswordValidator<User>> passwordValidators,
			IPasswordHasher<User> passwordHasher,
			IRepository<OrganizationUnit, long> organizationUnitRepository,
			IRoleManagementConfig roleManagementConfig,
			UserManager userManager,
			IUnitOfWorkManager unitOfWorkManager,
			IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
			IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository,

			//STQ MODIFIED
			IRepository<UserPasswordHistory, Guid> userPasswordHistoryRepository,
            IOptions<UserOptions> userOptions, 
            IEmailSettingsChecker emailSettingsChecker
            )
		{
			_roleManager = roleManager;
			_folderManager = folderManager;
			_ACLManager = aclManager;
			_userEmailer = userEmailer;
			_userListExcelExporter = userListExcelExporter;
			_notificationSubscriptionManager = notificationSubscriptionManager;
			_appNotifier = appNotifier;
			_rolePermissionRepository = rolePermissionRepository;
			_userPermissionRepository = userPermissionRepository;
			_userRoleRepository = userRoleRepository;
			_userPolicy = userPolicy;
			_passwordValidators = passwordValidators;
			_passwordHasher = passwordHasher;
			_organizationUnitRepository = organizationUnitRepository;
			_roleManagementConfig = roleManagementConfig;
			_userManager = userManager;
			_roleRepository = roleRepository;
			_unitOfWorkManager = unitOfWorkManager;
			AppUrlService = NullAppUrlService.Instance;

			_userOrganizationUnitRepository = userOrganizationUnitRepository;
			_organizationUnitRoleRepository = organizationUnitRoleRepository;

			// STQ MODIFIED
			_userPasswordHistoryRepository = userPasswordHistoryRepository;
            _userOptions = userOptions;
            _emailSettingsChecker = emailSettingsChecker;
        }
 
        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            var query = GetUsersFilteredQuery(input);

            var userCount = await query.CountAsync();

            var users = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
            );
        }

        //STQ MODIFIED
        [AbpAuthorize(AppPermissions.Pages_User)]
        public async Task<PagedResultDto<UserListDto>> GetUsersForSharing(GetUsersInput input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            //var query = GetUsersFilteredQuery(input);

            var query = UserManager.Users;  

            var userCount = await query.CountAsync();

            var users = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
                );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
        public async Task<FileDto> GetUsersToExcel(GetUsersToExcelInput input)
        {
            var query = GetUsersFilteredQuery(input);

            var users = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return _userListExcelExporter.ExportToFile(userListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            // STQ REVIEW PASSING OF Ids for new users
            if (!input.Id.HasValue && input.Id != -1)
            {
                input.Id = AbpSession.UserId;
            }

            //Getting all available roles
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync();

            var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                AllOrganizationUnits = ObjectMapper.Map<List<OrganizationUnitDto>>(allOrganizationUnits),
                MemberedOrganizationUnits = new List<string>(),
                AllowedUserNameCharacters = _userOptions.Value.AllowedUserNameCharacters,
                IsSMTPSettingsProvided = await _emailSettingsChecker.EmailSettingsValidAsync() 
            };

            // STQ REVIEW PASSING OF Ids for new users
            if (!input.Id.HasValue || input.Id == -1)
            {
                // Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled =
                        await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement
                            .TwoFactorLogin.IsEnabled),
                    IsLockoutEnabled =
                        await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut
                            .IsEnabled)
                };

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = ObjectMapper.Map<UserEditDto>(user);
                output.ProfilePictureId = user.ProfilePictureId;

                var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
                output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();

                var allRolesOfUsersOrganizationUnits = GetAllRoleNamesOfUsersOrganizationUnits(input.Id.Value);

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                    userRoleDto.InheritedFromOrganizationUnit =
                        allRolesOfUsersOrganizationUnits.Contains(userRoleDto.RoleName);
                }
            }

            return output;
        }

        // STQ MODIFIED
        [AbpAuthorize(AppPermissions.Pages_User_Profile_Edit)]
        public async Task<GetUserForEditOutput> GetUserProfileForEdit()
        {

            //Getting all available roles
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync();

            var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                AllOrganizationUnits = ObjectMapper.Map<List<OrganizationUnitDto>>(allOrganizationUnits),
                MemberedOrganizationUnits = new List<string>(),
                AllowedUserNameCharacters = _userOptions.Value.AllowedUserNameCharacters,
                IsSMTPSettingsProvided = await _emailSettingsChecker.EmailSettingsValidAsync()
            };

     
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync((long)AbpSession.UserId);

                output.User = ObjectMapper.Map<UserEditDto>(user);
                output.ProfilePictureId = user.ProfilePictureId;

                var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
                output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();

                var allRolesOfUsersOrganizationUnits = GetAllRoleNamesOfUsersOrganizationUnits((long)AbpSession.UserId);

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                    userRoleDto.InheritedFromOrganizationUnit =
                        allRolesOfUsersOrganizationUnits.Contains(userRoleDto.RoleName);
                }
 

            return output;
        }

        private List<string> GetAllRoleNamesOfUsersOrganizationUnits(long userId)
        {
            return (from userOu in _userOrganizationUnitRepository.GetAll()
                join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
                    .OrganizationUnitId
                join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
                where userOu.UserId == userId
                select userOuRoles.Name).ToList();
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName)
                    .ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

		[AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
		public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
		{
			var user = await UserManager.GetUserByIdAsync(input.Id);
			var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
			await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
		}

        //STQ MODIFIED
        [AbpAuthorize(AppPermissions.Pages_User_Profile_Edit)]
        public async Task UpdateEmbeddedUser(UserEditDto input)
        {             

            var user = await UserManager.FindByIdAsync(input.Id.Value.ToString());
			input.IsActive = true;

            //Update user properties
            ObjectMapper.Map(input, user); //Passwords is not mapped (see mapping configuration)
  
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
        public async Task CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {
            if (input.User.Id.HasValue)
            {
                await UpdateUserAsync(input);
            }
            else
            {
                await CreateUserAsync(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Unlock)]
        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

		[AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
		protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInput input)
		{


			// STQ MODIFIED
			if (UserManager.Users.Where(u => ( u.EmailAddress.ToLower() == input.User.EmailAddress.ToLower() || u.UserName.ToLower() == input.User.UserName.ToLower()) && u.Id != input.User.Id ).Any()  )
			{
				throw new UserFriendlyException(L("UserNameOrEmailInvalid"));
			}
			// STQ MODIFIED END

			var user = await UserManager.FindByIdAsync(input.User.Id.Value.ToString());
            
            //Update user properties
            ObjectMapper.Map(input.User, user); //Passwords is not mapped (see mapping configuration)

			if (input.SetRandomPassword)
			{
				var randomPassword = await _userManager.CreateRandomPassword();
				user.Password = _passwordHasher.HashPassword(user, randomPassword);
				input.User.Password = randomPassword;
			}
			else if (!input.User.Password.IsNullOrEmpty())
			{
				await UserManager.InitializeOptionsAsync(AbpSession.TenantId);

				///// STQ MODIFIED START				

				//var passAlreadyExist = _userPasswordHistoryRepository.GetAll()
				//	.Where(h => h.UserId == user.Id)
				//	.ToList()
				//	.Any(hash => {
				//		var res = _passwordHasher.VerifyHashedPassword(user, hash.PasswordHash, input.User.Password);
				//		return res == PasswordVerificationResult.Success;
				//	});

 

				//if (passAlreadyExist)
				//	throw new UserFriendlyException(L("PasswordUsed"));

				CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));

				var hashedpassword = _passwordHasher.HashPassword(user, input.User.Password);
				UserPasswordHistory userPasswordHistory = new UserPasswordHistory()
				{
					Id = Guid.NewGuid(),
					PasswordHash = hashedpassword,
					UserId = user.Id,
					TenantId = AbpSession.TenantId
				};

				//await _userPasswordHistoryRepository.InsertAsync(userPasswordHistory);
				/// STQ MODIFIED END 


			}

            //Update roles
            CheckErrors(await UserManager.SetRolesAsync(user, input.AssignedRoleNames));

            //update organization units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId, string.Empty),
                    input.User.Password
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        protected virtual async Task CreateUserAsync(CreateOrUpdateUserInput input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(AbpSession.GetTenantId());
            }

            var user = ObjectMapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)
            user.TenantId = AbpSession.TenantId;

            // STQ Modified
            if (string.IsNullOrEmpty( input.User.Password))
            {
				throw new UserFriendlyException(L("PasswordRequired"));
			}

			//Set password
			if (input.SetRandomPassword)
            {
                var randomPassword = await _userManager.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
                }

                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

			// INclude in the password history
			UserPasswordHistory userPasswordHistory = new UserPasswordHistory()
			{
				Id = Guid.NewGuid(),
				PasswordHash = user.Password,
				UserId = user.Id,
				TenantId = AbpSession.TenantId
			};

			//await _userPasswordHistoryRepository.InsertAsync(userPasswordHistory);
			CurrentUnitOfWork.SaveChanges();

			//Notifications
			await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
			await _appNotifier.WelcomeToTheApplicationAsync(user);
			await _appNotifier.NewUserRegisteredAsync(user);

            //Organization Units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

			//Records Folder
			//Folder folder = new Folder() { Name = "Your Records", Description = "", ParentId = new Guid("00000000-0000-0000-0000-000000000000"), Type = "T"};
			//ACL aCL = new ACL() { UserId = user.Id };

			//await _folderManager.CreateOrEditFolder(aCL, folder);

			//await CurrentUnitOfWork.SaveChangesAsync();

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


            //var email = input.User.EmailAddress.ToLower();
            //switch (email)
            //{
            //    case "accadmin@syntaq.com":
            //        await UserManager.AddToOrganizationUnitAsync(user.Id, 15);
            //        break;
            //    case "accuser@syntaq.com":
            //        await UserManager.AddToOrganizationUnitAsync(user.Id, 14);
            //        break;
            //    case "acccontentadmin@syntaq.com":
            //        await UserManager.AddToOrganizationUnitAsync(user.Id, 13);
            //        break;
            //    case "irduser@syntaq.com":
            //        await UserManager.AddToOrganizationUnitAsync(user.Id, 2);
            //        break;
            //    case "irdadmin@syntaq.com":
            //        await UserManager.AddToOrganizationUnitAsync(user.Id, 2);
            //        break;
            //    case "moeuser@syntaq.com":
            //        await UserManager.AddToOrganizationUnitAsync(user.Id, 2);
            //        break;
            //    case "moeadmin@syntaq.com":
            //        await UserManager.AddToOrganizationUnitAsync(user.Id, 2);
            //        break;
            //}


            //Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId,string.Empty),
                    input.User.Password
                );
            }
        }

        private async Task FillRoleNames(IReadOnlyCollection<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */
            var userIds = userListDtos.Select(u => u.Id);

            var userRoles = await _userRoleRepository.GetAll()
                .Where(userRole => userIds.Contains(userRole.UserId))
                .Select(userRole => userRole).ToListAsync();

            var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();

            foreach (var user in userListDtos)
            {
                var rolesOfUser = userRoles.Where(userRole => userRole.UserId == user.Id).ToList();
                user.Roles = ObjectMapper.Map<List<UserListRoleDto>>(rolesOfUser);
            }

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role != null)
                {
                    roleNames[roleId] = role.DisplayName;
                }
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    if (roleNames.ContainsKey(userListRoleDto.RoleId))
                    {
                        userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                    }
                }

                userListDto.Roles = userListDto.Roles.Where(r => r.RoleName != null).OrderBy(r => r.RoleName).ToList();
            }
        }

        private IQueryable<User> GetUsersFilteredQuery(IGetUsersInput input)
        {
            var query = UserManager.Users
                .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
                .WhereIf(input.OnlyLockedUsers,
                    u => u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > DateTime.UtcNow)
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            if (input.Permissions != null && input.Permissions.Any(p => !p.IsNullOrWhiteSpace()))
            {
                var staticRoleNames = _roleManagementConfig.StaticRoles.Where(
                    r => r.GrantAllPermissionsByDefault &&
                         r.Side == AbpSession.MultiTenancySide
                ).Select(r => r.RoleName).ToList();

                input.Permissions = input.Permissions.Where(p => !string.IsNullOrEmpty(p)).ToList();

                var userIds = from user in query
                    join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                    from ur in urJoined.DefaultIfEmpty()
                    join urr in _roleRepository.GetAll() on ur.RoleId equals urr.Id into urrJoined
                    from urr in urrJoined.DefaultIfEmpty()
                    join up in _userPermissionRepository.GetAll()
                        .Where(userPermission => input.Permissions.Contains(userPermission.Name)) on user.Id equals up.UserId into upJoined
                    from up in upJoined.DefaultIfEmpty()
                    join rp in _rolePermissionRepository.GetAll()
                        .Where(rolePermission => input.Permissions.Contains(rolePermission.Name)) on
                        new { RoleId = ur == null ? 0 : ur.RoleId } equals new { rp.RoleId } into rpJoined
                    from rp in rpJoined.DefaultIfEmpty()
                    where (up != null && up.IsGranted) ||
                          (up == null && rp != null && rp.IsGranted) ||
                          (up == null && rp == null && staticRoleNames.Contains(urr.Name))
                    group user by user.Id
                    into userGrouped
                    select userGrouped.Key;

                query = UserManager.Users.Where(e => userIds.Contains(e.Id));
            }

            return query;
        }
    }
}
