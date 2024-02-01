using Abp.Application.Services.Dto;
using Shouldly;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Syntaq.Falcon.Tests.Folders
{
    public class FolderAppService_Tests : AppTestBase
    {
        private readonly IFoldersAppService _foldersAppService;

        public FolderAppService_Tests()
        {
            _foldersAppService = Resolve<IFoldersAppService>();
        }

        [Fact(Skip = "specific reason")]
        public async Task Should_Get_Breadcrumbs()
        {
            //Arrange
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Nested Folder 1"));

            //Act
            var output = await _foldersAppService.GetBreadcrumbs(exitingFolder.Id.ToString(), exitingFolder.Type);

            //Assert
            output.Count.ShouldBe(3);
            output[0].Name.ShouldBe("Your Records");
            output[1].Name.ShouldBe("Folder To Update");
            output[2].Name.ShouldBe("Nested Folder 1");
        }

        [Fact]
        public async Task Should_Get_Folder_For_Edit()
        {
            //Arrange
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Folder To Update"));

            //Act
            var output = await _foldersAppService.GetFolderForEdit(new EntityDto<Guid>(Guid.Parse(exitingFolder.Id.ToString())));

            //Assert
            output.Folder.Name.ShouldBe("Folder To Update");
            output.Folder.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Create_Folder()
        {
            //Arrange
            var rootFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));

            CreateOrEditFolderDto folder = new CreateOrEditFolderDto()
            {
                Name = "Unit Test Folder 1",
                Description = "Folder for Unit Testing",
                ParentId = Guid.Parse(rootFolder.Id.ToString()),
                Type = "R"
            };

            //Act
            await _foldersAppService.CreateOrEdit(folder);

            //Assert
            UsingDbContext(context =>
            {
                var newFolder = context.Folders.FirstOrDefault(a => a.Name == "Unit Test Folder 1");
                newFolder.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Update_Folder()
        {
            //Arrange
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Folder To Update"));
            CreateOrEditFolderDto folder = new CreateOrEditFolderDto()
            {
                Id = Guid.Parse(exitingFolder.Id.ToString()),
                Name = "Folder Is Updated",
                Description = exitingFolder.Description,
                ParentId = Guid.Parse(exitingFolder.ParentId.ToString()),
                Type = exitingFolder.Type
            };

            //Act
            await _foldersAppService.CreateOrEdit(folder);

            //Assert
            UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Id == exitingFolder.Id)).Name.ShouldBe("Folder Is Updated");
        }

        [Fact]
        public async Task Should_Delete_Folder()
        {

            //Arrange
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Folder To Delete"));

            //Act
            await _foldersAppService.Delete(new EntityDto<Guid>(Guid.Parse(exitingFolder.Id.ToString())));

            //Assert
            UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Id == exitingFolder.Id &&  a.IsDeleted == false)).ShouldBeNull();
        }
    }
}
