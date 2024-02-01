using Syntaq.Falcon.Documents;
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
using Syntaq.Falcon.Documents.Dtos;
using GetAllForLookupTableInput = Syntaq.Falcon.Users.Dtos.GetAllForLookupTableInput;

namespace Syntaq.Falcon.Users
{
    /*[AbpAuthorize(AppPermissions.Pages_UserAcceptanceTypes)]*/
    [AbpAuthorize]
    public class UserAcceptanceTypesAppService : FalconAppServiceBase, IUserAcceptanceTypesAppService
    {
        private readonly IRepository<UserAcceptanceType, Guid> _userAcceptanceTypeRepository;
        private readonly IUserAcceptanceTypesExcelExporter _userAcceptanceTypesExcelExporter;
        private readonly IRepository<Template,Guid> _lookup_templateRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ITemplatesAppService _templatesAppService;

        public UserAcceptanceTypesAppService(ITemplatesAppService templatesAppService, IUnitOfWorkManager unitOfWorkManager, IRepository<UserAcceptanceType, Guid> userAcceptanceTypeRepository, IUserAcceptanceTypesExcelExporter userAcceptanceTypesExcelExporter , IRepository<Template, Guid> lookup_templateRepository) 
		  {
			_userAcceptanceTypeRepository = userAcceptanceTypeRepository;
			_userAcceptanceTypesExcelExporter = userAcceptanceTypesExcelExporter;
			_lookup_templateRepository = lookup_templateRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _templatesAppService = templatesAppService;
          }

        [AbpAuthorize(AppPermissions.Pages_UserAcceptanceTypes)]
        public async Task<PagedResultDto<GetUserAcceptanceTypeForViewDto>> GetAll(GetAllUserAcceptanceTypesInput input)
         {
            input.Filter = input.Filter?.Trim();

            var filteredUserAcceptanceTypes = _userAcceptanceTypeRepository.GetAll()
						.Include( e => e.TemplateFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(input.ActiveFilter > -1,  e => Convert.ToInt32(e.Active) == input.ActiveFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.TemplateNameFilter), e => e.TemplateFk != null && e.TemplateFk.Name.ToLower() == input.TemplateNameFilter.ToLower().Trim());

			var pagedAndFilteredUserAcceptanceTypes = filteredUserAcceptanceTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var userAcceptanceTypes = from o in pagedAndFilteredUserAcceptanceTypes
                         join o1 in _lookup_templateRepository.GetAll() on o.TemplateId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetUserAcceptanceTypeForViewDto() {
							UserAcceptanceType = new UserAcceptanceTypeDto
							{
                                Name = o.Name,
                                Active = o.Active,
                                Id = o.Id
							},
                         	TemplateName = s1 == null ? "" : s1.Name.ToString()
						};

            var totalCount = await filteredUserAcceptanceTypes.CountAsync();

            return new PagedResultDto<GetUserAcceptanceTypeForViewDto>(
                totalCount,
                await userAcceptanceTypes.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_UserAcceptanceTypes_Edit)]
		 public async Task<GetUserAcceptanceTypeForEditOutput> GetUserAcceptanceTypeForEdit(EntityDto<Guid> input)
         {
            var userAcceptanceType = await _userAcceptanceTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserAcceptanceTypeForEditOutput {UserAcceptanceType = ObjectMapper.Map<CreateOrEditUserAcceptanceTypeDto>(userAcceptanceType)};

		    if (output.UserAcceptanceType.TemplateId != null)
            {
                var _lookupTemplate = await _lookup_templateRepository.FirstOrDefaultAsync((Guid)output.UserAcceptanceType.TemplateId);
                output.TemplateName = _lookupTemplate.Name.ToString();
            }
			
            return output;
         }

         public async Task<GetUserAcceptanceTypeForEditOutput> GetUserAcceptanceTypeForView(EntityDto<Guid> input)
         {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            var userAcceptanceType = await _userAcceptanceTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserAcceptanceTypeForEditOutput {UserAcceptanceType = ObjectMapper.Map<CreateOrEditUserAcceptanceTypeDto>(userAcceptanceType)};

		    if (output.UserAcceptanceType.TemplateId != null)
            {
                var _lookupTemplate = await _lookup_templateRepository.FirstOrDefaultAsync((Guid)output.UserAcceptanceType.TemplateId);
                output.TemplateName = _lookupTemplate.Name.ToString();
            }
			
            return output;
         }

