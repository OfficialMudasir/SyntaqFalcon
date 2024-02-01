using Abp.Application.Services.Dto;
using Shouldly;
using Syntaq.Falcon.RecordPolicies;
using Syntaq.Falcon.RecordPolicies.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Syntaq.Falcon.EntityFrameworkCore;

namespace Syntaq.Falcon.Tests.RecordPolicies
{
    public class RecordPolicyActionsAppService_Tests : AppTestBase
    {
        private readonly IRecordPoliciesAppService _recordPolicyRepository;


        public RecordPolicyActionsAppService_Tests()
        {
            LoginAsHostAdmin();
            _recordPolicyRepository = Resolve<IRecordPoliciesAppService>();

        }

        //[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Get_All_RecordPolicy()
        {

            //Act
            var output = await _recordPolicyRepository.GetAll(new GetAllRecordPoliciesInput());

            //Assert
            //output.TotalCount.ShouldBe(2);
            output.Items.Count.ShouldBe(1); //because FilteredApps = FilteredApps.Concat(SharedApps).ToList();
        }

        ////[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Create_RecordPolicy()
        {
            //Arrange
            CreateOrEditRecordPolicyDto rp = new CreateOrEditRecordPolicyDto()
            {
                Name = "Record Policy tenant",
                AppliedTenantId =1
            };

            //Act
            await _recordPolicyRepository.CreateOrEdit(rp);

            //Assert
            UsingDbContext(context =>
            {
                var newrp = context.RecordPolicies.FirstOrDefault(a => a.Name == "Record Policy tenant");
                newrp.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Get_RecordPolicy_For_Edit()
        {
            var exitingEVH = UsingDbContext(context => context.RecordPolicies.FirstOrDefault(a => a.Name == "Default Record Policy"));
            //Act
            var output = await _recordPolicyRepository.GetRecordPolicyForEdit(new EntityDto<Guid>(Guid.Parse(exitingEVH.Id.ToString())));

            //Assert
            output.RecordPolicy.Name.ShouldBe("Default Record Policy");
            output.RecordPolicy.Name.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Edit_RecordPolicy()
        {
            var exitingEVH = UsingDbContext(context => context.RecordPolicies.FirstOrDefault(a => a.Name == "Default Record Policy"));
            exitingEVH.ShouldNotBeNull();

            //Arrange
            CreateOrEditRecordPolicyDto rp = new CreateOrEditRecordPolicyDto()
            {
                Id = Guid.Parse(exitingEVH.Id.ToString()),
                Name = "Record Policy Edit",
                AppliedTenantId = 1
            };

            //Act
            await _recordPolicyRepository.CreateOrEdit(rp);

            //Assert
            UsingDbContext(context =>
            {
                var newrp = context.RecordPolicies.FirstOrDefault(a => a.Name == "Record Policy Edit");
                newrp.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Get_RecordPolicyForView()
        {
            var exitingEVH = UsingDbContext(context => context.RecordPolicies.FirstOrDefault(a => a.Name == "Default Record Policy"));
            exitingEVH.ShouldNotBeNull();

            //Act
           var output = await _recordPolicyRepository.GetRecordPolicyForView(exitingEVH.Id);

            //Assert
            output.RecordPolicy.Name.ShouldBe("Default Record Policy");
            output.RecordPolicy.ShouldNotBeNull();
        }



    }
}
