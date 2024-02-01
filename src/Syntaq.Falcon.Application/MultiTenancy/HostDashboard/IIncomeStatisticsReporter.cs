using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syntaq.Falcon.MultiTenancy.HostDashboard.Dto;

namespace Syntaq.Falcon.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}