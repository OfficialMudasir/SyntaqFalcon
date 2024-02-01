using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Newtonsoft.Json;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Storage;
using Syntaq.Falcon.Web;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Timing.Timezone;
using Syntaq.Falcon.EntityFrameworkCore.Repositories;
using Syntaq.Falcon.EntityVersionHistories;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Projects.Exporting;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Tags;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Apps.Dtos;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Utility;
using Syntaq.Falcon.Projects.Dtos;

namespace Syntaq.Falcon.Projects
{
    public class ProjectManager : FalconDomainServiceBase
    {
        public IAbpSession _abpSession { get; set; }
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<ProjectRelease, Guid> _projectReleaseRepository;
        private readonly IRepository<Folder, Guid> _folderRepository;
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<FormRule, Guid> _formRuleRepository;
        private readonly IRepository<AppJob, Guid> _appJobRepository;
        private readonly IRepository<Template, Guid> _templateRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly ACLManager _ACLManager;

        public ProjectManager(
            IRepository<Project, Guid> projectRepository,
            IRepository<ProjectRelease, Guid> projectReleaseRepository,
            IRepository<Form, Guid> formRepository,
            IRepository<Folder, Guid> folderRepository,
            IRepository<FormRule, Guid> formRuleRepository,
            IRepository<AppJob, Guid> appJobRepository,
            IRepository<Template, Guid> templateRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ACLManager aclManager
        )
        {
            _ACLManager = aclManager;
            _projectRepository = projectRepository;
            _projectReleaseRepository = projectReleaseRepository;

            _folderRepository = folderRepository;
            _formRepository = formRepository;
            _templateRepository = templateRepository;

            _formRuleRepository = formRuleRepository;
            _appJobRepository = appJobRepository;

            _unitOfWorkManager = unitOfWorkManager;

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public MemoryStream ExportProject(Guid id)
        {

            // _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete);

            //ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            //if (ACLResult.IsAuthed || true)
            //{

            ProjectExport projectexport = new ProjectExport();

            var project = _projectRepository.GetAll().Include(p => p.RecordFk).ThenInclude(pr => pr.RecordMatters).FirstOrDefault(p => p.Id == id);
            if (project != null)
            {
                List<Form> forms = new List<Form>();
                List<FormRule> formRules = new List<FormRule>();
                List<Template> templates = new List<Template>();
                List<AppJob> appJobs = new List<AppJob>();
                foreach (RecordMatter rm in project.RecordFk.RecordMatters)
                {

                    var thisform = _formRepository.GetAll().Include(f => f.Folder).FirstOrDefault(f => f.Id == rm.FormId);

                    if (thisform != null)
                    {

                        var theseforms = _formRepository.GetAll().Include(f => f.Folder).Where(f => f.OriginalId == thisform.OriginalId);
                        theseforms.ToList().ForEach(tf =>
                        {

                            var form = _formRepository.GetAll().Include(f => f.Folder).FirstOrDefault(f => f.Id == tf.Id);
                            if (form != null && !forms.Any(f => f.Id == tf.Id))
                            {
                                forms.Add(form);

                                var formrules = _formRuleRepository.GetAll().Where(fr => fr.FormId == form.Id);
                                formrules.ToList().ForEach(r =>
                                {
                                    formRules.Add(r);
                                });

                                var appjob = _appJobRepository.GetAll().FirstOrDefault(a => a.EntityId == form.Id);
                                if (appjob != null && !appJobs.Any(a => a.Id == form.Id))
                                {
                                    appJobs.Add(appjob);
                                    CreateOrEditAppJobDto appJobDto = JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(appjob.Data);
                                    if (appJobDto != null)
                                    {
                                        foreach (CreateOrEditAppJobDocumentDto document in appJobDto.Document)
                                        {
                                            var elements = document.DocumentTemplateURL.Split(new string[] { "?", "=", "&" }, StringSplitOptions.None);
                                            Guid docGuid = Guid.NewGuid();
                                            elements.ToList().ForEach(e =>
                                            {
                                                if (Guid.TryParse(e, out var guid))
                                                {
                                                    docGuid = new Guid(e);
                                                }
                                            });


                                            elements = document.DocumentTemplateURL.Split(new string[] { "version=" }, StringSplitOptions.None);
                                            string docVer = "live";
                                            if (elements.Length > 1)
                                            {
                                                docVer = elements[1];
                                            }

                                            // Include All Versions
                                            _templateRepository.GetAll().Include(t => t.Folder).Where(t => t.Id == docGuid || t.OriginalId == docGuid).ToList().ForEach(t =>
                                            {
                                                if (!templates.Any(t2 => t2.Id == t.Id)) templates.Add(t);
                                            });


                                        }
                                    }

                                }

                                buildFormSchemas(forms, appJobs, templates, formRules, form);

                            }
                        });
                    }


                }

                projectexport.Project = project;
                projectexport.Forms = forms;
                projectexport.Templates = templates;
                projectexport.FormRules = formRules;
                projectexport.AppJobs = appJobs;

                string jsonProjectExport = Newtonsoft.Json.JsonConvert.SerializeObject(projectexport, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                var len = jsonProjectExport.Length;

                MemoryStream memoryStream = new MemoryStream();
                using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {

                    ZipArchiveEntry readmeEntry = zip.CreateEntry("Project.json");
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        writer.WriteLine(jsonProjectExport);
                    }

                }
                memoryStream.Position = 0;
                return memoryStream;

            }
            //}
            //else
            //{
            //    throw new UserFriendlyException("Not Authorised");
            //}

            return null;

        }

        private void buildFormSchemas(List<Form> forms, List<AppJob> appJobs, List<Template> templates, List<FormRule> formRules, Form parentForm)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete);

