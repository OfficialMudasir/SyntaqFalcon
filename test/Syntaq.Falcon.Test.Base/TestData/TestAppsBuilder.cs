using Abp.Runtime.Session;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.Apps;
using System;

namespace Syntaq.Falcon.Test.Base.TestData
{
    public class TestAppsBuilder
    {
        private readonly FalconDbContext _context;
        private readonly int _tenantId;
        public IAbpSession _abpSession { get; set; }

        public TestAppsBuilder(FalconDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateApps();
        }

        private void CreateApps()
        {
            var a1 = CreateApp("App To Update", "App for Unit Testing", "{}");
            var a2 = CreateApp("App To Delete", "App for Unit Testing", "{}");
            var aj1 = CreateAppJob("App Job To Update", "{}", a1.Id);
            var aj2 = CreateAppJob("App Job To Delete", "{}", a2.Id);
        }

        private Apps.App CreateApp(string Name, string Description, string Data)
        {
            var newApp = new Apps.App()
            {
                TenantId = _tenantId,
                CreatorUserId = 2,
                Name = Name,
                Description = Description,
                Data = Data
            };
            var a = _context.Apps.Add(newApp).Entity;
            _context.SaveChanges();
            var newACL = new ACL()
            {
                TenantId = _tenantId,
                EntityID = newApp.Id,
                Role = "O",
                UserId = 2,
                Type ="App"
            };
            _context.ACLs.Add(newACL);
            _context.SaveChanges();
            return a;
        }

        //private void CreateAppJobs()
        //{
        //    var a1 = CreateAppJob("App Job To Update", "{}", "{}");
        //    var a2 = CreateAppJob("App Job To Delete", "{}", "{}");
        //}

        private AppJob CreateAppJob(string Name, string Data, Guid AppId)
        {
            var newAppJob = new AppJob()
            {
                TenantId = _tenantId,
                Name = Name,
                Data = Data,
                AppId = AppId,
                
            };
            var aj = _context.AppJobs.Add(newAppJob).Entity;
            _context.SaveChanges();
            return aj;
        }
    }
}

