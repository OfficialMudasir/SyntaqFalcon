using Syntaq.Falcon.Authorization.Users;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.EntityVersionHistories.Exporting;
using Syntaq.Falcon.EntityVersionHistories.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Syntaq.Falcon.Storage;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.AccessControlList;

namespace Syntaq.Falcon.EntityVersionHistories
{
    [AbpAuthorize(AppPermissions.Pages_EntityVersionHistories)]
    public class EntityVersionHistoriesAppService : FalconAppServiceBase, IEntityVersionHistoriesAppService
    {
        private readonly IRepository<EntityVersionHistory, Guid> _entityVersionHistoryRepository;
        private readonly IEntityVersionHistoriesExcelExporter _entityVersionHistoriesExcelExporter;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly ACLManager _ACLManager;
        public EntityVersionHistoriesAppService(ACLManager aclManager, IRepository<EntityVersionHistory, Guid> entityVersionHistoryRepository, IEntityVersionHistoriesExcelExporter entityVersionHistoriesExcelExporter, IRepository<User, long> lookup_userRepository)
        {
            _ACLManager = aclManager;
            _entityVersionHistoryRepository = entityVersionHistoryRepository;
            _entityVersionHistoriesExcelExporter = entityVersionHistoriesExcelExporter;
            _lookup_userRepository = lookup_userRepository;

        }

        public async Task<PagedResultDto<GetEntityVersionHistoryForViewDto>> GetAll(GetAllEntityVersionHistoriesInput input)
        {

            input.Filter = input.Filter?.Trim();

            var filteredEntityVersionHistories = _entityVersionHistoryRepository.GetAll()
                        .Include(e => e.UserFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.VersionName.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Data.Contains(input.Filter) || e.Type.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VersionNameFilter), e => e.VersionName == input.VersionNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

            var pagedAndFilteredEntityVersionHistories = filteredEntityVersionHistories
                 .OrderBy(input.Sorting ?? "CreationTime desc")
                .PageBy(input);

            var entityVersionHistories = from o in pagedAndFilteredEntityVersionHistories
                                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                                         from s1 in j1.DefaultIfEmpty()
                                         select new
                                         {
                                             o.Name,
                                             o.Version,
                                             o.PreviousVersion,
                                             o.Type,
                                             Id = o.Id,
                                             o.CreationTime,
                                             UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                         };

            var totalCount = await filteredEntityVersionHistories.CountAsync();

            var dbList = await entityVersionHistories.ToListAsync();
            var results = new List<GetEntityVersionHistoryForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetEntityVersionHistoryForViewDto()
                {
                    EntityVersionHistory = new EntityVersionHistoryDto
                    {

                        Name = o.Name,
                        Version = o.Version,
                        PreviousVersion = o.PreviousVersion,
                        Type = o.Type,
                        Id = o.Id,
                        CreationTime = o.CreationTime
                    },
                    UserName = o.UserName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetEntityVersionHistoryForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetEntityVersionHistoryForViewDto> GetEntityVersionHistoryForView(Guid id)
        {
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });

            if (ACLResult.IsAuthed)
            {
                var entityVersionHistory = await _entityVersionHistoryRepository.GetAsync(id);

                var output = new GetEntityVersionHistoryForViewDto { EntityVersionHistory = ObjectMapper.Map<EntityVersionHistoryDto>(entityVersionHistory) };

                if (output.EntityVersionHistory.UserId != null)
                {
                    var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EntityVersionHistory.UserId);
                    output.UserName = _lookupUser?.Name?.ToString();
                }

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");

            }
        }

        //[AbpAuthorize(AppPermissions.Pages_EntityVersionHistories_Edit)]
        //public async Task<GetEntityVersionHistoryForEditOutput> GetEntityVersionHistoryForEdit(EntityDto<Guid> input)
        //{
        //    var entityVersionHistory = await _entityVersionHistoryRepository.FirstOrDefaultAsync(input.Id);

        //    var output = new GetEntityVersionHistoryForEditOutput { EntityVersionHistory = ObjectMapper.Map<CreateOrEditEntityVersionHistoryDto>(entityVersionHistory) };

        //    if (output.EntityVersionHistory.UserId != null)
        //    {
        //        var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EntityVersionHistory.UserId);
        //        output.UserName = _lookupUser?.Name?.ToString();
        //    }

        //    return output;
        //}

        //public async Task CreateOrEdit(CreateOrEditEntityVersionHistoryDto input)
        //{
        //    if (input.Id == null)
        //    {
        //        await Create(input);
        //    }
        //    else
        //    {
        //        await Update(input);
        //    }
        //}

        //[AbpAuthorize(AppPermissions.Pages_EntityVersionHistories_Create)]
        //protected virtual async Task Create(CreateOrEditEntityVersionHistoryDto input)
        //{
        //    var entityVersionHistory = ObjectMapper.Map<EntityVersionHistory>(input);

        //    if (AbpSession.TenantId != null)
        //    {
        //        entityVersionHistory.TenantId = (int?)AbpSession.TenantId;
        //    }

        //    await _entityVersionHistoryRepository.InsertAsync(entityVersionHistory);

        //}

        //[AbpAuthorize(AppPermissions.Pages_EntityVersionHistories_Edit)]
        //protected virtual async Task Update(CreateOrEditEntityVersionHistoryDto input)
        //{
        //    var entityVersionHistory = await _entityVersionHistoryRepository.FirstOrDefaultAsync((Guid)input.Id);
        //    ObjectMapper.Map(input, entityVersionHistory);

        //}

        //[AbpAuthorize(AppPermissions.Pages_EntityVersionHistories_Delete)]
        //public async Task Delete(EntityDto<Guid> input)
        //{
        //    await _entityVersionHistoryRepository.DeleteAsync(input.Id);
        //}

        public async Task<FileDto> GetEntityVersionHistoriesToExcel(GetAllEntityVersionHistoriesForExcelInput input)
        {

            var filteredEntityVersionHistories = _entityVersionHistoryRepository.GetAll()
                        .Include(e => e.UserFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.VersionName.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Data.Contains(input.Filter) || e.Type.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VersionNameFilter), e => e.VersionName == input.VersionNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

            var query = (from o in filteredEntityVersionHistories
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetEntityVersionHistoryForViewDto()
                         {
                             EntityVersionHistory = new EntityVersionHistoryDto
                             {
                                 Name = o.Name,
                                 Version = o.Version,
                                 PreviousVersion = o.PreviousVersion,
                                 Type = o.Type,
                                 Id = o.Id
                             },
                             UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                         });

            var entityVersionHistoryListDtos = await query.ToListAsync();

            return _entityVersionHistoriesExcelExporter.ExportToFile(entityVersionHistoryListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_EntityVersionHistories)]
        public async Task<List<EntityVersionHistoryUserLookupTableDto>> GetAllUserForTableDropdown()
        {
            return await _lookup_userRepository.GetAll()
                .Select(user => new EntityVersionHistoryUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user == null || user.Name == null ? "" : user.Name.ToString()
                }).ToListAsync();
        }

    }
}