using Abp.Application.Services.Dto;
using Shouldly;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Apps.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Syntaq.Falcon.EntityFrameworkCore;

namespace Syntaq.Falcon.Tests.Apps
{
    public class AppsAppService_Tests : AppTestBase
    {
        private readonly IAppsAppService _appsAppService;
        

        public AppsAppService_Tests()
        {
            _appsAppService = Resolve<IAppsAppService>();
            
        }

        //[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Get_All_Apps()
        {
           
            //Act
            var output = await _appsAppService.GetAll(new GetAllAppsInput());
            
            //Assert
            //output.TotalCount.ShouldBe(2);
            output.Items.Count.ShouldBe(4); //because FilteredApps = FilteredApps.Concat(SharedApps).ToList();
        }

        //[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Get_App_For_Edit()
        {
            //Arrange
            var exitingApp = UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Name == "App To Update"));

            //Act
            var output = await _appsAppService.GetAppForEdit(new EntityDto<Guid>(Guid.Parse(exitingApp.Id.ToString())));

            //Assert
            output.App.Name.ShouldBe("App To Update");
            output.App.ShouldNotBeNull();
        }

        //[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Create_App()
        {
            //Arrange
            CreateOrEditAppDto app = new CreateOrEditAppDto()
            {
                Name = "Unit Test App",
                Description = "App for Unit Testing",
                Data = "{}"
            };

            //Act
            await _appsAppService.CreateOrEdit(app);

            //Assert
            UsingDbContext(context =>
            {
                var newApp = context.Apps.FirstOrDefault(a => a.Name == "Unit Test App");
                newApp.ShouldNotBeNull();
            });
        }

        //[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Update_App()
        {
            //Arrange
            var exitingApp = UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Name == "App To Update"));
            CreateOrEditAppDto app = new CreateOrEditAppDto()
            {
                Id = Guid.Parse(exitingApp.Id.ToString()),
                Name = "App Is Updated",
                Description = exitingApp.Description,
                Data = exitingApp.Data
            };

            //Act
            await _appsAppService.CreateOrEdit(app);

            //Assert
            UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Id == exitingApp.Id)).Name.ShouldBe("App Is Updated");
        }

        [Fact(Skip = "specific reason")]
        //[Fact]
        public async Task Should_Update_App_Data()
        {
            //Arrange
            var exitingApp = UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Name == "App To Update"));
            CreateOrEditAppDto app = new CreateOrEditAppDto()
            {
                Id = Guid.Parse(exitingApp.Id.ToString()),
                Name = exitingApp.Name,
                Description = exitingApp.Description,
                Data = "{\"FirstName\":\"Tester\"}"
            };

            //Act
            await _appsAppService.CreateOrEditData(app);

            //Assert
            UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Id == exitingApp.Id)).Data.ShouldBe("{\"FirstName\":\"Tester\"}");
        }

        //[Fact(Skip = "specific reason")]
        [Fact]
        public async Task Should_Delete_App()
        {
            //Arrange
            var exitingApp = UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Name == "App To Delete"));

            //Act
            await _appsAppService.Delete(new EntityDto<Guid>(Guid.Parse(exitingApp.Id.ToString())));

            //Assert
            UsingDbContext(context => context.Apps.FirstOrDefault(a => a.Id == exitingApp.Id)).ShouldBeNull();
        }
    }
}
