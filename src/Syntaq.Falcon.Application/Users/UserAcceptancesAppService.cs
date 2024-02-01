using Syntaq.Falcon.Users;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Records;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.Users.Exporting;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using Abp.UI;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using System.Net.Mail;
using Syntaq.Falcon.Net.Emailing;
using Syntaq.Falcon.MultiTenancy.Dto;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Utility;

namespace Syntaq.Falcon.Users
{

    public class UserAcceptancesAppService : FalconAppServiceBase, IUserAcceptancesAppService
    {
        private readonly IRepository<UserAcceptance, Guid> _userAcceptanceRepository;
        private readonly IRepository<UserAcceptanceType, Guid> _userAcceptanceTypeRepository;

        private readonly IRepository<Template, Guid> _templateRepository;

        private readonly IUserAcceptancesExcelExporter _userAcceptancesExcelExporter;
        private readonly IRepository<UserAcceptanceType,Guid> _lookup_userAcceptanceTypeRepository;
        private readonly IRepository<User,long> _lookup_userRepository;
        private readonly IRepository<RecordMatterContributor,Guid> _lookup_recordMatterContributorRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserAcceptancesAppService(
            IUnitOfWorkManager unitOfWorkManager, 
            IRepository<UserAcceptance, Guid> userAcceptanceRepository,
            IRepository<UserAcceptanceType, Guid> userAcceptanceTypeRepository,
            IRepository<Template, Guid> templateRepository,
            IUserAcceptancesExcelExporter userAcceptancesExcelExporter , 
            IRepository<UserAcceptanceType, Guid> lookup_userAcceptanceTypeRepository, 
            IRepository<User, long> lookup_userRepository, 
            IRepository<RecordMatterContributor, Guid> lookup_recordMatterContributorRepository
        )
        {
            _userAcceptanceRepository = userAcceptanceRepository;
            _userAcceptanceTypeRepository = userAcceptanceTypeRepository;
            _templateRepository = templateRepository;
            _userAcceptancesExcelExporter = userAcceptancesExcelExporter;
            _lookup_userAcceptanceTypeRepository = lookup_userAcceptanceTypeRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_recordMatterContributorRepository = lookup_recordMatterContributorRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [AbpAuthorize(AppPermissions.Pages_UserAcceptances)]
        public async Task<PagedResultDto<GetUserAcceptanceForViewDto>> GetAllExcludeContributors(GetAllUserAcceptancesInput input) 
        {
            input.Filter = input.Filter?.Trim();

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            var filteredUserAcceptances = _userAcceptanceRepository.GetAll()
                        .Include(e => e.UserAcceptanceTypeFk)
                        .Include(e => e.UserFk)
                        .Include(e => e.RecordMatterContributorFk)
                        .Include(e => e.TenantFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), 
                            e => (
                                (e.TenantFk != null && e.TenantFk.TenancyName.ToLower() == input.Filter.ToLower().Trim()) ||
                                (e.UserFk != null && e.UserFk.UserName.ToLower() == input.Filter.ToLower().Trim())  ||
                                (e.UserFk != null && e.UserFk.Name.ToLower() == input.Filter.ToLower().Trim()) ||
                                (e.UserFk != null && e.UserFk.Surname.ToLower() == input.Filter.ToLower().Trim()) ||
                                (e.UserAcceptanceTypeFk != null && e.UserAcceptanceTypeFk.Name.ToLower() == input.Filter.ToLower().Trim())
                            ))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TenantNameFilter), e => e.TenantFk != null && e.TenantFk.TenancyName.ToLower() == input.TenantNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.UserName.ToLower() == input.UserNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserFirstNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserFirstNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserSurnameFilter), e => e.UserFk != null && e.UserFk.Surname.ToLower() == input.UserSurnameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserEmailFilter), e => e.UserFk != null && e.UserFk.EmailAddress.ToLower() == input.UserEmailFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserAcceptanceTypeNameFilter), e => e.UserAcceptanceTypeFk != null && e.UserAcceptanceTypeFk.Name.ToLower() == input.UserAcceptanceTypeNameFilter.ToLower().Trim())
                        .Where(e => e.UserId != null && e.RecordMatterContributorId == null)
                        .Where(e => e.TenantId == AbpSession.TenantId || AbpSession.TenantId == null);
            ;

            var pagedAndFilteredUserAcceptances = filteredUserAcceptances
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var userAcceptances = from o in pagedAndFilteredUserAcceptances.Include(i => i.TenantFk)
                                  join o1 in _lookup_userAcceptanceTypeRepository.GetAll() on o.UserAcceptanceTypeId equals o1.Id into j1
                                  from s1 in j1.DefaultIfEmpty()

                                  join o2 in _lookup_userRepository.GetAll() on o.UserId equals o2.Id into j2
                                  from s2 in j2.DefaultIfEmpty()

                                  join o3 in _lookup_recordMatterContributorRepository.GetAll() on o.RecordMatterContributorId equals o3.Id into j3
                                  from s3 in j3.DefaultIfEmpty()

                                  select new GetUserAcceptanceForViewDto()
                                  {
                                      UserAcceptance = new UserAcceptanceDto
                                      {
                                          Id = o.Id,
                                          CreationTime = o.CreationTime
                                      },
                                      UserAcceptanceTypeName = s1 == null ? "" : s1.Name.ToString(),
                                      UserName = s2 == null ? "" : Convert.ToString(s2.UserName),
                                      UserFirstName = s2 == null ? "" : Convert.ToString(s2.Name),
                                      UserSurname = s2 == null ? "" : Convert.ToString(s2.Surname),
                                      UserEmailAddress = s2 == null ? "" : Convert.ToString(s2.EmailAddress),

                                      TenantId = o.TenantFk.Id,
                                      TenantName = o.TenantFk.TenancyName,

                                      RecordMatterContributorName = s3 == null ? "" : s3.Name.ToString(),
                                  };

            var totalCount = await filteredUserAcceptances.CountAsync();

            return new PagedResultDto<GetUserAcceptanceForViewDto>(
                totalCount,
                await userAcceptances.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_UserAcceptances_Edit)]
		 public async Task<GetUserAcceptanceForEditOutput> GetUserAcceptanceForEdit(EntityDto<Guid> input)
         {
            var userAcceptance = await _userAcceptanceRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserAcceptanceForEditOutput {UserAcceptance = ObjectMapper.Map<CreateOrEditUserAcceptanceDto>(userAcceptance)};

		    if (output.UserAcceptance.UserAcceptanceTypeId != null)
            {
                var _lookupUserAcceptanceType = await _lookup_userAcceptanceTypeRepository.FirstOrDefaultAsync((Guid)output.UserAcceptance.UserAcceptanceTypeId);
                output.UserAcceptanceTypeName = _lookupUserAcceptanceType.Name.ToString();
            }

		    if (output.UserAcceptance.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.UserAcceptance.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }

		    if (output.UserAcceptance.RecordMatterContributorId != null)
            {
                var _lookupRecordMatterContributor = await _lookup_recordMatterContributorRepository.FirstOrDefaultAsync((Guid)output.UserAcceptance.RecordMatterContributorId);
                output.RecordMatterContributorName = _lookupRecordMatterContributor.Name.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserAcceptanceDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 private async Task Create(CreateOrEditUserAcceptanceDto input)
         {
            var userAcceptance = ObjectMapper.Map<UserAcceptance>(input);

			
			if (AbpSession.TenantId != null)
			{
				userAcceptance.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _userAcceptanceRepository.InsertAsync(userAcceptance);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserAcceptances_Edit)]
		 private async Task Update(CreateOrEditUserAcceptanceDto input)
         {
            var userAcceptance = await _userAcceptanceRepository.FirstOrDefaultAsync((Guid)input.Id);
             ObjectMapper.Map(input, userAcceptance);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserAcceptances_Delete)]
         public async Task Delete(EntityDto<Guid> input)
         {
            await _userAcceptanceRepository.DeleteAsync(input.Id);
         }

        [AbpAuthorize(AppPermissions.Pages_UserAcceptances_Delete)]
        public async Task DeleteSelected (List<Guid> input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            foreach (var item in input)
            {
            await _userAcceptanceRepository.DeleteAsync(item);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_UserAcceptances)]
        public async Task<FileDto> GetUserAcceptancesToExcel(GetAllUserAcceptancesForExcelInput input)
         {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            var filteredUserAcceptances = _userAcceptanceRepository.GetAll()
                        .Include(e => e.UserAcceptanceTypeFk)
                        .Include(e => e.UserFk)
                        .Include(e => e.RecordMatterContributorFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.Filter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserSurNameFilter), e => e.UserFk != null && e.UserFk.Surname.ToLower() == input.UserSurNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserEmailFilter), e => e.UserFk != null && e.UserFk.EmailAddress.ToLower() == input.UserEmailFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserAcceptanceTypeNameFilter), e => e.UserAcceptanceTypeFk != null && e.UserAcceptanceTypeFk.Name.ToLower() == input.UserAcceptanceTypeNameFilter.ToLower().Trim())
                        .Where(e => e.UserId != null && e.RecordMatterContributorId == null)
                        .Where(e => e.TenantId == AbpSession.TenantId);

            var query = (from o in filteredUserAcceptances
                         join o1 in _lookup_userAcceptanceTypeRepository.GetAll() on o.UserAcceptanceTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_userRepository.GetAll() on o.UserId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_recordMatterContributorRepository.GetAll() on o.RecordMatterContributorId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         select new GetUserAcceptanceForViewDto() { 
							UserAcceptance = new UserAcceptanceDto
							{
                                Id = o.Id
							},
                         	UserAcceptanceTypeName = s1 == null ? "" : s1.Name.ToString(),
                             UserName = s2 == null ? "" : Convert.ToString(s2.Name),
                             UserSurname = s2 == null ? "" : Convert.ToString(s2.Surname),
                             UserEmailAddress = s2 == null ? "" : Convert.ToString(s2.EmailAddress)
						 });


            var userAcceptanceListDtos = await query.ToListAsync();

            return _userAcceptancesExcelExporter.ExportToFile(userAcceptanceListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_UserAcceptances)]
         public async Task<PagedResultDto<UserAcceptanceUserAcceptanceTypeLookupTableDto>> GetAllUserAcceptanceTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userAcceptanceTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userAcceptanceTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserAcceptanceUserAcceptanceTypeLookupTableDto>();
			foreach(var userAcceptanceType in userAcceptanceTypeList){
				lookupTableDtoList.Add(new UserAcceptanceUserAcceptanceTypeLookupTableDto
				{
					Id = userAcceptanceType.Id.ToString(),
					DisplayName = userAcceptanceType.Name?.ToString()
				});
			}

            return new PagedResultDto<UserAcceptanceUserAcceptanceTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_UserAcceptances, AppPermissions.Pages_Tenants)]
        public async Task<PagedResultDto<UserAcceptanceUserLookupTableDto>> GetAllAcceptedUserForLookupTable(GetAllForLookupTableInput input)
         {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            var filteredUserAcceptances = _userAcceptanceRepository.GetAll()
                    .Include(e => e.UserFk)
                    .Include(e => e.TenantFk)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => (e.UserFk != null && e.UserFk.Name.ToLower().Contains(input.Filter.ToLower().Trim())) 
                        || (e.UserFk != null && e.UserFk.Surname.ToLower().Contains(input.Filter.ToLower().Trim()))
                        || (e.UserFk != null && e.UserFk.EmailAddress.ToLower().Contains(input.Filter.ToLower().Trim()))
                        || (e.TenantFk != null && e.TenantFk.TenancyName.ToLower().Contains(input.Filter.ToLower().Trim())))
                    .Where(e=> !e.IsDeleted)
                    .Where(e=> e.UserId != null && e.RecordMatterContributorId == null).ToList();

            var lookupTableDtoList = new List<UserAcceptanceUserLookupTableDto>();
			foreach(var user in filteredUserAcceptances)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                var tenancyName = user.TenantFk == null ? "" : user.TenantFk.TenancyName;

                bool containsItem = lookupTableDtoList.Any(item => item.Id == (long)user.UserId && item.TenancyCodeName == tenancyName);
                if (!containsItem)
                {
                    lookupTableDtoList.Add(new UserAcceptanceUserLookupTableDto
                    {
                        Id = user.UserId,
                        DisplayName = Convert.ToString(user.UserFk.Name),
                        SurName = Convert.ToString(user.UserFk.Surname),
                        Email = Convert.ToString(user.UserFk.EmailAddress),       
                        TenancyCodeName = tenancyName,
                        TenantId = user.TenantId
                    });
                }
			}

             var QueryableAcceptedUsersist = lookupTableDtoList.AsQueryable();

            var AcceptedUsersForViewList = QueryableAcceptedUsersist
                .OrderBy(e=> e.TenancyCodeName)
                .PageBy(input)
                .ToList();

            return new PagedResultDto<UserAcceptanceUserLookupTableDto>(
                lookupTableDtoList.Count(),
                AcceptedUsersForViewList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_UserAcceptances)]
         public async Task<PagedResultDto<UserAcceptanceRecordMatterContributorLookupTableDto>> GetAllRecordMatterContributorForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_recordMatterContributorRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var recordMatterContributorList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserAcceptanceRecordMatterContributorLookupTableDto>();
			foreach(var recordMatterContributor in recordMatterContributorList){
				lookupTableDtoList.Add(new UserAcceptanceRecordMatterContributorLookupTableDto
				{
					Id = recordMatterContributor.Id.ToString(),
					DisplayName = recordMatterContributor.Name?.ToString()
				});
			}

            return new PagedResultDto<UserAcceptanceRecordMatterContributorLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

        public string GetUserAcceptanceForRecordMatterContributor(Guid? recordMatterContributorId, long? userId)
        {
            if (userId != null) return _userAcceptanceRepository.FirstOrDefault(r => r.RecordMatterContributorId == recordMatterContributorId || r.UserId == userId) == null? L("Pending") : L("Accepted");
            return _userAcceptanceRepository.FirstOrDefault(r => r.RecordMatterContributorId == recordMatterContributorId) == null ? L("Pending") : L("Accepted");
        }

        public string CheckIfUserOrContributorAccepted(UserAcceptanceDto userAcceptanceDto)
        {
            string result = "pending";
            if (userAcceptanceDto.UserId != null && userAcceptanceDto.RecordMatterContributorId == null) // platform users
            {
                result = _userAcceptanceRepository.GetAll().Where(f => f.TenantId == AbpSession.TenantId && f.UserId == userAcceptanceDto.UserId)?.Count() == 0 ? "pending" : "accepted";
            }
            else if(userAcceptanceDto.UserId == null && userAcceptanceDto.RecordMatterContributorId != null)  // Project Contributor
            {
                result = _userAcceptanceRepository.GetAll().Where(f => f.RecordMatterContributorId == userAcceptanceDto.RecordMatterContributorId)?.Count() == 0 ? "pending" : "accepted";
            }

            return result;
        }
 
        ///
        /// VERSION 2.0
        /// 
        [AbpAllowAnonymous]
        public async Task<PagedResultDto<GetUserAcceptanceForViewDto>> GetRequiredAcceptancesForUser(GetRequiredUserAcceptancesInput input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var filteredUserAcceptanceTypes = _userAcceptanceTypeRepository.GetAll()
                            .Where(e => e.Active)
                            .Include(e => e.TemplateFk);

            var pagedAndFilteredUserAcceptanceTypes = filteredUserAcceptanceTypes
                .OrderBy(e => e.Name);

            var userAcceptanceTypes = (from o in pagedAndFilteredUserAcceptanceTypes
                                        join o1 in _userAcceptanceRepository.GetAll()
                                            .Where(e => AbpSession.UserId == null || e.UserId == AbpSession.UserId)
                                            .Where(e => input.RecordMatterContributorId == null || e.RecordMatterContributorId == input.RecordMatterContributorId)
                                        on o.Id equals o1.UserAcceptanceTypeId into j1
                                        from s1 in j1.DefaultIfEmpty()

                                          select new GetUserAcceptanceForViewDto()
                                          {                                                         
                                               UserAcceptance = new UserAcceptanceDto()
                                               { 
                                                   IsAccepted = s1 == null? false : true,
                                                   UserId = AbpSession.UserId,
                                                   RecordMatterContributorId = input.RecordMatterContributorId,
                                                                                                       
                                                   UserAcceptanceType = new UserAcceptanceTypeDto()
                                                   {
                                                        Active = o.Active,
                                                        Id = o.Id,
                                                        Name = o.Name,
                                                        TemplateId = o.TemplateId,
                                                        TemplateContent = s1 == null? GetAcceptanceTypeContentAsHtml(o.TemplateFk.Document): null
                                                   }
                                               }
                                           
                                          }).Where(e => ! e.UserAcceptance.IsAccepted);
 
            var totalCount = await filteredUserAcceptanceTypes.CountAsync();

            return new PagedResultDto<GetUserAcceptanceForViewDto>(
                totalCount,
                await userAcceptanceTypes.ToListAsync()
            );

        }

        private static string GetAcceptanceTypeContentAsHtml(byte[] content)
        {
            string result = String.Empty;
            // Can be anonymous
 
            if(content != null)
            {
                content = AsposeUtility.BytesToHTML(content);
                result = Convert.ToBase64String(content);
                result = string.Format("data:text/html;base64,{0}", result);
            }

            return result;

        }
 
        public async Task<bool> Accept(List<AcceptInput> input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            foreach (AcceptInput item in input)
            {
                var uat = _userAcceptanceTypeRepository.GetAll().Include(e => e.TemplateFk).FirstOrDefault(e => e.Id == new Guid(item.UserAcceptanceTypeId));

                int? tenantId = AbpSession.TenantId;

                Guid? rmcId = Guid.Empty;
                if (!string.IsNullOrEmpty(item.RecordMatterContributorId)){
                    rmcId = new Guid(item.RecordMatterContributorId);
                    var rmc = _lookup_recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.Id == rmcId);                
                    tenantId = rmc?.TenantId;                   
                }
                else{
                    rmcId = null;
                }

                if (uat != null)
                {
                     
                    var userAcceptance = new UserAcceptance()
                    {
                        AcceptedDocTemplateVersion = uat.TemplateFk.CurrentVersion,
                        TenantId = AbpSession.TenantId,
                        UserAcceptanceTypeId = uat.Id,
                        UserId = AbpSession.UserId,
                        RecordMatterContributorId = rmcId
                    };

                    await _userAcceptanceRepository.InsertAsync(userAcceptance);

                }
            }

            _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant);

            return true;

        }

    }
 
}