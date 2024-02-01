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
using static Syntaq.Falcon.Projects.ProjectConsts;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using NPOI.SS.Formula.Functions;
using static Syntaq.Falcon.Projects.ProjectDeploymentEnums;
using Abp.EntityFrameworkCore.EFPlus;

namespace Syntaq.Falcon.Projects
{
    [AbpAuthorize(AppPermissions.Pages_ProjectDeployments)]
    public class ProjectDeploymentsAppService : FalconAppServiceBase, IProjectDeploymentsAppService
    {
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<ProjectDeployment, Guid> _projectDeploymentRepository;
        private readonly IProjectDeploymentsExcelExporter _projectDeploymentsExcelExporter;
        private readonly IRepository<ProjectRelease, Guid> _lookup_projectReleaseRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProjectDeploymentsAppService(
            IRepository<Project, Guid> projectRepository,
            IRepository<ProjectDeployment, Guid> projectDeploymentRepository, 
            IProjectDeploymentsExcelExporter projectDeploymentsExcelExporter, 
            IRepository<ProjectRelease, Guid> lookup_projectReleaseRepository,
            IUnitOfWorkManager unitOfWorkManager
        )
        {
            _projectRepository = projectRepository;
            _projectDeploymentRepository = projectDeploymentRepository;
            _projectDeploymentsExcelExporter = projectDeploymentsExcelExporter;
            _lookup_projectReleaseRepository = lookup_projectReleaseRepository;

            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task<PagedResultDto<GetProjectDeploymentForViewDto>> GetAll(GetAllProjectDeploymentsInput input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var actionTypeFilter = input.ActionTypeFilter.HasValue
                        ? (ProjectDeploymentEnums.ProjectDeploymentActionType)input.ActionTypeFilter
                        : default;

            var filteredProjectDeployments = _projectDeploymentRepository.GetAll()
                        .Where(d => d.TenantId == AbpSession.TenantId)
                        .Include(e => e.ProjectReleaseFk)
                        .WhereIf(input.ReleaseId.HasValue, e => e.ProjectReleaseFk.Id == input.ReleaseId)
                        .WhereIf(input.ProjectId.HasValue, e => e.ProjectReleaseFk.ProjectId == input.ProjectId)
                        .WhereIf(input.EnvironmentId.HasValue, e => e.ProjectReleaseFk.ProjectEnvironmentId == input.EnvironmentId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Comments.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CommentsFilter), e => e.Comments == input.CommentsFilter)
                        .WhereIf(input.ActionTypeFilter.HasValue && input.ActionTypeFilter > -1, e => e.ActionType == actionTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectReleaseNameFilter), e => e.ProjectReleaseFk != null && e.ProjectReleaseFk.Name == input.ProjectReleaseNameFilter);


            if (input.Sorting != null)
            {
                if ((bool)(input.Sorting?.StartsWith("ProjectReleaseFk.VersionMajor")))
                {
                    var type = input.Sorting.EndsWith("asc") ? "asc" : "desc";
                    input.Sorting = input.Sorting.Replace(input.Sorting, $"ProjectReleaseFk.VersionMajor {type}, ProjectReleaseFk.VersionMinor {type}, ProjectReleaseFk.VersionRevision {type}");
                }
            }



            var pagedAndFilteredProjectDeployments = filteredProjectDeployments
                .OrderBy(input.Sorting ?? "CreationTime desc")
                .PageBy(input);

            var projectDeployments = from o in pagedAndFilteredProjectDeployments
                                     join o1 in _lookup_projectReleaseRepository.GetAll() on o.ProjectReleaseId equals o1.Id into j1
                                     from s1 in j1.DefaultIfEmpty()

                                     select new
                                     {
                                         o.ProjectReleaseFk,
                                         o.ProjectReleaseFk.ProjectId,
                                         o.Comments,
                                         o.ActionType,
                                         Id = o.Id,
                                         o.Enabled,
                                         ProjectReleaseId = o.ProjectReleaseId,
                                         ProjectReleaseName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                     };

            var totalCount = await filteredProjectDeployments.CountAsync();

