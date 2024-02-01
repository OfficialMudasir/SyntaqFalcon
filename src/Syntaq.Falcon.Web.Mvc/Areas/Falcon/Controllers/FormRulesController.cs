using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.FormRules;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AbpMvcAuthorize(AppPermissions.Pages_FormRules)]
	public class FormRulesController : FalconControllerBase
	{
		private readonly IFormRulesAppService _formRulesAppService;

		public FormRulesController(IFormRulesAppService formRulesAppService)
		{
			_formRulesAppService = formRulesAppService;
		}

		public ActionResult Index()
		{
			var model = new FormRulesViewModel
			{
				FilterText = ""
			};

			return View(model);
		}

		public FileResult GetRulesForDownload(string FormId, string RuleId = null)
		{
			string RuleName = null;
			Guid formId = Guid.Parse(FormId);
			List<CreateOrEditFormRuleDto> RulesToExport = _formRulesAppService.GetRulesForExport(formId, RuleId);
			if(RuleId != null)
			{
				dynamic RuleJSON = JsonConvert.DeserializeObject<dynamic>(RulesToExport.First().Rule);
				RuleName = RuleJSON.name;
			}
			string StringRulesToExport = JsonConvert.SerializeObject(RulesToExport);
			byte[] bytes = Encoding.ASCII.GetBytes(StringRulesToExport);
			return File(bytes, System.Net.Mime.MediaTypeNames.Application.Json, RuleId != null ? RuleName + "-export.json" : "rulesexport.json");
		}

		public PartialViewResult ImportRulesModal(string FormId)
		{
			ImportRulesViewModel model = new ImportRulesViewModel()
			{
				Id = FormId
			};
			return PartialView("_ImportRulesModal", model);
		}

		public bool UploadRules(string Id, FileDto input)
		{
			var file = Request.Form.Files.First();

			if (file.Length > 0)
			{
				string str = (new StreamReader(file.OpenReadStream())).ReadToEnd();

				try
				{
					bool valid = IsValidJson(str);

					if (valid)
					{
						var Result = _formRulesAppService.UploadRules(Id, str);
						if (Result == true)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
				}
				catch (Exception ex)
				{
					// Do something about the exception here
					return false;
				}
			}
			return false;
		}

		private static bool IsValidJson(string strInput)
		{
			strInput = strInput.Trim();
			if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
				(strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
			{
				try
				{
					var obj = JToken.Parse(strInput);
					return true;
				}
				catch (JsonReaderException)
				{
					//Exception in parsing json
					//Console.WriteLine(jex.Message);
					return false;
				}
				catch (Exception) //some other exception
				{
					//Console.WriteLine(ex.ToString());
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public async Task<PartialViewResult> ViewFormRuleModal(Guid id)
		{
			var getFormRuleForViewDto = await _formRulesAppService.GetFormRuleForView(id);

			var model = new FormRuleViewModel()
			{
				FormRule = getFormRuleForViewDto.FormRule
, FormName = getFormRuleForViewDto.FormName 

			};

			return PartialView("_ViewFormRuleModal", model);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_FormRules_Create, AppPermissions.Pages_FormRules_Edit)]
		public PartialViewResult FormLookupTableModal(Guid? id, string displayName)
		{
			var viewModel = new FormLookupTableViewModel()
			{
				Id = id.ToString(),
				DisplayName = displayName,
				FilterText = ""
			};

			return PartialView("_FormLookupTableModal", viewModel);
		}

	}
}