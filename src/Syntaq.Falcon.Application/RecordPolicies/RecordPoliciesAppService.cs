using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.RecordPolicies.Exporting;
using Syntaq.Falcon.RecordPolicies.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Syntaq.Falcon.Storage;
using Syntaq.Falcon.MultiTenancy;
using Abp.Domain.Uow;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.RecordPolicyActions;

namespace Syntaq.Falcon.RecordPolicies
{
    [AbpAuthorize(AppPermissions.Pages_RecordPolicies)]
    public class RecordPoliciesAppService : FalconAppServiceBase, IRecordPoliciesAppService
    {
        private readonly IRepository<RecordPolicy, Guid> _recordPolicyRepository;
        private readonly IRecordPoliciesExcelExporter _recordPoliciesExcelExporter;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<RecordPolicyAction, Guid> _recordPolicyActionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ACLManager _ACLManager;
        private readonly IRepository<ACL> _aclRepository;

        public RecordPoliciesAppService(IRepository<RecordPolicy, Guid> recordPolicyRepository, IRepository<ACL> aclRepository, ACLManager aclManager, IRecordPoliciesExcelExporter recordPoliciesExcelExporter, IRepository<Tenant> tenantRepository, IRepository<RecordPolicyAction, Guid> recordPolicyActionRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _ACLManager = aclManager;
            _unitOfWorkManager = unitOfWorkManager;
            _recordPolicyRepository = recordPolicyRepository;
            _recordPoliciesExcelExporter = recordPoliciesExcelExporter;
            _tenantRepository = tenantRepository;
            _recordPolicyActionRepository = recordPolicyActionRepository;
            _aclRepository = aclRepository;
        }

        public async Task<PagedResultDto<GetRecordPolicyForViewDto>> GetAll(GetAllRecordPoliciesInput input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            input.Filter = input.Filter?.Trim();

            var filteredRecordPolicies = _recordPolicyRepository.GetAll()
                 .WhereIf(AbpSession.TenantId.HasValue, e => e.AppliedTenantId == (int)AbpSession.TenantId);
            //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
            //.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
            //.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter);


            var pagedAndFilteredRecordPolicies = filteredRecordPolicies
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            if (pagedAndFilteredRecordPolicies.Count() == 0)
            {
                filteredRecordPolicies = _recordPolicyRepository.GetAll()
                   .WhereIf(AbpSession.TenantId.HasValue, e => e.AppliedTenantId == -1);

                pagedAndFilteredRecordPolicies = filteredRecordPolicies
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            }

            var recordPolicies = from o in pagedAndFilteredRecordPolicies
                                 join o1 in _tenantRepository.GetAll() on o.AppliedTenantId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()
                                 select new
                                 {
                                     o.Name,
                                     o.AppliedTenantId,
                                     tenantName = s1 == null ? "Default[All]" : s1.Name == null ? " " : s1.Name.ToString(),
                                     o.CreationTime,
                                     o.Id
                                 };

            var totalCount = await filteredRecordPolicies.CountAsync();

            var dbList = await recordPolicies.ToListAsync();
            var results = new List<GetRecordPolicyForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetRecordPolicyForViewDto()
                {
                    RecordPolicy = new RecordPolicyDto
                    {
                        Name = o.Name,
                        AppliedTenantId = o.AppliedTenantId,
                        Id = o.Id
                    },
                    AppliedTenantName = o.tenantName,
                };

                results.Add(res);
            }

            return new PagedResultDto<GetRecordPolicyForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetRecordPolicyForViewDto> GetRecordPolicyForView(Guid id)
        {
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = id, UserId = AbpSession.UserId });
            if (ACLResult.IsAuthed)
            {

                if (AbpSession.TenantId == null)
                {
                    _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                }
                var recordPolicy = await _recordPolicyRepository.GetAsync(id);

                var output = new GetRecordPolicyForViewDto { RecordPolicy = ObjectMapper.Map<RecordPolicyDto>(recordPolicy) };

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }

