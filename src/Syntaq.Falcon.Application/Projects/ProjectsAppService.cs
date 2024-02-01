using Syntaq.Falcon.Records;
using Syntaq.Falcon.Projects;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.Projects.Exporting;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using static Syntaq.Falcon.Projects.ProjectConsts;
using Syntaq.Falcon.EntityFrameworkCore.Repositories;
using Syntaq.Falcon.AccessControlList;
using Abp.UI;
using Abp.Domain.Uow;
using Syntaq.Falcon.Records.Dtos;
using GetAllForLookupTableInput = Syntaq.Falcon.Projects.Dtos.GetAllForLookupTableInput;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static Syntaq.Falcon.Records.RecordMatterContributorConsts;
using Syntaq.Falcon.ProjectTemplates.Dtos;
using Syntaq.Falcon.AccessControlList.Dtos;
using System.Xml;
using System.Text.Json;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Utility;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Apps.Dtos;
using System.IO.Compression;
using System.IO;
using Syntaq.Falcon.Folders;
using static Syntaq.Falcon.Records.RecordMatterConsts;
using Abp.Timing;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.Files.Dtos;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Syntaq.Falcon.Web;
using Microsoft.Extensions.Options;
using Abp.Timing.Timezone;
using Syntaq.Falcon.EntityVersionHistories;
using JsonDiffer;
using JsonDiffPatchDotNet;
using Syntaq.Falcon.Submissions;
using System.Collections;
using Syntaq.Falcon.Auditing;
using Abp.Auditing;
using Syntaq.Falcon.Migrations;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Authorization.Users;
using Org.BouncyCastle.Crypto.Paddings;
using Stripe;
using Abp.Authorization.Users;
using Syntaq.Falcon.Authorization.Roles;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Ganss.Xss;

namespace Syntaq.Falcon.Projects
{
    [AbpAuthorize(AppPermissions.Pages_Projects)]
    public class ProjectsAppService : FalconAppServiceBase, IProjectsAppService
    {

        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<ProjectRelease, Guid> _projectReleaseRepository;
        private readonly IRepository<ProjectDeployment, Guid> _projectDeploymentRepository;
        private readonly IRepository<ProjectRelease, Guid> _lookup_projectReleaseRepository;
        private readonly ProjectManager _projectManager;

        private readonly IRepository<Folder, Guid> _folderRepository;
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<FormRule, Guid> _formRuleRepository;
        private readonly IRepository<AppJob, Guid> _appJobRepository;
        private readonly IRepository<Template, Guid> _templateRepository;
        private readonly IProjectsExcelExporter _projectsExcelExporter;
        private readonly IRepository<Record, Guid> _lookup_recordRepository;

        private readonly ICustomRecordsRepository _customrecordRepository;
        private readonly ICustomProjectsRepository _customProjectRepository;

        private readonly RecordManager _recordManager;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
        private readonly IRepository<RecordMatterAudit, Guid> _recordMatterAuditRepository;
        private readonly IRepository<ACL> _aclRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly ACLManager _ACLManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        private readonly IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IRepository<EntityVersionHistory, Guid> _entityVersionHistoryRepository;
        private readonly IOptions<StorageConnection> _storageConnection;

        private readonly IRepository<TagEntity, Guid> _tagEntityRepository;
        private readonly IRepository<TagValue, Guid> _tagValueRepository;

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IRepository<Submission, Guid> _submissionRepository;

        public ProjectsAppService(
            IRepository<Project, Guid> projectRepository,
            IRepository<ProjectRelease, Guid> projectReleaseRepository,
            IRepository<ProjectDeployment, Guid> projectDeploymentRepository,
            IRepository<ProjectRelease, Guid> lookup_projectReleaseRepository,
            ProjectManager projectManager,
            IRepository<Form, Guid> formRepository,
            IRepository<Folder, Guid> folderRepository,
            IRepository<FormRule, Guid> formRuleRepository,
            IRepository<AppJob, Guid> appJobRepository,
            IRepository<Template, Guid> templateRepository,
            IRepository<ACL> aclRepository,
            IProjectsExcelExporter projectsExcelExporter,
            IRepository<Record, Guid> lookup_recordRepository,
            ICustomRecordsRepository customrecordRepository,
            ICustomProjectsRepository customProjectRepository,
            RecordManager recordManager,
            IRepository<RecordMatter, Guid> recordMatterRepository,
            IRepository<RecordMatterAudit, Guid> recordMatterAuditRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ACLManager aclManager,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository,
            IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
            IRepository<EntityVersionHistory, Guid> entityVersionHistoryRepository,
            IRepository<TagEntity, Guid> tagEntityRepository,
            IRepository<TagValue, Guid> tagValueRepository,
            IOptions<StorageConnection> storageConnection,
            ITimeZoneConverter timeZoneConverter,
            IRepository<Submission, Guid> submissionRepository
        )
        {
            _ACLManager = aclManager;
            _projectRepository = projectRepository;
            _projectReleaseRepository = projectReleaseRepository;
            _projectDeploymentRepository = projectDeploymentRepository;
            _lookup_projectReleaseRepository = lookup_projectReleaseRepository;
            _projectManager = projectManager;

            _folderRepository = folderRepository;
            _formRepository = formRepository;
            _templateRepository = templateRepository;
            _aclRepository = aclRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;

            _ACLManager = aclManager;
            _formRuleRepository = formRuleRepository;
            _appJobRepository = appJobRepository;

            _projectsExcelExporter = projectsExcelExporter;
            _lookup_recordRepository = lookup_recordRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _customrecordRepository = customrecordRepository;
            _customProjectRepository = customProjectRepository;
            _recordManager = recordManager;
            _recordMatterContributorRepository = recordMatterContributorRepository;
            _recordMatterRepository = recordMatterRepository;
            _recordMatterAuditRepository = recordMatterAuditRepository;
            _recordMatterItemRepository = recordMatterItemRepository;
            _entityVersionHistoryRepository = entityVersionHistoryRepository;

            _tagEntityRepository = tagEntityRepository;
            _tagValueRepository = tagValueRepository;

            _timeZoneConverter = timeZoneConverter;

            _storageConnection = storageConnection;
            _submissionRepository = submissionRepository;
        }

        // ?? Remove ?
        public async Task<PagedResultDto<GetProjectForViewDto>> GetAll(GetAllProjectsInput input)
        {
            input.Filter = input.Filter?.Trim();

            var statusFilter = input.StatusFilter.HasValue
                        ? (ProjectStatus)input.StatusFilter
                        : default;
            var typeFilter = input.TypeFilter.HasValue
                ? (ProjectType)input.TypeFilter
                : default;
          
            var filteredProjects = _projectRepository.GetAll()
                        .Include(e => e.RecordFk)
                        .Where(e => e.Type == ProjectType.User && e.CreatorUserId == AbpSession.UserId && e.Archived == false)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(input.StatusFilter.HasValue && input.StatusFilter > -1, e => e.Status == statusFilter)
                        .WhereIf(input.TypeFilter.HasValue && input.TypeFilter > -1, e => e.Type == typeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RecordRecordNameFilter), e => e.RecordFk != null && e.RecordFk.RecordName == input.RecordRecordNameFilter)
                        .Select(o => new
                        {
                            o.RecordId,
                            o.Name,
                            o.Description,
                            o.Status,
                            o.Type,
                            o.Id,
                            o.CreationTime,
                            LastModificationTime = o.LastModificationTime == null ? o.CreationTime : (DateTime)o.LastModificationTime
                        }
                );

            var pagedAndFilteredProjects = filteredProjects
               .OrderBy(input.Sorting ?? "LastModificationTime desc")
               .PageBy(input);

            var projects = from o in pagedAndFilteredProjects
                           join o1 in _lookup_recordRepository.GetAll() on o.RecordId equals o1.Id into j1
                           from s1 in j1.DefaultIfEmpty()
                           select new GetProjectForViewDto()
                           {
                               Project = new ProjectDto
                               {
                                   Name = o.Name,
                                   Description = o.Description,
                                   Status = o.Status,
                                   Type = o.Type,
                                   Id = o.Id,
                                   CreationTime = o.CreationTime,
                                   LastModificationTime = o.LastModificationTime == null ? o.CreationTime : (DateTime)o.LastModificationTime,
                               },
                               RecordRecordName = s1 == null || s1.RecordName == null ? "" : s1.RecordName.ToString()
                           };
 

            var totalCount = await filteredProjects.CountAsync();

            return new PagedResultDto<GetProjectForViewDto>(
                totalCount,
                await projects.ToListAsync()
            );

        }

        // Projects shared NOT ProjectSteps
        //public async Task<PagedResultDto<GetProjectForViewDto>> GetSharedProjects(GetAllProjectsInput input)
        //{       

        //     var statusFilter = input.StatusFilter.HasValue
        //             ? (ProjectStatus)input.StatusFilter
        //             : default;
        //    var typeFilter = input.TypeFilter.HasValue
        //        ? (ProjectType)input.TypeFilter
        //        : default;

        //    var sharedProjectIds = _customProjectRepository.GetSharedProjects(AbpSession.UserId).Where(p => p.Type == ProjectType.User).Select(p => p.Id).Distinct();


        //    var filteredProjects = _projectRepository.GetAll()
        //                .Include(e => e.RecordFk)
        //                .Where(e => e.Type == ProjectType.User && sharedProjectIds.Contains(e.Id) && e.Archived == false)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
        //                .WhereIf(input.StatusFilter.HasValue && input.StatusFilter > -1, e => e.Status == statusFilter)
        //                .WhereIf(input.TypeFilter.HasValue && input.TypeFilter > -1, e => e.Type == typeFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.RecordRecordNameFilter), e => e.RecordFk != null && e.RecordFk.RecordName == input.RecordRecordNameFilter)
        //                .Select(o => new
        //                {
        //                    o.RecordId,
        //                    o.Name,
        //                    o.Description,
        //                    o.Status,
        //                    o.Type,
        //                    o.Id,
        //                    o.CreationTime,
        //                    LastModificationTime = o.LastModificationTime == null ? o.CreationTime : (DateTime)o.LastModificationTime
        //                }
        //        );

        //    var pagedAndFilteredProjects = filteredProjects
        //       .OrderBy(input.Sorting ?? "LastModificationTime desc")
        //       .PageBy(input);

        //    var projects = from o in pagedAndFilteredProjects
        //                   join o1 in _lookup_recordRepository.GetAll() on o.RecordId equals o1.Id into j1
        //                   from s1 in j1.DefaultIfEmpty()
        //                   select new GetProjectForViewDto()
        //                   {
        //                       Project = new ProjectDto
        //                       {
        //                           Name = o.Name,
        //                           Description = o.Description,
        //                           Status = o.Status,
        //                           Type = o.Type,
        //                           Id = o.Id,
        //                           CreationTime = o.CreationTime,
        //                           LastModificationTime = o.LastModificationTime == null ? o.CreationTime : (DateTime)o.LastModificationTime,
        //                       },
        //                       RecordRecordName = s1 == null || s1.RecordName == null ? "" : s1.RecordName.ToString()
        //                   };



        //    var totalCount = await filteredProjects.CountAsync();

        //    return new PagedResultDto<GetProjectForViewDto>(
        //        totalCount,
        //        await projects.ToListAsync()
        //    );

        //}

