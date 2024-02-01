using Syntaq.Falcon.RecordPolicies;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.RecordPolicyActions.Exporting;
using Syntaq.Falcon.RecordPolicyActions.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Syntaq.Falcon.Storage;
using Abp.Domain.Uow;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.AccessControlList;

namespace Syntaq.Falcon.RecordPolicyActions
{
    [AbpAuthorize(AppPermissions.Pages_RecordPolicyActions)]
    public class RecordPolicyActionsAppService : FalconAppServiceBase, IRecordPolicyActionsAppService
    {
        private readonly IRepository<RecordPolicyAction, Guid> _recordPolicyActionRepository;
        private readonly IRecordPolicyActionsExcelExporter _recordPolicyActionsExcelExporter;
        private readonly IRepository<RecordPolicy, Guid> _lookup_recordPolicyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ACLManager _ACLManager;
        private readonly IRepository<RecordPolicy, Guid> _recordPolicyRepository;

        public RecordPolicyActionsAppService(IRepository<RecordPolicyAction, Guid> recordPolicyActionRepository, ACLManager aclManager, IRepository<Tenant> tenantRepository, IRecordPolicyActionsExcelExporter recordPolicyActionsExcelExporter, IRepository<RecordPolicy, Guid> lookup_recordPolicyRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<RecordPolicy, Guid> recordPolicyRepository)
        {
            _ACLManager = aclManager;
            _recordPolicyActionRepository = recordPolicyActionRepository;
            _recordPolicyActionsExcelExporter = recordPolicyActionsExcelExporter;
            _lookup_recordPolicyRepository = lookup_recordPolicyRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantRepository = tenantRepository;
            _recordPolicyRepository = recordPolicyRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_RecordPolicyActions)]
        public async Task<PagedResultDto<GetRecordPolicyActionForViewDto>> GetAll(GetAllRecordPolicyActionsInput input)
        {
            //AppliedTenantId==-1 is default record delete policy
            //step1: filter the tenant Id if it is not host
            //step2: if there is no policy setup in this tenant using default policies
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            input.Filter = input.Filter?.Trim();

            var filteredRecordDeletePolicies = _recordPolicyActionRepository.GetAll()
                 .WhereIf(AbpSession.TenantId.HasValue, e => e.AppliedTenantId == (int)AbpSession.TenantId)
                  .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter));


            var pagedAndFilteredRecordDeletePolicies = filteredRecordDeletePolicies
                .OrderBy(input.Sorting ?? "AppliedTenantId asc")
                .PageBy(input);

            if (pagedAndFilteredRecordDeletePolicies.Count() == 0)
            {
                //get default rules, appliedtenantId is -1
                filteredRecordDeletePolicies = _recordPolicyActionRepository.GetAll()
                   .WhereIf(AbpSession.TenantId.HasValue, e => e.AppliedTenantId == -1);

                pagedAndFilteredRecordDeletePolicies = filteredRecordDeletePolicies
                .OrderBy(input.Sorting ?? "AppliedTenantId asc")
                .PageBy(input);

            }

            var recordDeletePolicies = from o in pagedAndFilteredRecordDeletePolicies
                                       join o1 in _tenantRepository.GetAll() on o.AppliedTenantId equals o1.Id into j1
                                       from s1 in j1.DefaultIfEmpty()
                                       select new
                                       {
                                           o.Name,
                                           o.AppliedTenantId,
                                           o.ExpireDays,
                                           o.Active,
                                           o.Type,
                                           o.RecordStatus,
                                           Id = o.Id,
                                           tenantName = s1 == null ? "Default[All]" : s1.Name == null ? " " : s1.Name.ToString(),
                                       };

            var totalCount = await filteredRecordDeletePolicies.CountAsync();

            var dbList = await recordDeletePolicies.ToListAsync();
            var results = new List<GetRecordPolicyActionForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetRecordPolicyActionForViewDto()
                {
                    RecordPolicyAction = new RecordPolicyActionDto
                    {

                        Name = o.Name,
                        AppliedTenantId = o.AppliedTenantId,
                        ExpireDays = o.ExpireDays,
                        Active = o.Active,
                        Type = o.Type,
                        RecordStatus = o.RecordStatus,
                        Id = o.Id,
                    },
                    AppliedTenantName = o.tenantName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetRecordPolicyActionForViewDto>(
                totalCount,
                results
            );

        }
        
        public async Task<PagedResultDto<GetRecordPolicyActionForViewDto>> GetAllByRecordId(GetAllRecordPolicyActionsInput input)
        {
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.RecordPolicyId, UserId = AbpSession.UserId });
            if (ACLResult.IsAuthed)
            {
                var filteredRecordDeletePolicies = _recordPolicyActionRepository.GetAll()
                      .WhereIf(input.RecordPolicyId != null, rd => rd.RecordPolicyId == input.RecordPolicyId)
                      .ToList();


                var recordDeletePolicies = from o in filteredRecordDeletePolicies
                                           select new
                                           {
                                               o.Name,
                                               o.AppliedTenantId,
                                               o.ExpireDays,
                                               o.Active,
                                               o.Type,
                                               o.RecordStatus,
                                               o.RecordPolicyId,
                                               Id = o.Id
                                           };

                var results = new List<GetRecordPolicyActionForViewDto>();

                foreach (var o in recordDeletePolicies)
                {
                    var res = new GetRecordPolicyActionForViewDto()
                    {
                        RecordPolicyAction = new RecordPolicyActionDto
                        {
                            Name = o.Name,
                            AppliedTenantId = o.AppliedTenantId,
                            ExpireDays = o.ExpireDays,
                            Active = o.Active,
                            Type = o.Type,
                            RecordStatus = o.RecordStatus,
                            RecordPolicyId = o.RecordPolicyId,
                            Id = o.Id,
                        }
                    };

                    results.Add(res);
                }

                var totalCount = filteredRecordDeletePolicies.Count();

                return new PagedResultDto<GetRecordPolicyActionForViewDto>(
                    totalCount,
                    results
                );
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<GetRecordPolicyActionForViewDto> GetRecordPolicyActionForView(Guid id)
        {
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = id, UserId = AbpSession.UserId });
            if (ACLResult.IsAuthed)
            {
                var recordpolicyaction = await _recordPolicyActionRepository.GetAsync(id);

                var output = new GetRecordPolicyActionForViewDto { RecordPolicyAction = ObjectMapper.Map<RecordPolicyActionDto>(recordpolicyaction) };

                if (output.RecordPolicyAction.RecordPolicyId != null)
                {
                    var _lookupTenantName = await _tenantRepository.FirstOrDefaultAsync(t => t.Id == output.RecordPolicyAction.AppliedTenantId);

                    output.AppliedTenantName = _lookupTenantName == null ? "Default[All]" : _lookupTenantName?.Name?.ToString();
                }

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }

        [AbpAuthorize(AppPermissions.Pages_RecordPolicyActions_Edit)]
        public async Task<GetRecordPolicyActionForEditOutput> GetRecordPolicyActionForEdit(EntityDto<Guid> input)
        {
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId });
            if (ACLResult.IsAuthed)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                var recordpolicyaction = await _recordPolicyActionRepository.FirstOrDefaultAsync(input.Id);

                var output = new GetRecordPolicyActionForEditOutput { RecordPolicyAction = ObjectMapper.Map<CreateOrEditRecordPolicyActionDto>(recordpolicyaction) };

                if (output.RecordPolicyAction.RecordPolicyId != null)
                {
                    var _lookupRecordPolicy = await _lookup_recordPolicyRepository.FirstOrDefaultAsync((Guid)output.RecordPolicyAction.RecordPolicyId);
                    output.RecordPolicyName = _lookupRecordPolicy?.Name?.ToString();
                }
                // var output = new GetrecordpolicyactionForEditOutput { };

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }

        public async Task CreateOrEdit(CreateOrEditRecordPolicyActionDto input)
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


        //only host user can create record delete policy
        //cannot create duplicate record delete policy for one same tenant
        [AbpAuthorize(AppPermissions.Pages_RecordPolicyActions_Create)]
        protected virtual async Task Create(CreateOrEditRecordPolicyActionDto input)
        {
            if (AbpSession.TenantId == null)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                var recordpolicyaction = ObjectMapper.Map<RecordPolicyAction>(input);

                if (AbpSession.TenantId != null)
                {
                    recordpolicyaction.TenantId = (int?)AbpSession.TenantId;
                }

                var policyList = _recordPolicyActionRepository.GetAll()
                   .Where(e => e.AppliedTenantId == input.AppliedTenantId && e.Type == input.Type && e.RecordStatus == input.RecordStatus)
                   .ToList();

                if (policyList.Count == 0)
                {
                    await _recordPolicyActionRepository.InsertAsync(recordpolicyaction);

                    await _ACLManager.AddACL(new ACL()
                    {
                        UserId = AbpSession.UserId,
                        Type = "recordpolicyaction",
                        EntityID = recordpolicyaction.Id,
                        Role = "O"
                    });

                    _unitOfWorkManager.Current.SaveChanges();
                }
                else
                {
                    throw new UserFriendlyException("Cannot create a same policy type to a tenant!");
                }
            }
            else
            {
                throw new UserFriendlyException("Only Host Administrator can create record policies");
            }

        }

        //tenant user edit default record delete policy will create new record policy, and record delete policy on its tenant
        [AbpAuthorize(AppPermissions.Pages_RecordPolicyActions_Edit)]
        protected virtual async Task Update(CreateOrEditRecordPolicyActionDto input)
        {
            //step1: tenant create their own record delete policy based on the default rules
            //copy default policy and create record policy, and record policy actions to this tenant.
            //or update existed policy actions

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                if (AbpSession.TenantId != null && input.AppliedTenantId == -1)
                {
                    //if there are record policy existed in this tenant
                    //prevent tenant admin change the default policy
                    var recordPolicyTenant = await _recordPolicyRepository.FirstOrDefaultAsync(rp => rp.AppliedTenantId == AbpSession.TenantId);
                    if (recordPolicyTenant != null)
                    {
                        throw new UserFriendlyException("Tenant user cannot modify default policy");
                    }

                    var recordPolicy = await _recordPolicyRepository.FirstOrDefaultAsync((Guid)input.RecordPolicyId);
                    if (recordPolicy != null)
                    {
                        var newRecordPolicy = new RecordPolicy()
                        {
                            TenantId = AbpSession.TenantId,
                            Name = "RecordPolicy For Tenant" + AbpSession.TenantId.ToString(),
                            AppliedTenantId = (int)AbpSession.TenantId,
                        };
                        var recordPolicyId = await _recordPolicyRepository.InsertAndGetIdAsync(newRecordPolicy);

                        await _ACLManager.AddACL(new ACL()
                        {
                            UserId = AbpSession.UserId,
                            Type = "RecordPolicy",
                            EntityID = recordPolicy.Id,
                            Role = "O"
                        });

                        var recordDeletePolicies = _recordPolicyActionRepository.GetAll()
                          .WhereIf(input.RecordPolicyId != null, rd => rd.RecordPolicyId == input.RecordPolicyId)
                          .ToList();
                        foreach (var recordDeletePolicy in recordDeletePolicies)
                        {
                            if (recordDeletePolicy.Id == input.Id)
                            {
                                var newrdp = new RecordPolicyAction()
                                {
                                    TenantId = AbpSession.TenantId,
                                    Name = input.Name,
                                    AppliedTenantId = (int)AbpSession.TenantId,
                                    ExpireDays = input.ExpireDays,
                                    Active = input.Active,
                                    Type = input.Type,
                                    RecordStatus = input.RecordStatus,
                                    RecordPolicyId = recordPolicyId,
                                };
                                await _recordPolicyActionRepository.InsertAsync(newrdp);

                                await _ACLManager.AddACL(new ACL()
                                {
                                    UserId = AbpSession.UserId,
                                    Type = "RecordPolicyAction",
                                    EntityID = newrdp.Id,
                                    Role = "O"
                                });
                            }
                            else
                            {
                                var newrdp = new RecordPolicyAction()
                                {
                                    TenantId = AbpSession.TenantId,
                                    Name = recordDeletePolicy.Name,
                                    AppliedTenantId = (int)AbpSession.TenantId,
                                    ExpireDays = recordDeletePolicy.ExpireDays,
                                    Active = recordDeletePolicy.Active,
                                    Type = recordDeletePolicy.Type,
                                    RecordStatus = recordDeletePolicy.RecordStatus,
                                    RecordPolicyId = recordPolicyId,

                                };
                                await _recordPolicyActionRepository.InsertAsync(newrdp);

                                await _ACLManager.AddACL(new ACL()
                                {
                                    UserId = AbpSession.UserId,
                                    Type = "RecordPolicyAction",
                                    EntityID = newrdp.Id,
                                    Role = "O"
                                });
                            }

                        }
                        _unitOfWorkManager.Current.SaveChanges();

                    }
                }
                else
                {
                    ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = (Guid)input.Id, UserId = AbpSession.UserId });

                    if (ACLResult.IsAuthed)
                    {
                        var recordDeletePolicy = await _recordPolicyActionRepository.FirstOrDefaultAsync((Guid)input.Id);
                        ObjectMapper.Map(input, recordDeletePolicy);
                    }
                    else
                    {
                        throw new UserFriendlyException("Not Authorised");
                    }
                }
            }

        public async Task<FileDto> GetRecordPolicyActionsToExcel(GetAllRecordPolicyActionsForExcelInput input)
        {

            var filteredRecordPolicyActions = _recordPolicyActionRepository.GetAll()
                        .Include(e => e.RecordPolicyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(input.MinExpireDaysFilter != null, e => e.ExpireDays >= input.MinExpireDaysFilter)
                        .WhereIf(input.MaxExpireDaysFilter != null, e => e.ExpireDays <= input.MaxExpireDaysFilter)
                        .WhereIf(input.ActiveFilter.HasValue && input.ActiveFilter > -1, e => (input.ActiveFilter == 1 && e.Active) || (input.ActiveFilter == 0 && !e.Active))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RecordPolicyNameFilter), e => e.RecordPolicyFk != null && e.RecordPolicyFk.Name == input.RecordPolicyNameFilter);

            var query = (from o in filteredRecordPolicyActions
                         join o1 in _lookup_recordPolicyRepository.GetAll() on o.RecordPolicyId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetRecordPolicyActionForViewDto()
                         {
                             RecordPolicyAction = new RecordPolicyActionDto
                             {
                                 Name = o.Name,
                                 AppliedTenantId = o.AppliedTenantId,
                                 ExpireDays = o.ExpireDays,
                                 Active = o.Active,
                                 Type = o.Type,
                                 RecordStatus = o.RecordStatus,
                                 Id = o.Id
                             }
                            
                         });

            var recordPolicyActionListDtos = await query.ToListAsync();

            return _recordPolicyActionsExcelExporter.ExportToFile(recordPolicyActionListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_RecordPolicyActions)]
        public async Task<PagedResultDto<RecordPolicyActionRecordPolicyLookupTableDto>> GetAllRecordPolicyForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_recordPolicyRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var recordPolicyList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<RecordPolicyActionRecordPolicyLookupTableDto>();
            foreach (var recordPolicy in recordPolicyList)
            {
                lookupTableDtoList.Add(new RecordPolicyActionRecordPolicyLookupTableDto
                {
                    Id = recordPolicy.Id.ToString(),
                    DisplayName = recordPolicy.Name?.ToString()
                });
            }

            return new PagedResultDto<RecordPolicyActionRecordPolicyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }


    }
}