            if (parentForm != null)
            {
                JObject formSchemaJObj = JObject.Parse(parentForm.Schema);
                var nestedFormTokens = formSchemaJObj.FindMatchingTokens("type", "nestedform");
                var popFormTokens = formSchemaJObj.FindMatchingTokens("type", "popupform");

                nestedFormTokens.AddRange(popFormTokens);

                foreach (JToken nestedform in nestedFormTokens)
                {
                    string formid = "";

                    // NestedForms use FormId
                    //Popup Forms use formid
                    // :( :( :( :(

                    if (nestedform["FormId"] != null)
                    {
                        formid = nestedform["FormId"].ToString();
                    }

                    if (nestedform["formId"] != null)
                    {
                        formid = nestedform["formId"].ToString();
                    }


                    var thisform = _formRepository.GetAll().Include(f => f.Folder).FirstOrDefault(f => f.Id == new Guid(formid));
                    if (thisform != null)
                    {

                        var theseforms = _formRepository.GetAll().Include(f => f.Folder).Where(f => f.OriginalId == thisform.OriginalId);
                        theseforms.ToList().ForEach(tf =>
                        {

                            var form = _formRepository.GetAll().Include(f => f.Folder).FirstOrDefault(f => f.Id == tf.Id);
                            if (form != null && !forms.Any(f => f.Id == tf.Id))
                            {
                                forms.Add(form);

                                var formrules = _formRuleRepository.GetAll().Where(fr => fr.FormId == form.Id);
                                formrules.ToList().ForEach(r =>
                                {
                                    formRules.Add(r);
                                });

                                var appjob = _appJobRepository.GetAll().FirstOrDefault(args => args.EntityId == form.Id);
                                if (appjob != null && !appJobs.Any(a => a.Id == form.Id))
                                {
                                    appJobs.Add(appjob);

                                    CreateOrEditAppJobDto appJobDto = JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(appjob.Data);
                                    if (appJobDto != null)
                                    {
                                        foreach (CreateOrEditAppJobDocumentDto document in appJobDto.Document)
                                        {


                                            var elements = document.DocumentTemplateURL.Split(new string[] { "?", "=", "&" }, StringSplitOptions.None);
                                            Guid docGuid = Guid.NewGuid();
                                            elements.ToList().ForEach(e =>
                                            {
                                                if (Guid.TryParse(e, out var guid))
                                                {
                                                    docGuid = new Guid(e);
                                                }
                                            });


                                            elements = document.DocumentTemplateURL.Split(new string[] { "version=" }, StringSplitOptions.None);
                                            string docVer = "live";
                                            if (elements.Length > 1)
                                            {
                                                docVer = elements[1];
                                            }

                                            // Include All Versions
                                            _templateRepository.GetAll().Include(t => t.Folder).Where(t => t.Id == docGuid || t.OriginalId == docGuid).ToList().ForEach(t =>
                                            {
                                                if (!templates.Any(t2 => t2.Id == t.Id)) templates.Add(t);
                                            });

                                        }
                                    }
                                }
                                buildFormSchemas(forms, appJobs, templates, formRules, form);
                            }
                        });
                    }
                }
            }
        }

        public GetFormFromReleaseOutput GetFormFromRelease(Guid releaseId, Guid? formId)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            GetFormFromReleaseOutput result = new GetFormFromReleaseOutput();
            Form f = null;

            ProjectRelease pr = _projectReleaseRepository.FirstOrDefault(releaseId);
            if (pr != null)
            {
                ProjectExport pt = null; // Project Template
                using (var stream = new MemoryStream(pr.Artifact))
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
                        pt = JsonConvert.DeserializeObject<ProjectExport>(input.Project);
                    }
                }

                result.ProjectEnvironmentId = pr.ProjectEnvironmentId;
                result.Project = pt.Project;
                result.AppJobs = pt.AppJobs;
                if (pt.Forms != null)
                {
                    if (formId.HasValue)
                    {
                        f = pt.Forms.First(f => f.Id == formId);
                    }
                 
                    result.Form = f;
                }

            }
          
            return result;
        }
    }

    public class GetFormFromReleaseOutput{

        public List<AppJob> AppJobs { get; set; }        
        public Form Form { get; set; }
        public Project Project { get; set; }

        public Guid? ProjectEnvironmentId { get; set; }

    }
}
