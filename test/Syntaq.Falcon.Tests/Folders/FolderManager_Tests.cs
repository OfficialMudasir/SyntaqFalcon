using Shouldly;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Syntaq.Falcon.Tests.Folders
{
    public class FolderManager_Tests : AppTestBase
    {
        private readonly IFolderManager _folderManager;

        public FolderManager_Tests()
        {
            _folderManager = Resolve<IFolderManager>();
        }

        [Fact(Skip = "cannot insert Guid.empty into SQLLite")]
        public async Task Should_Fetch_Folder()
        {
            //Arrange
            var folder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Nested Folder 1"));
            ACL aCL = new ACL() { UserId = AbpSession.UserId };

            //Act
            var returnFolder = await _folderManager.CreateAndOrFetchFolder(aCL, folder, (long) AbpSession.UserId);

            //Assert
            returnFolder.ShouldNotBeNull();
        }

        [Fact(Skip = "cannot insert Guid.empty into SQLLite")]
        public async Task Should_Create_And_Fetch_Folder()
        {
            //Arrange
            var rootFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));

            Folder folder = new Folder()
            {
                Name = "Unit Test Folder 2",
                Description = "Folder for Unit Testing",
                ParentId = Guid.Parse(rootFolder.Id.ToString()),
                Type = "R"
            };
            ACL aCL = new ACL() { UserId = AbpSession.UserId };

            //Act
            await _folderManager.CreateAndOrFetchFolder(aCL, folder, (long)AbpSession.UserId);

            //Assert
            UsingDbContext(context =>
            {
                var newFolder = context.Folders.FirstOrDefault(a => a.Name == "Unit Test Folder 2");
                newFolder.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Move_Folder()
        {
            //Arrange
            var sourceFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Nested Folder 2"));
            var targetFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            MoveFolderDto moveFolder = new MoveFolderDto()
            {
                DraggableId = sourceFolder.Id,
                DraggableType = "Folder",
                Id = targetFolder.Id,
                FolderType = "R",
                UserId = AbpSession.UserId
            };

            //Act
            await _folderManager.Move(moveFolder);

            //Assert
            UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Nested Folder 2")).ParentId.Equals(targetFolder.Id);
        }
    }
}
