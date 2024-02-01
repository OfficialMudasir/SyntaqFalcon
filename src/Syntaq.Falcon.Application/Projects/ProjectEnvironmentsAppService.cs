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

namespace Syntaq.Falcon.Projects
{
    [AbpAuthorize(AppPermissions.Pages_ProjectEnvironments)]
    public class ProjectEnvironmentsAppService : FalconAppServiceBase, IProjectEnvironmentsAppService
    {
        private readonly IRepository<ProjectEnvironment, Guid> _projectEnvironmentRepository;
        private readonly IRepository<ProjectRelease, Guid> _projectReleaseRepository;
        private readonly IProjectEnvironmentsExcelExporter _projectEnvironmentsExcelExporter;

        public ProjectEnvironmentsAppService(
            IRepository<ProjectEnvironment, Guid> projectEnvironmentRepository, 
            IRepository<ProjectRelease, Guid> projectreleaseRepository,
            IProjectEnvironmentsExcelExporter projectEnvironmentsExcelExporter
        )
        {
            _projectEnvironmentRepository = projectEnvironmentRepository;
            _projectEnvironmentsExcelExporter = projectEnvironmentsExcelExporter;
            _projectReleaseRepository = projectreleaseRepository;
        }

        public async Task<PagedResultDto<GetProjectEnvironmentForViewDto>> GetAll(GetAllProjectEnvironmentsInput input)
        {
            var environmentTypeFilter = input.EnvironmentTypeFilter.HasValue
                        ? (ProjectEnvironmentConsts.ProjectEnvironmentType)input.EnvironmentTypeFilter
                        : default;

            var filteredProjectEnvironments = _projectEnvironmentRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(input.EnvironmentTypeFilter.HasValue && input.EnvironmentTypeFilter > -1, e => e.EnvironmentType == environmentTypeFilter);

            var pagedAndFilteredProjectEnvironments = filteredProjectEnvironments
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var projectEnvironments = from o in pagedAndFilteredProjectEnvironments
                                      select new
                                      {

                                          o.Name,
                                          o.Description,
                                          o.EnvironmentType,
                                          Id = o.Id
                                      };

            var totalCount = await filteredProjectEnvironments.CountAsync();

            var dbList = await projectEnvironments.ToListAsync();
            var results = new List<GetProjectEnvironmentForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetProjectEnvironmentForViewDto()
                {
                    ProjectEnvironment = new ProjectEnvironmentDto
                    {

                        Name = o.Name,
                        Description = o.Description,
                        EnvironmentType = o.EnvironmentType,
                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetProjectEnvironmentForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetProjectEnvironmentForViewDto> GetProjectEnvironmentForView(Guid id)
        {
            var projectEnvironment = await _projectEnvironmentRepository.GetAsync(id);

            var output = new GetProjectEnvironmentForViewDto { ProjectEnvironment = ObjectMapper.Map<ProjectEnvironmentDto>(projectEnvironment) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ProjectEnvironments_Edit)]
        public async Task<GetProjectEnvironmentForEditOutput> GetProjectEnvironmentForEdit(EntityDto<Guid> input)
        {
            var projectEnvironment = await _projectEnvironmentRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetProjectEnvironmentForEditOutput { ProjectEnvironment = ObjectMapper.Map<CreateOrEditProjectEnvironmentDto>(projectEnvironment) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditProjectEnvironmentDto input)
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

        [AbpAuthorize(AppPermissions.Pages_ProjectEnvironments_Create)]
        protected virtual async Task Create(CreateOrEditProjectEnvironmentDto input)
        {
            var projectEnvironment = ObjectMapper.Map<ProjectEnvironment>(input);

            if (AbpSession.TenantId != null)
            {
                projectEnvironment.TenantId = (int?)AbpSession.TenantId;
            }

            await _projectEnvironmentRepository.InsertAsync(projectEnvironment);

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectEnvironments_Edit)]
        protected virtual async Task Update(CreateOrEditProjectEnvironmentDto input)
        {
            var projectEnvironment = await _projectEnvironmentRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, projectEnvironment);

        }

        [AbpAuthorize(AppPermissions.Pages_ProjectEnvironments_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {

            if (_projectReleaseRepository.GetAll().Any(pr => pr.ProjectEnvironmentId == input.Id))
            {
                throw new UserFriendlyException( L("DeleteEnvironmentError"));
            }

            await _projectEnvironmentRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetProjectEnvironmentsToExcel(GetAllProjectEnvironmentsForExcelInput input)
        {
            var environmentTypeFilter = input.EnvironmentTypeFilter.HasValue
                        ? (ProjectEnvironmentConsts.ProjectEnvironmentType)input.EnvironmentTypeFilter
                        : default;

            var filteredProjectEnvironments = _projectEnvironmentRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(input.EnvironmentTypeFilter.HasValue && input.EnvironmentTypeFilter > -1, e => e.EnvironmentType == environmentTypeFilter);

            var query = (from o in filteredProjectEnvironments
                         select new GetProjectEnvironmentForViewDto()
                         {
                             ProjectEnvironment = new ProjectEnvironmentDto
                             {
                                 Name = o.Name,
                                 Description = o.Description,
                                 EnvironmentType = o.EnvironmentType,
                                 Id = o.Id
                             }
                         });

            var projectEnvironmentListDtos = await query.ToListAsync();

            return _projectEnvironmentsExcelExporter.ExportToFile(projectEnvironmentListDtos);
        }

    }
}