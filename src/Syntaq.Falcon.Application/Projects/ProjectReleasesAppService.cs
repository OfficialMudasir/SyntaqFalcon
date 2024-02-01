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
using Abp.UI;
using Syntaq.Falcon.Storage;
using System.IO;
using NUglify.Helpers;
using Syntaq.Falcon.Notifications;
using Syntaq.Falcon.Authorization.Users;
using Abp;
using Abp.Notifications;
using crypto;
using NPOI.SS.Formula.Functions;
using Microsoft.AspNetCore.Identity;
using Syntaq.Falcon.Authorization.Roles;
using Abp.Zero.Configuration;
using Syntaq.Falcon.Tags;
using Abp.Domain.Uow;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Abp.EntityFrameworkCore.Repositories;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using Abp.Localization;
using static Syntaq.Falcon.Authorization.Roles.StaticRoleNames;
using System.Globalization;

namespace Syntaq.Falcon.Projects
{
    [AbpAuthorize(AppPermissions.Pages_ProjectReleases)]
    public class ProjectReleasesAppService : FalconAppServiceBase, IProjectReleasesAppService
    {
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<ProjectRelease, Guid> _projectReleaseRepository;
        private readonly IRepository<ProjectDeployment, Guid> _projectDeploymentRepository;
        private readonly IRepository<ProjectTenant, Guid> _projectTenantRepository;
        private readonly IRepository<TagEntity, Guid> _tagEntityRepository;
        private readonly IRepository<ACL> _aclRepository;
        private readonly ACLManager _ACLManager;

