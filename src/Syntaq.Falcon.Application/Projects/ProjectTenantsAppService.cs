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
using Abp.Domain.Uow;
using NUglify.JavaScript.Syntax;
using Syntaq.Falcon.ASIC;

namespace Syntaq.Falcon.Projects
{
    [AbpAuthorize(AppPermissions.Pages_ProjectTenants)]
    public class ProjectTenantsAppService : FalconAppServiceBase, IProjectTenantsAppService
    {
        private readonly IRepository<ProjectTenant, Guid> _projectTenantRepository;
        private readonly IRepository<ProjectDeployment, Guid> _projectDeploymentRepository;
        private readonly IProjectTenantsExcelExporter _projectTenantsExcelExporter;
        private readonly IRepository<ProjectEnvironment, Guid> _lookup_projectEnvironmentRepository;
        private readonly IRepository<Project, Guid> _lookup_projectRepository;


        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProjectTenantsAppService(

            IRepository<ProjectTenant, Guid> projectTenantRepository,
            IRepository<ProjectDeployment, Guid> projectDeploymentRepository,
            IProjectTenantsExcelExporter projectTenantsExcelExporter, 
            IRepository<ProjectEnvironment, Guid> lookup_projectEnvironmentRepository,
            IRepository<Project, Guid> lookup_projectRepository,
            IUnitOfWorkManager unitOfWorkManager
        )
        {
            _projectTenantRepository = projectTenantRepository;
            _projectDeploymentRepository = projectDeploymentRepository;
            _projectTenantsExcelExporter = projectTenantsExcelExporter;
            _lookup_projectEnvironmentRepository = lookup_projectEnvironmentRepository;
            _lookup_projectRepository = lookup_projectRepository;

            _unitOfWorkManager = unitOfWorkManager;

        }


        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Edit)]
        public async Task<PagedResultDto<GetProjectTenantForViewDto>> GetAll(GetAllProjectTenantsInput input)
        {

            var filteredProjectTenants = _projectTenantRepository.GetAll()
                        .Include(e => e.ProjectEnvironmentFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinSubscriberTenantIdFilter != null, e => e.SubscriberTenantId >= input.MinSubscriberTenantIdFilter)
                        .WhereIf(input.MaxSubscriberTenantIdFilter != null, e => e.SubscriberTenantId <= input.MaxSubscriberTenantIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectIdFilter.ToString()), e => e.ProjectId.ToString() == input.ProjectIdFilter.ToString())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IdFilter.ToString()), e => e.Id.ToString() == input.IdFilter.ToString())
                        .WhereIf(input.EnabledFilter.HasValue && input.EnabledFilter > -1, e => (input.EnabledFilter == 1 && e.Enabled) || (input.EnabledFilter == 0 && !e.Enabled))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectEnvironmentNameFilter), e => e.ProjectEnvironmentFk != null && e.ProjectEnvironmentFk.Name == input.ProjectEnvironmentNameFilter);

            var pagedAndFilteredProjectTenants = filteredProjectTenants;
                //.OrderBy(input.Sorting ?? "id asc")
                //.PageBy(input);

            var projectTenants = from o in pagedAndFilteredProjectTenants
                                 join o1 in _lookup_projectEnvironmentRepository.GetAll() on o.ProjectEnvironmentId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 select new
                                 {

                                     o.SubscriberTenantId,
                                     o.ProjectId,
                                     o.Enabled,
                                     Id = o.Id,
                                     ProjectEnvironmentName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                 };

            var totalCount = await filteredProjectTenants.CountAsync();

            var dbList = await projectTenants.ToListAsync();
            var results = new List<GetProjectTenantForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetProjectTenantForViewDto()
                {
                    ProjectTenant = new ProjectTenantDto
                    {

                        SubscriberTenantId = o.SubscriberTenantId,
                        SubscriberTenantName = TenantManager.GetById(o.SubscriberTenantId).TenancyName,
                        ProjectId = o.ProjectId,
                        Enabled = o.Enabled,
                        Id = o.Id,
                    },
                    ProjectEnvironmentName = o.ProjectEnvironmentName
                };

                results.Add(res);
            } 

             if (input.Sorting == "Tenant Name asc" || input.Sorting == "subscriberTenantName asc")
            {
                results = results.OrderBy(r => r.ProjectTenant.SubscriberTenantName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "Tenant Name desc" || input.Sorting == "subscriberTenantName desc")
            {
                results = results.OrderByDescending(r => r.ProjectTenant.SubscriberTenantName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "projectEnvironmentFk.name asc")
            {
                results = results.OrderBy(r => r.ProjectEnvironmentName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "projectEnvironmentFk.name desc")
            {
                results = results.OrderByDescending(r => r.ProjectEnvironmentName)
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

            return new PagedResultDto<GetProjectTenantForViewDto>(
                totalCount,
                results
            );

        }


        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Edit)]
        public async Task<PagedResultDto<GetProjectTenantForViewDto>> GetAllForTenant(GetAllProjectTenantsInput input)
        {

            // 
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var filteredProjectTenants = _projectTenantRepository.GetAll()
                        .Include(e => e.ProjectEnvironmentFk)
                        .Where(e => e.SubscriberTenantId == AbpSession.TenantId)
                        //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectIdFilter.ToString()), e => e.ProjectId.ToString() == input.ProjectIdFilter.ToString())
                        .WhereIf(input.EnabledFilter.HasValue && input.EnabledFilter > -1, e => (input.EnabledFilter == 1 && e.Enabled) || (input.EnabledFilter == 0 && !e.Enabled))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectEnvironmentNameFilter), e => e.ProjectEnvironmentFk != null && e.ProjectEnvironmentFk.Name == input.ProjectEnvironmentNameFilter);

            //.Join(
            //       _lookup_projectRepository.GetAll(),
            //       pt => pt.ProjectId,
            //       p => p.Id,
            //       (pt, p) => new
            //       {
            //           pt.SubscriberTenantId,
            //           pt.ProjectId,
            //           pt.Enabled,
            //           pt.ProjectEnvironmentId,
            //           Id = pt.Id,
            //           ProjectEnvironmentName = pt.ProjectEnvironmentFk == null || pt.ProjectEnvironmentFk.Name == null ? "" : pt.ProjectEnvironmentFk.Name.ToString(),
            //           ProjectName = p == null || p.Name == null ? "" : p.Name.ToString(),
            //           ProjectDescription = p == null || p.Description == null ? "" : p.Description.ToString(),
            //       }
            //   );

 
            var pagedAndFilteredProjectTenants = filteredProjectTenants;
                //.OrderBy(input.Sorting ?? "id asc")
                //.PageBy(input);

            var projectTenants = (from o in pagedAndFilteredProjectTenants
                                 join o1 in _lookup_projectEnvironmentRepository.GetAll() on o.ProjectEnvironmentId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 join o2 in _lookup_projectRepository.GetAll() on o.ProjectId equals o2.Id into j2
                                 from s2 in j2.DefaultIfEmpty()

                                 where string.IsNullOrEmpty(input.Filter) || (s2 != null && s2.Name.ToLower().Contains(input.Filter))

                                  select new
                                 {
                                     o.SubscriberTenantId,
                                     o.ProjectId,
                                     o.Enabled,
                                     Id = o.Id,
                                     ProjectEnvironmentName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                                     ProjectEnvironmentId = o.ProjectEnvironmentId,
                                     ProjectName = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                                     ProjectDescription= s2 == null || s2.Description == null ? "" : s2.Description.ToString(),
                                 });

 
            var totalCount = await filteredProjectTenants.CountAsync();

            var dbList = await projectTenants.ToListAsync(); 
            var results = new List<GetProjectTenantForViewDto>();
            foreach (var o in dbList)
            {
                var res = new GetProjectTenantForViewDto()
                {
                    ProjectTenant = new ProjectTenantDto
                    {

                        SubscriberTenantId = o.SubscriberTenantId,
                        SubscriberTenantName = TenantManager.GetById(o.SubscriberTenantId).TenancyName,
                        ProjectId = o.ProjectId,
                        ProjectEnvironmentId= o.ProjectEnvironmentId,
                        Enabled = o.Enabled,
                        Id = o.Id,
                    },
                    ProjectEnvironmentName = o.ProjectEnvironmentName,
                    ProjectName = o.ProjectName,
                    ProjectDescription= o.ProjectDescription,
                };

                results.Add(res);
            }

            if (input.Sorting == "ProjectName asc")
            {
                results = results.OrderBy(r => r.ProjectName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "ProjectName desc") {
                results = results.OrderByDescending(r => r.ProjectName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            }
            else if (input.Sorting == "projectEnvironmentFk.name asc")
            {
                results = results.OrderBy(r => r.ProjectEnvironmentName)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList(); 
            }
            else if (input.Sorting == "projectEnvironmentFk.name desc")
            {
                results = results.OrderByDescending(r => r.ProjectEnvironmentName)
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



            return new PagedResultDto<GetProjectTenantForViewDto>(
                totalCount,
                results
            );

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Edit)]
        public async Task<GetProjectTenantForViewDto> GetProjectTenantForView(Guid id)
        {
            var projectTenant = await _projectTenantRepository.GetAsync(id);

            var output = new GetProjectTenantForViewDto { ProjectTenant = ObjectMapper.Map<ProjectTenantDto>(projectTenant) };

            if (output.ProjectTenant.ProjectEnvironmentId != null)
            {
                var _lookupProjectEnvironment = await _lookup_projectEnvironmentRepository.FirstOrDefaultAsync((Guid)output.ProjectTenant.ProjectEnvironmentId);
                output.ProjectEnvironmentName = _lookupProjectEnvironment?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Edit)]
        public async Task<GetProjectTenantForEditOutput> GetProjectTenantForEdit(EntityDto<Guid> input)
        {
            var projectTenant = await _projectTenantRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetProjectTenantForEditOutput { ProjectTenant = ObjectMapper.Map<CreateOrEditProjectTenantDto>(projectTenant) };

            var tenant = TenantManager.GetById(projectTenant.SubscriberTenantId);

            output.ProjectTenant.SubscriberTenantName = tenant.TenancyName;

            if (output.ProjectTenant.ProjectEnvironmentId != null)
            {
                var _lookupProjectEnvironment = await _lookup_projectEnvironmentRepository.FirstOrDefaultAsync((Guid)output.ProjectTenant.ProjectEnvironmentId);
                output.ProjectEnvironmentName = _lookupProjectEnvironment?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditProjectTenantDto input)
        {

            foreach (string tname in input.SubscriberTenantName.Split(','))
            {

                var tenant = TenantManager.FindByTenancyName(tname.Trim());

                if (tenant != null)
                {


                    var pt = _projectTenantRepository.GetAll().FirstOrDefault(i => i.Id == input.Id);
                    //var pt = _projectTenantRepository.GetAll().FirstOrDefault(i => i.SubscriberTenantId == tenant.Id && i.ProjectId == input.ProjectId && i.ProjectEnvironmentId == input.ProjectEnvironmentId);

                    //if (_projectTenantRepository.GetAll().Any(i => i.SubscriberTenantId == tenant.Id && i.ProjectId == input.ProjectId && i.ProjectEnvironmentId == input.ProjectEnvironmentId))
                    //{
                    //    throw new UserFriendlyException("Tenant is already Subscribed to this Project");
                    //}

                    input.SubscriberTenantId = tenant.Id;
                    if (input.Id == null && pt == null)
                    {
                        await Create(input);
                    }
                    else
                    {
                        //input.Id = pt.Id;

                        await Update(input);
                    }
                }

            }

                // Assign by Tenant Name

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Create)]
        protected virtual async Task Create(CreateOrEditProjectTenantDto input)
        {
            var projectTenant = ObjectMapper.Map<ProjectTenant>(input);

            if (AbpSession.TenantId != null)
            {
                projectTenant.TenantId = (int?)AbpSession.TenantId;
            }

            if (_projectTenantRepository.GetAll().Any(t => t.ProjectId == input.ProjectId && t.SubscriberTenantId == input.SubscriberTenantId && t.ProjectEnvironmentId == input.ProjectEnvironmentId))
            {
                throw new UserFriendlyException("Tenant has already been added for this Environment.");
            }
            else
            {
                await _projectTenantRepository.InsertAsync(projectTenant);
            }



        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Edit)]
        protected virtual async Task Update(CreateOrEditProjectTenantDto input)
        {
            var projectTenant = await _projectTenantRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, projectTenant);

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTenants_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var projectTenant = _projectTenantRepository.GetAll().Where(t => 
                t.Id == input.Id && 
                (t.SubscriberTenantId == AbpSession.TenantId || t.TenantId == AbpSession.TenantId)).FirstOrDefault();

            if (projectTenant != null)
            {
                if (_projectDeploymentRepository.GetAll().Include(d => d.ProjectReleaseFk).Any(d => 
                        d.TenantId == AbpSession.TenantId && 
                        d.ProjectReleaseFk.ProjectId == projectTenant.ProjectId 
                        && d.ActionType == ProjectDeploymentEnums.ProjectDeploymentActionType.Active)
                ){
                    throw new UserFriendlyException("Cannot delete Subscription when there are active Deployments.");
                }


                await _projectTenantRepository.DeleteAsync(input.Id);
            }

        }

        public async Task<FileDto> GetProjectTenantsToExcel(GetAllProjectTenantsForExcelInput input)
        {

            var filteredProjectTenants = _projectTenantRepository.GetAll()
                        .Include(e => e.ProjectEnvironmentFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinSubscriberTenantIdFilter != null, e => e.SubscriberTenantId >= input.MinSubscriberTenantIdFilter)
                        .WhereIf(input.MaxSubscriberTenantIdFilter != null, e => e.SubscriberTenantId <= input.MaxSubscriberTenantIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectIdFilter.ToString()), e => e.ProjectId.ToString() == input.ProjectIdFilter.ToString())
                        .WhereIf(input.EnabledFilter.HasValue && input.EnabledFilter > -1, e => (input.EnabledFilter == 1 && e.Enabled) || (input.EnabledFilter == 0 && !e.Enabled))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectEnvironmentNameFilter), e => e.ProjectEnvironmentFk != null && e.ProjectEnvironmentFk.Name == input.ProjectEnvironmentNameFilter);

            var query = (from o in filteredProjectTenants
                         join o1 in _lookup_projectEnvironmentRepository.GetAll() on o.ProjectEnvironmentId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetProjectTenantForViewDto()
                         {
                             ProjectTenant = new ProjectTenantDto
                             {
                                 SubscriberTenantId = o.SubscriberTenantId,
                                 ProjectId = o.ProjectId,
                                 Enabled = o.Enabled,
                                 Id = o.Id
                             },
                             ProjectEnvironmentName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                         });

            var projectTenantListDtos = await query.ToListAsync();

            return _projectTenantsExcelExporter.ExportToFile(projectTenantListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectTenants)]
        public async Task<PagedResultDto<ProjectTenantProjectEnvironmentLookupTableDto>> GetAllProjectEnvironmentForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_projectEnvironmentRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var projectEnvironmentList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ProjectTenantProjectEnvironmentLookupTableDto>();
            foreach (var projectEnvironment in projectEnvironmentList)
            {
                lookupTableDtoList.Add(new ProjectTenantProjectEnvironmentLookupTableDto
                {
                    Id = projectEnvironment.Id.ToString(),
                    DisplayName = projectEnvironment.Name?.ToString()
                });
            }

            return new PagedResultDto<ProjectTenantProjectEnvironmentLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}