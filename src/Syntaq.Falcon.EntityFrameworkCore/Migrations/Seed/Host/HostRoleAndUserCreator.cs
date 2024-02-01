using System;
using System.Linq;
using Abp;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Notifications;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Authorization.Roles;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.Notifications;

namespace Syntaq.Falcon.Migrations.Seed.Host
{
    public class HostRoleAndUserCreator
    {
        private readonly FalconDbContext _context;

        public HostRoleAndUserCreator(FalconDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            //Admin role for host

            var adminRoleForHost = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Host.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = _context.Roles.Add(new Role(null, StaticRoleNames.Host.Admin, StaticRoleNames.Host.Admin) { IsStatic = true, IsDefault = true }).Entity;
                _context.SaveChanges();
            }

            //admin user for host

            var adminUserForHost = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (adminUserForHost == null)
            {
                var user = new User
                {
                    TenantId = null,
                    UserName = AbpUserBase.AdminUserName,
                    Name = "admin",
                    Surname = "admin",
                    EmailAddress = "admin@aspnetzero.com",
                    IsEmailConfirmed = true,
                    ShouldChangePasswordOnNextLogin = false,
                    IsActive = true,
                    Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                };

                user.SetNormalizedNames();

                adminUserForHost = _context.Users.Add(user).Entity;
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
                _context.SaveChanges();

                //User account of admin user
                _context.UserAccounts.Add(new UserAccount
                {
                    TenantId = null,
                    UserId = adminUserForHost.Id,
                    UserName = AbpUserBase.AdminUserName,
                    EmailAddress = adminUserForHost.EmailAddress
                });

                //STQ MODIFIED
                // Default User Fodlers for Admin Host
                // Root Form Folder and ACL
                Folders.Folder folder = new Folders.Folder()
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000000"),
                    Name = "User Root",
                    ParentId =  null,
                    Type = "F",
                    CreatorUserId = adminUserForHost.Id,
                    LastModifierUserId = adminUserForHost.Id,
                };

                _context.Folders.Add(folder);
                _context.SaveChanges();

                Guid entityId = Guid.NewGuid();
                AccessControlList.ACL acl = new AccessControlList.ACL()
                {
                    CreatorUserId = adminUserForHost.Id,
                    LastModifierUserId = adminUserForHost.Id,
                    Role = "O",
                    Type  = "Folder",
                    UserId = adminUserForHost.Id,
                    EntityID = entityId
                };

                _context.ACLs.Add(acl);
                _context.SaveChanges();



                  folder = new Folders.Folder()
                {
                    Id = entityId,
                    Name = "Your Forms",
                    ParentId = null,//  new Guid("00000000-0000-0000-0000-000000000000"),
                    Type = "F",
                    CreatorUserId = adminUserForHost.Id,
                    LastModifierUserId = adminUserForHost.Id,
                };

                _context.Folders.Add(folder);
                _context.SaveChanges();


                // Root Template Folder and ACL
                entityId = Guid.NewGuid();
                acl = new AccessControlList.ACL()
                {
                    CreatorUserId = adminUserForHost.Id,
                    LastModifierUserId = adminUserForHost.Id,
                    Role = "O",
                    Type = "Folder",
                    UserId = adminUserForHost.Id,
                    EntityID = entityId
                };

                _context.ACLs.Add(acl);
                _context.SaveChanges();

                folder = new Folders.Folder()
                {
                    Id = entityId,
                    Name = "Your Templates",
                    ParentId =  null, // new Guid("00000000-0000-0000-0000-000000000000"),
                    Type = "T",
                    CreatorUserId = adminUserForHost.Id,
                    LastModifierUserId = adminUserForHost.Id,
                };

                _context.Folders.Add(folder);
                _context.SaveChanges();

                // Root Records Folder and ACL
                entityId = Guid.NewGuid();
                acl = new AccessControlList.ACL()
                {
                    CreatorUserId = adminUserForHost.Id,
                    LastModifierUserId = adminUserForHost.Id,
                    Role = "O",
                    Type = "Folder",
                    UserId = adminUserForHost.Id,
                    EntityID = entityId
                };

                _context.ACLs.Add(acl);
                _context.SaveChanges();

                folder = new Folders.Folder()
                {
                    Id = entityId,
                    Name = "Your Records",
                    ParentId =  null, //  new Guid("00000000-0000-0000-0000-000000000000"),
                    Type = "R",
                    CreatorUserId = adminUserForHost.Id,
                    LastModifierUserId = adminUserForHost.Id,
                };

                _context.Folders.Add(folder);
                _context.SaveChanges();

                // FormTypes
                Forms.FormType formtype = new Forms.FormType()
                {
                    Name = "UserForm"
                };
                _context.FormTypes.Add(formtype);
                _context.SaveChanges();

                formtype = new Forms.FormType()
                {
                    Name = "PaymentForm"
                };
                _context.FormTypes.Add(formtype);
                _context.SaveChanges();

                //Notification subscriptions
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), null, adminUserForHost.Id, AppNotificationNames.NewTenantRegistered));
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), null, adminUserForHost.Id, AppNotificationNames.NewUserRegistered));
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), null, adminUserForHost.Id, AppNotificationNames.FormTemplateUpdate));


                _context.SaveChanges();
            }
        }
    }
}