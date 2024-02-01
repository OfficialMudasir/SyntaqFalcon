using Abp.Application.Services;
using Syntaq.Falcon.Tenants.Dashboard.Dto;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Tenants.Dashboard
{
    //STQ MODIFIED
    public interface ITenantDashboardAppService : IApplicationService
    {
        GetMemberActivityOutput GetMemberActivity();

        Task<GetDashboardDataOutput> GetDashboardData(GetDashboardDataInput input);
        Task<GetDashboardProjectDataOutput> GetDashboardProjectData(GetDashboardProjectDataInput input);

        GetDailySalesOutput GetDailySales();

        GetProfitShareOutput GetProfitShare();

        GetSalesSummaryOutput GetSalesSummary(GetSalesSummaryInput input);

        GetTopStatsOutput GetTopStats();

        GetRegionalStatsOutput GetRegionalStats();

        GetGeneralStatsOutput GetGeneralStats();

        Task<GetSubmissionLimitOutput> GetSubmissionLimit(GetSubmissionLimitInput input);

        GetTodaySubmissionsOutput GetTodaySubmissions(GetTodaySubmissionsInput input);

        GetNewUsersOutput GetNewUsersForWidget(GetDashboardDataInput input);

        int GetDocumentsStatusCountForWidget(GetDocumentsStatusCountInput input);
    }
}
