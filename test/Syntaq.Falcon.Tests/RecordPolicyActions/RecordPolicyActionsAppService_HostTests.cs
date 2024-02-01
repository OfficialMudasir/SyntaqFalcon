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
    public class RecordPolicyActionsAppService_HostTests : AppTestBase
    {
        private readonly IRecordPolicyActionsAppService _recordPolicyActionsRepository;


        public RecordPolicyActionsAppService_HostTests()
        {
            LoginAsHostAdmin();
            _recordPolicyActionsRepository = Resolve<IRecordPolicyActionsAppService>();

        }

        //[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Get_All_RecordPolicyActions_Host()
        {

            //Act
            var output = await _recordPolicyActionsRepository.GetAll(new GetAllRecordPolicyActionsInput());

            //Assert
            //output.TotalCount.ShouldBe(2);
            output.Items.Count.ShouldBe(1); //because FilteredApps = FilteredApps.Concat(SharedApps).ToList();
        }


        [Fact]
        public async Task Should_Get_RecordPolicyAction_For_Edit_Host()
        {
            var exitingEVH = UsingDbContext(context => context.RecordPolicyActions.FirstOrDefault(a => a.Name == "Default Record Policy Action"));
            //Act
            var output = await _recordPolicyActionsRepository.GetRecordPolicyActionForEdit(new EntityDto<Guid>(Guid.Parse(exitingEVH.Id.ToString())));

            //Assert
            output.RecordPolicyAction.Name.ShouldBe("Default Record Policy Action");
            output.RecordPolicyAction.Name.ShouldNotBeNull();

        }



        [Fact]
        public async Task Should_Edit_RecordPolicyAction_Host()
        {
            var exitingRpa = UsingDbContext(context => context.RecordPolicyActions.FirstOrDefault(a => a.Name == "Default Record Policy Action"));
            //Act
            exitingRpa.ShouldNotBeNull();

            //Arrange
            CreateOrEditRecordPolicyActionDto rpa = new CreateOrEditRecordPolicyActionDto()
            {
                Name = "Record Policy Action Edit",
                AppliedTenantId = 1,
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
        }


        [Fact]
        public async Task Should_Create_RecordPolicyAction_Host()
        {
            var exitingRecordpolicy = UsingDbContext(context => context.RecordPolicies.FirstOrDefault(a => a.Name == "Default Record Policy"));
            exitingRecordpolicy.ShouldNotBeNull();

            //Arrange
            CreateOrEditRecordPolicyActionDto rp = new CreateOrEditRecordPolicyActionDto()
            {
                Name = "Test HardDelete Policy",
                AppliedTenantId = -1,
                ExpireDays = 30,
                Active = true,
                Type = RecordPolicyActionType.HardDelete, //softdelete
                RecordStatus = RecordStatusType.IsDeleted, //Archived
                RecordPolicyId = exitingRecordpolicy.Id,
            };

            //Act
            await _recordPolicyActionsRepository.CreateOrEdit(rp);

            //Assert
            UsingDbContext(context =>
            {
                var newrp = context.RecordPolicyActions.FirstOrDefault(a => a.Name == "Test HardDelete Policy");
                newrp.ShouldNotBeNull();
            });
        }

        public async Task Should_GetRecordPolicyAction_ForView_Host()
        {
            var exitingRecordpolicy = UsingDbContext(context => context.RecordPolicies.FirstOrDefault(a => a.Name == "Default Record Policy Action"));
            exitingRecordpolicy.ShouldNotBeNull();

            //Arrange
            var output = await _recordPolicyActionsRepository.GetRecordPolicyActionForView(exitingRecordpolicy.Id);

            //Assert
            output.RecordPolicyAction.Name.ShouldBe("Default Record Policy Action");
            output.RecordPolicyAction.ShouldNotBeNull();
        }

        


    }
}
