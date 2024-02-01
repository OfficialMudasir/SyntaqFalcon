using Abp.Application.Services.Dto;
using Shouldly;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Apps.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Syntaq.Falcon.Tests.Apps
{
    public class AppJobsAppService_Tests : AppTestBase
    {
        private readonly IAppJobsAppService _appJobsAppService;

        public AppJobsAppService_Tests()
        {
            _appJobsAppService = Resolve<IAppJobsAppService>();
        }

        //[Fact]
        //public async Task Should_Get_All_App_Jobs()
        //{
        //    //Act
        //    var output = await _appJobsAppService.GetAll(new GetAllAppJobsInput());

        //    //Assert
        //    output.Items.Count.ShouldBe(2);
        //}

        [Fact]
        public async Task Should_Get_App_Jobs_By_App_Id()
        {
            //Arrange
            var exitingApp = UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Name == "App To Update"));

            //Act
            var output = await _appJobsAppService.GetJobsByAppId(new EntityDto<Guid>(Guid.Parse(exitingApp.Id.ToString())));

            //Assert
            output.Items.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Should_Get_App_Job_For_Edit()
        {
            //Arrange
            var exitingAppJob = UsingDbContext(context => context.AppJobs.FirstOrDefault(a => a.Name == "App Job To Update"));

            //Act
            var output = await _appJobsAppService.GetAppJobForEdit(new EntityDto<Guid>(Guid.Parse(exitingAppJob.Id.ToString())));

            //Assert
            output.AppJobName.ShouldBe("App Job To Update");
            output.AppJob.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Create_App_Job()
        {
            //Arrange
            var exitingApp = UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Name == "App To Update"));
            CreateOrEditAppJobDto app = new CreateOrEditAppJobDto()
            {
                Name = "Unit Test App Job",
                Data = "{}",
                AppId = exitingApp.Id,
                TenantId = AbpSession.TenantId,
                Form = new CreateOrEditAppJobFormDto(),
                Document = new List<CreateOrEditAppJobDocumentDto>(),
                RecordMatter = new List<CreateOrEditAppJobRecordMatterDto>(),
                User = new CreateOrEditAppJobUserDto(),
                WorkFlow = new CreateOrEditAppJobWorkFlowDto(),
            };

            //Act
            await _appJobsAppService.CreateOrEdit(app);

            //Assert
            UsingDbContext(context =>
            {
                var newApp = context.AppJobs.FirstOrDefault(a => a.Name == "Unit Test App Job");
                newApp.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Update_App_Job()
        {
            //Arrange
            var exitingAppJob = UsingDbContext(context => context.AppJobs.FirstOrDefault(a => a.Name == "App Job To Update"));
            CreateOrEditAppJobDto app = new CreateOrEditAppJobDto()
            {
                Id = Guid.Parse(exitingAppJob.Id.ToString()),
                Name = "App Job Is Updated",
                Data = "{}",
                AppId = exitingAppJob.AppId,
                TenantId = AbpSession.TenantId,
                Form = new CreateOrEditAppJobFormDto(),
                Document = new List<CreateOrEditAppJobDocumentDto>(),
                RecordMatter = new List<CreateOrEditAppJobRecordMatterDto>(),
                User = new CreateOrEditAppJobUserDto(),
                WorkFlow = new CreateOrEditAppJobWorkFlowDto()
            };

            //Act
            await _appJobsAppService.CreateOrEdit(app);

            //Assert
            UsingDbContext(context => context.AppJobs.FirstOrDefault(a => a.Id == exitingAppJob.Id)).Name.ShouldBe("App Job Is Updated");
        }

        [Fact]
        public async Task Should_Delete_App_Job()
        {
            //Arrange
            var exitingAppJob = UsingDbContext(context => context.AppJobs.FirstOrDefault(a => a.Name == "App Job To Delete"));

            //Act
            await _appJobsAppService.Delete(new EntityDto<Guid>(Guid.Parse(exitingAppJob.Id.ToString())));

            //Assert
            UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Id == exitingAppJob.Id)).ShouldBeNull();
        }
    }
}
