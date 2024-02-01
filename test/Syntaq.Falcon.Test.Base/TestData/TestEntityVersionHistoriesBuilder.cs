using Abp.Runtime.Session;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.Apps;
using System;
using Syntaq.Falcon.EntityVersionHistories;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Projects;

namespace Syntaq.Falcon.Test.Base.TestData
{
    public class TestEntityVersionHistoriesBuilder
    {
        private readonly FalconDbContext _context;
        private readonly int _tenantId;
        public IAbpSession _abpSession { get; set; }

        public TestEntityVersionHistoriesBuilder(FalconDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateEntityVersionHistories();
        }

        private void CreateEntityVersionHistories()
        {
           CreateFormHistory("test form","{}");
           CreateTemplateHistory("test template", "{}");
           CreateProjectHistory("test project template", "{}");
        }

        private void CreateFormHistory(string Name, string Data)
        {
            var newevh = new EntityVersionHistory()
            {
                Data = Data,
                Name = Name,
                Description = "Create a New Form", // 	
                Version = 1,
                VersionName = "Version 1",
                PreviousVersion = 1, //live version	
                EntityId = new Guid(),

                TenantId = null,
                Type = "Form",
                PreviousData = "{}",
                NewData = "{}"
            };

           _context.EntityVersionHistories.Add(newevh);
            

            ACL newACL = new ACL()
            {
                TenantId = null,
                EntityID = newevh.Id,
                Type = "EntityVersionHistory",
                Role = "O",
            };
            _context.ACLs.Add(newACL);
            _context.SaveChanges();
           
        }

        //private void CreateAppJobs()
        //{
        //    var a1 = CreateAppJob("App Job To Update", "{}", "{}");
        //    var a2 = CreateAppJob("App Job To Delete", "{}", "{}");
        //}

        private void CreateTemplateHistory(string name, string data)
        {
            var newevh = new EntityVersionHistory()
            {
                Name = name,
                Description = "Create a New template", // 	
                Version = 1,
                VersionName = "Version 1",
                PreviousVersion = 1, //live version	
                EntityId = new Guid(),

                TenantId = null,
                Type = "Template",
            };

            _context.EntityVersionHistories.Add(newevh);

            ACL newACL = new ACL()
            {
                TenantId = null,
                EntityID = newevh.Id,
                Type = "EntityVersionHistory",
                Role = "O",
            };
            _context.ACLs.Add(newACL);
            _context.SaveChanges();
        }


        private void CreateProjectHistory(string Name, string Data)
        {
            var project = new Project()
            {
                Id = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                Name = "GMC ProjectTemplate",
                Description ="Create project template",
                Status = 0,
                Type = 0,
                Enabled = true,
                Archived =false,
                CreatorUserId =1,
            };
            _context.Projects.Add(project);

            var newevh = new EntityVersionHistory()
            {
                Data = Data,
                Name = Name,
                Description = "Create a New Project template", // 	
                Version = 1,
                VersionName = "Version 1",
                PreviousVersion = 1, //live version	
                EntityId = new Guid(),

                TenantId = null,
                PreviousData = "{}",
                NewData = "{}",
                Type = "ProjectTemplate",
            };

            _context.EntityVersionHistories.Add(newevh);

            ACL newACL = new ACL()
            {
                TenantId = null,
                EntityID = newevh.Id,
                Type = "EntityVersionHistory",
                Role = "O",
            };
            _context.ACLs.Add(newACL);
            _context.SaveChanges();
        }
    }
}

