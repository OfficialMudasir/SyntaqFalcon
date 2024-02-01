using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syntaq.Falcon.MultiTenancy;


namespace Syntaq.Falcon.Web.Hangfire
{
    public class HangfireService
    {
        public static void InitializeJobs()
        {
            // BackgroundJob.Enqueue<ProjectExpirationCheckWorker>(x => x.Execute(0));
          RecurringJob.AddOrUpdate<ProjectExpirationCheckWorker>(x => x.Execute(0), Cron.Daily,TimeZoneInfo.Local);
        }
    }
}