            var dbList = await projectDeployments.ToListAsync();
            var results = new List<GetProjectDeploymentForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetProjectDeploymentForViewDto()
                {
                    ProjectDeployment = new ProjectDeploymentDto
                    {

                        Comments = o.Comments,
                        ActionType = o.ActionType,
                        Enabled = o.Enabled,
                        Id = o.Id,
                        ProjectRelease = new ProjectReleaseDto()
                        {
                            Id = o.ProjectReleaseFk.Id,
                            ProjectId = o.ProjectReleaseFk.ProjectId,
                            CreationTime = o.ProjectReleaseFk.CreationTime,
                            Name = o.ProjectReleaseFk.Name,
                            Notes = o.ProjectReleaseFk.Notes,
                            ProjectEnvironmentId = o.ProjectReleaseFk.ProjectEnvironmentId,
                            ReleaseType = o.ProjectReleaseFk.ReleaseType ,
                            VersionMajor = o.ProjectReleaseFk.VersionMajor,
                            VersionMinor = o.ProjectReleaseFk.VersionMinor,
                            VersionRevision = o.ProjectReleaseFk.VersionRevision,
                        }
                    },
                    ProjectId = o.ProjectId,
                    ProjectReleaseId = (Guid)o.ProjectReleaseId,
                    ProjectReleaseName = o.ProjectReleaseName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetProjectDeploymentForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<PagedResultDto<GetProjectDeploymentForViewDto>> GetAllForRelease(GetAllProjectDeploymentsInput input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var actionTypeFilter = input.ActionTypeFilter.HasValue
                        ? (ProjectDeploymentEnums.ProjectDeploymentActionType)input.ActionTypeFilter
                        : default;

            var filteredProjectDeployments = _projectDeploymentRepository.GetAll()
                        //.Where(d => d.TenantId == AbpSession.TenantId)
                        .Include(e => e.ProjectReleaseFk)
                        .Where(e => e.ProjectReleaseFk.Id == input.ReleaseId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Comments.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CommentsFilter), e => e.Comments == input.CommentsFilter)
                        .WhereIf(input.ActionTypeFilter.HasValue && input.ActionTypeFilter > -1, e => e.ActionType == actionTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectReleaseNameFilter), e => e.ProjectReleaseFk != null && e.ProjectReleaseFk.Name == input.ProjectReleaseNameFilter);

            // Old Code
            var pagedAndFilteredProjectDeployments = filteredProjectDeployments;
                //.OrderBy(input.Sorting ?? "id asc")
                //.PageBy(input);

            var projectDeployments = from o in pagedAndFilteredProjectDeployments
                                     join o1 in _lookup_projectReleaseRepository.GetAll() on o.ProjectReleaseId equals o1.Id into j1
                                     from s1 in j1.DefaultIfEmpty()

                                     select new
                                     {
                                         o.TenantId,
                                         o.ProjectReleaseFk,
                                         o.ProjectReleaseFk.ProjectId,
                                         o.Comments,
                                         o.ActionType,
                                         o.Enabled,
                                         Id = o.Id,
                                         o.CreationTime,
                                         ProjectReleaseId = o.ProjectReleaseId,
                                         ProjectReleaseName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                     };

            var totalCount = await filteredProjectDeployments.CountAsync();

            var dbList = await projectDeployments.ToListAsync();
            var results = new List<GetProjectDeploymentForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetProjectDeploymentForViewDto()
                {
                    ProjectDeployment = new ProjectDeploymentDto
                    {

                        Comments = o.Comments,
                        ActionType = o.ActionType,
                        Id = o.Id,
                        Enabled = o.Enabled,
                        CreationTime = o.CreationTime,
                        ProjectRelease = new ProjectReleaseDto()
                        {
                            Id = o.ProjectReleaseFk.Id,
                            ProjectId = o.ProjectReleaseFk.ProjectId,
                            CreationTime = o.ProjectReleaseFk.CreationTime,
                            Name = o.ProjectReleaseFk.Name,
                            Notes = o.ProjectReleaseFk.Notes,
                            ProjectEnvironmentId = o.ProjectReleaseFk.ProjectEnvironmentId,
                            ReleaseType = o.ProjectReleaseFk.ReleaseType,
                            VersionMajor = o.ProjectReleaseFk.VersionMajor,
                            VersionMinor = o.ProjectReleaseFk.VersionMinor,
                            VersionRevision = o.ProjectReleaseFk.VersionRevision,
                        }
                    },
                    ProjectId = o.ProjectId,
                    ProjectReleaseId = (Guid)o.ProjectReleaseId,
                    ProjectReleaseName = o.ProjectReleaseName,
                    TenantName = o.TenantId == null? String.Empty :  TenantManager.GetById((int)o.TenantId)?.TenancyName,
                    TenantId = o.TenantId == null ?  null : (int)o.TenantId
                };

                results.Add(res);
            }

