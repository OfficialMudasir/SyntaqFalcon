using Abp.Application.Services.Dto;
using Shouldly;
using Syntaq.Falcon.RecordPolicyActions;
using Syntaq.Falcon.RecordPolicyActions.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.RecordPolicies.Dtos;
using static Syntaq.Falcon.RecordPolicyActions.RecordPolicyActionConsts;

namespace Syntaq.Falcon.Tests.RecordPolicyActions
{
    public class RecordPolicyActionsAppService_TenantTests : AppTestBase
    {
        private readonly IRecordPolicyActionsAppService _recordPolicyActionsRepository;


        public RecordPolicyActionsAppService_TenantTests()
        {
            LoginAsDefaultTenantAdmin();
            _recordPolicyActionsRepository = Resolve<IRecordPolicyActionsAppService>();

        }

        //[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Get_All_RecordPolicyActions_Tenant()
        {

            //Act
            var output = await _recordPolicyActionsRepository.GetAll(new GetAllRecordPolicyActionsInput());

            //Assert
            output.Items.Count.ShouldBe(1); //because FilteredApps = FilteredApps.Concat(SharedApps).ToList();
        }

        [Fact]
        public async Task Should_Get_All_RecordPolicyActions_ByRecordId_Tenant()
        {
            var exitingRp = UsingDbContext(context => context.RecordPolicies.FirstOrDefault(a => a.Name == "Default Record Policy"));
            //Act
            exitingRp.ShouldNotBeNull();

            GetAllRecordPolicyActionsInput rpai = new GetAllRecordPolicyActionsInput()
            {
                RecordPolicyId = exitingRp.Id,
            };

            //Act
            var output = await _recordPolicyActionsRepository.GetAllByRecordId(rpai);

            //Assert
            output.Items.Count.ShouldBe(1); //because FilteredApps = FilteredApps.Concat(SharedApps).ToList();
        }


        [Fact]
        public async Task Should_Edit_RecordPolicyAction_Tenant()
        {
            var exitingRpa = UsingDbContext(context => context.RecordPolicyActions.FirstOrDefault(a => a.Name == "Default Record Policy Action"));
            //Act
            exitingRpa.ShouldNotBeNull();

            //Arrange
            CreateOrEditRecordPolicyActionDto rpa = new CreateOrEditRecordPolicyActionDto()
            {
                Name = "Record Policy Action Edit",
                AppliedTenantId = -1,
                Id = exitingRpa.Id,
                ExpireDays = 30,
                Active = true,
                Type = RecordPolicyActionType.SoftDelete, //softdelete
                RecordStatus = RecordStatusType.Archived, //Archived
                RecordPolicyId = exitingRpa.RecordPolicyId
            };

            //Act
            await _recordPolicyActionsRepository.CreateOrEdit(rpa);

            //Assert
            UsingDbContext(context =>
            {
                var newrp = context.RecordPolicyActions.FirstOrDefault(a => a.Name == "Record Policy Action Edit");
                newrp.ShouldNotBeNull();
            });

            UsingDbContext(context =>
            {
                var newrp = context.RecordPolicies.FirstOrDefault(a => a.TenantId == 1);
                newrp.ShouldNotBeNull();
            });
        }

    }
}
