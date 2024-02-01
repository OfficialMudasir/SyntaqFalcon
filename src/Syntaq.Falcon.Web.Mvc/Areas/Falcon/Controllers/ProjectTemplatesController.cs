using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.ProjectTemplates.Dtos;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.Tags.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectDeployments;
using Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectTemplates;
using Syntaq.Falcon.Web.Controllers;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_Projects)]
    public class ProjectTemplatesController : FalconControllerBase
    {
        private readonly IProjectsAppService _projectsAppService;
        private readonly IFormsAppService _formsAppService;
        private readonly ITagsAppService _tagAppService;
        private readonly ITagEntityTypesAppService _tagsEntityTypesAppService;
        private readonly ITagEntitiesAppService _tagsEntitiesAppService;

        public ProjectTemplatesController(
            IProjectsAppService projectsAppService, 
            IFormsAppService formsAppService,
            ITagsAppService tagAppService,
            ITagEntityTypesAppService tagsEntityTypesAppService,
            ITagEntitiesAppService tagsEntitiesAppService
            )
        {
            _projectsAppService = projectsAppService;
            _formsAppService = formsAppService;
            _tagAppService = tagAppService;
            _tagsEntityTypesAppService = tagsEntityTypesAppService;
            _tagsEntitiesAppService = tagsEntitiesAppService;
        }

        public ActionResult Index()
        {

            return View();
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectTemplates_Create, AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task<ViewResult> CreateOrEdit(Guid? Id)
        {
            //initial a empty ProjectTemplateDto
            CreateOrEditProjectTemplateDto projectTemplate = new CreateOrEditProjectTemplateDto
            {
                Name = String.Empty,
                Description = "",
                StepsSchema = new List<ProjectTemplateStepDto>(),
                Enabled = true,
                Version = 1
            };
            projectTemplate.StepsSchema.Add(new ProjectTemplateStepDto
            {
                StepName = L("Step") + " 1",
                FormId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
            });

            PagedResultDto<GetTagEntityTypeForViewDto> tags = new PagedResultDto<GetTagEntityTypeForViewDto>();
            PagedResultDto<GetTagEntityForViewDto> entityTags = new PagedResultDto<GetTagEntityForViewDto>();

            if (Id.HasValue)
            {
                projectTemplate = await _projectsAppService.GetProjectTemplatesForEdit(new EntityDto<Guid> { Id = (Guid)Id });
            }

            // Get the Posible Tags for this Project Template
            GetAllTagEntityTypesInput taginput = new GetAllTagEntityTypesInput()
            {
                EntityTypeFilter = (int)EntityType.Project
            };

            tags = await _tagsEntityTypesAppService.GetAll(taginput);

            GetAllTagEntitiesInput tagentityinput = new GetAllTagEntitiesInput()
            {
                EntityIdFilter = Id
            };
            entityTags = await _tagsEntitiesAppService.GetAll(tagentityinput);

            // what the version is looking for? FormId or Form's originalId?
            List<FormListDto> formsList = _formsAppService.GetFormsList("all", "all");

            var viewModel = new CreateOrEditProjectTemplateViewModel
            {
                ProjectTemplate = projectTemplate,
                FormsList = formsList,
                Tags = tags,
                EntityTags = entityTags,
                VersionHistory = projectTemplate.ProjectId.HasValue? await _projectsAppService.GetVersionHistory(new EntityDto<Guid>((Guid)projectTemplate.ProjectId)):  new List<GetProjectTemplateForView>()
            };

            return View("CreateOrEdit", viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectTemplates_Create, AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? projecttemplateId)
        {
            //initial a empty ProjectTemplateDto
            CreateOrEditProjectTemplateDto projectTemplate = new CreateOrEditProjectTemplateDto
            {
                Name = "",
                Description = "",
                StepsSchema = new List<ProjectTemplateStepDto>(),
                Enabled = true
            };
            projectTemplate.StepsSchema.Add(new ProjectTemplateStepDto {
                StepName="",
                FormId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
            });

            PagedResultDto<GetTagEntityTypeForViewDto> tags = new PagedResultDto<GetTagEntityTypeForViewDto>();
            PagedResultDto<GetTagEntityForViewDto> entityTags = new PagedResultDto<GetTagEntityForViewDto>();

            if (projecttemplateId.HasValue)
            {
                projectTemplate = await _projectsAppService.GetProjectTemplatesForEdit(new EntityDto<Guid> { Id = (Guid)projecttemplateId });
            }

            // Get the Posible Tags for this Project Template
            GetAllTagEntityTypesInput taginput = new GetAllTagEntityTypesInput()
            {
                EntityTypeFilter = (int)EntityType.Project
            };

            tags = await _tagsEntityTypesAppService.GetAll(taginput);

            GetAllTagEntitiesInput tagentityinput = new GetAllTagEntitiesInput()
            {
                EntityIdFilter = projecttemplateId
            };
            entityTags = await _tagsEntitiesAppService.GetAll(tagentityinput);

            // what the version is looking for? FormId or Form's originalId?
            List<FormListDto> formsList = _formsAppService.GetFormsList("all", "all");

            var viewModel = new CreateOrEditProjectTemplateViewModel
            {
                ProjectTemplate = projectTemplate,
                FormsList = formsList,
                Tags = tags,
                EntityTags = entityTags
            };


            return PartialView("_CreateOrEditModal", viewModel);
		}

        public async Task<PartialViewResult> ViewProjectTemplateModal(Guid ProjectTemplateId)
        {

            var projectTemplate = await _projectsAppService.GetProjectTemplateForView(ProjectTemplateId);


            var viewModel = new ViewProjectTemplateViewModel
            {
                Name = projectTemplate.Name,
                Description = projectTemplate.Description,
                CreationTime = projectTemplate.CreationTime,
                LastModificationTime = projectTemplate.LastModificationTime,
                Enabled = projectTemplate.Enabled,
                StepsSchema = projectTemplate.StepsSchema

            };

            return PartialView("_ViewProjectTemplateModal", viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Projects_Create, AppPermissions.Pages_Projects_Edit)]
        public FileResult Export(Guid ProjectId)
        {
            MemoryStream returnedProject = _projectsAppService.ExportProject(ProjectId);

            byte[] returnedFileByte = returnedProject.ToArray();
            return File(returnedFileByte, "application/zip", "Project.zip");
        }
       

        public PartialViewResult ImportModal()
        {
            return PartialView("_ImportModal");
        }


        public bool Import()
        {

            try
            {
                var file = Request.Form.Files.First();
                if (file.Length > 0)
                {

                    using (var stream = file.OpenReadStream())
                    using (var archive = new System.IO.Compression.ZipArchive(stream))
                    {
                        var zipentry = archive.GetEntry("Project.json");
                        using (StreamReader reader = new StreamReader(zipentry.Open()))
                        {
                            var projectjson = reader.ReadToEnd();

                            ImportProjectInput input = new ImportProjectInput()
                            {
                                Project = projectjson
                            };
                            _projectsAppService.ImportProject(input);
                            return true;
                        }
                    }
                }  
            }
            catch (Exception ex)
            {
               // throw new Abp.UI.UserFriendlyException(ex.Message);
                return false;
            }
          
            return false;
        }

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    //Exception in parsing json
                    //Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public async Task<PartialViewResult> CreateOrEditTagsModal(Guid projectTemplateId)
        {

            PagedResultDto<GetTagEntityTypeForViewDto> tags = new PagedResultDto<GetTagEntityTypeForViewDto>();
            PagedResultDto<GetTagEntityForViewDto> entityTags = new PagedResultDto<GetTagEntityForViewDto>();

            //var projectTemplate = await _projectsAppService.GetProjectTemplatesForEdit(new EntityDto<Guid> { Id = (Guid)projectTemplateId });

            // Get the Posible Tags for this Project Template
            GetAllTagEntityTypesInput taginput = new GetAllTagEntityTypesInput()
            {
                EntityTypeFilter = (int)EntityType.Project,
                Sorting = ""
            };

            tags = await _tagsEntityTypesAppService.GetAll(taginput);
            GetAllTagEntitiesInput tagentityinput = new GetAllTagEntitiesInput()
            {
                EntityIdFilter = projectTemplateId,
                 
            };
            entityTags = await _tagsEntitiesAppService.GetAll(tagentityinput);

            var viewModel = new CreateOrEditProjectTemplateViewModel
            {
                ProjectTemplate = new CreateOrEditProjectTemplateDto()
                {
                    Id = projectTemplateId,
                    Name = "Temp"
                },
                Tags = tags,
                EntityTags = entityTags
            };

            return PartialView("_CreateOrEditTagsModal", viewModel);
        }


        [AbpMvcAuthorize(AppPermissions.Pages_Projects_Edit, AppPermissions.Pages_Projects_Create)]
        public PartialViewResult ProjectReleaseLookupTableModal(Guid? id, Guid? projectTemplateId, Guid? projectId, string displayName)
        {
            var viewModel = new ProjectDeploymentProjectReleaseLookupTableViewModel()
            {
                ProjectTemplateId = projectTemplateId.ToString(),
                ProjectId = projectId.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_ProjectReleaseLookupTableModal", viewModel);
        }

    }
}
