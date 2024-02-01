using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Forms.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Forms
{
    [AbpAuthorize(AppPermissions.Pages_FormRules)]
    public class FormRulesAppService : FalconAppServiceBase, IFormRulesAppService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<FormRule, Guid> _formRuleRepository;

        private readonly ACLManager _ACLManager;

        public FormRulesAppService(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Form, Guid> formRepository,
            IRepository<FormRule, Guid> formRuleRepository,
            ACLManager aclManager
        )
        {
            _ACLManager = aclManager;
            _formRepository = formRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _formRuleRepository = formRuleRepository;
        }

        public async Task<PagedResultDto<GetFormRuleForViewDto>> GetRulesByFormId(GetAllFormRulesInput input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                var form = _formRepository.FirstOrDefault(e => e.Id == input.Id);

                if (form != null)
                {
                    IQueryable<GetFormRuleForViewDto> filteredFormRules = null;
                    if (input.Sorting == "formRule.rule asc")
                    {
                        filteredFormRules = (from f in _formRuleRepository.GetAll()
                                             where f.FormId == input.Id
                                             && (input.IsEnabledFilter == true ? f.IsEnabled == input.IsEnabledFilter : true)
                                             orderby f.Rule
                                             select new GetFormRuleForViewDto()
                                             {
                                                 FormRule = ObjectMapper.Map<FormRuleDto>(f),
                                                 FormName = form.Name == null ? "" : form.Name
                                             });

                    }
                    else
                    {
                        filteredFormRules = (from f in _formRuleRepository.GetAll()
                                             where f.FormId == input.Id
                                             && (input.IsEnabledFilter == true ? f.IsEnabled == input.IsEnabledFilter : true)
                                             orderby f.Rule descending
                                             select new GetFormRuleForViewDto()
                                             {
                                                 FormRule = ObjectMapper.Map<FormRuleDto>(f),
                                                 FormName = form.Name == null ? "" : form.Name
                                             });
                    }

                    var totalCount = filteredFormRules.ToList().Count();

                    var formRules = filteredFormRules.PageBy(input).ToList();

                    return new PagedResultDto<GetFormRuleForViewDto>(
                        totalCount,
                        formRules
                    );

                }

                return new PagedResultDto<GetFormRuleForViewDto>(
                    0,
                    null
                );
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }


        }

        public List<CreateOrEditFormRuleDto> GetRulesForExport(Guid FormId, string RuleId = null)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = FormId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                IQueryable<FormRule> formRules = _formRuleRepository.GetAll().Where(i => i.FormId == FormId);
                if (!string.IsNullOrEmpty(RuleId))
                {
                    formRules = formRules.Where(i => i.Id == Guid.Parse(RuleId));
                }
                List<CreateOrEditFormRuleDto> RulesToExport = new List<CreateOrEditFormRuleDto>();
                formRules.ToList().ForEach(i =>
                {
                    RulesToExport.Add(new CreateOrEditFormRuleDto()
                    {
                        Id = i.Id,
                        CreationTime = i.CreationTime,
                        CreatorUserId = i.CreatorUserId,
                        TenantId = i.TenantId,
                        Rule = i.Rule,
                        FormId = i.FormId,
                        LastModificationTime = i.LastModificationTime,
                        LastModifierUserId = i.LastModifierUserId
                    });
                });
                return RulesToExport;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }




        }

        public async Task<bool> ToggleRule(Guid Id, bool Toggle)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                var formRule = await _formRuleRepository.GetAsync(Id);
                formRule.IsEnabled = Toggle;
                await Update(formRule);
                return true;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<GetFormRuleForViewDto> GetFormRuleForView(Guid id)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                var formRule = await _formRuleRepository.GetAsync(id);

                var output = new GetFormRuleForViewDto { FormRule = ObjectMapper.Map<FormRuleDto>(formRule) };

                if (output.FormRule.FormId != null)
                {
                    var form = await _formRepository.FirstOrDefaultAsync((Guid)output.FormRule.FormId);
                    output.FormName = form.Name.ToString();
                }

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }


        }

        private Guid? CheckDuplication(FormRuleDto i, List<FormRule> existedFormRules)
        {
            JObject ruleobj = JObject.Parse(i.Rule);
            string rulename = Convert.ToString(ruleobj["name"]);

            foreach (FormRule fr in existedFormRules)
            {
                JObject existedRuleObj = JObject.Parse(fr.Rule);
                string existedrulename = Convert.ToString(existedRuleObj["name"]);
                if (existedrulename == rulename)
                {
                    return fr.Id;
                }
            }
            return null;
        }

		public bool UploadRules(string Id, string Schema)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = new Guid( Id), UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				List<FormRule> existedFormRules = _formRuleRepository.GetAll().Where(f => f.FormId == Guid.Parse(Id)).ToList();
				List<FormRuleDto> UploadedRules = JsonConvert.DeserializeObject<List<FormRuleDto>>(Schema);
				
				UploadedRules.ForEach( i =>
				{

                    using (var unitOfWork = _unitOfWorkManager.Begin())
					{
						var id = CheckDuplication(i, existedFormRules);
						if (id == null)
						{

							FormRule fRule = new FormRule()
							{
								CreationTime = DateTime.Now,
								CreatorUserId = AbpSession.TenantId,
								TenantId = AbpSession.TenantId,
								FormId = Guid.Parse(Id),
								Rule = i.Rule,
							};

							if (AbpSession.TenantId != null)
							{
								fRule.TenantId = AbpSession.TenantId;
							}

							_formRuleRepository.Insert(fRule);

						}
						else
						{

							FormRule fRule = _formRuleRepository.FirstOrDefault((Guid)id);
							fRule.Rule = fRule.Rule;
							_formRuleRepository.Update(fRule);
 
						}
						unitOfWork.Complete();

                    }


                });
				return true;
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}
		}

        public async Task<string> CreateOrEdit(FormRuleDto input)
        {
            FormRule formRule = new FormRule()
            {
                CreationTime = input.CreationTime,
                CreatorUserId = input.CreatorUserId,
                TenantId = AbpSession.TenantId,
                Id = input.Id != null ? Guid.Parse(input.Id.ToString()) : new Guid("00000000-0000-0000-0000-000000000000"),
                FormId = input.FormId,
                Rule = input.Rule
            };

            if (input.Id == null)
            {
                FormRule NewRule = await Create(formRule);
                return NewRule.Id.ToString();
            }
            else
            {

                formRule = await _formRuleRepository.FirstOrDefaultAsync(formRule.Id);
                formRule.Rule = input.Rule;
                await _formRuleRepository.UpdateAsync(formRule);

                // await Update(formRule);
                return input.Id.ToString();
 
            }
        }

        [AbpAuthorize(AppPermissions.Pages_FormRules_Create)]
        private async Task<FormRule> Create(FormRule input)
        {
            var formRule = ObjectMapper.Map<FormRule>(input);

            if (AbpSession.TenantId != null)
            {
                formRule.TenantId = AbpSession.TenantId;
            }

            formRule = await _formRuleRepository.InsertAsync(formRule);
            return formRule;
        }

        [AbpAuthorize(AppPermissions.Pages_FormRules_Edit)]
        private async Task Update(FormRule input)
        {
            FormRule formRule = await _formRuleRepository.FirstOrDefaultAsync(input.Id);
            formRule.Rule = input.Rule;
            await _formRuleRepository.UpdateAsync(formRule);
        }

        [AbpAuthorize(AppPermissions.Pages_FormRules_Delete)]
        public async Task<bool> Delete(EntityDto<Guid> input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Delete", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                await _formRuleRepository.DeleteAsync(input.Id);
                var IsDeleted = _formRuleRepository.FirstOrDefault(input.Id);
                return IsDeleted != null ? true : false;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }
    }
}