        public async Task<PagedResultDto<ShareProjectForViewDto>> GetSharedProjects(GetAllProjectsInput input)
        {
            var statusFilter = input.StatusFilter.HasValue
                        ? (ProjectStatus)input.StatusFilter
                        : default;
            var typeFilter = input.TypeFilter.HasValue
                ? (ProjectType)input.TypeFilter
                : default;

            var sharedProjects = _recordMatterContributorRepository.GetAll()
                .Where(c => c.UserId == AbpSession.UserId && c.Enabled == true)
                .Join(_recordMatterRepository.GetAll().Where(rm => rm.TenantId == AbpSession.TenantId),
                RecordMatterContributor => RecordMatterContributor.RecordMatterId,
                RecordMatter => RecordMatter.Id,
                (RecordMatterContributor, RecordMatter) => new
                {
                    Organization = RecordMatterContributor.OrganizationName,
                    //RecordMatterContributor.Name,
                    Role = RecordMatterContributor.StepRole,
                    //RecordMatterContributor.Email,
                    RecordMatterContributor.CreatorUserId,
                    RecordMatter.RecordId,
                    ProjectStepName = RecordMatter.RecordMatterName,
                    Status = RecordMatterContributor.Status,
                    Action = RecordMatterContributor.StepAction, //maybe useless
                    RecordMatterContributor.AccessToken,
                    RecordMatterContributor.RecordMatterId
                }
                )
                .Join(_projectRepository.GetAll()
                .Where(p => p.TenantId == AbpSession.TenantId && p.Type == ProjectConsts.ProjectType.User)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter)),
                RecordMatterContributor => RecordMatterContributor.RecordId,
                Project => Project.RecordId,
                (RecordMatterContributor, Project) => new
                {
                    RecordMatterContributor.Organization,
                    //RecordMatterContributor.Name,
                    RecordMatterContributor.Role,
                    //RecordMatterContributor.Email,
                    RecordMatterContributor.CreatorUserId,
                    ProjectName = Project.Name,
                    RecordMatterContributor.ProjectStepName,
                    RecordMatterContributor.Status,
                    RecordMatterContributor.Action,
                    LastModificationTime = Project.LastModificationTime,
                    RecordMatterContributor.AccessToken,
                    RecordMatterContributor.RecordMatterId
                })
                .OrderBy(input.Sorting ?? "LastModificationTime desc");

            var pagedAndFilteredp = sharedProjects.PageBy(input);


            var projects = from c in pagedAndFilteredp
                           select new ShareProjectForViewDto()
                           {
                               Role = c.Role,
                               ProjectName = c.ProjectName,
                               ProjectStepName = c.ProjectStepName,
                               Status = c.Status,
                               Action = c.Action,
                               // ActionCode = c.Action,
                               LastModificationTime = c.LastModificationTime,
                               AccessToken = c.AccessToken,
                               RecordMatterId = (Guid)c.RecordMatterId
                           };


            var totalCount = await sharedProjects.CountAsync();

            try
            {
                return new PagedResultDto<ShareProjectForViewDto>(
                    totalCount,
                    await projects.ToListAsync()
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        public async Task<PagedResultDto<GetProjectForViewDto>> GetForUser(GetAllProjectsInput input)
        {

            // Or get projects where you are in a roles that permission has been granted to


            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

           // var sharedProjects = _customProjectRepository.GetSharedProjects(AbpSession.UserId).Where(p => p.Type == ProjectType.Template).Select(p => p.Id).Distinct();
            var statusFilter = input.StatusFilter.HasValue
                        ? (ProjectStatus)input.StatusFilter
                        : default;

            var typeFilter = input.TypeFilter.HasValue
                ? (ProjectType)input.TypeFilter
                : default;

            List<Guid> tags2 = new List<Guid>();
            if (!string.IsNullOrEmpty(input.Tags))
            {
                tags2 = JsonConvert.DeserializeObject<List<ProjectTagsDto>>(input.Tags).Select(i => i.Name).ToList();
            }
            
            var tagvalues = _tagValueRepository.GetAll().Where(tv => tags2.Contains( tv.Id )).Select(i => i.Id).ToList();
            var tagentities = _tagEntityRepository.GetAll().Where(te => tagvalues.Contains((Guid)te.TagValueId)).Select(i => i.EntityId).ToList();

            //var filteredProjects = _projectRepository.GetAll()
            //            .Include(e => e.RecordFk)
            //            .Where(e => e.CreatorUserId == AbpSession.UserId || (sharedProjects.Contains(e.Id))) // Include Shared projects
            //            .Where(e => e.Type == ProjectType.Template && e.Enabled == true)
            //            .WhereIf(tags2.Count > 0, e => tagentities.Contains(e.Id))
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
            //            .WhereIf(input.StatusFilter.HasValue && input.StatusFilter > -1, e => e.Status == statusFilter)
            //            .WhereIf(input.TypeFilter.HasValue && input.TypeFilter > -1, e => e.Type == typeFilter)
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.RecordRecordNameFilter), e => e.RecordFk != null && e.RecordFk.RecordName == input.RecordRecordNameFilter);

            //var pagedAndFilteredProjects = filteredProjects
            //    .OrderBy("name asc");


            //var query = from auditLog in _auditLogRepository.GetAll()
            //            join user in _userRepository.GetAll() on auditLog.UserId equals user.Id into userJoin
            //            from joinedUser in userJoin.DefaultIfEmpty()
            //            where auditLog.ExecutionTime >= input.StartDate && auditLog.ExecutionTime <= input.EndDate
            //            select new AuditLogAndUser { AuditLog = auditLog, User = joinedUser };


            //var result = from projectRelease in _projectReleaseRepository.GetAll()

            //             join project in _projectRepository.GetAll() on projectRelease.ProjectId equals project.Id into projectJoin
            //             from joinedProject in projectJoin

            //             join projectDeployment in _projectDeploymentRepository.GetAll() on projectRelease.Id equals projectDeployment.ProjectReleaseId into projectDeploymentJoin
            //             from joinedDeployment in projectDeploymentJoin

            //             join a in _aclRepository.GetAll() on joinedDeployment.Id equals a.EntityID

            //             join ut in _userOrganizationUnitRepository.GetAll() on a.OrganizationUnitId equals ut.OrganizationUnitId
            //             where ut.UserId == AbpSession.UserId  || ut.UserId == AbpSession.UserId
            //             select new { Release = projectRelease, ProjectName = joinedProject.Name, Name = joinedProject.Name };

            var orgacls = (from a in _aclRepository.GetAll()
                          join ut in _userOrganizationUnitRepository.GetAll() on a.OrganizationUnitId equals ut.OrganizationUnitId
                          where ut.UserId == AbpSession.UserId
                          select   a.EntityID  ) ;

            var acls = (from a in _aclRepository.GetAll()
                          where a.UserId == AbpSession.UserId
                          select  a.EntityID  ) ;

            var acllist =  acls.Concat(orgacls).ToList();

            var filteredReleases = from projectRelease in _projectReleaseRepository.GetAll().Include(r => r.ProjectEnvironmentFk)
                                   join project in _projectRepository.GetAll() on projectRelease.ProjectId equals project.Id into projectJoin
                                   from joinedProject in projectJoin
                                   join projectDeployment in _projectDeploymentRepository.GetAll() on projectRelease.Id equals projectDeployment.ProjectReleaseId into projectDeploymentJoin
                                   from joinedDeployment in projectDeploymentJoin
                                   where joinedDeployment.TenantId == AbpSession.TenantId &&
                                   (string.IsNullOrEmpty(input.Filter) || joinedProject.Name.ToLower().Contains(input.Filter)) &&
                                   (string.IsNullOrEmpty(input.NameFilter) || joinedProject.Name.ToLower().Contains(input.NameFilter)) &&
                                   joinedDeployment.Enabled == true &&
                                   joinedDeployment.ActionType == ProjectDeploymentEnums.ProjectDeploymentActionType.Active &&
                                   (tags2.Count == 0 ? true : tagentities.Contains(joinedDeployment.Id)) &&                                   
                                   acllist.Contains(joinedDeployment.Id)

                                   //join a in _aclRepository.GetAll() on joinedDeployment.Id equals a.EntityID
                                   //join ut in _userOrganizationUnitRepository.GetAll() on a.OrganizationUnitId equals ut.OrganizationUnitId
                                   //where ut.UserId == AbpSession.UserId

                                   //join a2 in _aclRepository.GetAll() on joinedDeployment.Id equals a2.EntityID
                                   //where a2.UserId == AbpSession.UserId

                                   select new { Release = projectRelease, ProjectName = joinedProject.Name, Name  = joinedProject.Name, Description = joinedProject.Description};
 

            var pagedAndFilteredReleases = filteredReleases
                .OrderBy("ProjectName asc");

            var projects = from o in pagedAndFilteredReleases
                           select new GetProjectForViewDto()
                           {
                               Project = new ProjectDto
                               {
                                    Id = o.Release.Id,
                                    Name = o.ProjectName,
                                    Description = o.Description,
                                    ReleaseId = o.Release.Id,
                                    ReleaseNotes = o.Release.Notes,
                                    VersionMajor =  o.Release.VersionMajor,
                                    VersionMinor = o.Release.VersionMinor,
                                    VersionRevision = o.Release.VersionRevision,
                                    ProjectEnvironmentId = o.Release.ProjectEnvironmentId,
                                    ProjectEnvironmentName = o.Release.ProjectEnvironmentFk.Name,
                                    ProjectEnvironmentType = o.Release.ProjectEnvironmentFk.EnvironmentType ,
                                    ProjectEnvironmentTypeDescription = Convert.ToString(o.Release.ProjectEnvironmentFk.EnvironmentType),
                                    ReleaseDate = o.Release.CreationTime
                               },
                               RecordRecordName = "Need to Get Name from GetFromUser method => "
                           };

            var totalCount = await pagedAndFilteredReleases.CountAsync();

            return new PagedResultDto<GetProjectForViewDto>(
                totalCount,
                await projects.ToListAsync()
            );
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
                            theseforms.ToList().ForEach(tf => {

                                var form = _formRepository.GetAll().Include(f => f.Folder).FirstOrDefault(f => f.Id == tf.Id);
                                if (form != null && !forms.Any(f => f.Id == tf.Id))
                                {
                                    forms.Add(form);

                                    var formrules = _formRuleRepository.GetAll().Where(fr => fr.FormId == form.Id);
                                    formrules.ToList().ForEach(r => {
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
                                                elements.ToList().ForEach(e => {
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
                                                _templateRepository.GetAll().Include(t => t.Folder).Where(t => t.Id == docGuid || t.OriginalId == docGuid).ToList().ForEach(t => {
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

                    string jsonProjectExport = Newtonsoft.Json.JsonConvert.SerializeObject(projectexport, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});

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
        
        public  GetRecordMatterItemForDownload GetDocumentForDownload(Guid input, int version, string format)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "View",
                    EntityId = input,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId
                });

                if (ACLResult.IsAuthed)
                {

                    var recordmatteritem = _recordMatterItemRepository.GetAll().Where(i => i.Id == input).FirstOrDefault();
                    Byte[] bydoc = recordmatteritem.Document;
                    var AllowedFormats = recordmatteritem.AllowedFormats;
                    RecordMatterItemForDownloadType type = RecordMatterItemForDownloadType.PDF;

                    if (bydoc != null)
                    {
                        bool draft = false;

                        // Get the status of the step for draft watermark
                        //Set the Step Status
                        // Check to see if all contributions are complete for step
                        // Inprogress = any contributor not competed
                        var recordMatter = _recordMatterRepository.FirstOrDefault(e => e.Id == recordmatteritem.RecordMatterId);                       
                        draft = recordMatter.Status == RecordMatterConsts.RecordMatterStatus.Final ? false : true;                       

                        if (AllowedFormats.Contains("W") && (format == "docx" || format == "word" || format == "doc"))
                        {
                            bydoc = AsposeUtility.BytesToWord(bydoc, draft);
                            type = RecordMatterItemForDownloadType.Word;
                        }
                        else if (AllowedFormats.Contains("P") && format == "pdf")
                        {
                            bydoc = AsposeUtility.BytesToPdf(bydoc, draft);
                            type = RecordMatterItemForDownloadType.PDF;
                        }
                        else if (AllowedFormats.Contains("H") && format == "html")
                        {
                            bydoc = AsposeUtility.BytesToHTML(bydoc);
                            type = RecordMatterItemForDownloadType.HTML;
                        }
                        else
                        {
                            type = RecordMatterItemForDownloadType.Disallow;
                        }
                    }

                    var filename = Path.GetFileNameWithoutExtension(recordmatteritem.DocumentName);
                    var output = new GetRecordMatterItemForDownload
                    {
                        RecordMatterItem = new RecordMatterItemForDownloadDto
                        {
                            Document = type != RecordMatterItemForDownloadType.Disallow ? bydoc : null,
                            DocumentName = type != RecordMatterItemForDownloadType.Disallow ? filename + "." + format : null,
                            Type = type
                        }
                    };
                    return output;

                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public MemoryStream ExportProjectDocument(Guid id)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete, AbpDataFilters.MayHaveTenant);

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed || true)
            {
                ProjectExport projectexport = new ProjectExport();
                var project = _projectRepository.GetAll().Include(p => p.RecordFk).ThenInclude(pr => pr.RecordMatters).FirstOrDefault(p => p.Id == id);

                if (project != null){
                    try{
                        var flag = false;
                        MemoryStream memoryStream = new MemoryStream();
                        using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)){

                            foreach (RecordMatter rm in project.RecordFk.RecordMatters){
                                var rmis = _recordMatterItemRepository.GetAll().Where(r => r.RecordMatterId == rm.Id).OrderByDescending(r => r.CreationTime).ToList();
                                foreach (RecordMatterItem rmi in rmis){

                                    if (rmi != null && !string.IsNullOrEmpty(rmi.DocumentName) && rmi.AllowedFormats != null){
                                        flag = true;

                                        if (rmi.AllowedFormats.Contains("W")){
                                            var rmid = GetDocumentForDownload(rmi.Id, 1, "docx");

                                            if (rmid!=null){
                                                if (rmi.Document != null){
                                                    ZipArchiveEntry readmeEntry = zip.CreateEntry(rmid.RecordMatterItem.DocumentName);
                                                    using (var zipStream = readmeEntry.Open())
                                                    {
                                                        zipStream.Write(rmid.RecordMatterItem.Document, 0, rmid.RecordMatterItem.Document.Length);
                                                    }
                                                }
                                            }                                           
                                        }

                                        if (rmi.AllowedFormats.Contains("P")){
                                            var rmid = GetDocumentForDownload(rmi.Id, 1, "pdf");
                                            if (rmid != null){
                                                if (rmi.Document != null){
                                                    ZipArchiveEntry readmeEntry = zip.CreateEntry(rmid.RecordMatterItem.DocumentName);
                                                    using (var zipStream = readmeEntry.Open())
                                                    {
                                                        zipStream.Write(rmid.RecordMatterItem.Document, 0, rmid.RecordMatterItem.Document.Length);
                                                    }
                                                }
                                            }                                            
                                        }
                                    }
                                }

                                //write file in the zip
                                var GroupId = rmis.Count > 0 ? rmis.FirstOrDefault().GroupId.ToString() : null;
                                if (rm.HasFiles == true)
                                {
                                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnection.Value.ConnectionString);
                                    string cloudStorageAccountTable = _storageConnection.Value.BlobStorageContainer;
                                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                                    CloudBlobContainer container = blobClient.GetContainerReference(cloudStorageAccountTable);
                                    IEnumerable<IListBlobItem> blobs = container.ListBlobs("file-uploads" + "/" + rm.RecordId + "/" + rm.Id + "/" + GroupId + "/", false);

                                    List<string> blobNames = blobs.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();

                                    blobNames.ForEach(i =>
                                    {
                                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(i.ToString());
                                        if (blockBlob.Exists())
                                        {
                                            MemoryStream memStream = new MemoryStream();
                                            blockBlob.DownloadToStream(memStream);
                                            string filename = i.Substring(124, i.Length - 124);
                                            ZipArchiveEntry zipItem = zip.CreateEntry(filename);
                                            using (MemoryStream originalFileMemoryStream = new MemoryStream(memStream.ToArray()))
                                            {
                                                originalFileMemoryStream.Position = 0;
                                                using (Stream entryStream = zipItem.Open())
                                                {
                                                    originalFileMemoryStream.CopyTo(entryStream);
                                                }
                                            }


                                        }
                                    });

                                }
                                //-------------finish download upload file by record matter id

                            }

                        }
                        return memoryStream;

                    }
                    catch (Exception e)
                    {

                    }
                }

            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

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
                                formrules.ToList().ForEach(r => {
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
                                            elements.ToList().ForEach(e => {
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
                                            _templateRepository.GetAll().Include(t => t.Folder).Where(t => t.Id == docGuid || t.OriginalId == docGuid).ToList().ForEach(t => {
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


        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Create)]
        public async Task ImportProject(ImportProjectInput input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete);

            //{
            //  "Project": { },
            //  "Forms": []
            //  "FormRules": [],
            //  "AppJobs": [],
            //  "Templates": []
            //}

            ProjectExport projectexport = JsonConvert.DeserializeObject<ProjectExport>(input.Project);
            //ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = projectexport.Project.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            //if (ACLResult.IsAuthed || true)
            //{

                int? tenantId = null;
                if (AbpSession.TenantId != null)
                {
                    tenantId = (int?)AbpSession.TenantId;
                }

                // START RECORD
                Record RecordIn = projectexport.Project.RecordFk;
                RecordIn.Folder = null;
                RecordIn.FolderId = null;
                RecordIn.TenantId = tenantId;
                RecordIn.UserId = AbpSession.UserId;
                RecordIn.User = null;
                RecordIn.CreatorUserId = AbpSession.UserId;
                RecordIn.LastModifierUserId = AbpSession.UserId;
                RecordIn.LastModificationTime = DateTime.Now;

                if (!_customrecordRepository.GetAll().Any(r => r.Id == projectexport.Project.RecordId))
                {
                    await _customrecordRepository.InsertAsync(RecordIn);
                    await _ACLManager.AddACL(new ACL()
                    {
                        CreatorUserId = AbpSession.UserId,
                        TenantId = AbpSession.TenantId,
                        UserId = AbpSession.UserId,
                        OrganizationUnitId = null,
                        Type = "Record",
                        EntityID = RecordIn.Id,
                        Role = "O"
                    });

                }
                else
                {
                    await _customrecordRepository.UpdateAsync(RecordIn);
                }

                //START RECORDMATTERS
                foreach (RecordMatter recordmatterIn in RecordIn.RecordMatters)
                {

                    //ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = recordmatterIn.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
                    //if (ACLResult.IsAuthed || true)
                    //{
                        recordmatterIn.TenantId = tenantId;
                        recordmatterIn.CreatorUserId = AbpSession.UserId;
                        recordmatterIn.UserId = AbpSession.UserId;
                        recordmatterIn.LastModifierUserId = AbpSession.UserId;
                        recordmatterIn.LastModificationTime = DateTime.Now;

                        if (recordmatterIn.DeleterUserId != null)
                        {
                            recordmatterIn.DeleterUserId = AbpSession.UserId;
                            recordmatterIn.IsDeleted = true;
                            recordmatterIn.DeletionTime = DateTime.Now;
                        }
 
                        if (!_recordMatterRepository.GetAll().Any(r => r.Id == recordmatterIn.Id))
                        {
                            await _recordMatterRepository.InsertAsync(recordmatterIn);
                            await _ACLManager.AddACL(new ACL()
                            {
                                CreatorUserId = AbpSession.UserId,
                                TenantId = AbpSession.TenantId,
                                UserId = AbpSession.UserId,
                                OrganizationUnitId = null,
                                Type = "RecordMatter",
                                EntityID = recordmatterIn.Id,
                                Role = "O"
                            });
                        }
                        else
                        {
                            await _recordMatterRepository.UpdateAsync(recordmatterIn);
                        }
                    //}
                    //else
                    //{
                    //    throw new UserFriendlyException("Not Authorised");
                    //}
                }

                CurrentUnitOfWork.SaveChanges();

                //END RECORDMATTERS
                //END RECORD

                // START PROJECT
                Project projectIn = projectexport.Project;
                projectIn.TenantId = tenantId;
                projectIn.CreatorUserId = AbpSession.UserId;
                projectIn.LastModifierUserId = AbpSession.UserId;
                projectIn.LastModificationTime = DateTime.Now;

                if (!_projectRepository.GetAll().Any(p => p.Id == projectIn.Id))
                {
                    await _projectRepository.InsertAsync(projectIn);
                    await _ACLManager.AddACL(new ACL()
                    {
                        CreatorUserId = AbpSession.UserId,
                        TenantId = AbpSession.TenantId,
                        UserId = AbpSession.UserId,
                        OrganizationUnitId = null,
                        Type = "Project",
                        EntityID = projectIn.Id,
                        Role = "O"
                    });
                }
                else
                {
                    projectIn.CreationTime = DateTime.Now;
                    await _projectRepository.UpdateAsync(projectIn);
                }

                CurrentUnitOfWork.SaveChanges();

                // START FORMS
                foreach (Form formIn in projectexport.Forms)
                {
                    //Add Folder if required
                    //ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = formIn.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
                    //if (ACLResult.IsAuthed || true)
                    //{
                        //var folder = _folderRepository.GetAll().FirstOrDefault(f => f.Id == formIn.FolderId);
                        if (!_folderRepository.GetAll().Any(f => f.Id == formIn.FolderId))
                        {
                            Guid rootFolderId = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, "F");
                            formIn.Folder.ParentId = rootFolderId;
                            formIn.Folder.Name = formIn.Folder.Name == "Your Forms" ? projectexport.Project.Name : formIn.Folder.Name;
                            formIn.Folder.TenantId = tenantId;
                            formIn.Folder.ParentFolder = null;
                            formIn.Folder.CreationTime = DateTime.Now;
                            // Need the Root Folder

                            if (formIn.DeleterUserId != null)
                            {
                                formIn.DeleterUserId = AbpSession.UserId;
                                formIn.IsDeleted = true;
                                formIn.DeletionTime = DateTime.Now;

                                formIn.Folder.DeleterUserId = AbpSession.UserId;
                                formIn.Folder.IsDeleted = true;
                                formIn.Folder.DeletionTime = DateTime.Now;

                            }

                            var folder = _folderRepository.GetAll().FirstOrDefault(f => f.Id == formIn.FolderId);

                            await _folderRepository.InsertAsync(formIn.Folder);

                            await _ACLManager.AddACL(new ACL()
                            {
                                CreatorUserId = AbpSession.UserId,
                                TenantId = AbpSession.TenantId,
                                UserId = AbpSession.UserId,
                                OrganizationUnitId = null,
                                Type = "Folder",
                                EntityID = formIn.Folder.Id,
                                Role = "O"
                            });
                        }

                        formIn.Folder = null;
                        formIn.TenantId = tenantId;
                        formIn.CreatorUserId = AbpSession.UserId;
                        formIn.LastModifierUserId = AbpSession.UserId;
                        formIn.LastModificationTime = DateTime.Now;

                        if (!_formRepository.GetAll().Any(f => f.Id == formIn.Id))
                        {
                            // Todo Form FolderId
                            formIn.CreationTime = DateTime.Now;
                            await _formRepository.InsertAsync(formIn);

                            await _ACLManager.AddACL(new ACL()
                            {
                                CreatorUserId = AbpSession.UserId,
                                TenantId = AbpSession.TenantId,
                                UserId = AbpSession.UserId,
                                OrganizationUnitId = null,
                                Type = "Form",
                                EntityID = formIn.Id,
                                Role = "O"
                            });

                        }
                        else
                        {
                            await _formRepository.UpdateAsync(formIn);
                        }


                        CurrentUnitOfWork.SaveChanges();

                        //// Add Original Item if not exists
                        //if (!_formRepository.GetAll().Any(f => f.Id == formIn.OriginalId))
                        //{
                        //    // Todo Form FolderId
                        //    formIn.Id = formIn.OriginalId;
                        //    formIn.Version = 1;
                        //    formIn.CreationTime = DateTime.Now;
                        //    await _formRepository.InsertAsync(formIn);

                        //    await _ACLManager.AddACL(new ACL()
                        //    {
                        //        CreatorUserId = AbpSession.UserId,
                        //        TenantId = AbpSession.TenantId,
                        //        UserId = AbpSession.UserId,
                        //        OrganizationUnitId = null,
                        //        Type = "Form",
                        //        EntityID = formIn.Id,
                        //        Role = "O"
                        //    });

                        //}


                        CurrentUnitOfWork.SaveChanges();

                    //}
                    //else
                    //{
                    //    throw new UserFriendlyException("Not Authorised");
                    //}

                    CurrentUnitOfWork.SaveChanges();
                }
                // END FORMS

                //START FormRules
                foreach (FormRule formRuleIn in projectexport.FormRules)
                {

                    //ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = formRuleIn.FormId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
                    //if (ACLResult.IsAuthed || true)
                    //{
                        formRuleIn.TenantId = tenantId;
                        formRuleIn.CreatorUserId = AbpSession.UserId;
                        formRuleIn.LastModifierUserId = AbpSession.UserId;
                        formRuleIn.LastModificationTime = DateTime.Now;
                        formRuleIn.Form = null;
                        if (!_formRuleRepository.GetAll().Any(f => f.Id == formRuleIn.Id))
                        {
                            // Todo Form FolderId
                            formRuleIn.CreationTime = DateTime.Now;
                            await _formRuleRepository.InsertAsync(formRuleIn);

                        }
                        else
                        {
                            await _formRuleRepository.UpdateAsync(formRuleIn);
                        }
                    //}
                    //else
                    //{
                    //    throw new UserFriendlyException("Not Authorised");
                    //}
                    CurrentUnitOfWork.SaveChanges();
                }
                //END FormRules

                //START AppRules
                foreach (AppJob appJobIn in projectexport.AppJobs)
                {
                    //ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = (Guid)appJobIn.EntityId, UserId = AbpSession.UserId });
                    //if (ACLResult.IsAuthed || true)
                    //{
                        appJobIn.TenantId = tenantId;

                        CreateOrEditAppJobDto appJobDto = JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(appJobIn.Data);
                        if (appJobDto.RecordMatter != null)
                        {
                            foreach (CreateOrEditAppJobRecordMatterDto recordmatter in appJobDto.RecordMatter)
                            {
                                recordmatter.Assignees = new List<GrantACLDto>();
                                recordmatter.TeamIds = null;
                                recordmatter.Teams = new List<CreateOrEditAppJobTeamDto>();
                                recordmatter.Users = new List<CreateOrEditAppJobUserDto>();
                                recordmatter.UserIds = null;
                            }

                            string data = Newtonsoft.Json.JsonConvert.SerializeObject(appJobDto);
                            appJobIn.Data = data;
                        }

                        if (!_appJobRepository.GetAll().Any(f => f.Id == appJobIn.Id))
                        {
                            // Todo Form FolderId
                            await _appJobRepository.InsertAsync(appJobIn);
                        }
                        else
                        {
                            await _appJobRepository.UpdateAsync(appJobIn);
                        }
                    //}
                    //else
                    //{
                    //    throw new UserFriendlyException("Not Authorised");
                    //}
                    CurrentUnitOfWork.SaveChanges();
                }
                //END AppRules

                //START Templates
                foreach (Template templateIn in projectexport.Templates)
                {
                    //ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = templateIn.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
                    //if (ACLResult.IsAuthed || true)
                    //{
                        //Add Folder if required
                        var folder = _folderRepository.GetAll().FirstOrDefault(f => f.Id == templateIn.FolderId);
                        if (folder == null)
                        {
                            Guid rootFolderId = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, "T");
                            templateIn.Folder.ParentId = rootFolderId;
                            templateIn.Folder.TenantId = tenantId;
                            templateIn.Folder.Name = templateIn.Folder.Name == "Your Templates" ? projectexport.Project.Name : templateIn.Folder.Name;
                            templateIn.Folder.CreationTime = DateTime.Now;
                            // Need the Root Folder
                            await _folderRepository.InsertAsync(templateIn.Folder);

                            await _ACLManager.AddACL(new ACL()
                            {
                                CreatorUserId = AbpSession.UserId,
                                TenantId = AbpSession.TenantId,
                                UserId = AbpSession.UserId,
                                OrganizationUnitId = null,
                                Type = "Folder",
                                EntityID = templateIn.Folder.Id,
                                Role = "O"
                            });
                        }

                        templateIn.TenantId = tenantId;
                        templateIn.Folder = null;
                        templateIn.CreatorUserId = AbpSession.UserId;

                        if (!_templateRepository.GetAll().Any(f => f.Id == templateIn.Id))
                        {
                            // Todo Form FolderId
                            templateIn.CreationTime = DateTime.Now;
                            await _templateRepository.InsertAsync(templateIn);
                            await _ACLManager.AddACL(new ACL()
                            {
                                CreatorUserId = AbpSession.UserId,
                                TenantId = AbpSession.TenantId,
                                UserId = AbpSession.UserId,
                                OrganizationUnitId = null,
                                Type = "DocumentTemplate",
                                EntityID = templateIn.Id,
                                Role = "O"
                            });
                        }
                        else
                        {
                            await _templateRepository.UpdateAsync(templateIn);
                        }

                        CurrentUnitOfWork.SaveChanges();

                    //}
                    //else
                    //{
                    //    throw new UserFriendlyException("Not Authorised");
                    //}
                }
                //END Templates
            //}
            //else
            //{
            //    throw new UserFriendlyException("Not Authorised");
            //}
        }

        public  List<FilesDto> GetFiles(Guid RecordId, Guid RecordMatterId,string RecordMatterItemGroupId)
        {

            List<ACL> acls = _ACLManager.FetchAllUserACLs(new GetAllACLsInput() { UserId = (long)AbpSession.UserId });
            if (acls.Exists(i => i.EntityID == RecordId || i.EntityID == RecordMatterId))
            {

                List<FilesDto> Files = new List<FilesDto>();
                MemoryStream memoryStream = new MemoryStream();

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnection.Value.ConnectionString);
                string cloudStorageAccountTable = _storageConnection.Value.BlobStorageContainer;


                // If Submission ID is supplied then get he GroupID
               // string RecordMatterItemGroupId = RecordMatterItemGroupId;
                
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(cloudStorageAccountTable);
                IEnumerable<IListBlobItem> blobs = container.ListBlobs("file-uploads" + "/" + RecordId + "/" + RecordMatterId + "/" + RecordMatterItemGroupId + "/", false);

                List<string> blobNames = blobs.OfType<CloudBlockBlob>().Select(b => b.Name).Distinct().ToList();

                blobNames.ForEach(i =>
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(i.ToString());
                    blockBlob.FetchAttributes();
                    var blobLength = blockBlob.Properties.Length;

                    var fname = Path.GetFileName(i);

                    var Result = new FilesDto()
                    {
                            FileName = fname,
                            Size = "0",
                            RecordId = RecordId.ToString(),
                            RecordMatterId = RecordMatterId.ToString(),
                            RecordMatterItemGroupId = RecordMatterItemGroupId.ToString()
                        
                    };
                    Files.Add(Result);
                });

                return Files;

            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<GetProjectForViewDto> GetProjectForView(Guid id)
         {
            //await _projectRepository.InsertAsync(newproject);
            //await _ACLManager.AddACL(new ACL()
            //{
            //    UserId = AbpSession.UserId,
            //    Type = "Project",
            //    EntityID = newproject.Id,
            //    Role = "O"
            //});

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId});
            if (ACLResult.IsAuthed)
            {
                
                var project = await _projectRepository.GetAsync(id);
                var output = new GetProjectForViewDto { Project = ObjectMapper.Map<ProjectDto>(project) };
                // Used to record contributor useraccpetances 
                //output.Project.ContributorsAcceptances = new Dictionary<Guid?, string>();

                if (project.CreatorUserId != null)
                {
                    var CreatorUser = await UserManager.GetUserByIdAsync((long)project.CreatorUserId);
                    output.Project.CreatorUserName = CreatorUser == null ? string.Empty : CreatorUser.UserName;
                }

                if (project.LastModifierUserId != null)
                {
                    var LastModifiedUser = await UserManager.GetUserByIdAsync((long)project.LastModifierUserId);
                    output.Project.LastModifiedUserName = LastModifiedUser == null ? string.Empty : LastModifiedUser.UserName;
                }

                if (output.Project.RecordId != null)
                {

                     // 
                     var record = _customrecordRepository.GetAllWithRecords().FirstOrDefault(e => e.Id == output.Project.RecordId);

                    // DTO MAPPING DOES NOT WORK TODO
                    //var r = _lookup_recordRepository.GetAll()
                    //    .Include(r => r.RecordMatters)
                    //    .ThenInclude(rm => rm.RecordMatterItems)
                    //    .FirstOrDefault(e => e.Id == output.Project.RecordId);

                    //var record = ObjectMapper.Map<RecordDto>(r);

                    //var r = _lookup_recordRepository.GetAll()
                    //    .Include(r => r.RecordMatters)
                    //    .ThenInclude(rm => rm.RecordMatterItems)
                    //    .FirstOrDefault(e => e.Id == output.Project.RecordId);

                    //var record = ObjectMapper.Map<RecordDto>(r);

                    if (record.RecordMatters != null)
                    {
                        record.RecordMatters = record.RecordMatters.OrderBy(e => e.Order).ToList();
                        Records.Dtos.RecordDto recorddto = ObjectMapper.Map<Records.Dtos.RecordDto>(record) ;

                        // Remove Steps where validation rules fail
                        List<Guid> invalidsteps = new List<Guid>();
                        foreach (RecordMatterDto recordmatter in recorddto.RecordMatters) {
                            if (!ValidateRecordMatterStep(recordmatter))
                            {
                                invalidsteps.Add(recordmatter.Id);
                            }                    
                        }

                        foreach (Guid invalidstep in invalidsteps)
                        {
                            recorddto.RecordMatters.Remove(recorddto.RecordMatters.FirstOrDefault(e => e.Id == invalidstep));
                        }

                        // Set the ProjectStep Statuses
                        recorddto.RecordMatters.ForEach(rm => {

                        //Set the Step Status
                            // Check to see if all contributions are complete for step
                            // Inprogress = any contributor not competed
                            var contributors = _recordMatterContributorRepository.GetAll().Where(e => e.RecordMatterId == rm.Id);

                            var reviewcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Canceled).Count();
                            var reviewcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Approved).Count();
                            var reviewnew = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Awaiting).Count();
                            var endorserejected = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Rejected).Count();
                            var reviewtotal = reviewcanceled + reviewcomplete + reviewnew + endorserejected;

                            var approvedcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Canceled).Count();
                            var approvedcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Approved).Count();
                            var approvednew = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Awaiting).Count();
                            var approvedrejected = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Rejected).Count();
                            var approvedtotal = approvedcanceled + approvedcomplete + approvednew + approvedrejected;

                            RecordMatterConsts.RecordMatterStatus recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Draft;

                            var recordmatteritemaudits =
                                (from a in _recordMatterAuditRepository.GetAll().Include(r =>r.UserFk).OrderByDescending(e => e.CreationTime)
                                 where a.RecordMatterId == rm.Id
                                 select new RecordMatterAuditDto ()
                                     {
                                        CreationTime = AbpSession.TenantId == null? a.CreationTime : (DateTime)_timeZoneConverter.Convert(a.CreationTime, (int)AbpSession.TenantId, (long)AbpSession.UserId),
                                        Id = a.Id,  
                                        RecordMatterId = a.RecordMatterId,
                                        Status = a.Status,
                                        UserId = a.UserId,
                                        UserName = a.UserFk != null ? a.UserFk.UserName : null
                                 }
                                 ).ToList();
 
                            rm.RecordMatterAudits = recordmatteritemaudits;


                        });

                        output.Project.Record = recorddto;

                        //get uploaded files
                        output.Uploadfiles = new List<FilesDto>();
                        foreach (RecordMatterDto recordmatter in recorddto.RecordMatters)
                        {

                            recordmatter.FormId = recordmatter.FormId == null ? recordmatter.RecordMatterItems.FirstOrDefault() == null ? recordmatter.FormId : recordmatter.RecordMatterItems.FirstOrDefault().FormId : recordmatter.FormId;
                            //FormId = rm.FormId == null ? rm.RecordMatterItems.FirstOrDefault() == null ? rm.FormId : rm.RecordMatterItems.FirstOrDefault().FormId : rm.FormId,

                            if (recordmatter.HasFiles) {
                                if (recordmatter.RecordMatterItems.Count > 0)
                                {
                                    var fs = GetFiles(record.Id, recordmatter.Id, recordmatter.RecordMatterItems.FirstOrDefault().GroupId.ToString());
                                    output.Uploadfiles.AddRange(fs);
                                }
                                else {
                                    var fs = GetFiles(record.Id, recordmatter.Id,null);
                                    output.Uploadfiles.AddRange(fs);
                                }
                                
                            }
                        }

                        var flag = false;
                        //get submissionId
                        foreach (RecordMatterDto recordmatter in recorddto.RecordMatters)
                        {
                            if (recordmatter.RecordMatterItems.Count > 0)
                            {
                                foreach (RecordMatterItemDto recordmatteritem in recordmatter.RecordMatterItems)
                                {
                                    Guid? submissionId = _recordMatterItemRepository.FirstOrDefault(recordmatteritem.Id)?.SubmissionId;
                                    if (submissionId != null)
                                    {
                                        recordmatteritem.SubmissionStatus = _submissionRepository.FirstOrDefault((Guid)submissionId)?.SubmissionStatus;
                                        if(recordmatteritem.SubmissionStatus== "Assembling")
                                            flag = true;
                                     //   output.submissionId = submissionId;
                                    }
                                }

                            }
                        }
                        //end of get submission Id

                        if(flag == false)
                        {
                            //recordmatteritem is the lastest update according to the recordmatter
                            var newsubmission = _submissionRepository.GetAll().Where(x => x.SubmissionStatus == "Assembling"&& x.RecordId == recorddto.Id).OrderBy("LastModificationTime desc").FirstOrDefault();
                          
                            if (newsubmission != null)
                            {
                                TimeSpan span = (TimeSpan)(Clock.Now - newsubmission.LastModificationTime);
                                if (span.TotalMinutes < 2)
                                {
                                    var rm = recorddto.RecordMatters.FirstOrDefault(x => x.Id == newsubmission.RecordMatterId);
                                    if(rm != null)
                                    {
                                        var rmi = rm.RecordMatterItems.FirstOrDefault();
                                        if (rmi != null){
                                            rmi.SubmissionStatus = "Assembling";
                                            rmi.SubmissionId = newsubmission.Id;
                                        }
                                    }
                                }
                            }
                        }

                    }

                    var _lookupRecord = _lookup_recordRepository.FirstOrDefault((Guid)output.Project.RecordId);
                    output.RecordRecordName = project.Name; // _lookupRecord?.RecordName?.ToString();

                    // Get all the RecordMatterIds
                    var recordmatterids = (
                        from o in record.RecordMatters
                        select o.Id
                    ).ToList();

                    output.Project.Contributors = (

                        from o in _recordMatterContributorRepository.GetAll().OrderByDescending(o => o.CreationTime)
                        where recordmatterids.Contains((Guid)o.RecordMatterId)
                        select new RecordMatterContributorDto()
                        {
                            AccessToken = o.AccessToken,
                            Comments = o.Comments,
                            Complete = o.Complete,   
                            Email = o.Email,
                            EmailBCC = o.EmailBCC,    
                            EmailCC = o.EmailCC, 
                            EmailFrom = o.EmailFrom,    
                            Enabled = o.Enabled,   
                            FormId = o.FormId,    
                            Id = o.Id,
                            Message = o.Message,    
                            Name = o.Name, 
                            OrganizationName = o.OrganizationName,
                            RecordMatterId = o.RecordMatterId,
                            Status = o.Status,
                            StepAction = o.StepAction,
                            StepRole = o.StepRole,
                            StepStatus = o.StepStatus,
                            Subject = o.Subject,
                            Time = o.Time,
                            UserId = o.UserId 
                        }
                        //select ObjectMapper.Map<RecordMatterContributorDto>(o)
                    ).ToList();


                    // Get the Release
                    var release = _lookup_projectReleaseRepository.GetAll().FirstOrDefault(r => r.Id == output.Project.ReleaseId);

                    if (release != null)
                    {
                        var tenantname = "Host";
                        if (release.TenantId.HasValue)
                            tenantname = Convert.ToString( await TenantManager.GetTenantName((int)release.TenantId));

                        output.Release = new ProjectReleaseDto()
                        {
                            CreationTime = release.CreationTime,
                            Id = release.Id,
                            Name = release.Name,
                            Notes = release.Notes,
                            ProjectEnvironmentId = release.ProjectEnvironmentId,
                            ReleaseType = release.ReleaseType,
                            ProjectId = release.ProjectId,
                            Required = release.Required,
                            VersionMajor = release.VersionMajor,
                            VersionMinor = release.VersionMinor,
                            VersionRevision = release.VersionRevision,   
                            TenantName = tenantname
                        };


                        // This causes DataReader Issue ///////////////////////////////////
                        var releases = _lookup_projectReleaseRepository.GetAll()
                            .Where(r => r.ProjectId == release.ProjectId && r.ProjectEnvironmentId == release.ProjectEnvironmentId)
                            .Select(r => r.Id).ToList();

                        var deployments = (from o in _projectDeploymentRepository.GetAll().Include(i => i.ProjectReleaseFk)
                                           //.Where(d => d.ActionType == ProjectDeploymentEnums.ProjectDeploymentActionType.Active)
                                           .Where(d => releases.Contains((Guid)d.ProjectReleaseId))
                                           select new ProjectDeploymentDto()
                                           {
                                               ActionType = o.ActionType,
                                               Id = o.Id,
                                               Comments = o.Comments,
                                               ProjectReleaseId = o.ProjectReleaseId,
                                               CreationTime = o.CreationTime,
                                               ProjectRelease = new ProjectReleaseDto()
                                               {
                                                    Id = o.ProjectReleaseFk.Id,
                                                    Name = o.ProjectReleaseFk.Name,
                                                    Notes  = o.ProjectReleaseFk.Notes,
                                                    Required = o.ProjectReleaseFk.Required,
                                                   CreationTime = o.ProjectReleaseFk.CreationTime
                                               }
                                           }); ;
                        output.Deployments = deployments.OrderBy(d => d.CreationTime).ToList();
                    }

                }

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

 
         }
 

        private Boolean ValidateRecordMatterStep(RecordMatterDto recordmatter)
        {
            // EDIT
            // NOW USing XPATH

            // EXAMPLE
            // string schemaJson = @"{
            //  'properties': {
            //    'approved': {'type': 'boolean'},
            //    'name': {'type': 'boolean'},
            //    'NumberTest': {
            //          'type': 'number',
            //          'minimum': 25,
            //          'maximum': 100
            //    }
            //  }
            //}";

            //schemaJson = @"{
            //  'properties': {
            //    'PaymentAmount': {
            //      'type': 'string'
            //    }
            //  },
            //  'if': {
            //    'properties': {
            //      'MyUserDataUserName': {
            //        'const': 'admin'
            //      }
            //    }
            //  },
            //  'then': {
            //    'properties': {
            //      'MyUserDataName': {
            //        'const': 'Bruce Fallen'
            //      }
            //    }
            //  }
            //}";

            var result = true;
            var rm = _recordMatterRepository.FirstOrDefault(r => r.Id == recordmatter.Id);

            if (!string.IsNullOrEmpty(rm.Filter))
            {
                var data = _lookup_recordRepository.FirstOrDefault(r => r.Id == rm.RecordId).Data;
                if (string.IsNullOrEmpty(data))
                {
                    data = "{}";
                }

                try
                {
                    //string schemaJson = rm.RulesSchema;
                    //JObject submission = JsonConvert.DeserializeObject<JObject>(rm.Data);

                    //if (!string.IsNullOrEmpty(schemaJson))
                    //{
                    //    try
                    //    {
                    //        JSchema rulesSchema = JSchema.Parse(schemaJson);
                    //        result = submission.IsValid(rulesSchema);
                    //    }
                    //    catch
                    //    {
                    //        // If Error reject test
                    //        result = false;
                    //    }
                    //}


                    XmlNode node = null;
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(JsonConvert.DeserializeXNode(data, "Data").ToString());
                        node = doc.DocumentElement.SelectSingleNode(rm.Filter);

                        if (node == null) result = false;

                    }
                    catch (Exception) { result = false; }


                }
                catch
                {
                    result = true;
                }
            }



            return result;

        }

        //[UnitOfWork]
        //public async Task<String> StartProject(Guid id, string projectname, string description)
        //{

        //    // Get ID's of projects shared with this Tenant
        //    _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

        //   // string result = "/Falcon/forms/load?OriginalId={0}&RecordMatterId={1}&RecordMatterItemId=00000000-0000-0000-0000-000000000000&ProjectId={2}&version=live";
        //    string result = "/Falcon/forms/loadInProject?OriginalId={0}&RecordMatterId={1}&RecordMatterItemId=&ProjectId={2}&version=live";

        //    try
        //    {
        //        var projectids = _ACLManager.GetAllSharedEntities(AbpSession.TenantId, "Project");
        //        var project = _projectRepository.GetAll().FirstOrDefault(p => (p.Id == id && p.TenantId == AbpSession.TenantId) || (p.Id == id && projectids.Contains(id)));

        //        if (project != null)
        //        {

        //            // 1. Copy the Record
        //            // 2. Copy the RecordMatters
        //            // 3. Copy the RecordMatterItems
        //            // 4. Copy the Project record and assign the record
        //            // 5. Set to User not Template

        //            var record = _customrecordRepository.GetAllWithRecords()
        //                .Where(e => e.Id == project.RecordId).First();

        //            if (record != null)
        //            {
        //                var newrecord = new Record()
        //                {
        //                    Comments = record.Comments,
        //                    Data = record.Data,
        //                    FolderId = null, // MUST be null for Projects Guid.NewGuid(), // null for projects?
        //                    RecordName = string.IsNullOrEmpty(projectname) ? record.RecordName : projectname,
        //                    UserId = AbpSession.UserId,
        //                    TenantId = AbpSession.TenantId != null ? (int?)AbpSession.TenantId : null
        //                };

        //                await _recordManager.CreateOrEditRecord(new ACL()
        //                {
        //                    UserId = AbpSession.UserId
        //                }, newrecord);

        //                await _ACLManager.AddACL(new ACL()
        //                {
        //                    UserId = AbpSession.UserId,
        //                    Type = "Record",
        //                    EntityID = newrecord.Id,
        //                    Role = "O"
        //                });

        //                string formid = string.Empty;
        //                string recordmatterid = string.Empty;

        //                foreach (RecordMatter recordmatter in record.RecordMatters.OrderBy(r => r.Order))
        //                {

        //                    // Custom repo does not return the data
        //                    var rm = _recordMatterRepository.FirstOrDefault(r => r.Id == recordmatter.Id);

        //                    var newrecordmatter = new RecordMatter()
        //                    {
        //                        Comments = recordmatter.Comments,
        //                        Data = rm.Data,
        //                        HasFiles = recordmatter.HasFiles,
        //                        IsDeleted = false,
        //                        OrganizationUnitId = recordmatter.OrganizationUnitId,
        //                        RecordId = newrecord.Id,
        //                        RecordMatterName = recordmatter.RecordMatterName,
        //                        Status = RecordMatterConsts.RecordMatterStatus.New,
        //                        UserId = AbpSession.UserId,
        //                        TenantId = AbpSession.TenantId != null ? (int?)AbpSession.TenantId : null,
        //                        FormId = recordmatter.FormId,
        //                        Order = recordmatter.Order,
        //                        RequireReview = recordmatter.RequireReview,
        //                        RequireApproval = recordmatter.RequireApproval,
        //                        Filter = rm.Filter,
        //                    };

        //                    var rmi = await _recordManager.CreateAndOrFetchRecordMatter(new ACL()
        //                    {
        //                        UserId = AbpSession.UserId
        //                    }, newrecordmatter);

        //                    // Create new groupId, Only single groups per Project => Step
        //                    Guid groupid = Guid.NewGuid();

        //                    foreach (RecordMatterItem recordmatteritem in recordmatter.RecordMatterItems)
        //                    {

        //                        var newrecordmatteritem = new RecordMatterItem()
        //                        {
        //                            AllowedFormats = recordmatteritem.AllowedFormats,
        //                            AllowHtmlAssignees = recordmatteritem.AllowHtmlAssignees,
        //                            AllowPdfAssignees = recordmatteritem.AllowPdfAssignees,
        //                            AllowWordAssignees = recordmatteritem.AllowWordAssignees,
        //                            Document = recordmatteritem.Document,
        //                            DocumentName = recordmatteritem.DocumentName,
        //                            ErrorDetails = recordmatteritem.ErrorDetails,
        //                            FormId = recordmatteritem.FormId,
        //                            FormURI = recordmatteritem.FormURI,
        //                            GroupId = groupid,
        //                            HasDocument = recordmatteritem.HasDocument,
        //                            LockOnBuild = recordmatteritem.LockOnBuild,
        //                            RecordMatterId = newrecordmatter.Id,
        //                            IsDeleted = false,
        //                            OrganizationUnitId = recordmatter.OrganizationUnitId,
        //                            UserId = AbpSession.UserId,
        //                            TenantId = AbpSession.TenantId != null ? (int?)AbpSession.TenantId : null
        //                        };

        //                        await _recordManager.CreateAndOrFetchRecordMatterItem(newrecordmatteritem);

        //                    }

        //                    //NEED TO DO: filter first and then open the project, if the first step is hidden, it will return error
        //                    // Used for the Return URL

        //                    //get the first recordmatter
        //                    if (recordmatter == record.RecordMatters.First())
        //                    {
        //                        formid = Convert.ToString(rmi.FormId);
        //                        recordmatterid = Convert.ToString(rmi.Id);
        //                    }
        //                }

        //                Guid newprojectid = Guid.NewGuid();
        //                var newproject = new Project()
        //                {
        //                    //Description = project.Description,
        //                    Description = string.IsNullOrEmpty(description) ? project.Name : description,
        //                    Name = string.IsNullOrEmpty(projectname) ? project.Name : projectname,
        //                    RecordId = newrecord.Id,
        //                    Status = ProjectStatus.New,
        //                    Type = ProjectType.User,
        //                    Id = newprojectid,
        //                    ProjectTemplateId=id,
        //                };

        //                if (AbpSession.TenantId != null)
        //                {
        //                    newproject.TenantId = (int?)AbpSession.TenantId;
        //                }

        //                await _projectRepository.InsertAsync(newproject);
        //                await _ACLManager.AddACL(new ACL()
        //                {
        //                    UserId = AbpSession.UserId,
        //                    Type = "Project",
        //                    EntityID = newproject.Id,
        //                    Role = "O"
        //                });

        //                _unitOfWorkManager.Current.SaveChanges();
        //                result = String.Format(result, formid, recordmatterid, Convert.ToString(newprojectid));
        //                return result;

        //            }
        //            else
        //            {
        //                throw new UserFriendlyException("Permission Denied");
        //            }

        //        }
        //        else
        //        {
        //            throw new UserFriendlyException("Project does not contain Records and is not configured");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //Guid newprojectid = Guid.NewGuid();
        //        return string.Empty;
        //        //Console.WriteLine(e.Message);
        //        // statement(s)
        //    }
        //}

        [UnitOfWork]
        public async Task<String> StartProject(Guid releaseId, Guid projectId, string projectname, string description, Guid? recordId)
        {

            // Get ID's of projects shared with this Tenant
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            string result = "/Falcon/forms/loadRelease?ProjectId={0}&FormId={1}&RecordMatterId={2}";

            try
            {

                GetFormFromReleaseOutput rOutput = _projectManager.GetFormFromRelease(releaseId, null);

                // TODO SHARING ACL CHECKING
                //var projectids = _ACLManager.GetAllSharedEntities(AbpSession.TenantId, "Project");
                //var project = _projectRepository.GetAll().FirstOrDefault(p => (p.Id == id && p.TenantId == AbpSession.TenantId) || (p.Id == id && projectids.Contains(id)));

                if (rOutput.Project != null)
                {
                    // A. Get from Project Release Archive
                    // 1. Copy the Record
                    // 2. Copy the RecordMatters
                    // 3. Copy the RecordMatterItems
                    // 4. Copy the Project record and assign the record
                    // 5. Set to User not Template
        
                    rOutput.Project.ReleaseId = releaseId;
                    var record = rOutput.Project.RecordFk;

                    // If upgrading from old Project then get old record and apply the data
                    // Apply recordmatter data to each step in the project
                    // If there is an extra step the apply the record data
                    string olddata = string.Empty;
                    if (recordId.HasValue)
                    {
                        var oldrecord = _lookup_recordRepository.GetAll().FirstOrDefault(r => r.Id == recordId);
                        if (oldrecord != null)
                            olddata = oldrecord.Data;
                    }

                    if (record != null)
                    {
                        var newrecord = new Record()
                        {
                            Comments = record.Comments,
                            Data = string.IsNullOrEmpty(olddata) ? record.Data : olddata,
                            FolderId = null, // MUST be null for Projects Guid.NewGuid(), // null for projects?
                            RecordName = string.IsNullOrEmpty(projectname) ? record.RecordName : projectname,
                            UserId = AbpSession.UserId,
                            TenantId = AbpSession.TenantId != null ? (int?)AbpSession.TenantId : null
                        };

                        await _recordManager.CreateOrEditRecord(new ACL()
                        {
                            UserId = AbpSession.UserId
                        }, newrecord);

                        await _ACLManager.AddACL(new ACL()
                        {
                            UserId = AbpSession.UserId,
                            Type = "Record",
                            EntityID = newrecord.Id,
                            Role = "O"
                        });

                        string formid = string.Empty;
                        string recordmatterid = string.Empty;

 
                        foreach (RecordMatter recordmatter in record.RecordMatters.OrderBy(r => r.Order))
                        {

                            var data = string.Empty;

                            // Custom repo does not return the data
                            var rm = _recordMatterRepository.FirstOrDefault(r => r.Id == recordmatter.Id);
                            if(rm != null) data = rm.Data;

                            var newrecordmatter = new RecordMatter()
                            {
                                Comments = recordmatter.Comments,
                                Data = string.IsNullOrEmpty(olddata)? data : olddata,
                                HasFiles = recordmatter.HasFiles,
                                IsDeleted = false,
                                OrganizationUnitId = recordmatter.OrganizationUnitId,
                                RecordId = newrecord.Id,
                                RecordMatterName = recordmatter.RecordMatterName,
                                Status = RecordMatterConsts.RecordMatterStatus.New,
                                UserId = AbpSession.UserId,
                                TenantId = AbpSession.TenantId != null ? (int?)AbpSession.TenantId : null,
                                FormId = recordmatter.FormId,
                                Order = recordmatter.Order,
                                RequireReview = recordmatter.RequireReview,
                                RequireApproval = recordmatter.RequireApproval,
                                Filter = recordmatter.Filter,
                            };

                            var rmi = await _recordManager.CreateAndOrFetchRecordMatter(new ACL()
                            {
                                UserId = AbpSession.UserId
                            }, newrecordmatter);

                            // Create new groupId, Only single groups per Project => Step
                            Guid groupid = Guid.NewGuid();

                            foreach (RecordMatterItem recordmatteritem in recordmatter.RecordMatterItems)
                            {

                                var newrecordmatteritem = new RecordMatterItem()
                                {
                                    AllowedFormats = recordmatteritem.AllowedFormats,
                                    AllowHtmlAssignees = recordmatteritem.AllowHtmlAssignees,
                                    AllowPdfAssignees = recordmatteritem.AllowPdfAssignees,
                                    AllowWordAssignees = recordmatteritem.AllowWordAssignees,
                                    Document = recordmatteritem.Document,
                                    DocumentName = recordmatteritem.DocumentName,
                                    ErrorDetails = recordmatteritem.ErrorDetails,
                                    FormId = recordmatteritem.FormId,
                                    FormURI = recordmatteritem.FormURI,
                                    GroupId = groupid,
                                    HasDocument = recordmatteritem.HasDocument,
                                    LockOnBuild = recordmatteritem.LockOnBuild,
                                    RecordMatterId = newrecordmatter.Id,
                                    IsDeleted = false,
                                    OrganizationUnitId = recordmatter.OrganizationUnitId,
                                    UserId = AbpSession.UserId,
                                    TenantId = AbpSession.TenantId != null ? (int?)AbpSession.TenantId : null
                                };

                                await _recordManager.CreateAndOrFetchRecordMatterItem(newrecordmatteritem);

                            }

                            //NEED TO DO: filter first and then open the project, if the first step is hidden, it will return error
                            // Used for the Return URL

                            //get the first recordmatter
                            if (recordmatter == record.RecordMatters.OrderBy(r => r.Order).First())
                            {
                                formid = Convert.ToString(rmi.FormId);
                                recordmatterid = Convert.ToString(rmi.Id);
                            }
                        }

                        Guid newprojectid = Guid.NewGuid();
                        var newproject = new Project()
                        {
                            Description = description,
                            Name = projectname,
                            RecordId = newrecord.Id,
                            Status = ProjectStatus.New,
                            Type = ProjectType.User,
                            Id = newprojectid,
                            ProjectTemplateId = projectId,
                            ReleaseId = releaseId,
                            ProjectEnvironmentId = rOutput.ProjectEnvironmentId
                        };

                        if (AbpSession.TenantId != null)
                        {
                            newproject.TenantId = (int?)AbpSession.TenantId;
                        }

                        await _projectRepository.InsertAsync(newproject);
                        await _ACLManager.AddACL(new ACL()
                        {
                            UserId = AbpSession.UserId,
                            Type = "Project",
                            EntityID = newproject.Id,
                            Role = "O"
                        });

                        _unitOfWorkManager.Current.SaveChanges();
                        result = String.Format(result, Convert.ToString(newprojectid), formid, recordmatterid);
                        return result;

                    }
                    else
                    {
                        throw new UserFriendlyException("Permission Denied");
                    }

                }
                else
                {
                    throw new UserFriendlyException("Project does not contain Records and is not configured");
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Projects_Edit)]
		 public async Task<GetProjectForEditOutput> GetProjectForEdit(EntityDto<Guid> input)
         {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                var project = await _projectRepository.FirstOrDefaultAsync(input.Id);
           
		        var output = new GetProjectForEditOutput {Project = ObjectMapper.Map<CreateOrEditProjectDto>(project)};

		        if (output.Project.RecordId != null)
                {
                    var _lookupRecord = await _lookup_recordRepository.FirstOrDefaultAsync((Guid)output.Project.RecordId);
                    output.RecordRecordName = _lookupRecord?.RecordName?.ToString();
                }
			
                return output;

            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

         }


		 public async Task CreateOrEdit(CreateOrEditProjectDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{

                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = (Guid)input.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
                if (ACLResult.IsAuthed)
                {
                    await Update(input);
                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }


			}
         }

        [AbpAuthorize(AppPermissions.Pages_Projects_Create)]
        protected virtual async Task Create(CreateOrEditProjectDto input)
        {
            var project = ObjectMapper.Map<Project>(input);

            //maybe add asigned ACL
            ACL aCL = new ACL()
            {
                UserId = AbpSession.UserId,
                Role = "O"
            };

            if (AbpSession.TenantId != null)
            {
                project.TenantId = (int?)AbpSession.TenantId;
                aCL.TenantId = (int?)AbpSession.TenantId;
            }


            await _projectRepository.InsertAsync(project);

            aCL.EntityID = project.Id;
            aCL.Type = "ProjectTemplate";
            await _ACLManager.AddACL(aCL);


        }

        [AbpAuthorize(AppPermissions.Pages_Projects_Edit)]
        protected virtual async Task Update(CreateOrEditProjectDto input)
        {
            var project = await _projectRepository.FirstOrDefaultAsync((Guid)input.Id);
            input.Type = project.Type;
            input.Status = project.Status;
            ObjectMapper.Map(input, project);
        }

		 [AbpAuthorize(AppPermissions.Pages_Projects_Delete)]
         public async Task Delete(EntityDto<Guid> input)
         {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

               
                await _projectRepository.DeleteAsync(input.Id);
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

            
         }

        public async Task RestoreProject(EntityDto<Guid> input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                
                var project = await _projectRepository.FirstOrDefaultAsync((Guid)input.Id);
                project.Archived = false;
                await _projectRepository.UpdateAsync(project);

                CurrentUnitOfWork.SaveChanges();
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

       
        public async Task ArchiveProject(EntityDto<Guid> input)
        {
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                var project = await _projectRepository.FirstOrDefaultAsync((Guid)input.Id);
                project.Archived = true;
                //await _projectRepository.DeleteAsync(input.Id);
                await _projectRepository.UpdateAsync(project);
                CurrentUnitOfWork.SaveChanges();
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

          
        }

#if STQ_PRODUCTION

        public async Task<Dto.FileDto> GetProjectsToExcel(GetAllProjectsForExcelInput input)
         {
			var statusFilter = input.StatusFilter.HasValue
                        ? (ProjectStatus) input.StatusFilter
                        : default;			
					var typeFilter = input.TypeFilter.HasValue
                        ? (ProjectType) input.TypeFilter
                        : default;			
					
			var filteredProjects = _projectRepository.GetAll()
						.Include( e => e.RecordFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description == input.DescriptionFilter)
						.WhereIf(input.StatusFilter.HasValue && input.StatusFilter > -1, e => e.Status == statusFilter)
						.WhereIf(input.TypeFilter.HasValue && input.TypeFilter > -1, e => e.Type == typeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.RecordRecordNameFilter), e => e.RecordFk != null && e.RecordFk.RecordName == input.RecordRecordNameFilter);

            var query = (from o in filteredProjects
                         join o1 in _lookup_recordRepository.GetAll() on o.RecordId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetProjectForViewDto()
                         {
                             Project = new ProjectDto
                             {
                                 Name = o.Name,
                                 Description = o.Description,
                                 Status = o.Status,
                                 Type = o.Type,
                                 Id = o.Id
                             },
                             RecordRecordName = s1 == null || s1.RecordName == null ? "" : s1.RecordName.ToString()
                         });


            var projectListDtos = await query.ToListAsync();

            return _projectsExcelExporter.ExportToFile(projectListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_Projects)]
        public async Task<PagedResultDto<ProjectRecordLookupTableDto>> GetAllRecordForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_recordRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.RecordName != null && e.RecordName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var recordList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ProjectRecordLookupTableDto>();
            foreach (var record in recordList)
            {
                lookupTableDtoList.Add(new ProjectRecordLookupTableDto
                {
                    Id = record.Id.ToString(),
                    DisplayName = record.RecordName?.ToString()
                });
            }

            return new PagedResultDto<ProjectRecordLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

#endif

        [AbpAuthorize(AppPermissions.Pages_Projects_Edit)]
        public async Task DraftStep(DraftInput input)
        {

            var recordmatter = _recordMatterRepository.GetAll().Where(e => e.Id == input.Id && e.CreatorUserId == AbpSession.UserId).FirstOrDefault();
            if (recordmatter == null)
            {
                throw new UserFriendlyException("Not Authorised");
            }
            else
            {
                recordmatter.Status = input.Draft ? RecordMatterConsts.RecordMatterStatus.Draft : recordmatter.Status;

                var rma = new RecordMatterAudit()
                {
                    Data = recordmatter.Data,
                    RecordMatterId = recordmatter.Id,
                    Status = (RecordMatterStatus)recordmatter.Status,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId
                };

                await _recordMatterAuditRepository.InsertAsync(rma);

            }

        }

        [AbpAuthorize(AppPermissions.Pages_Projects_Edit)]
        public async Task FinaliseUnlockedStep(FinaliseUnlockInput input)
        {

            var recordmatter = _recordMatterRepository.GetAll().Where(e => e.Id == input.Id && e.CreatorUserId == AbpSession.UserId).FirstOrDefault();
            if (recordmatter == null)
            {
                throw new UserFriendlyException("Not Authorised");
            }
            else
            {
               // recordmatter.Status = RecordMatterConsts.RecordMatterStatus.FinalUnlocked;
                await _recordMatterRepository.UpdateAsync(recordmatter);

                recordmatter.Status = input.FinaliseUnlock ? RecordMatterConsts.RecordMatterStatus.Draft : RecordMatterConsts.RecordMatterStatus.Final;
                
               
                //not running the assemblying process. if there is data flow through, futher further feature will be addeded.
                Project project = _projectRepository.GetAll().FirstOrDefault(e => e.RecordId == recordmatter.RecordId);
                if (project != null)
                {
                    // if all are new then new
                    var final = !_lookup_recordRepository.GetAllIncluding(e => e.RecordMatters).Where(r => r.Id == recordmatter.RecordId).Any(e => e.RecordMatters.Any(rm => rm.Status != RecordMatterConsts.RecordMatterStatus.Final));
                    if (project.Status == ProjectConsts.ProjectStatus.Completed && final)
                    {
                        project.Status = ProjectConsts.ProjectStatus.InProgress;
                    }
                    
                }

                var rma = new RecordMatterAudit()
                {
                    Data = recordmatter.Data,
                    RecordMatterId = recordmatter.Id,
                    Status = (RecordMatterStatus)recordmatter.Status,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId
                };

                await _recordMatterAuditRepository.InsertAsync(rma);

            }

            CurrentUnitOfWork.SaveChanges();
        }

             
       

        [AbpAuthorize(AppPermissions.Pages_Projects_Edit)]
        public async Task FinaliseStep(FinaliseInput input)
        {

            var recordmatter = _recordMatterRepository.FirstOrDefault(e => e.Id == input.Id && e.CreatorUserId == AbpSession.UserId);
            if (recordmatter == null)
            {
                throw new UserFriendlyException("Not Authorised");
            }
            else
            {

                var recordmatters = _recordMatterRepository.GetAll().Where(e => e.RecordId == recordmatter.Id);

                // Set the ProjectStep Statuses
                recordmatters.ToList().ForEach(rm => {

                    //Set the Step Status
                    // Check to see if all contributions are complete for step
                    // Inprogress = any contributor not competed
                    var contributors = _recordMatterContributorRepository.GetAll().Where(e => e.RecordMatterId == rm.Id);
                    var reviewcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Canceled).Count();
                    var reviewcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Approved).Count();
                    var reviewnew = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Awaiting && e.Enabled).Count();
                    var reviewrejected = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Rejected).Count();
                    var reviewtotal = reviewcanceled + reviewcomplete + reviewnew + reviewrejected;

                    var approvedcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Canceled).Count();
                    var approvedcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Approved).Count();
                    var approvednew = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Awaiting && e.Enabled).Count();
                    var approvedrejected = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Rejected).Count();
                    var approvedtotal = approvedcanceled + approvedcomplete + approvednew + approvedrejected;

                    if (reviewtotal > 0)
                    {
                        if (reviewnew > 0)
                        {
                            throw new UserFriendlyException("Project Step has uncompleted Review actions.");
                        }
                        if (reviewrejected > 0)
                        {
                            throw new UserFriendlyException("Project Step has rejected Review actions.");
                        }
                    }

                    if (approvednew + approvedrejected + approvedcanceled > 0)
                    {
                        throw new UserFriendlyException("Project Step has uncompleted Approve actions.");
                    }

                    if (recordmatter.Status == RecordMatterConsts.RecordMatterStatus.Final)
                    {
                        throw new UserFriendlyException("Project Step is already at Final Status.");
                    }


                });

                recordmatter.Status = input.Finalise ? RecordMatterConsts.RecordMatterStatus.Final : RecordMatterConsts.RecordMatterStatus.Share;

                // Set the Status = Finalise
                // "ProjectSteps": {
                //    "0": {
                //          "stepId": "0a707bc3-537d-482c-70f4-08d99418b6ff",
                //          "name": "Step 1",
                //          "status": "Share"
                //          }
                //  },
                //if (!string.IsNullOrEmpty(recordmatter.Data))
                //{
                //    JObject rmdata = JObject.Parse(recordmatter.Data);
                //    List<JToken> projectSteps = rmdata.SelectToken("ProjectSteps").ToList();
                //    if (projectSteps != null)
                //    {
                //        foreach (JToken step in projectSteps)
                //        {
                //            if (step.First != null)
                //            {
                //                var stepId = step.First.Value<string>("stepId") ?? "";
                //                if (stepId == Convert.ToString(recordmatter.Id))
                //                {                                    
                //                    step.First["status"] = "Final";
                //                    recordmatter.Data = rmdata.ToString();
                //                    CurrentUnitOfWork.SaveChanges();
                //                }
                //            }                            
                //        }
                //    }
                //}

                var rma = new RecordMatterAudit()
                {
                    Data = recordmatter.Data,
                    RecordMatterId = recordmatter.Id,
                    Status = (RecordMatterStatus)recordmatter.Status,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId
                };

                await _recordMatterAuditRepository.InsertAsync(rma);

            }

        }

        [AbpAuthorize(AppPermissions.Pages_Projects_Edit)]
        public async Task PublishStep(PublishInput input)
        {

            var recordmatter = _recordMatterRepository.GetAll().Where(e => e.Id == input.Id && e.CreatorUserId == AbpSession.UserId).FirstOrDefault();
            if (recordmatter == null)
            {
                throw new UserFriendlyException("Not Authorised");
            }
            else
            {
                var recordmatters = _recordMatterRepository.GetAll().Where(e => e.RecordId == recordmatter.Id);

                // Set the ProjectStep Statuses
                recordmatters.ToList().ForEach(rm => {

                    //Set the Step Status
                    // Check to see if all contributions are complete for step
                    // Inprogress = any contributor not competed
                    var contributors = _recordMatterContributorRepository.GetAll().Where(e => e.RecordMatterId == rm.Id);

                    var reviewcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Canceled).Count();
                    var reviewcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Approved && e.Enabled).Count();
                    var reviewnew = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Awaiting && e.Enabled).Count();
                    var reviewrejected = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Rejected).Count();
                    var reviewtotal = reviewcanceled + reviewcomplete + reviewnew + reviewrejected;

                    var approvedcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Canceled).Count();
                    var approvedcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Approved).Count();
                    var approvednew = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Awaiting && e.Enabled).Count();
                    var approvedrejected = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Rejected).Count();
                    var approvedtotal = approvedcanceled + approvedcomplete + approvednew + approvedrejected;


                    if (reviewtotal > 0)
                    {
                        if (reviewnew > 0)
                        {
                            throw new UserFriendlyException("Project Step has uncompleted Review actions.");
                        }
                        if (reviewrejected > 0)
                        {
                            throw new UserFriendlyException("Project Step has rejected Review actions.");
                        }
                    }

                    if (recordmatter.Status == RecordMatterConsts.RecordMatterStatus.Final)
                    {
                        throw new UserFriendlyException("Project Step is already at Final Status.");
                    }


                });

                recordmatter.Status = input.Publish ? RecordMatterConsts.RecordMatterStatus.Share : RecordMatterConsts.RecordMatterStatus.Draft;


                var rma = new RecordMatterAudit()
                {
                    Data = recordmatter.Data,
                    RecordMatterId = recordmatter.Id,
                    Status = (RecordMatterStatus)recordmatter.Status,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId
                };

                await _recordMatterAuditRepository.InsertAsync(rma);


                //// Run the assembly for this RecordMatter
                //string json = "{\"RecordMatterId\" : \"" + recordmatter.Id.ToString() + "\" }";
                //JObject frmInput = JObject.Parse(json);
                //_formAppService.Run(json);

            }

        }
         
        //Project Templates part
        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task<PagedResultDto<GetProjectTemplatesForViewDto>> GetAllProjectTemplates(GetAllProjectTemplatesInput input)
        {

            // All with Pages_Templates_Edit permission can view and edit Project templates

            var orgacls = (from a in _aclRepository.GetAll()
                           join ut in _userOrganizationUnitRepository.GetAll() on a.OrganizationUnitId equals ut.OrganizationUnitId
                           where ut.UserId == AbpSession.UserId
                           select a.EntityID);

            var acls = (from a in _aclRepository.GetAll()
                        where a.UserId == AbpSession.UserId
                        select a.EntityID);

            var acllist = acls.Concat(orgacls).ToList();

            var projectTemplateIds = (from c in _projectRepository.GetAll()
                        where c.ProjectId != null && 
                        c.Type == ProjectType.Template  &&
                        acllist.Contains(c.Id)
                        group c by c.ProjectId into g
                        orderby g.Key descending
                        select g.OrderByDescending(x => x.Version).FirstOrDefault().Id).ToList();

            //var projectTemplateIds = _projectRepository.GetAll()
            //    .Where(i => i.Type == ProjectType.Template)
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
            //    .OrderBy(p => p.Version)
            //    .GroupBy(p => p.ProjectId)
            //    .Select(p => p.Last().ProjectId);

            var projectTemplates = _projectRepository.GetAll()
                .Where(i => i.Type == ProjectType.Template && projectTemplateIds.Contains((Guid)i.Id))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter));
    
            var pagedAndFilteredProjects = projectTemplates
                .OrderBy(input.Sorting ?? "CreationTime desc")
                .PageBy(input).ToList();

            List<Guid> containSharedProject = new List<Guid>();
            var user = UserManager.GetUserById((long)AbpSession.UserId);
            if (await UserManager.IsInRoleAsync(user, StaticRoleNames.Host.Admin))
            {
                pagedAndFilteredProjects.ForEach(o =>
                {
                    if (hasSteps(o.Id))
                    {
                        containSharedProject.Add(o.Id);
                    }
                });
            };

            List<GetProjectTemplatesForViewDto> forViewResult = new List<GetProjectTemplatesForViewDto>();
            pagedAndFilteredProjects.ForEach(pt =>
            {
                forViewResult.Add(new GetProjectTemplatesForViewDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    Description = pt.Description,
                    CreationTime = pt.CreationTime,
                    Enabled = pt.Enabled,
                    Type = ProjectTemplateType.Template
                });

            });

            // Get Project Templates End

            ////// Get Project Deployments Start


            //var deployedProjectIds = _projectDeploymentRepository.GetAll()
            //            .Include(e => e.ProjectReleaseFk)
            //            .Select(pd => pd.ProjectReleaseFk.ProjectId);

            //var projectTemplateDeployments = _projectRepository.GetAll()
            //   .Where(i => deployedProjectIds.Contains(i.Id))
            //   .Where(i => i.Type == ProjectType.Template)
            //   .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter));


            //var pagedAndFilteredProjectTemplateDeployments = projectTemplateDeployments
            //    .OrderBy(input.Sorting ?? "id asc")
            //    .PageBy(input).ToList();


            //pagedAndFilteredProjectTemplateDeployments.ForEach(pt =>
            //{
            //    forViewResult.Add(new GetProjectTemplatesForViewDto
            //    {
            //        Id = pt.Id,
            //        Name = pt.Name,
            //        Description = pt.Description,
            //        CreationTime = pt.CreationTime,
            //        Enabled = pt.Enabled,
            //        Type = ProjectTemplateType.Deployment
            //    });
            //});
 
            //// Get Project Deployments End

            var totalCount = await projectTemplates.CountAsync(); // + await projectDeployments.CountAsync();

            return new PagedResultDto<GetProjectTemplatesForViewDto>(
                totalCount,
                forViewResult
                );
        }

        private Boolean hasSteps(Guid projecttemplateId)
        {
            try
            {
                var acls = _aclRepository.GetAllIncluding(i => i.User, i => i.OrganizationUnit)
                        .Where(i =>
                            i.EntityID == projecttemplateId &&
                            i.CreatorUserId != i.UserId &&
                            i.TargetTenantId != null
                        ).ToList();

                var result = (acls.Count > 0);
                return result;
            }
            catch
            {
                return false;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task<PagedResultDto<GetProjectTemplatesForViewDto>> GetAllSharedProjectTemplates(PagedAndSortedResultRequestDto input)
        {

            // Get ID's of projects shared with this Tenant
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var projectids = _ACLManager.GetAllSharedEntities(AbpSession.TenantId, "Project");

            //TODO: check ACL for share template.
            var projectTemplates = _projectRepository.GetAll()
                .Where(i => projectids.Contains(i.Id))
                .OrderBy(input.Sorting ?? "CreationTime desc")
                .PageBy(input).ToList();

            List<GetProjectTemplatesForViewDto> forViewResult = new List<GetProjectTemplatesForViewDto>();

            projectTemplates.ForEach(pt =>
            {
                var tenancyName = "Host";
                MultiTenancy.Tenant tenant = null;
                if (pt.TenantId != null)
                {
                    tenant = TenantManager.GetById((int)pt.TenantId);
                    if (tenant != null)
                        tenancyName = tenant.TenancyName;
                }

                var acl = _aclRepository.GetAll().FirstOrDefault(a => a.EntityID == pt.Id && a.TargetTenantId == AbpSession.TenantId);
                forViewResult.Add(new GetProjectTemplatesForViewDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    TenancyName = tenancyName,
                    Description = pt.Description,
                    CreationTime = pt.CreationTime,
                    Enabled = pt.Enabled,
                    Accepted = acl.Accepted
                });

            });

            return new PagedResultDto<GetProjectTemplatesForViewDto>(
                projectids.Count(),
                forViewResult
                );
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task CreateOrEditProjectTemplate(CreateOrEditProjectTemplateDto input)
        {

            if (input.Id == null || ! _projectRepository.GetAll().Any(p => p.Id == input.Id))
            {
                //Create project template
                await CreateProjectTemplate(input);

            }
            else
            {
                //Edit Project Template
                await EditProjectTemplate(input);
            }

        }

        public async Task<int> CreateProjectVersion(CreateOrEditProjectTemplateDto input)
        {

            //if (_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId }).IsAuthed ||
            //    _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.OriginalId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId }).IsAuthed)
            //{

                if (_projectRepository.GetAll().Any(p => p.Id == (Guid)input.Id))
                {
                    input.Id = Guid.NewGuid();
                }

                input.ProjectId = input.ProjectId;

                var maxver  = _projectRepository.GetAll().
                        Where(p => p.ProjectId == input.ProjectId).
                        OrderByDescending(p => p.Version).Select(p => p.Version).FirstOrDefault();
                
                input.Version = maxver + 1;

                await CreateProjectTemplate(input);

                 return input.Version;
            //}
            //else
            //{
            //    throw new UserFriendlyException("Not Authorised");
            //}

        }

        public async Task<List<GetProjectTemplateForView>> GetVersionHistory(EntityDto<Guid> input)
        {

            //ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            //if (ACLResult.IsAuthed)
            //{
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                var filteredTemplates = _projectRepository.GetAll().Where(i => i.ProjectId == input.Id).OrderByDescending(i => i.Version);
                var query = (from o in filteredTemplates select new GetProjectTemplateForView() { Id = o.Id, Version = o.Version });
                return new List<GetProjectTemplateForView>(
                    query.ToList()
                );
            //}
            //else
            //{
            //    throw new UserFriendlyException("Not Authorised");
            //}

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Create)]
        private async Task CreateProjectTemplate(CreateOrEditProjectTemplateDto input)
        {

            // Project template should have step
            if (input.StepsSchema.Count() != 0)
            {
                //1 Create a new Record
                Record newRecord = new Record()
                {
                    Id = Guid.NewGuid(), // need a new Id.
                    RecordName = input.Name, //Project name
                    FolderId = null,
                    Data = null, // Do we need any data for this record?
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId != null ? (int?)AbpSession.TenantId : null
                };
                //maybe add asigned ACL
                ACL aCL = new ACL()
                {
                    UserId = AbpSession.UserId
                };

                await _recordManager.CreateOrEditRecord(aCL, newRecord);


                //2. Create RecordMatter bases on the steps.
                try
                {
                    foreach (ProjectTemplateStepDto step in input.StepsSchema)
                    {
                        var newRecordMatter = new RecordMatter()
                        {
                            Id = Guid.NewGuid(), // need a new Id?.
                            Comments = step.Description,
                            Data = null, // DO we need data?
                            HasFiles = false, // Defualt?
                            IsDeleted = false,
                            OrganizationUnitId = null,
                            RecordId = newRecord.Id,
                            RecordMatterName = step.StepName ?? "No Document Name",
                            Status = null,
                            UserId = AbpSession.UserId,
                            TenantId = AbpSession.TenantId != null ? AbpSession.TenantId : null,
                            FormId = step.FormId,
                            Order = step.Order,
                            Filter = step.Filter,
                            RequireApproval = step.RequireApproval//,
                            //RequireReview = step.RequireReview
                        };

                        await _recordManager.CreateAndOrFetchRecordMatter(new ACL()
                        {
                            UserId = AbpSession.UserId
                        }, newRecordMatter);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    // create recordMatter error
                }

                var pid = input.Id.HasValue? input.Id : Guid.NewGuid();
                if (!input.ProjectId.HasValue) input.ProjectId = pid;

                CreateOrEditProjectDto newProjectObj = new CreateOrEditProjectDto
                {
                    Id = pid,
                    Name = input.Name,
                    Description = input.Description,
                    Status = ProjectStatus.New,
                    Type = ProjectType.Template,
                    RecordId = newRecord.Id,
                    Enabled = input.Enabled,
                    Version = input.Version,
                    VersionDescription = input.VersionDescription,
                    ProjectId  = input.ProjectId 
                };
                await Create(newProjectObj);

                //add to version history when create a new project templete
                try
                {
                    input.Id = newProjectObj.Id;

                    await UpdateVersionAudit(null, input, "Create a Project template");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }


                //// Remove then add Tags
                //var tags = _tagEntityRepository.GetAll().Where(t => t.EntityId == input.Id).ToList();
                //tags.ForEach(t =>
                //{
                //    _tagEntityRepository.Delete(t);
                //});

                //input.Tags.ForEach(t =>
                //{
                //    TagEntity tagentity = new TagEntity()
                //    {
                //        Id = Guid.NewGuid(),
                //        EntityId = (Guid)newProjectObj.Id,
                //        EntityType = EntityType.Project,
                //        TagValueId = t.Name,
                //        TenantId = AbpSession.TenantId,
                //        CreatorUserId = AbpSession.UserId
                //    };

                //    _tagEntityRepository.Insert(tagentity);
                //});

                //if (input.Assignees != null)
                //{
                //    input.Assignees.ForEach(async a =>
                //    {
                //        await _ACLManager.AddACL(new ACL()
                //        {
                //            CreatorUserId = AbpSession.UserId,
                //            TenantId = AbpSession.TenantId,
                //            UserId = a.Type == "User" ? a.Id : null,
                //            OrganizationUnitId = a.Type == "Team" ? a.Id : null,
                //            Type = "Project",
                //            EntityID = (Guid)newProjectObj.Id,
                //            Role = "V" // set as View
                //        });
                //    });
                //}

            }
            else
            {
                throw new UserFriendlyException("Project Step error");
            }
        }

        //new, old
        public JToken GetDataDiff(string dataA, string dataB)
        {

            JToken result = JToken.Parse("{}");

            dataA = string.IsNullOrEmpty(dataA) ? "{}" : dataA;
            dataB = string.IsNullOrEmpty(dataB) ? "{}" : dataB;

            var j1 = JToken.Parse(dataA);
            var j2 = JToken.Parse(dataB);

            var jdp = new JsonDiffPatch();
            result = jdp.Diff(j1, j2);

            // Changed library to fix Form save error
            //result = JsonDifferentiator.Differentiate(j2, j1);
            
            return result;
        }

        private async Task UpdateVersionAudit(CreateOrEditProjectTemplateDto ptOld, CreateOrEditProjectTemplateDto ptNew, string Description)
        {

            var jDiff = JToken.FromObject(ptNew);

            //compare the steps of project template
            //if (ptOld != null)
            //{
                string ptold = ptOld == null ? "{}":JsonConvert.SerializeObject(ptOld);
                string ptnew = JsonConvert.SerializeObject(ptNew);
                jDiff = GetDataDiff(ptnew, ptold);
           // }


            //update the version history entity
            var rma = new EntityVersionHistory()
            {
                Data = jDiff == null ? "{}" : jDiff.ToString(),
                Name = ptNew.Name,
                Description = Description,
                Version = 1,
                PreviousVersion = 1, //live version
                EntityId = (Guid)ptNew.Id,
                UserId = AbpSession.UserId,
                TenantId = AbpSession.TenantId,
                VersionName = "Version 1",
                Type = "Project Template",
                PreviousData = ptOld==null?"{}": JsonConvert.SerializeObject(ptOld),
                NewData = JsonConvert.SerializeObject(ptNew)
        };

            await _entityVersionHistoryRepository.InsertAsync(rma);

            ACL aCL = new ACL()
            {
                UserId = AbpSession.UserId,
                TenantId = AbpSession.TenantId,
                EntityID = rma.Id,
                Type = "EntityVersionHistory",
                Role = "O",
            };
            await _ACLManager.AddACL(aCL);
        }


        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        private async Task EditProjectTemplate(CreateOrEditProjectTemplateDto input)
        {

            // Project template should have step
            if (input.StepsSchema.Count() != 0)
            {

                //var projectTemplate = _projectRepository.GetAll().FirstOrDefault(pt => pt.Id == (Guid)input.Id && pt.CreatorUserId == AbpSession.UserId);
                var projectTemplate = _projectRepository.GetAll().FirstOrDefault(pt => pt.Id == (Guid)input.Id );
                var tags = _tagEntityRepository.GetAll().Where(t => t.EntityId == input.Id).ToList();

                if (projectTemplate == null)
                {
                    // before using ACL check.
                    throw new UserFriendlyException(L("ProjectTemplateEditError"));
                }

                var currentMattersIdList = _recordMatterRepository.GetAll()
                    .Where(e => e.RecordId == projectTemplate.RecordId)
                     .Select(e => e.Id.ToString()).ToList();
                //get old project template steps
                var dataA = _recordMatterRepository.GetAll()
                    .Where(e => e.RecordId == projectTemplate.RecordId)
                    .Select(d => new ProjectTemplateStepDto
                    {
                        RecordMatterId = d.Id,
                        StepName = d.RecordMatterName,
                        FormId = d.FormId,
                        Description = d.Comments,
                        Filter = d.Filter,
                        Order = d.Order,
                        RequireApproval = d.RequireApproval
                    })
                    .OrderBy("Order asc")
                    .ToList();

                // Get old tags for the template
                var tagsOld = new List<ProjectTagsDto>();

                // update tag repository
                tags.ForEach(o =>
                {
                    tagsOld.Add(new ProjectTagsDto()
                    {
                        Name = (Guid)o.TagValueId,
                        //tagValueName = _tagValueRepository.GetAll().FirstOrDefault(tv => tv.Id == (Guid)o.TagValueId).Value,
                        Value = "true"
                    });
                });

                //get old project 
                var ptOld = new CreateOrEditProjectTemplateDto()
                {
                    Id = projectTemplate.Id,
                    Name = projectTemplate.Name,
                    Description = projectTemplate.Description,
                    StepsSchema = dataA,
                    Enabled = projectTemplate.Enabled,
                    Tags = tagsOld
                };
                // var ptOld = ObjectMapper.Map<CreateOrEditProjectTemplateDto>(projectTemplate);




                await UpdateVersionAudit(ptOld, input, "Update a Project template");
                
            //1 When the input has recordmatterid, then update the recordmatter,
            //2 If the input recordMatterId is null, create new recordmatter
            //3 If the cuurentMatterId doesn't in input, delete the recordmatter.

            //4 Finally update the projectTemplate with Name, Description
            //Update Recormatters
                try
                {

                    foreach (ProjectTemplateStepDto step in input.StepsSchema)
                    {
                        string stepRecordMatterId = step.RecordMatterId.ToString().ToUpper();
                        if (!string.IsNullOrEmpty(stepRecordMatterId)
                            && currentMattersIdList.Contains(stepRecordMatterId, StringComparer.InvariantCultureIgnoreCase))
                        {
                            // If this step' RecordMatterId on the currentMatterIdList, 
                            //do updating and remove from currentMatterIdList
                            Guid stepRMId = Guid.Parse(stepRecordMatterId);
                            var recordMatter = _recordMatterRepository.GetAll().Where(rm => rm.Id == stepRMId).First();
                            //Update Name,FormId,Comments(Description),Order
                            recordMatter.RecordMatterName = step.StepName ?? "No Document Name";
                            recordMatter.FormId = step.FormId;
                            recordMatter.Comments = step.Description;
                            recordMatter.Order = step.Order;
                            recordMatter.RequireApproval = step.RequireApproval;
                            recordMatter.Filter = step.Filter;
                            //recordMatter.RequireReview = step.RequireReview;
                            await _recordManager.CreateAndOrFetchRecordMatter(new ACL()
                            {
                                UserId = AbpSession.UserId
                            }, recordMatter);

                            currentMattersIdList.Remove(stepRecordMatterId);
                        }
                        else
                        {
                            // Create a new recordmatter for this step
                            var newRecordMatter = new RecordMatter()
                            {
                                Id = Guid.NewGuid(), // need a new Id?.
                                Comments = step.Description,
                                Data = null, // DO we need data?
                                HasFiles = false, // Defualt?
                                IsDeleted = false,
                                OrganizationUnitId = null,
                                RecordId = (Guid)projectTemplate.RecordId,
                                RecordMatterName = step.StepName ?? "No Document Name",
                                Filter = step.Filter,
                                Status = null,
                                UserId = AbpSession.UserId,
                                TenantId = AbpSession.TenantId != null ? AbpSession.TenantId : null,
                                FormId = step.FormId,
                                Order = step.Order,
                                RequireApproval = step.RequireApproval,
                                //RequireReview = step.RequireReview
                            };
                            await _recordManager.CreateAndOrFetchRecordMatter(new ACL()
                            {
                                UserId = AbpSession.UserId
                            }, newRecordMatter);
                        }

                    }

                    //Finally, Delete all recordmatters which still contain in currentMatterIdList
                    currentMattersIdList.ForEach(rmid =>
                    {
                        _recordMatterRepository.DeleteAsync(Guid.Parse(rmid));
                    });

                }
                catch
                {
                    // create,update,delete recordMatter error
                }
                projectTemplate.Name = input.Name;
                projectTemplate.Description = input.Description;
                projectTemplate.Enabled = input.Enabled;
                await _projectRepository.UpdateAsync(projectTemplate);


                // Remove then add Tags
                tags.ForEach(t =>
                {
                    _tagEntityRepository.Delete(t);
                });

                if (input.Tags != null)
                {
                    input.Tags.ForEach(t =>
                    {
                        TagEntity tagentity = new TagEntity()
                        {
                            Id = Guid.NewGuid(),
                             EntityId = (Guid)input.Id,
                             EntityType = EntityType.Project,
                             TagValueId = t.Name,  
                             TenantId = AbpSession.TenantId,
                            CreatorUserId = AbpSession.UserId
                        };

                        _tagEntityRepository.Insert(tagentity);
                    });
                }
 
                // //Update ACL
                // //1. remove all ACL
                // await _ACLManager.RemoveAllACLForEntityAsync(projectTemplate.Id, false);

                ////2. add Assignees[] into sfaACL, as View
                //if (input.Assignees != null)
                //{
                //    input.Assignees.ForEach(async a =>
                //    {
                //        await _ACLManager.AddACL(new ACL()
                //        {
                //            CreatorUserId = AbpSession.UserId,
                //            TenantId = AbpSession.TenantId,
                //            UserId = a.Type == "User" ? a.Id : null,
                //            OrganizationUnitId = a.Type == "Team" ? a.Id : null,
                //            Type = "Project",
                //            EntityID = projectTemplate.Id,
                //            Role = "V" // Set as View
                //        });

                //    });
                //}
 
            }
            else
            {
                //error Step null
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Delete)]
        public async Task DeleteProjectTemplate(EntityDto<Guid> input)
        {
            // Do we need to delete the Record and RecordMatters?

            var p = _projectRepository.GetAll().FirstOrDefault(p => p.Id == input.Id);
            if (p != null)
            {
                if (_projectReleaseRepository.GetAll().Any(pr => pr.ProjectId == p.ProjectId))
                {
                    throw new UserFriendlyException("Cannot delete as this Project has active Releases");
                }
                else
                {
                    await _projectRepository.DeleteAsync(p);
                }
            }


            //await _projectRepository.DeleteAsync(pt => pt.Id == input.Id && pt.CreatorUserId == AbpSession.UserId);
        }

        //[AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        //public async Task AcceptSharedProjectTemplate(EntityDto<Guid> input)
        //{
        //    // Acl is shared (created by a different tenant)
        //    _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
        //    if (_aclRepository.GetAll().Any(a => a.EntityID == input.Id && a.TargetTenantId == AbpSession.TenantId && a.Type == "project"))
        //    {
        //        var acl = _aclRepository.GetAll().FirstOrDefault(a => a.EntityID == input.Id && a.TargetTenantId == AbpSession.TenantId && a.Type == "project");
        //        acl.Accepted = true;
        //        await _aclRepository.UpdateAsync(acl);
        //    }
        //    else
        //    {
        //        throw new UserFriendlyException(L("PermissionDenied"));
        //    }
        //}

        //[AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        //public async Task RemoveSharedProjectTemplate(EntityDto<Guid> input)
        //{
        //    // Acl is shared (created by a different tenant)
        //    _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
        //    if (_aclRepository.GetAll().Any(a => a.EntityID == input.Id && a.TargetTenantId == AbpSession.TenantId && a.Type == "project"))
        //    {
        //        var acl = _aclRepository.GetAll().FirstOrDefault(a => a.EntityID == input.Id && a.TargetTenantId == AbpSession.TenantId && a.Type == "project");
        //        await _aclRepository.DeleteAsync(acl);
        //    }
        //    else
        //    {
        //        throw new UserFriendlyException(L("PermissionDenied"));
        //    }
        //}

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task<CreateOrEditProjectTemplateDto> GetProjectTemplatesForEdit(EntityDto<Guid> input)
        {
            // All with permission can edit
            //var projectTemplate = _projectRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id && pt.CreatorUserId == AbpSession.UserId);
            var projectTemplate = _projectRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id);
            if (projectTemplate == null)
            {
                // before using ACL check.
                throw new UserFriendlyException(L("ProjectTemplateEditError"));
            }
            List<ProjectTemplateStepDto> stepsSchema = getStepsSchema((Guid)projectTemplate.RecordId);

            List<ACL> acls = _ACLManager.GetAllACLByEntityId(projectTemplate.Id, "Project");
            List<GrantACLDto> assignees = new List<GrantACLDto>();
            acls.ForEach(acl =>
            {
                if (acl.UserId != null && acl.OrganizationUnitId != null)
                {
                    assignees.Add(new GrantACLDto
                    {
                        Id = (int?)(acl.UserId ?? acl.OrganizationUnitId),
                        value = acl.UserId != null ? acl.User.UserName : acl.OrganizationUnit.DisplayName,
                        Type = acl.UserId != null ? "User" : "Team"
                    });
                }
            });
            var output = new CreateOrEditProjectTemplateDto
            {
                Id = projectTemplate.Id,
                Name = projectTemplate.Name,
                Description = projectTemplate.Description,
                StepsSchema = stepsSchema,
                Enabled = projectTemplate.Enabled,
                Version = projectTemplate.Version,
                ProjectId = projectTemplate.ProjectId,
                Assignees = assignees

            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task<CreateOrEditProjectTemplateDto> GetProjectTemplatesTagsForEdit(EntityDto<Guid> input)
        {
            // All with permission can edit
            //var projectTemplate = _projectRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id && pt.CreatorUserId == AbpSession.UserId);
            var projectTemplate = _projectRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id);
            if (projectTemplate == null)
            {
                // before using ACL check.
                throw new UserFriendlyException(L("ProjectTemplateEditError"));
            }
            List<ProjectTemplateStepDto> stepsSchema = getStepsSchema((Guid)projectTemplate.RecordId);

            List<ACL> acls = _ACLManager.GetAllACLByEntityId(projectTemplate.Id, "Project");
            List<GrantACLDto> assignees = new List<GrantACLDto>();
            acls.ForEach(acl =>
            {
                if (acl.UserId != null && acl.OrganizationUnitId != null)
                {
                    assignees.Add(new GrantACLDto
                    {
                        Id = (int?)(acl.UserId ?? acl.OrganizationUnitId),
                        value = acl.UserId != null ? acl.User.UserName : acl.OrganizationUnit.DisplayName,
                        Type = acl.UserId != null ? "User" : "Team"
                    });
                }
            });
            var output = new CreateOrEditProjectTemplateDto
            {
                Id = projectTemplate.Id,
                Name = projectTemplate.Name,
                Description = projectTemplate.Description,
                StepsSchema = stepsSchema,
                Enabled = projectTemplate.Enabled,

                Assignees = assignees

            };

            return output;
        }


        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task CreateOrEditProjectTemplateTags(CreateOrEditProjectTemplateDto input)
        {

            // Remove then add Tags
            var tags = _tagEntityRepository.GetAll().Where(t => t.EntityId == input.Id).ToList();
            tags.ForEach(t =>
            {
                _tagEntityRepository.Delete(t);
            });

            input.Tags.ForEach(t =>
            {
                TagEntity tagentity = new TagEntity()
                {
                    Id = Guid.NewGuid(),
                    EntityId = (Guid)input.Id,
                    EntityType = EntityType.Project,
                    TagValueId = t.Name,
                    TenantId = AbpSession.TenantId,
                    CreatorUserId = AbpSession.UserId
                };

                _tagEntityRepository.Insert(tagentity);
            });

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTemplates_Edit)]
        public async Task<GetProjectTemplateForView> GetProjectTemplateForView(Guid id)
        {

            //var projectTemplate = _projectRepository.GetAll().FirstOrDefault(pt => pt.Id == id && pt.CreatorUserId == AbpSession.UserId);
            var projectTemplate = _projectRepository.GetAll().FirstOrDefault(pt => pt.Id == id );
            if (projectTemplate == null)
            {
                // before using ACL check.
                throw new UserFriendlyException(L("ProjectTemplateEditError"));
            }

            List<ProjectTemplateStepDto> stepsSchema = getStepsSchema((Guid)projectTemplate.RecordId);

            var output = new GetProjectTemplateForView
            {
                Name = projectTemplate.Name,
                Description = projectTemplate.Description,
                CreationTime = projectTemplate.CreationTime,
                LastModificationTime = projectTemplate.LastModificationTime ?? projectTemplate.CreationTime,
                StepsSchema = stepsSchema,

                Enabled = projectTemplate.Enabled

            };

            return output;
        }

        private List<ProjectTemplateStepDto> getStepsSchema(Guid projectTemplateRecordId)
        {

            var stepsRecordMatters = _recordMatterRepository.GetAll()
                .Where(rm => rm.RecordId == projectTemplateRecordId)
                .OrderBy("Order asc").ToList();

            List<ProjectTemplateStepDto> stepsSchema = new List<ProjectTemplateStepDto>();
            stepsRecordMatters.ForEach(srm => {
                stepsSchema.Add(new ProjectTemplateStepDto
                {
                    StepName = srm.RecordMatterName,
                    FormId = srm.FormId,
                    RecordMatterId = srm.Id,
                    Description = srm.Comments,
                    Order = srm.Order,
                    RequireApproval = srm.RequireApproval,
                    Filter = srm.Filter
                    //RequireReview = srm.RequireReview
                });
            });
            return stepsSchema;
        }

        public async Task<PagedResultDto<GetProjectForViewDto>> GetArchivedProject(GetAllProjectsInput input)
        {


            var statusFilter = input.StatusFilter.HasValue
                        ? (ProjectStatus)input.StatusFilter
                        : default;
            var typeFilter = input.TypeFilter.HasValue
                ? (ProjectType)input.TypeFilter
                : default;

            var filteredProjects = _projectRepository.GetAll()
                    .Include(e => e.RecordFk)
                    .Where(e => e.Type == ProjectType.User && e.CreatorUserId == AbpSession.UserId && e.Archived == true)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                    .WhereIf(input.StatusFilter.HasValue && input.StatusFilter > -1, e => e.Status == statusFilter)
                    .WhereIf(input.TypeFilter.HasValue && input.TypeFilter > -1, e => e.Type == typeFilter)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.RecordRecordNameFilter), e => e.RecordFk != null && e.RecordFk.RecordName == input.RecordRecordNameFilter);

            var pagedAndFilteredProjects = filteredProjects
                .OrderBy(input.Sorting ?? "LastModificationTime desc")
                .PageBy(input);

            var projects = from o in pagedAndFilteredProjects
                           join o1 in _lookup_recordRepository.GetAll() on o.RecordId equals o1.Id into j1
                           from s1 in j1.DefaultIfEmpty()

                           select new GetProjectForViewDto()
                           {
                               Project = new ProjectDto
                               {
                                   Name = o.Name,
                                   Description = o.Description,
                                   Status = o.Status,
                                   Type = o.Type,
                                   Id = o.Id,
                                   CreationTime = o.CreationTime,
                                   LastModificationTime = o.LastModificationTime == null ? o.CreationTime : (DateTime)o.LastModificationTime,
                               },
                               RecordRecordName = s1 == null || s1.RecordName == null ? "" : s1.RecordName.ToString()
                           };


            var totalCount = await filteredProjects.CountAsync();

            return new PagedResultDto<GetProjectForViewDto>(
           totalCount,
           await projects.ToListAsync()
       );


        }
    }


    class ProjectExport
    {
        public Project Project { get; set; }
        public List<Form> Forms { get; set; } = new List<Form>();
        public List<Record> Records { get; set; } // Steps
        public List<FormRule> FormRules { get; set; } = new List<FormRule>();
        public List<AppJob> AppJobs { get; set; } = new List<AppJob>();
        public List<Template> Templates { get; set; } = new List<Template>();
    }
}