            if (input.Sorting == "tenantName asc")
            {
                results = results.OrderBy(r => r.TenantName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "tenantName desc")
            {
                results = results.OrderByDescending(r => r.TenantName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "actionType asc")
            {
                results = results.OrderBy(r => r.ProjectDeployment.ActionType)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "actionType desc")
            {
                results = results.OrderByDescending(r => r.ProjectDeployment.ActionType)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "enabled asc")
            {
                results = results.OrderBy(r => r.ProjectDeployment.Enabled)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "enabled desc")
            {
                results = results.OrderByDescending(r => r.ProjectDeployment.Enabled)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else
            {
                results = results.OrderBy(r => r.ProjectName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }


            return new PagedResultDto<GetProjectDeploymentForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<PagedResultDto<GetProjectDeploymentForViewDto>> GetAllByProject(GetAllProjectDeploymentsInput input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var deployedProjectIds = _projectDeploymentRepository.GetAll()
                    .Where(d => d.TenantId == AbpSession.TenantId)
                    .Include(e => e.ProjectReleaseFk)
                    .Select(pd => pd.ProjectReleaseFk.ProjectId);


            var deployedProjectIdse = _projectDeploymentRepository.GetAll();

            var projectTemplateDeployments = _projectRepository.GetAll()
               .Where(i => deployedProjectIds.Contains(i.Id))
               .Where(i => i.Type == ProjectType.Template)
               .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter));

            var pagedAndFilteredProjectTemplateDeployments = projectTemplateDeployments
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input).ToList();

 
            var totalCount = pagedAndFilteredProjectTemplateDeployments.Count();

            var results = new List<GetProjectDeploymentForViewDto>();

            foreach (var o in pagedAndFilteredProjectTemplateDeployments)
            {
                var res = new GetProjectDeploymentForViewDto()
                {
                    ProjectDeployment = new ProjectDeploymentDto
                    {

                        Comments = o.Description,
                        ActionType =  ProjectDeploymentEnums.ProjectDeploymentActionType.Active,
                        Id = o.Id,
                    },
                    ProjectId = (Guid)o.ProjectId,
                    //ProjectReleaseId = (Guid)o.ReleaseId,
                    CreatedDateTime    = o.CreationTime,  
                    ProjectName = o.Name
                };

                results.Add(res);
            }


            //var actionTypeFilter = input.ActionTypeFilter.HasValue
            //            ? (ProjectDeploymentEnums.ProjectDeploymentActionType)input.ActionTypeFilter
            //            : default;

            //var filteredProjectDeployments = _projectDeploymentRepository.GetAll()
            //            .Include(e => e.ProjectReleaseFk)
            //            .Where(i => deployedProjectIds.Contains(i.ProjectReleaseFk.ProjectId))
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Comments.Contains(input.Filter))
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.CommentsFilter), e => e.Comments == input.CommentsFilter)
            //            .WhereIf(input.ActionTypeFilter.HasValue && input.ActionTypeFilter > -1, e => e.ActionType == actionTypeFilter)
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectReleaseNameFilter), e => e.ProjectReleaseFk != null && e.ProjectReleaseFk.Name == input.ProjectReleaseNameFilter);

            //var pagedAndFilteredProjectDeployments = filteredProjectDeployments
            //    .OrderBy(input.Sorting ?? "id asc")
            //    .PageBy(input);

            //var projectDeployments = from o in pagedAndFilteredProjectDeployments
            //                         join o1 in _lookup_projectReleaseRepository.GetAll() on o.ProjectReleaseId equals o1.Id into j1
            //                         from s1 in j1.DefaultIfEmpty()

            //                         select new
            //                         {
            //                             o.ProjectReleaseFk.ProjectId,
            //                             o.Comments,
            //                             o.ActionType,
            //                             Id = o.Id,
            //                             ProjectReleaseId = o.ProjectReleaseId,
            //                             ProjectReleaseName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
            //                         };

            //var totalCount = await filteredProjectDeployments.CountAsync();

            //var dbList = await projectDeployments.ToListAsync();
            //var results = new List<GetProjectDeploymentForViewDto>();

            //foreach (var o in dbList)
            //{
            //    var res = new GetProjectDeploymentForViewDto()
            //    {
            //        ProjectDeployment = new ProjectDeploymentDto
            //        {

            //            Comments = o.Comments,
            //            ActionType = o.ActionType,
            //            Id = o.Id,
            //        },
            //        ProjectId = o.ProjectId,
            //        ProjectReleaseId = (Guid)o.ProjectReleaseId,
            //        ProjectReleaseName = o.ProjectReleaseName
            //    };

            //    results.Add(res);
            //}

            return new PagedResultDto<GetProjectDeploymentForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetProjectDeploymentForViewDto> GetProjectDeploymentForView(Guid id)
        {
            var projectDeployment = await _projectDeploymentRepository.GetAsync(id);

            var output = new GetProjectDeploymentForViewDto { ProjectDeployment = ObjectMapper.Map<ProjectDeploymentDto>(projectDeployment) };

            if (output.ProjectDeployment.ProjectReleaseId != null)
            {
                var _lookupProjectRelease = await _lookup_projectReleaseRepository.FirstOrDefaultAsync((Guid)output.ProjectDeployment.ProjectReleaseId);
                output.ProjectReleaseName = _lookupProjectRelease?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectDeployments_Edit)]
        public async Task<GetProjectDeploymentForEditOutput> GetProjectDeploymentForEdit(EntityDto<Guid> input)
        {
            // Deployments can be related to Releases held in other Tenancies
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var projectDeployment = await _projectDeploymentRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetProjectDeploymentForEditOutput { ProjectDeployment = ObjectMapper.Map<CreateOrEditProjectDeploymentDto>(projectDeployment) };

            if (output.ProjectDeployment.ProjectReleaseId != null)
            {
                var _lookupProjectRelease = await _lookup_projectReleaseRepository.FirstOrDefaultAsync((Guid)output.ProjectDeployment.ProjectReleaseId);
                output.ProjectReleaseName = _lookupProjectRelease?.Name?.ToString();
                output.ProjectReleaseNotes = _lookupProjectRelease?.Notes?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditProjectDeploymentDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectDeployments_Create)]
        protected virtual async Task Create(CreateOrEditProjectDeploymentDto input)
        {

            var projectDeployment = ObjectMapper.Map<ProjectDeployment>(input);

            if (string.IsNullOrEmpty(input.TenantName))
            {
                if (AbpSession.TenantId != null)
                {
                    projectDeployment.TenantId = (int?)AbpSession.TenantId;
                }
            }
            else
            {
                var tid =  TenantManager.GetTenantId(input.TenantName).Result;
                projectDeployment.TenantId = (int?)tid;
            }

            if (projectDeployment.TenantId == null)
            {
                throw new UserFriendlyException("Tenant not Found");
            }

            if (_projectDeploymentRepository.GetAll().Any(d => d.ProjectReleaseId == input.ProjectReleaseId && d.TenantId == projectDeployment.TenantId))
            {
                throw new UserFriendlyException("Deployment alreay exists for Tenant");
            }
            else
            {
                await _projectDeploymentRepository.InsertAsync(projectDeployment);
            }



        }

        [AbpAuthorize(AppPermissions.Pages_ProjectDeployments_Edit)]
        protected virtual async Task Update(CreateOrEditProjectDeploymentDto input)
        {


            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            var projectDeployment =  _projectDeploymentRepository.GetAll().Include(p => p.ProjectReleaseFk).Where(p => p.Id == (Guid)input.Id).FirstOrDefault() ;


            if (projectDeployment.ActionType != ProjectDeploymentActionType.New &&  input.ActionType == ProjectDeploymentActionType.New)
            {
                throw new UserFriendlyException("Cannot set a Deployment to 'New'");
            }

            ObjectMapper.Map(input, projectDeployment);


            // If Deployment has been auto accepted set other deployments to Inactive
            if (input.ActionType == ProjectDeploymentEnums.ProjectDeploymentActionType.Active)
            {
                var pds = _projectDeploymentRepository.GetAll().Include(i => i.ProjectReleaseFk).Where(p =>
                    p.ProjectReleaseFk.ProjectId == projectDeployment.ProjectReleaseFk.ProjectId &&
                    p.ProjectReleaseFk.ProjectEnvironmentId == projectDeployment.ProjectReleaseFk.ProjectEnvironmentId &&
                     p.TenantId == AbpSession.TenantId &&
                    p.ActionType == ProjectDeploymentEnums.ProjectDeploymentActionType.Active &&
                    p.Id != projectDeployment.Id
                ).ToList();

                pds.ForEach(e => e.ActionType = ProjectDeploymentEnums.ProjectDeploymentActionType.InActive);
                _projectDeploymentRepository.GetDbContext().UpdateRange(pds);

            }

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectDeployments_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {

            var d = _projectDeploymentRepository.GetAll().FirstOrDefault(d => d.Id == input.Id);
            if(d != null)
            {
                if (_projectRepository.GetAll().Any(p => p.ReleaseId == d.ProjectReleaseId))
                {
                    throw new UserFriendlyException(L("Cannot delete. This Deployment has active projects."));
                }
            }

            await _projectDeploymentRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectDeployments_Edit)]
        public async Task SetType(Guid Id, int type)
        {

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {

                var projectDeployment = _projectDeploymentRepository.GetAll().Include(p => p.ProjectReleaseFk).Where(p => p.Id == Id).FirstOrDefault();


                // Do not update if no change to Action Type
                if(projectDeployment.ActionType == (ProjectDeploymentEnums.ProjectDeploymentActionType)type)
                {
                    return;
                }

                if (_projectDeploymentRepository.GetAll().Include(i => i.ProjectReleaseFk).Any(p => 
                    p.ProjectReleaseFk.ProjectEnvironmentId == projectDeployment.ProjectReleaseFk.ProjectEnvironmentId && 
                    p.ProjectReleaseFk.ProjectId == projectDeployment.ProjectReleaseFk.ProjectId && 
                    p.TenantId == projectDeployment.TenantId && p.CreationTime > projectDeployment.CreationTime))
                {
                    throw new UserFriendlyException("You cannot make an older Deployment active.");
                }

                if (type == 0)
                {
                    throw new UserFriendlyException("You cannot set a Deployment to 'New'.");
                }

                if((ProjectDeploymentActionType)type == ProjectDeploymentActionType.Active)
                {
                    var pds = _projectDeploymentRepository.GetAll().Include(i => i.ProjectReleaseFk).Where(p => 
                        p.ProjectReleaseFk.ProjectEnvironmentId == projectDeployment.ProjectReleaseFk.ProjectEnvironmentId && 
                        p.ProjectReleaseFk.ProjectId == projectDeployment.ProjectReleaseFk.ProjectId &&
                        p.TenantId == projectDeployment.TenantId).ToList();
                    foreach (ProjectDeployment pd in pds)
                    {
                        pd.ActionType = ProjectDeploymentActionType.InActive;
                    }
                }
                _unitOfWorkManager.Current.SaveChanges();

                projectDeployment.ActionType = (ProjectDeploymentActionType)type;
                await _projectDeploymentRepository.UpdateAsync(projectDeployment);

            }

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectDeployments_Edit)]
        public async Task SetEnabled(Guid Id, bool enabled)
        {
            Guid releaseId = Guid.Empty;
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var pd = _projectDeploymentRepository.GetAll().Include(p => p.ProjectReleaseFk).Where(p => p.Id == Id).FirstOrDefault();

                if (pd.ProjectReleaseFk.TenantId == AbpSession.TenantId)
                {
                    pd.Enabled = enabled;
                    await _projectDeploymentRepository.UpdateAsync(pd);
                }
                else
                {
                    throw new UserFriendlyException(L("Permission Denied."));
                }

            }

        }

        public async Task<FileDto> GetProjectDeploymentsToExcel(GetAllProjectDeploymentsForExcelInput input)
        {
            var actionTypeFilter = input.ActionTypeFilter.HasValue
                        ? (ProjectDeploymentEnums.ProjectDeploymentActionType)input.ActionTypeFilter
                        : default;

            var filteredProjectDeployments = _projectDeploymentRepository.GetAll()
                        .Include(e => e.ProjectReleaseFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Comments.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CommentsFilter), e => e.Comments == input.CommentsFilter)
                        .WhereIf(input.ActionTypeFilter.HasValue && input.ActionTypeFilter > -1, e => e.ActionType == actionTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectReleaseNameFilter), e => e.ProjectReleaseFk != null && e.ProjectReleaseFk.Name == input.ProjectReleaseNameFilter);

            var query = (from o in filteredProjectDeployments
                         join o1 in _lookup_projectReleaseRepository.GetAll() on o.ProjectReleaseId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetProjectDeploymentForViewDto()
                         {
                             ProjectDeployment = new ProjectDeploymentDto
                             {
                                 Comments = o.Comments,
                                 ActionType = o.ActionType,
                                 Id = o.Id
                             },
                             ProjectReleaseName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                         });

            var projectDeploymentListDtos = await query.ToListAsync();

            return _projectDeploymentsExcelExporter.ExportToFile(projectDeploymentListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectDeployments)]
        public async Task<PagedResultDto<ProjectDeploymentProjectReleaseLookupTableDto>> GetAllProjectReleaseForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_projectReleaseRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var projectReleaseList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ProjectDeploymentProjectReleaseLookupTableDto>();
            foreach (var projectRelease in projectReleaseList)
            {
                lookupTableDtoList.Add(new ProjectDeploymentProjectReleaseLookupTableDto
                {
                    Id = projectRelease.Id.ToString(),
                    DisplayName = projectRelease.Name?.ToString()
                });
            }

            return new PagedResultDto<ProjectDeploymentProjectReleaseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}