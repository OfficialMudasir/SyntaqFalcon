using Abp.Runtime.Session;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.RecordPolicies;
using System;
using Syntaq.Falcon.RecordPolicyActions;
using static Syntaq.Falcon.RecordPolicyActions.RecordPolicyActionConsts;

namespace Syntaq.Falcon.Test.Base.TestData
{
    public class TestRecordPoliciesBuilder
    {
        private readonly FalconDbContext _context;
        
        public IAbpSession _abpSession { get; set; }

        public TestRecordPoliciesBuilder(FalconDbContext context)
        {
            _context = context;
           
        }

        public void Create()
        {
            CreateRecordPolicies();
        }

        private void CreateRecordPolicies()
        {
            var rp = CreateRecordPolicy("Default Record Policy");
            CreateRecordPolicyActions("Default Record Policy Action", rp.Id);
        }

        private RecordPolicy CreateRecordPolicy(string name)
        {
            var rp = new RecordPolicy()
            {
                Name = name,
                AppliedTenantId =-1,
            };

            _context.RecordPolicies.Add(rp);
            _context.SaveChanges();
            var newACL = new ACL()
            {
                EntityID = rp.Id,
                Role = "O",
                Type = "RecordPolicy"
            };
            _context.ACLs.Add(newACL);
            _context.SaveChanges();

            return rp;
        }


        private void CreateRecordPolicyActions(string name, Guid RecordPolicyId)
        {
            var rpa = new RecordPolicyAction()
            {
                Name = name,
                AppliedTenantId = -1,
                ExpireDays =30,
                Active = true,
                Type = RecordPolicyActionType.SoftDelete, //softdelete
                RecordStatus= RecordStatusType.Archived, //Archived
                RecordPolicyId = RecordPolicyId,
            };
            _context.RecordPolicyActions.Add(rpa);
            _context.SaveChanges();

            var newACL = new ACL()
            {
                Type = "RecordPolicyAction",
                EntityID = RecordPolicyId,
                Role = "O"
            };
            _context.ACLs.Add(newACL);
            _context.SaveChanges();

        }



    }
}