        [AbpAuthorize(AppPermissions.Pages_RecordPolicies_Edit)]
        public async Task<GetRecordPolicyForEditOutput> GetRecordPolicyForEdit(EntityDto<Guid> input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId });
            if (ACLResult.IsAuthed)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                var recordPolicy = await _recordPolicyRepository.FirstOrDefaultAsync(input.Id);

                var tenant = TenantManager.Tenants;

                var recordDeletetenant = from o in tenant
                                         select new
                                         {
                                             o.Name,
                                             o.Id,
                                         };

                var dbList = await recordDeletetenant.ToListAsync();

                var tlist = new List<TenantIdNameListDto>();

                foreach (var t in dbList)
                {
                    tlist.Add(new TenantIdNameListDto
                    {
                        Id = t.Id,
                        Name = t.Name.ToString()
                    });
                }

                var output = new GetRecordPolicyForEditOutput { RecordPolicy = ObjectMapper.Map<CreateOrEditRecordPolicyDto>(recordPolicy), TenantList = tlist };

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }

        //only host will use this create button
        [AbpAuthorize(AppPermissions.Pages_RecordPolicies_Create)]
        public async Task<GetRecordPolicyForEditOutput> GetRecordPolicyForCreate()
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var tenant = TenantManager.Tenants;

            var recordPolicytenant = (from o in tenant
                                      select new
                                      {
                                          o.Name,
                                          o.Id,
                                      }
                                     ).OrderBy(o => o.Name.Trim());

            var dbList = await recordPolicytenant.ToListAsync();

            var tlist = new List<TenantIdNameListDto>();

            foreach (var t in dbList)
            {
                tlist.Add(new TenantIdNameListDto
                {
                    Id = t.Id,
                    Name = t.Name.ToString()
                });
            }

            var output = new GetRecordPolicyForEditOutput
            {
                RecordPolicy = new CreateOrEditRecordPolicyDto(),
                TenantList = tlist
            };

            return output;

        }


        public async Task<Guid> CreateOrEdit(CreateOrEditRecordPolicyDto input)
        {
            Guid recordPolicyId;
            if (input.Id == null)
            {
                return recordPolicyId = await Create(input);
            }
            else
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id ?? new Guid("00000000-0000-0000-0000-000000000000"), UserId = AbpSession.UserId });

                if (ACLResult.IsAuthed)
                {
                    await Update(input);
                    recordPolicyId = (Guid)input.Id;
                    return recordPolicyId;
                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }
            }


        }

        //only host user can create record delete policy
        //cannot create duplicate record policy for one same tenant
        [AbpAuthorize(AppPermissions.Pages_RecordPolicies_Create)]
        protected virtual async Task<Guid> Create(CreateOrEditRecordPolicyDto input)
        {

            if (AbpSession.TenantId == null)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                var recordPolicy = ObjectMapper.Map<RecordPolicy>(input);

                if (AbpSession.TenantId != null)
                {
                    recordPolicy.TenantId = (int?)AbpSession.TenantId;

                }

                var policyList = _recordPolicyRepository.GetAll()
                    .Where(e => e.AppliedTenantId == input.AppliedTenantId).ToList();

                if (policyList.Count == 0)
                {
                    await _recordPolicyRepository.InsertAndGetIdAsync(recordPolicy);

                    await _ACLManager.AddACL(new ACL()
                    {
                        UserId = AbpSession.UserId,
                        Type = "RecordPolicy",
                        EntityID = recordPolicy.Id,
                        Role = "O"
                    });

                    _unitOfWorkManager.Current.SaveChanges();

                    return recordPolicy.Id;


                }
                else
                {
                    return new Guid("00000000-0000-0000-0000-000000000000");
                    //throw new Exception("Cannot create same policy to a tenant!");
                }

            }
            else
            {
                throw new UserFriendlyException("Only Host Administrator can create record policies");
            }



        }

        [AbpAuthorize(AppPermissions.Pages_RecordPolicies_Edit)]
        protected virtual async Task Update(CreateOrEditRecordPolicyDto input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            var recordPolicy = await _recordPolicyRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, recordPolicy);
        }

        //[AbpAuthorize(AppPermissions.Pages_RecordPolicies_Delete)]
        //public async Task Delete(EntityDto<Guid> input)
        //{
        //    ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId });

        //    if (ACLResult.IsAuthed)
        //    {
        //        _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

        //        var rdps = _recordDeletePolicyRepository.GetAll().Where(e => e.RecordPolicyId == input.Id).ToList();
        //        foreach (var rdp in rdps)
        //        {
        //            var acl = _aclRepository.FirstOrDefault(i => i.EntityID == rdp.Id);
        //            await _aclRepository.DeleteAsync(acl);

        //            await  _recordDeletePolicyRepository.DeleteAsync(rdp);
        //        }

        //        var aclr = _aclRepository.FirstOrDefault(i => i.EntityID == input.Id);
        //        await _aclRepository.DeleteAsync(aclr);

        //        await _recordPolicyRepository.DeleteAsync(input.Id);
        //    }
        //    else
        //    {
        //        throw new UserFriendlyException("Not Authorised");
        //    }
        //}

        public async Task<FileDto> GetRecordPoliciesToExcel(GetAllRecordPoliciesForExcelInput input)
        {

            var filteredRecordPolicies = _recordPolicyRepository.GetAll();
            //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
            //.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
            //.WhereIf(input.ActiveFilter.HasValue && input.ActiveFilter > -1, e => (input.ActiveFilter == 1 && e.Active) || (input.ActiveFilter == 0 && !e.Active))
            //.WhereIf(input.MinAppliedTenantIdFilter != null, e => e.AppliedTenantId >= input.MinAppliedTenantIdFilter)
            //.WhereIf(input.MaxAppliedTenantIdFilter != null, e => e.AppliedTenantId <= input.MaxAppliedTenantIdFilter);
            //.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter);

            var query = (from o in filteredRecordPolicies
                         select new GetRecordPolicyForViewDto()
                         {
                             RecordPolicy = new RecordPolicyDto
                             {
                                 Name = o.Name,
                                 AppliedTenantId = o.AppliedTenantId,
                                 Id = o.Id
                             }
                         });

            var recordPolicyListDtos = await query.ToListAsync();

            return _recordPoliciesExcelExporter.ExportToFile(recordPolicyListDtos);
        }



    }
}