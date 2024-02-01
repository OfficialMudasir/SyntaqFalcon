using System;
using System.Collections.Generic;
using System.Text;
using Abp.Auditing;
using Abp.BackgroundJobs;
using Hangfire.Server;

namespace Syntaq.Falcon.ASIC
{
    class ASICStatusCheckWorker
    {
    }
    //[Audited]
    //public class ASICStatusCheckWorker : IBackgroundProcess
    //{

    //	public void CheckStatus(BackgroundProcessContext context)
    //	{
    //		// Do something
    //		// Use context.Wait(TimeSpan) to wait between executions
    //	}
    //}
}
