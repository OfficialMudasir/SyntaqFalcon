using System.Threading.Tasks;
using Abp.Application.Services;
using Syntaq.Falcon.MultiTenancy.HostDashboard.Dto;

namespace Syntaq.Falcon.MultiTenancy.HostDashboard
{
    public interface IHostDashboardAppService : IApplicationService
    {
        //STQ MODIFIED
        Task<TopStatsData> GetTopStatsData(GetTopStatsInput input);
        Task<GetRecentTenantsOutput> GetRecentTenantsData();

        Task<GetExpiringTenantsOutput> GetSubscriptionExpiringTenantsData();

        Task<GetIncomeStatisticsDataOutput> GetIncomeStatistics(GetIncomeStatisticsDataInput input);

        Task<GetEditionTenantStatisticsOutput> GetEditionTenantStatistics(GetEditionTenantStatisticsInput input);
    }
}