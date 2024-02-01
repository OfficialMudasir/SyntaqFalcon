using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
//STQ MODIFIED
using Syntaq.Falcon.Authorization.Roles;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Features;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Layout;
using Syntaq.Falcon.Web.Areas.Falcon.Startup;
using Syntaq.Falcon.Web.Views;
//STQ MODIFIED
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Domain.Repositories;
using Abp.Application.Features;
using System;

namespace Syntaq.Falcon.Web.Areas.Falcon.Views.Shared.Components.FalconMenu
{
    public class FalconMenuViewComponent : FalconViewComponent
    {
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly IAbpSession _abpSession;
        private readonly TenantManager _tenantManager;

        // STQ MODIFIED
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<Role> _roleRepository;

        public FalconMenuViewComponent(
            IUserNavigationManager userNavigationManager,
            IAbpSession abpSession,
            TenantManager tenantManager,
            IFeatureChecker featureChecker,
            UserManager userManager, // STQ MODIFIED
            RoleManager roleManager, // STQ MODIFIED
            IRepository<Role> roleRepository // STQ MODIFIED
        )
        {
            _userNavigationManager = userNavigationManager;
            _abpSession = abpSession;
            _tenantManager = tenantManager;
            _featureChecker = featureChecker;

            // STQ MODIFIED
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLeftMenuUsed">set to true for rendering left aside menu</param>
        /// <param name="iconMenu">set to render main menu items as icons. Only valid for left menu</param>
        /// <param name="currentPageName">Name of the current pagae</param>
        /// <param name="height">height of the menu</param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(bool isLeftMenuUsed, bool iconMenu = false, string currentPageName = null, string height = "auto")
        {

 

            var model = new MenuViewModel
            {
                Menu = await _userNavigationManager.GetMenuAsync(FalconNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
                Height = height,
                CurrentPageName = currentPageName,
                IconMenu = iconMenu
            };

            //STQ MODIFIED
            if (this.User != null)
            {

                var isAuthor = false;
                var isAdmin = false;

                try
                {

                var authorRole = await _roleManager.FindByNameAsync("Author");
                if (authorRole != null)
                {
                    var user = _userManager.FindByIdAsync(System.Convert.ToString(AbpSession.UserId)).Result;

                    if (user != null )
                        isAuthor = await _userManager.IsInRoleAsync(user, authorRole.Name);

                    if (user != null )
                        isAdmin = await _userManager.IsInRoleAsync(user, StaticRoleNames.Host.Admin);


                    if (isAuthor && !isAdmin)
                    {

                        model.Menu = new UserMenu();
                        model.Menu.Name = "App";
                        model.Menu.DisplayName = "Main Menu";
                        model.Menu.Items = new List<UserMenuItem>();
                        model.Menu.Items.Add(new UserMenuItem()
                        {
                            DisplayName = "Dashboard",
                            Name = "Dashboard",
                            Url = "Falcon/Dashboard",
                            Icon = "fas fa-chart-line",
                            IsEnabled = true,
                            IsVisible = true,
                            Order = 0,
                            Items = new List<UserMenuItem>(),
                        });

                        model.Menu.Items.Add(new UserMenuItem()
                        {
                            DisplayName = "Projects",
                            Name = "Projects",
                            Url = "Falcon/Projects",
                            Icon = "fas fa-project-diagram",
                            IsEnabled = true,
                            IsVisible = true,
                            Order = 1,
                            Items = new List<UserMenuItem>(),
                        });
                    }

                }
                }
                catch (Exception ex)
                {

                }



            }

            //var isEnabled = _featureChecker.IsEnabled(AppFeatures.SubmissionLimit);
            if (AbpSession.TenantId == null)
            {
                return GetView(model, isLeftMenuUsed);
            }

            var tenant = await _tenantManager.GetByIdAsync(AbpSession.TenantId.Value);
            if (tenant.EditionId.HasValue)
            {
                return GetView(model, isLeftMenuUsed);
            }

            var subscriptionManagement = FindMenuItemOrNull(model.Menu.Items, FalconPageNames.Tenant.SubscriptionManagement);
            if (subscriptionManagement != null)
            {
                subscriptionManagement.IsVisible = false;
            }

            return GetView(model, isLeftMenuUsed);

        }

        public UserMenuItem FindMenuItemOrNull(IList<UserMenuItem> userMenuItems, string name)
        {
            if (userMenuItems == null)
            {
                return null;
            }

            foreach (var menuItem in userMenuItems)
            {
                if (menuItem.Name == name)
                {
                    return menuItem;
                }

                var found = FindMenuItemOrNull(menuItem.Items, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private IViewComponentResult GetView(MenuViewModel model, bool isLeftMenuUsed)
        {
            return View(isLeftMenuUsed ? "Default" : "Top", model);
        }
    }
}
