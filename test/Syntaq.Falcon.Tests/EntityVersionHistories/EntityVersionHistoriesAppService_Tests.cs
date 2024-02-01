using Abp.Application.Services.Dto;
using Shouldly;
using Syntaq.Falcon.EntityVersionHistories;
using Syntaq.Falcon.EntityVersionHistories.Dtos;
using Syntaq.Falcon.Features;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Syntaq.Falcon.Forms;
using System;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Documents.Dtos;
using Syntaq.Falcon.ProjectTemplates.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Tests.EntityVersionHistories
{
    // ReSharper disable once InconsistentNaming
    public class EntityVersionHistoriesAppService_Tests : AppTestBase
    {
        private readonly IFormsAppService _formRepository;
        private readonly IProjectsAppService _projectRepository;
        private readonly ITemplatesAppService _templateRepository;
        private readonly IEntityVersionHistoriesAppService _entityVersionHistoryRepository;

        public EntityVersionHistoriesAppService_Tests()
        {
            LoginAsHostAdmin();

            _formRepository = Resolve<IFormsAppService>();
            _projectRepository = Resolve<IProjectsAppService>();
            _templateRepository = Resolve<ITemplatesAppService>();
            _entityVersionHistoryRepository = Resolve<IEntityVersionHistoriesAppService>();
        }

        [Fact]
        public async Task Should_Get_All_EntityVersionHistory()
        {
            //Act
            var output = await _entityVersionHistoryRepository.GetAll(new GetAllEntityVersionHistoriesInput());

            //Assert
            output.Items.Count.ShouldBe(3);
        }

        [Fact]
        public async Task Should_Create_EntityVersionHistories_FormCreate()
        {
            //var initialTaskCount = UsingDbContext(context => context.Tasks.Count());
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            //Arrange
            CreateOrEditFormDto form = new CreateOrEditFormDto
            {
                Id = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                Name = "GMC",
                VersionName = "Version 1",
                Description = "form description",
                PaymentEnabled = true,
                PaymentAmount = 0,
                PaymentCurrency = null,
                PaymentProcess = null,
                PaymentPublishableToken = null,
                Version = 1,
                CurrentVersion = 1,
                FolderId = exitingFolder.Id,
                Schema = null,
                Script = null,
                RulesScript = null,
                OriginalId = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                IsIndex = "1"
            };

            //Assert.Throws<DbUpdateException>(() => DbContext.SaveChanges());

            //Act
            await _formRepository.CreateOrEdit(form);

            //Assert
            UsingDbContext(context =>
            {
                var newVH = context.EntityVersionHistories.FirstOrDefault(a => a.Name == "GMC");
                newVH.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Create_EntityVersionHistories_FormEdit()
        {

            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            //Arrange
            CreateOrEditFormDto formold = new CreateOrEditFormDto
            {
                Id = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                Name = "GMC",
                VersionName = "Version 1",
                Description = "form description",
                PaymentEnabled = true,
                PaymentAmount = 0,
                PaymentCurrency = null,
                PaymentProcess = null,
                PaymentPublishableToken = null,
                Version = 1,
                CurrentVersion = 1,
                FolderId = exitingFolder.Id,
                Schema = null,
                Script = null,
                RulesScript = null,
                OriginalId = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                IsIndex = "1"
            };

            //Act
            await _formRepository.CreateOrEdit(formold);


            //Arrange
            var exitingEVH = UsingDbContext(context => context.EntityVersionHistories.FirstOrDefault(a => a.Name == "GMC"));
            exitingEVH.ShouldNotBeNull();


            CreateOrEditFormDto formnew = new CreateOrEditFormDto
            {
                Id = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                Name = "GMC",
                VersionName = "Version 1",
                Description = "form description",
                PaymentEnabled = true,
                PaymentAmount = 0,
                PaymentCurrency = null,
                PaymentProcess = null,
                PaymentPublishableToken = null,
                Version = 1,
                CurrentVersion = 1,
                FolderId = exitingFolder.Id,
                Schema = "{\"type\": \"form\", \"display\": \"form\"}",
                Script = null,
                RulesScript = null,
                OriginalId = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                IsIndex = "1"
            };


            //Act
            await _formRepository.CreateOrEdit(formnew);

            
            //Assert
            UsingDbContext(context =>
            {

                var newVH = context.EntityVersionHistories.First(a => a.Name == "GMC" && a.Description == "Update the live Version");
                newVH.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Create_EntityVersionHistories_TemplateCreate()
        {
            //var initialTaskCount = UsingDbContext(context => context.Tasks.Count());
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            //Arrange
            CreateOrEditTemplateDto template = new CreateOrEditTemplateDto
            {
                Id = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                Name = "GMC Template",
                Version = 1,
                CurrentVersion = 1,
                FolderId = exitingFolder.Id,
                LockToTenant =false,
                OriginalId = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
            };

            //Assert.Throws<DbUpdateException>(() => DbContext.SaveChanges());

            //Act
            await _templateRepository.CreateOrEdit(template);

            //Assert
            UsingDbContext(context =>
            {
                var newVH = context.EntityVersionHistories.FirstOrDefault(a => a.Name == "GMC Template");
                newVH.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Create_EntityVersionHistories_TemplateEdit()
        {
            //var initialTaskCount = UsingDbContext(context => context.Tasks.Count());
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            //Arrange
            CreateOrEditTemplateDto template = new CreateOrEditTemplateDto
            {
                Id = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                Name = "GMC Template",
                Version = 1,
                CurrentVersion = 1,
                FolderId = exitingFolder.Id,
                LockToTenant = false,
                OriginalId = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
            };

            //Act
            await _templateRepository.CreateOrEdit(template);

            //Assert
            //Arrange
            var exitingEVH = UsingDbContext(context => context.EntityVersionHistories.FirstOrDefault(a => a.Name == "GMC Template"));
            exitingEVH.ShouldNotBeNull();

            CreateOrEditTemplateDto templatenew = new CreateOrEditTemplateDto
            {
                Id = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                Name = "GMC Template",
                Comments = "update GMC template description",
                Version = 1,
                CurrentVersion = 1,
                FolderId = exitingFolder.Id,
                LockToTenant = false,
                OriginalId = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
            };

            await _templateRepository.CreateOrEdit(templatenew);

            //Assert
            UsingDbContext(context =>
            {
                var newVH = context.EntityVersionHistories.First(a => a.Name == "GMC Template" && a.Description == "Update the live Version");
                newVH.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Create_EntityVersionHistories_ProjectTemplateCreate()
        {
            //var initialTaskCount = UsingDbContext(context => context.Tasks.Count());
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            //Arrange
            var pts = new ProjectTemplateStepDto
            {
                StepName = "s1",
                FormId = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
            };

            var lists = new List<ProjectTemplateStepDto>();
            lists.Add(pts);

            CreateOrEditProjectTemplateDto pt = new CreateOrEditProjectTemplateDto
            {
                Name = "GMC ProjectTemplate",
                Description = null,
                StepsSchema = lists,
                Tags = new List<ProjectTagsDto>(),
            };

            //Assert.Throws<DbUpdateException>(() => DbContext.SaveChanges());

            //Act
            await _projectRepository.CreateOrEditProjectTemplate(pt);

            //Assert
            UsingDbContext(context =>
            {
                var newVH = context.EntityVersionHistories.FirstOrDefault(a => a.Name == "GMC ProjectTemplate");
                newVH.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Create_EntityVersionHistories_ProjectTemplateEdit()
        {
            //var initialTaskCount = UsingDbContext(context => context.Tasks.Count());
            //var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            ////Arrange
            var pts = new ProjectTemplateStepDto
            {
                StepName = "s1",
                FormId = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
            };

            var lists = new List<ProjectTemplateStepDto>();
            lists.Add(pts);

            //CreateOrEditProjectTemplateDto pt = new CreateOrEditProjectTemplateDto
            //{
            //    Name = "GMC ProjectTemplate",
            //    Description = null,
            //    StepsSchema = lists,
            //    Tags = new List<ProjectTagsDto>(),
            //};

            ////Assert.Throws<DbUpdateException>(() => DbContext.SaveChanges());

            ////Act
            //await _projectRepository.CreateOrEditProjectTemplate(pt);

            //Assert
            var exitingEVH = UsingDbContext(context => context.Projects.FirstOrDefault(a => a.Name == "GMC ProjectTemplate"));
            exitingEVH.ShouldNotBeNull();

            CreateOrEditProjectTemplateDto ptnew = new CreateOrEditProjectTemplateDto
            {
                Id = Guid.Parse("7aa46998-f41c-4438-b31a-8bd15ab2308b"),
                Name = "GMC ProjectTemplate",
                Description = "Update the live Version",
                StepsSchema = lists,
                Tags = new List<ProjectTagsDto>(),
            };

            await _projectRepository.CreateOrEditProjectTemplate(ptnew);

            //Assert
            UsingDbContext(context =>
            {
                var newVH = context.EntityVersionHistories.First(a => a.Name == "GMC ProjectTemplate" && a.Description == "Update the live Version");
                newVH.ShouldNotBeNull();
            });


        }





    }
}
