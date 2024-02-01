using System.Collections.Generic;
using Syntaq.Falcon.Web.DashboardCustomization;


namespace Syntaq.Falcon.Web.Areas.Falcon.Startup
{
    public class DashboardViewConfiguration
    {
        public Dictionary<string, WidgetViewDefinition> WidgetViewDefinitions { get; } = new Dictionary<string, WidgetViewDefinition>();

        public Dictionary<string, WidgetFilterViewDefinition> WidgetFilterViewDefinitions { get; } = new Dictionary<string, WidgetFilterViewDefinition>();

        public DashboardViewConfiguration()
        {
            var jsAndCssFileRoot = "/Areas/Falcon/Views/CustomizableDashboard/Widgets/";
            var viewFileRoot = "Falcon/Widgets/";

            #region FilterViewDefinitions

            WidgetFilterViewDefinitions.Add(FalconDashboardCustomizationConsts.Filters.FilterDateRangePicker,
                new WidgetFilterViewDefinition(
                    FalconDashboardCustomizationConsts.Filters.FilterDateRangePicker,
                     "~/Areas/Falcon/Views/Shared/Components/CustomizableDashboard/Widgets/DateRangeFilter.cshtml",
                    jsAndCssFileRoot + "DateRangeFilter/DateRangeFilter.min.js",
                    jsAndCssFileRoot + "DateRangeFilter/DateRangeFilter.min.css")
            );

            //add your filters iew definitions here
            #endregion

            #region WidgetViewDefinitions

            #region TenantWidgets

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.DailySales,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.DailySales,
                    viewFileRoot + "DailySales",
                    jsAndCssFileRoot + "DailySales/DailySales.min.js",
                    jsAndCssFileRoot + "DailySales/DailySales.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.GeneralStats,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.GeneralStats,
                    viewFileRoot + "GeneralStats",
                    jsAndCssFileRoot + "GeneralStats/GeneralStats.min.js",
                    jsAndCssFileRoot + "GeneralStats/GeneralStats.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.ProfitShare,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.ProfitShare,
                    viewFileRoot + "ProfitShare",
                    jsAndCssFileRoot + "ProfitShare/ProfitShare.min.js",
                    jsAndCssFileRoot + "ProfitShare/ProfitShare.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.MemberActivity,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.MemberActivity,
                    viewFileRoot + "MemberActivity",
                    jsAndCssFileRoot + "MemberActivity/MemberActivity.min.js",
                    jsAndCssFileRoot + "MemberActivity/MemberActivity.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.RegionalStats,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.RegionalStats,
                    viewFileRoot + "RegionalStats",
                    jsAndCssFileRoot + "RegionalStats/RegionalStats.min.js",
                    jsAndCssFileRoot + "RegionalStats/RegionalStats.min.css",
                    12,
                    10));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.SalesSummary,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.SalesSummary,
                    viewFileRoot + "SalesSummary",
                    jsAndCssFileRoot + "SalesSummary/SalesSummary.min.js",
                    jsAndCssFileRoot + "SalesSummary/SalesSummary.min.css",
                    6,
                    10));

            //WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.TopStats,
            //    new WidgetViewDefinition(
            //        FalconDashboardCustomizationConsts.Widgets.Tenant.TopStats,
            //        viewFileRoot + "TopStats",
            //        jsAndCssFileRoot + "TopStats/TopStats.min.js",
            //        jsAndCssFileRoot + "TopStats/TopStats.min.css",
            //        12,
            //        10));
            //===========================Custom--------
            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.SubmissionLimit,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.SubmissionLimit,
                    viewFileRoot + "SubmissionLimit",
                    jsAndCssFileRoot + "SubmissionLimit/SubmissionLimit.min.js",
                    jsAndCssFileRoot + "SubmissionLimit/SubmissionLimit.min.css",
                    4,
                    6));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.TodaySubmissions,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.TodaySubmissions,
                    viewFileRoot + "TodaySubmissions",
                    jsAndCssFileRoot + "TodaySubmissions/TodaySubmissions.min.js",
                    jsAndCssFileRoot + "TodaySubmissions/TodaySubmissions.min.css",
                    4,
                    6));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.NewUsers,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.NewUsers,
                    viewFileRoot + "NewUsers",
                    jsAndCssFileRoot + "NewUsers/NewUsers.min.js",
                    jsAndCssFileRoot + "NewUsers/NewUsers.min.css",
                    4,
                    6));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.MostRecentSubmissions,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.MostRecentSubmissions,
                    viewFileRoot + "MostRecentSubmissions",
                    jsAndCssFileRoot + "MostRecentSubmissions/MostRecentSubmissions.min.js",
                    jsAndCssFileRoot + "MostRecentSubmissions/MostRecentSubmissions.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectStatus,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectStatus,
                    viewFileRoot + "ProjectStatus",
                    jsAndCssFileRoot + "ProjectStatus/ProjectStatus.min.js",
                    jsAndCssFileRoot + "ProjectStatus/ProjectStatus.min.css",
                    12,
                    7));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectCycle,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectCycle,
                    viewFileRoot + "ProjectCycle",
                    jsAndCssFileRoot + "ProjectCycle/ProjectCycle.min.js",
                    jsAndCssFileRoot + "ProjectCycle/ProjectCycle.min.css",
                    4,
                    8));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectStatusChart,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectStatusChart,
                    viewFileRoot + "ProjectStatusChart",
                    jsAndCssFileRoot + "ProjectStatusChart/ProjectStatusChart.min.js",
                    jsAndCssFileRoot + "ProjectStatusChart/ProjectStatusChart.min.css",
                    4,
                    8));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.ActionRequiredChart,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.ActionRequiredChart,
                    viewFileRoot + "ActionRequiredChart",
                    jsAndCssFileRoot + "ActionRequiredChart/ActionRequiredChart.min.js",
                    jsAndCssFileRoot + "ActionRequiredChart/ActionRequiredChart.min.css",
                    4,
                    8));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.WaitingContributor,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.WaitingContributor,
                    viewFileRoot + "WaitingContributor",
                    jsAndCssFileRoot + "WaitingContributor/WaitingContributor.min.js",
                    jsAndCssFileRoot + "WaitingContributor/WaitingContributor.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.RecentTemplates,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.RecentTemplates,
                    viewFileRoot + "RecentTemplates",
                    jsAndCssFileRoot + "RecentTemplates/RecentTemplates.min.js",
                    jsAndCssFileRoot + "RecentTemplates/RecentTemplates.min.css"));


            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.WaitingOthers,
             new WidgetViewDefinition(
                 FalconDashboardCustomizationConsts.Widgets.Tenant.WaitingOthers,
                 viewFileRoot + "WaitingOthers",
                 jsAndCssFileRoot + "WaitingOthers/WaitingOthers.min.js",
                 jsAndCssFileRoot + "WaitingOthers/WaitingOthers.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectTemplateUsageCount,
             new WidgetViewDefinition(
                 FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectTemplateUsageCount,
                 viewFileRoot + "ProjectTemplateUsageCount",
                 jsAndCssFileRoot + "ProjectTemplateUsageCount/ProjectTemplateUsageCount.min.js",
                 jsAndCssFileRoot + "ProjectTemplateUsageCount/ProjectTemplateUsageCount.min.css",
                 4,
                 8));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectRecentDocuments,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.ProjectRecentDocuments,
                    viewFileRoot + "ProjectRecentDocuments",
                    jsAndCssFileRoot + "ProjectRecentDocuments/ProjectRecentDocuments.min.js",
                    jsAndCssFileRoot + "ProjectRecentDocuments/ProjectRecentDocuments.min.css"));
            //Document count--
            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnNew,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnNew,
                    viewFileRoot + "DocumentOnNew",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnNew.js",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnNew.css",
                    4,
                    6));
            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnDraft,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnDraft,
                    viewFileRoot + "DocumentOnDraft",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnDraft.js",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnDraft.css",
                    4,
                    6));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnPublish,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnPublish,
                    viewFileRoot + "DocumentOnPublish",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnPublish.js",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnPublish.css",
                    4,
                    6));


            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnFinal,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnFinal,
                    viewFileRoot + "DocumentOnFinal",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnFinal.js",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnFinal.css",
                    4,
                    6));


            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnAll,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Tenant.DocumentOnAll,
                    viewFileRoot + "DocumentOnAll",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnAll.js",
                    jsAndCssFileRoot + "DocumentOnCount/DocumentOnAll.css",
                    4,
                    6));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Tenant.StartNewProject,
               new WidgetViewDefinition(
                   FalconDashboardCustomizationConsts.Widgets.Tenant.StartNewProject,
                   viewFileRoot + "StartNewProject",
                   jsAndCssFileRoot + "StartNewProject/StartNewProject.min.js",
                   jsAndCssFileRoot + "StartNewProject/StartNewProject.min.css",
                   4,
                   6));

            //add your tenant side widget definitions here
            #endregion

            #region HostWidgets

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Host.IncomeStatistics,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Host.IncomeStatistics,
                    viewFileRoot + "IncomeStatistics",
                    jsAndCssFileRoot + "IncomeStatistics/IncomeStatistics.min.js",
                    jsAndCssFileRoot + "IncomeStatistics/IncomeStatistics.min.css"));

            //WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Host.TopStats,
            //    new WidgetViewDefinition(
            //        FalconDashboardCustomizationConsts.Widgets.Host.TopStats,
            //        viewFileRoot + "HostTopStats",
            //        jsAndCssFileRoot + "HostTopStats/HostTopStats.min.js",
            //        jsAndCssFileRoot + "HostTopStats/HostTopStats.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Host.EditionStatistics,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Host.EditionStatistics,
                    viewFileRoot + "EditionStatistics",
                    jsAndCssFileRoot + "EditionStatistics/EditionStatistics.min.js",
                    jsAndCssFileRoot + "EditionStatistics/EditionStatistics.min.css"));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Host.SubscriptionExpiringTenants,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Host.SubscriptionExpiringTenants,
                    viewFileRoot + "SubscriptionExpiringTenants",
                    jsAndCssFileRoot + "SubscriptionExpiringTenants/SubscriptionExpiringTenants.min.js",
                    jsAndCssFileRoot + "SubscriptionExpiringTenants/SubscriptionExpiringTenants.min.css",
                    6,
                    10));

            WidgetViewDefinitions.Add(FalconDashboardCustomizationConsts.Widgets.Host.RecentTenants,
                new WidgetViewDefinition(
                    FalconDashboardCustomizationConsts.Widgets.Host.RecentTenants,
                    viewFileRoot + "RecentTenants",
                    jsAndCssFileRoot + "RecentTenants/RecentTenants.min.js",
                    jsAndCssFileRoot + "RecentTenants/RecentTenants.min.css"));

            //add your host side widgets definitions here
            #endregion

            #endregion
        }
    }
}
