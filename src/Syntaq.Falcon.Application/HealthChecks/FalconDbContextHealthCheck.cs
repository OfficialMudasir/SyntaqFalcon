using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Syntaq.Falcon.EntityFrameworkCore;

namespace Syntaq.Falcon.HealthChecks
{
    public class FalconDbContextHealthCheck : IHealthCheck
    {
        private readonly DatabaseCheckHelper _checkHelper;

        public FalconDbContextHealthCheck(DatabaseCheckHelper checkHelper)
        {
            _checkHelper = checkHelper;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (_checkHelper.Exist("db"))
            {
                return Task.FromResult(HealthCheckResult.Healthy("FalconDbContext connected to database."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("FalconDbContext could not connect to database"));
        }
    }
}