        public async Task<List<UserAcceptanceTypeDto>> GetAllActiveUserAcceptanceTypesForView()
        {

            return new List<UserAcceptanceTypeDto>();

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            var result = _userAcceptanceTypeRepository.GetAll()
                .Where(e => e.Active)
                .OrderBy(e =>e.CreationTime)
                .Select(f => new UserAcceptanceTypeDto { Id = f.Id, Name = f.Name, Active = f.Active, TemplateId = f.TemplateId })
                .ToList();

            foreach (var item in result)
            {
                GetTemplateForEditOutput getTemplateForEditOutput = await _templatesAppService.GetTemplateForUserAcceptanceViewers(new EntityDto<Guid> { Id = (Guid)item.TemplateId });
                item.CurrentAcceptanceDocTemplateVersion = getTemplateForEditOutput.Template.Version;
            }

            return result.Distinct().ToList();
        }

        public async Task CreateOrEdit(CreateOrEditUserAcceptanceTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_UserAcceptanceTypes_Create)]
		 private async Task Create(CreateOrEditUserAcceptanceTypeDto input)
         {
            var userAcceptanceType = ObjectMapper.Map<UserAcceptanceType>(input);

			
			if (AbpSession.TenantId != null)
			{
				userAcceptanceType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _userAcceptanceTypeRepository.InsertAsync(userAcceptanceType);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserAcceptanceTypes_Edit)]
		 private async Task Update(CreateOrEditUserAcceptanceTypeDto input)
         {
            var userAcceptanceType = await _userAcceptanceTypeRepository.FirstOrDefaultAsync((Guid)input.Id);
             ObjectMapper.Map(input, userAcceptanceType);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserAcceptanceTypes_Delete)]
         public async Task Delete(EntityDto<Guid> input)
         {
            await _userAcceptanceTypeRepository.DeleteAsync(input.Id);
         }

        [AbpAuthorize(AppPermissions.Pages_UserAcceptanceTypes)]
        public async Task<FileDto> GetUserAcceptanceTypesToExcel(GetAllUserAcceptanceTypesForExcelInput input)
         {
			
			var filteredUserAcceptanceTypes = _userAcceptanceTypeRepository.GetAll()
						.Include( e => e.TemplateFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(input.ActiveFilter > -1,  e => Convert.ToInt32(e.Active) == input.ActiveFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.TemplateNameFilter), e => e.TemplateFk != null && e.TemplateFk.Name.ToLower() == input.TemplateNameFilter.ToLower().Trim());

			var query = (from o in filteredUserAcceptanceTypes
                         join o1 in _lookup_templateRepository.GetAll() on o.TemplateId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetUserAcceptanceTypeForViewDto() { 
							UserAcceptanceType = new UserAcceptanceTypeDto
							{
                                Name = o.Name,
                                Active = o.Active,
                                Id = o.Id
							},
                         	TemplateName = s1 == null ? "" : s1.Name.ToString()
						 });


            var userAcceptanceTypeListDtos = await query.ToListAsync();

            return _userAcceptanceTypesExcelExporter.ExportToFile(userAcceptanceTypeListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_UserAcceptanceTypes)]
         public async Task<PagedResultDto<UserAcceptanceTypeTemplateLookupTableDto>> GetAllTemplateForLookupTable(GetAllForLookupTableInput input)
         {

            var query = _lookup_templateRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e=> e.Name.Contains(input.Filter))
                .Where(e=> e.Version == e.CurrentVersion); // Only include the live version

            var totalCount = await query.CountAsync();

            var templateList = await query
                .PageBy(input)
                .OrderBy(x => x.Name)
                .ToListAsync();

			var lookupTableDtoList = new List<UserAcceptanceTypeTemplateLookupTableDto>();
			foreach(var template in templateList){
				lookupTableDtoList.Add(new UserAcceptanceTypeTemplateLookupTableDto
				{
					Id = template.Id.ToString(),
					DisplayName = template.Name?.ToString()
				});
			}

            return new PagedResultDto<UserAcceptanceTypeTemplateLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}