        private readonly IProjectReleasesExcelExporter _projectReleasesExcelExporter;
        private readonly IRepository<ProjectEnvironment, Guid> _lookup_projectEnvironmentRepository;
        private readonly ProjectManager _projectManager;
        private readonly IAppNotifier _appNotifier;
        private readonly RoleManager _roleManager;
        private readonly INotificationPublisher _notificationPublisher;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProjectReleasesAppService(
            IRepository<Project, Guid> projectRepository,
            IRepository<ProjectRelease, Guid> projectReleaseRepository,
            IRepository<ProjectDeployment, Guid> projectDeploymentRepository,
            IRepository<ProjectTenant, Guid> projectTenantRepository,
            IRepository<TagEntity, Guid> tagEntityRepository,
            IRepository<ACL> aclRepository,
            ACLManager aclManager,
            IProjectReleasesExcelExporter projectReleasesExcelExporter, 
            IRepository<ProjectEnvironment, Guid> lookup_projectEnvironmentRepository,
            ProjectManager projectManager,
            IAppNotifier appNotifier,
            RoleManager roleManager,
            INotificationPublisher notificationPublisher,
            IUnitOfWorkManager unitOfWorkManager
        )        
        {
            _aclRepository = aclRepository;
            _ACLManager = aclManager;
            _projectRepository = projectRepository;
            _projectReleaseRepository = projectReleaseRepository;
            _projectDeploymentRepository = projectDeploymentRepository;
            _projectTenantRepository = projectTenantRepository;
            _tagEntityRepository = tagEntityRepository;
            _lookup_projectEnvironmentRepository = lookup_projectEnvironmentRepository;

            _projectManager = projectManager;
            _appNotifier = appNotifier;
            _roleManager = roleManager;
            _notificationPublisher = notificationPublisher;
            _unitOfWorkManager = unitOfWorkManager;

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Edit)]
        public async Task<PagedResultDto<GetProjectReleaseForViewDto>> GetAll(GetAllProjectReleasesInput input)
        {
            var releaseTypeFilter = input.ReleaseTypeFilter.HasValue
                        ? (ProjectReleaseEnums.ProjectReleaseType)input.ReleaseTypeFilter
                        : default;

            var filteredProjectReleases = _projectReleaseRepository.GetAll()
                        .Include(e => e.ProjectEnvironmentFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Notes.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NotesFilter), e => e.Notes == input.NotesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectIdFilter.ToString()), e => e.ProjectId.ToString() == input.ProjectIdFilter.ToString())
                        .WhereIf(input.RequiredFilter.HasValue && input.RequiredFilter > -1, e => (input.RequiredFilter == 1 && e.Required) || (input.RequiredFilter == 0 && !e.Required))
                        .WhereIf(input.MinVersionMajorFilter != null, e => e.VersionMajor >= input.MinVersionMajorFilter)
                        .WhereIf(input.MaxVersionMajorFilter != null, e => e.VersionMajor <= input.MaxVersionMajorFilter)
                        .WhereIf(input.MinVersionMinorFilter != null, e => e.VersionMinor >= input.MinVersionMinorFilter)
                        .WhereIf(input.MaxVersionMinorFilter != null, e => e.VersionMinor <= input.MaxVersionMinorFilter)
                        .WhereIf(input.MinVersionRevisionFilter != null, e => e.VersionRevision >= input.MinVersionRevisionFilter)
                        .WhereIf(input.MaxVersionRevisionFilter != null, e => e.VersionRevision <= input.MaxVersionRevisionFilter)
                        .WhereIf(input.ReleaseTypeFilter.HasValue && input.ReleaseTypeFilter > -1, e => e.ReleaseType == releaseTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectEnvironmentNameFilter), e => e.ProjectEnvironmentFk != null && e.ProjectEnvironmentFk.Name == input.ProjectEnvironmentNameFilter);

            var pagedAndFilteredProjectReleases = filteredProjectReleases
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var projectReleases = from o in pagedAndFilteredProjectReleases
                                  join o1 in _lookup_projectEnvironmentRepository.GetAll() on o.ProjectEnvironmentId equals o1.Id into j1
                                  from s1 in j1.DefaultIfEmpty()

                                  select new
                                  {

                                      o.Name,
                                      o.Notes,
                                      o.ProjectId,
                                      o.CreationTime,
                                      o.Required,
                                      o.VersionMajor,
                                      o.VersionMinor,
                                      o.VersionRevision,
                                      o.ReleaseType,
                                      Id = o.Id,
                                      ProjectEnvironmentName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                  };

            var totalCount = await filteredProjectReleases.CountAsync();

            var dbList = await projectReleases.ToListAsync();
            var results = new List<GetProjectReleaseForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetProjectReleaseForViewDto()
                {
                    ProjectRelease = new ProjectReleaseDto
                    {

                        Name = o.Name,
                        Notes = o.Notes,
                        CreationTime = o.CreationTime,
                        ProjectId = o.ProjectId,
                        Required = o.Required,
                        VersionMajor = o.VersionMajor,
                        VersionMinor = o.VersionMinor,
                        VersionRevision = o.VersionRevision,
                        ReleaseType = o.ReleaseType,
                        Id = o.Id,
                    },
                    ProjectEnvironmentName = o.ProjectEnvironmentName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetProjectReleaseForViewDto>(
                totalCount,
                results
            );

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Edit)]
        public async Task<GetProjectReleaseForViewDto> GetProjectReleaseForView(Guid id)
        {
            var projectRelease = await _projectReleaseRepository.GetAsync(id);

            var output = new GetProjectReleaseForViewDto { ProjectRelease = ObjectMapper.Map<ProjectReleaseDto>(projectRelease) };

            if (output.ProjectRelease.ProjectEnvironmentId != null)
            {
                var _lookupProjectEnvironment = await _lookup_projectEnvironmentRepository.FirstOrDefaultAsync((Guid)output.ProjectRelease.ProjectEnvironmentId);
                output.ProjectEnvironmentName = _lookupProjectEnvironment?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectReleases_Edit)]
        public async Task<GetProjectReleaseForEditOutput> GetProjectReleaseForEdit(EntityDto<Guid> input)
        {

            var projectRelease = await _projectReleaseRepository.FirstOrDefaultAsync(input.Id);
            var output = new GetProjectReleaseForEditOutput { ProjectRelease = ObjectMapper.Map<CreateOrEditProjectReleaseDto>(projectRelease) };

            if (output.ProjectRelease.ProjectEnvironmentId != null)
            {
                var _lookupProjectEnvironment = await _lookup_projectEnvironmentRepository.FirstOrDefaultAsync((Guid)output.ProjectRelease.ProjectEnvironmentId);
                output.ProjectEnvironmentName = _lookupProjectEnvironment?.Name?.ToString();
            }

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant)){
                // If not deployments then allow editing
                if (await _projectDeploymentRepository.GetAll().AnyAsync(i => i.ProjectReleaseId == input.Id))
                output.HasDeployments = true;
            }


            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectReleases_Edit)]
        public async Task<GetProjectReleaseVersionForEditOutput> GetProjectReleaseVersionForEdit(EntityDto<Guid> input)
        {

            var output = new GetProjectReleaseVersionForEditOutput {  Id = input.Id };
            if (_projectReleaseRepository.GetAll().Any(pr => pr.ProjectId == input.Id))
            {
                var pr = await _projectReleaseRepository.GetAll().Where(pr => pr.ProjectId == input.Id).OrderBy(p => p.VersionMajor).ThenBy(p => p.VersionMajor).ThenBy(p => p.VersionMinor).LastAsync();
 
                output.VersionMajor = pr.VersionMajor;
                output.VersionMinor = pr.VersionMinor;  
                output.VersionRevision = pr.VersionRevision;
           
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditProjectReleaseDto input)
        {
            if (input.Id == null || input.ReleaseIdToClone != Guid.Empty)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectReleases_Create)]
        protected virtual async Task Clone(Guid releaseId, Guid environmentId)
        {

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectReleases_Create)]
        protected virtual async Task Create(CreateOrEditProjectReleaseDto input)
        {

            //List<int> tenantIds = new List<int>();
            //List<UserIdentifier> users = new List<UserIdentifier>();

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var projectRelease = ObjectMapper.Map<ProjectRelease>(input);
                
                if(input.ReleaseIdToClone == Guid.Empty)
                {
                    MemoryStream memoryStream = _projectManager.ExportProject(input.ProjectTemplateId);
                    projectRelease.Artifact = memoryStream.ToArray();
                }
                else
                {
                    var clonerelease = _projectReleaseRepository.GetAll().FirstOrDefault(r => r.Id == input.ReleaseIdToClone);
                    projectRelease.Artifact = clonerelease.Artifact;
                    projectRelease.Id = Guid.NewGuid();
                }

                if (AbpSession.TenantId != null)
                {
                    projectRelease.TenantId = (int?)AbpSession.TenantId;
                }

                await _projectReleaseRepository.InsertAsync(projectRelease);

                // TODO DUPLICATED CODE
                if (input.DeployToSubscribers)
                {
                    input.ProjectReleaseId = projectRelease.Id;
                    //DeployToSubscribers(input);

                    List<int> tenantIds = new List<int>();
                    List<UserIdentifier> users = new List<UserIdentifier>();


                    // Create Deployments using ProjectTenants
                    /// Get The Project Tenants from the 
                    var projectTenants = _projectTenantRepository.GetAll().Where(i =>
                            i.ProjectId == input.ProjectId &&
                            (i.ProjectEnvironmentId == input.ProjectEnvironmentId || i.ProjectEnvironmentId == null) &&
                            i.Enabled &&
                            (input.ProjectTenants.Contains((int)i.SubscriberTenantId))
                        ).ToList();

                    // Add extra Tenants if not in list
                    input.ProjectTenants.ForEach(t => {

                        if (!projectTenants.Any(pt => pt.SubscriberTenantId == t))
                        {
                            //Add to collection
                            ProjectTenant projectTenant = new ProjectTenant();
                            projectTenant.Enabled = true;
                            projectTenant.ProjectId = input.ProjectId;
                            projectTenant.SubscriberTenantId = t;
                            projectTenant.ProjectEnvironmentId = input.ProjectEnvironmentId;
                            projectTenants.Add(projectTenant);
                        }
                    });

                    projectTenants.ForEach(t => {

                        ProjectDeployment projectDeployment = new ProjectDeployment();
                        projectDeployment.TenantId = t.SubscriberTenantId;

                        // Auto Accept
                        projectDeployment.ActionType = input.Required ? ProjectDeploymentEnums.ProjectDeploymentActionType.Active : ProjectDeploymentEnums.ProjectDeploymentActionType.New;
                        projectDeployment.ProjectReleaseId = input.ProjectReleaseId;

                        // If Deployment has been auto accepted set other deployments to Inactive
                        if (input.Required)
                        {
                            var pds = _projectDeploymentRepository.GetAll().Include(i => i.ProjectReleaseFk).Where(p =>
                                p.ProjectReleaseFk.ProjectId == input.ProjectId &&
                                p.ProjectReleaseFk.ProjectEnvironmentId == input.ProjectEnvironmentId &&
                                p.TenantId == t.SubscriberTenantId &&
                                p.ActionType == ProjectDeploymentEnums.ProjectDeploymentActionType.Active &&
                                p.Id != projectDeployment.Id
                            );

                            pds.ForEach(e => e.ActionType = ProjectDeploymentEnums.ProjectDeploymentActionType.InActive);
                            _projectDeploymentRepository.GetDbContext().UpdateRange(pds);
                        }

                        _projectDeploymentRepository.Insert(projectDeployment);

                        // Add Deployment Tags
                        var tagEntities = _tagEntityRepository.GetAll().Where(e => e.EntityId == t.Id).ToList();
                        tagEntities.ForEach(t => {
                            var tagEntity = new TagEntity();
                            tagEntity.Id = Guid.NewGuid();
                            tagEntity.TenantId = projectDeployment.TenantId;
                            tagEntity.EntityId = projectDeployment.Id;
                            tagEntity.TagValueId = t.TagValueId;
                            tagEntity.EntityType = 0;
                            tagEntity.CreationTime = DateTime.Now;
                            _tagEntityRepository.InsertAsync(tagEntity);
                        });

                        tenantIds.Add(t.SubscriberTenantId);

                        // Get the Acls defined in the ProjectTenant Subscription to apply to the new Deployment
                        UnitOfWorkManager.Current.SetTenantId(t.SubscriberTenantId); // Set the tenantid as disabling tenantfilter stops notifications from working
                        var acls = _aclRepository.GetAll().Where(e => e.EntityID == t.Id).ToList();
                        acls.ForEach(a => {

                            ACL acl = new ACL()
                            {
                                EntityID = projectDeployment.Id,
                                UserId = a.UserId,
                                OrganizationUnitId = a.OrganizationUnitId,
                                Role = a.Role,
                                TargetTenantId = a.TargetTenantId,
                                TenantId = a.TenantId,
                                Type = a.Type
                            };

                            _ACLManager.AddACL(acl);
                        });

                    });

                    // Build List of Users to notifiy   
                    tenantIds.ToList().ForEach(t => {

                        // Create Notification Here
                        // Get the Tenant Admins and Tenant Approvers 
                        UnitOfWorkManager.Current.SetTenantId(t);
                        var adminRole = _roleManager.GetRoleByName("admin");

                        UserManager.Users.Where(
                           u => u.Roles.Any(r => r.RoleId == adminRole.Id)
                           ).Select(u => new UserIdentifier(t, u.Id)).ToList().ForEach(u => { users.Add(u); });
                    });

                    var project = _projectRepository.GetAll().FirstOrDefault(p => p.Id == input.ProjectTemplateId);
                    var projectName = project == null ? input.Name : project.Name;

                    UnitOfWorkManager.Current.SetTenantId(AbpSession.TenantId);
                    using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        var message = "A new release of " + projectName + " is available. " + input.Notes;

                        var u = users.ToArray();
                        await _notificationPublisher.PublishAsync(
                        AppNotificationNames.SimpleMessage,
                        new MessageNotificationData(message),
                        severity: NotificationSeverity.Info,
                        userIds: u
                        );
                    }

                }

                await UnitOfWorkManager.Current.SaveChangesAsync();
          
            }

        }

        //private void DeployToSubscribers(CreateOrEditProjectReleaseDto input)
        //{


        //    List<int> tenantIds = new List<int>();
        //    List<UserIdentifier> users = new List<UserIdentifier>();


        //    // Create Deployments using ProjectTenants
        //    /// Get The Project Tenants from the 
        //    var projectTenants = _projectTenantRepository.GetAll().Where(i =>
        //            i.ProjectId == input.ProjectId &&
        //            (i.ProjectEnvironmentId == input.ProjectEnvironmentId || i.ProjectEnvironmentId == null) &&
        //            i.Enabled &&
        //            (input.ProjectTenants.Contains((int)i.SubscriberTenantId))
        //        ).ToList();

        //        // Add extra Tenants if not in list
        //        input.ProjectTenants.ForEach(t => {

        //            if (!projectTenants.Any(pt => pt.SubscriberTenantId == t))
        //            {
        //                //Add to collection
        //                ProjectTenant projectTenant = new ProjectTenant();
        //                projectTenant.Enabled = true;
        //                projectTenant.ProjectId = input.ProjectId;
        //                projectTenant.SubscriberTenantId = t;
        //                projectTenant.ProjectEnvironmentId = input.ProjectEnvironmentId;
        //                projectTenants.Add(projectTenant);
        //            }
        //        });

        //        projectTenants.ForEach(t => {

        //            ProjectDeployment projectDeployment = new ProjectDeployment();
        //            projectDeployment.TenantId = t.SubscriberTenantId;

        //            // Auto Accept
        //            projectDeployment.ActionType = input.Required ? ProjectDeploymentEnums.ProjectDeploymentActionType.Active : ProjectDeploymentEnums.ProjectDeploymentActionType.New;
        //            projectDeployment.ProjectReleaseId = input.ProjectReleaseId;

        //            // If Deployment has been auto accepted set other deployments to Inactive
        //            if (input.Required)
        //            {
        //                var pds = _projectDeploymentRepository.GetAll().Include(i => i.ProjectReleaseFk).Where(p =>
        //                    p.ProjectReleaseFk.ProjectId == input.ProjectId &&
        //                    p.ProjectReleaseFk.ProjectEnvironmentId == input.ProjectEnvironmentId &&
        //                    p.TenantId == t.SubscriberTenantId &&
        //                    p.ActionType == ProjectDeploymentEnums.ProjectDeploymentActionType.Active &&
        //                    p.Id != projectDeployment.Id
        //                );
        //                pds.ForEach(e => e.ActionType = ProjectDeploymentEnums.ProjectDeploymentActionType.InActive);
        //                _projectDeploymentRepository.GetDbContext().UpdateRange(pds);
        //            }

        //            _projectDeploymentRepository.Insert(projectDeployment);

        //            // Add Deployment Tags
        //            var tagEntities = _tagEntityRepository.GetAll().Where(e => e.EntityId == t.Id).ToList();
        //            tagEntities.ForEach(t => {
        //                var tagEntity = new TagEntity();
        //                tagEntity.Id = Guid.NewGuid();
        //                tagEntity.TenantId = projectDeployment.TenantId;
        //                tagEntity.EntityId = projectDeployment.Id;
        //                tagEntity.TagValueId = t.TagValueId;
        //                tagEntity.EntityType = 0;
        //                tagEntity.CreationTime = DateTime.Now;
        //                _tagEntityRepository.InsertAsync(tagEntity);
        //            });

        //            tenantIds.Add(t.SubscriberTenantId);

        //            // Get the Acls defined in the ProjectTenant Subscription to apply to the new Deployment
        //            UnitOfWorkManager.Current.SetTenantId(t.SubscriberTenantId); // Set the tenantid as disabling tenantfilter stops notifications from working
        //            var acls = _aclRepository.GetAll().Where(e => e.EntityID == t.Id).ToList();
        //            acls.ForEach(a => {

        //                ACL acl = new ACL()
        //                {
        //                    EntityID = projectDeployment.Id,
        //                    UserId = a.UserId,
        //                    OrganizationUnitId = a.OrganizationUnitId,
        //                    Role = a.Role,
        //                    TargetTenantId = a.TargetTenantId,
        //                    TenantId = a.TenantId,
        //                    Type = a.Type
        //                };

        //                _ACLManager.AddACL(acl);
        //            });

        //        });

        //        // Build List of Users to notifiy   
        //        tenantIds.ToList().ForEach(t => {

        //            // Create Notification Here
        //            // Get the Tenant Admins and Tenant Approvers 
        //            UnitOfWorkManager.Current.SetTenantId(t);
        //            var adminRole = _roleManager.GetRoleByName("admin");

        //            UserManager.Users.Where(
        //               u => u.Roles.Any(r => r.RoleId == adminRole.Id)
        //               ).Select(u => new UserIdentifier(t, u.Id)).ToList().ForEach(u => { users.Add(u); });
        //        });

        //        var project = _projectRepository.GetAll().FirstOrDefault(p => p.Id == input.ProjectTemplateId);
        //        var projectName = project == null ? input.Name : project.Name;

        //        UnitOfWorkManager.Current.SetTenantId(AbpSession.TenantId);
        //        using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
        //        {
        //            var message = "A new release of " + projectName + " is available. " + input.Notes;

        //            var u = users.ToArray();
        //              _notificationPublisher.PublishAsync(
        //              AppNotificationNames.SimpleMessage,
        //              new MessageNotificationData(message),
        //              severity: NotificationSeverity.Info,
        //              userIds: u
        //              );
        //        }
        //    UnitOfWorkManager.Current.SaveChangesAsync();
        //}
 
        [AbpAuthorize(AppPermissions.Pages_ProjectReleases_Edit)]
        protected virtual async Task Update(CreateOrEditProjectReleaseDto input)
        {
            var projectRelease = await _projectReleaseRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, projectRelease);

            projectRelease.Notes = input.Notes;

            // TODO DUPLICATED CODE
            if (input.DeployToSubscribers)
            {
                input.ProjectReleaseId = projectRelease.Id;
                //DeployToSubscribers(input);

                List<int> tenantIds = new List<int>();
                List<UserIdentifier> users = new List<UserIdentifier>();


                // Create Deployments using ProjectTenants
                /// Get The Project Tenants from the 
                var projectTenants = _projectTenantRepository.GetAll().Where(i =>
                        i.ProjectId == input.ProjectId &&
                        (i.ProjectEnvironmentId == input.ProjectEnvironmentId || i.ProjectEnvironmentId == null) &&
                        i.Enabled &&
                        (input.ProjectTenants.Contains((int)i.SubscriberTenantId))
                    ).ToList();

                // Add extra Tenants if not in list
                input.ProjectTenants.ForEach(t => {

                    if (!projectTenants.Any(pt => pt.SubscriberTenantId == t))
                    {
                        //Add to collection
                        ProjectTenant projectTenant = new ProjectTenant();
                        projectTenant.Enabled = true;
                        projectTenant.ProjectId = input.ProjectId;
                        projectTenant.SubscriberTenantId = t;
                        projectTenant.ProjectEnvironmentId = input.ProjectEnvironmentId;
                        projectTenants.Add(projectTenant);
                    }
                });

                projectTenants.ForEach(t => {

                    ProjectDeployment projectDeployment = new ProjectDeployment();
                    projectDeployment.TenantId = t.SubscriberTenantId;

                    // Auto Accept
                    projectDeployment.ActionType = input.Required ? ProjectDeploymentEnums.ProjectDeploymentActionType.Active : ProjectDeploymentEnums.ProjectDeploymentActionType.New;
                    projectDeployment.ProjectReleaseId = input.ProjectReleaseId;

                    // If Deployment has been auto accepted set other deployments to Inactive
                    if (input.Required)
                    {
                        var pds = _projectDeploymentRepository.GetAll().Include(i => i.ProjectReleaseFk).Where(p =>
                            p.ProjectReleaseFk.ProjectId == input.ProjectId &&
                            p.ProjectReleaseFk.ProjectEnvironmentId == input.ProjectEnvironmentId &&
                            p.TenantId == t.SubscriberTenantId &&
                            p.ActionType == ProjectDeploymentEnums.ProjectDeploymentActionType.Active &&
                            p.Id != projectDeployment.Id
                        );
                        pds.ForEach(e => e.ActionType = ProjectDeploymentEnums.ProjectDeploymentActionType.InActive);
                        _projectDeploymentRepository.GetDbContext().UpdateRange(pds);
                    }

                    _projectDeploymentRepository.Insert(projectDeployment);

                    // Add Deployment Tags
                    var tagEntities = _tagEntityRepository.GetAll().Where(e => e.EntityId == t.Id).ToList();
                    tagEntities.ForEach(t => {
                        var tagEntity = new TagEntity();
                        tagEntity.Id = Guid.NewGuid();
                        tagEntity.TenantId = projectDeployment.TenantId;
                        tagEntity.EntityId = projectDeployment.Id;
                        tagEntity.TagValueId = t.TagValueId;
                        tagEntity.EntityType = 0;
                        tagEntity.CreationTime = DateTime.Now;
                        _tagEntityRepository.InsertAsync(tagEntity);
                    });

                    tenantIds.Add(t.SubscriberTenantId);

                    // Get the Acls defined in the ProjectTenant Subscription to apply to the new Deployment
                    UnitOfWorkManager.Current.SetTenantId(t.SubscriberTenantId); // Set the tenantid as disabling tenantfilter stops notifications from working
                    var acls = _aclRepository.GetAll().Where(e => e.EntityID == t.Id).ToList();
                    acls.ForEach(a => {

                        ACL acl = new ACL()
                        {
                            EntityID = projectDeployment.Id,
                            UserId = a.UserId,
                            OrganizationUnitId = a.OrganizationUnitId,
                            Role = a.Role,
                            TargetTenantId = a.TargetTenantId,
                            TenantId = a.TenantId,
                            Type = a.Type
                        };

                        _ACLManager.AddACL(acl);
                    });

                });

                // Build List of Users to notifiy   
                tenantIds.ToList().ForEach(t => {

                    // Create Notification Here
                    // Get the Tenant Admins and Tenant Approvers 
                    UnitOfWorkManager.Current.SetTenantId(t);
                    var adminRole = _roleManager.GetRoleByName("admin");

                    UserManager.Users.Where(
                       u => u.Roles.Any(r => r.RoleId == adminRole.Id)
                       ).Select(u => new UserIdentifier(t, u.Id)).ToList().ForEach(u => { users.Add(u); });
                });

                var project = _projectRepository.GetAll().FirstOrDefault(p => p.Id == input.ProjectTemplateId);
                var projectName = project == null ? input.Name : project.Name;

                UnitOfWorkManager.Current.SetTenantId(AbpSession.TenantId);
                using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var message = "A new release of " + projectName + " is available. " + input.Notes;

                    var u = users.ToArray();
                    await _notificationPublisher.PublishAsync(
                    AppNotificationNames.SimpleMessage,
                    new MessageNotificationData(message),
                    severity: NotificationSeverity.Info,
                    userIds: u
                    );
                }

            }

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectReleases_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            if (_projectDeploymentRepository.GetAll().Any(pd => pd.ProjectReleaseId == input.Id))
            {
                throw new UserFriendlyException("Cannot delete Release that has Deployments");
            }

            _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant);
            await _projectReleaseRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetProjectReleasesToExcel(GetAllProjectReleasesForExcelInput input)
        {
            var releaseTypeFilter = input.ReleaseTypeFilter.HasValue
                        ? (ProjectReleaseEnums.ProjectReleaseType)input.ReleaseTypeFilter
                        : default;

            var filteredProjectReleases = _projectReleaseRepository.GetAll()
                        .Include(e => e.ProjectEnvironmentFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Notes.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NotesFilter), e => e.Notes == input.NotesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectIdFilter.ToString()), e => e.ProjectId.ToString() == input.ProjectIdFilter.ToString())
                        .WhereIf(input.RequiredFilter.HasValue && input.RequiredFilter > -1, e => (input.RequiredFilter == 1 && e.Required) || (input.RequiredFilter == 0 && !e.Required))
                        .WhereIf(input.MinVersionMajorFilter != null, e => e.VersionMajor >= input.MinVersionMajorFilter)
                        .WhereIf(input.MaxVersionMajorFilter != null, e => e.VersionMajor <= input.MaxVersionMajorFilter)
                        .WhereIf(input.MinVersionMinorFilter != null, e => e.VersionMinor >= input.MinVersionMinorFilter)
                        .WhereIf(input.MaxVersionMinorFilter != null, e => e.VersionMinor <= input.MaxVersionMinorFilter)
                        .WhereIf(input.MinVersionRevisionFilter != null, e => e.VersionRevision >= input.MinVersionRevisionFilter)
                        .WhereIf(input.MaxVersionRevisionFilter != null, e => e.VersionRevision <= input.MaxVersionRevisionFilter)
                        .WhereIf(input.ReleaseTypeFilter.HasValue && input.ReleaseTypeFilter > -1, e => e.ReleaseType == releaseTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectEnvironmentNameFilter), e => e.ProjectEnvironmentFk != null && e.ProjectEnvironmentFk.Name == input.ProjectEnvironmentNameFilter);

            var query = (from o in filteredProjectReleases
                         join o1 in _lookup_projectEnvironmentRepository.GetAll() on o.ProjectEnvironmentId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetProjectReleaseForViewDto()
                         {
                             ProjectRelease = new ProjectReleaseDto
                             {
                                 Name = o.Name,
                                 Notes = o.Notes,
                                 ProjectId = o.ProjectId,
                                 Required = o.Required,
                                 VersionMajor = o.VersionMajor,
                                 VersionMinor = o.VersionMinor,
                                 VersionRevision = o.VersionRevision,
                                 ReleaseType = o.ReleaseType,
                                 Id = o.Id
                             },
                             ProjectEnvironmentName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                         });

            var projectReleaseListDtos = await query.ToListAsync();

            return _projectReleasesExcelExporter.ExportToFile(projectReleaseListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectReleases)]
        public async Task<PagedResultDto<ProjectReleaseProjectEnvironmentLookupTableDto>> GetAllProjectEnvironmentForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_projectEnvironmentRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var projectEnvironmentList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ProjectReleaseProjectEnvironmentLookupTableDto>();
            foreach (var projectEnvironment in projectEnvironmentList)
            {
                lookupTableDtoList.Add(new ProjectReleaseProjectEnvironmentLookupTableDto
                {
                    Id = projectEnvironment.Id.ToString(),
                    DisplayName = projectEnvironment.Name?.ToString()
                });
            }

            return new PagedResultDto<ProjectReleaseProjectEnvironmentLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}