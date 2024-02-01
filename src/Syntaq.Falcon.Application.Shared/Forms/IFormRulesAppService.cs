using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Forms
{
    public interface IFormRulesAppService : IApplicationService 
    {
        //Task<PagedResultDto<GetFormRuleForViewDto>> GetAll(GetAllFormRulesInput input);  

        List<CreateOrEditFormRuleDto> GetRulesForExport(Guid FormId, string RuleId = null);

        Task<GetFormRuleForViewDto> GetFormRuleForView(Guid id);

        Task<bool> ToggleRule(Guid Id, bool Toggle);

        //Task<GetFormRuleForEditOutput> GetFormRuleForEdit(EntityDto<Guid> input);

        Task<string> CreateOrEdit(FormRuleDto input);

        Task<bool> Delete(EntityDto<Guid> input);
		
		//Task<PagedResultDto<FormLookupTableDto>> GetAllFormForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<GetFormRuleForViewDto>> GetRulesByFormId(GetAllFormRulesInput input);

        bool UploadRules(string Id, string Schema);
    }
}