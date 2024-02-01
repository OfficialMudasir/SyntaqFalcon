using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Authorization.Users.Dto;
//using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Payments;
using Syntaq.Falcon.Payments.Dto;
using Syntaq.Falcon.Settings.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Payments;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	public class PaymentsController : FalconControllerBase
	{
		private readonly ITenantAppService _tenantAppService;
		private readonly IFormsAppService _formsAppService;
		private readonly IUserAppService _userAppService;
		private readonly PaymentManager _paymentManager;

		private readonly IOptions<StripeConnection> _stripeConnection;


		public PaymentsController(ITenantAppService tenantAppService, IFormsAppService formsAppService, IUserAppService userAppService, PaymentManager paymentManager, IOptions<StripeConnection> stripeConnection)
		{
			_tenantAppService = tenantAppService;
			_formsAppService = formsAppService;
			_userAppService = userAppService;
			_paymentManager = paymentManager;
			_stripeConnection = stripeConnection;
		}

		[HttpGet]
		public ActionResult GetStripeConfiguration(string sectionName, string paramName)
		{
			return Json(new { _stripeConnection.Value.PublishableKey });
		}

		[AllowAnonymous]
		public ActionResult StripeConfirm()
		{
			try
			{


				StripeResponseDto StripeResponse = JsonConvert.DeserializeObject<StripeResponseDto>(HttpContext.Request.Query["state"].ToString());
				string StripeCode = HttpContext.Request.Query["code"];

				//if (HttpContext.Request["error_description"] != null)
				//{
				//	HttpContext.Session["error_description"] = Request["error_description"];
				//	Response.Redirect("/");
				//}

				//WebRequest request = HttpWebRequest.Create(UserConstants.StripeBaseURL + STRIPE_CLIENTID + "&code=" + code);
				//WebRequest request = HttpWebRequest.Create("https://connect.stripe.com/oauth/token?grant_type=authorization_code&client_id=ca_4hViPiy34RAG942jq8uZBog5H3KrtYsW&code=" + StripeCode);     // Test

				WebRequest request = HttpWebRequest.Create("https://connect.stripe.com/oauth/token?grant_type=authorization_code&client_id=ca_4hViuZYu6vXAfhq4hfbHPP7fCELmDIZ1&code=" + StripeCode); // Live

				request.Method = "POST";
				//request.Headers.Add("Authorization", "Bearer " + "sk_test_GQs911aBSBbTR0RsXgCdNVb5"); // needs to ne secret key from appsettings// Test
				//request.Headers.Add("Authorization", "Bearer " + "pk_test_Sn2vUvQHzTTIkP0JH8AxtKuA"); // SYNTAQ :/ 
				request.Headers.Add("Authorization", "Bearer " + "sk_live_t2hxVwZ1SauVnuasRBwip1Om"); // Live :/ 

				//request.Headers.Add("Authorization", "Bearer " + _stripeConnection.Value.PublishableKey); 

				var response = request.GetResponse();

				Stream dataStream = response.GetResponseStream();
				StreamReader reader = new StreamReader(dataStream);
				string responseFromServer = reader.ReadToEnd();

				Dictionary<string, string> responseValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseFromServer);

				StripeTokensDto stripeTokensDto = new StripeTokensDto()
				{
					StripeResponse = StripeResponse,
					AccessToken = responseValues["access_token"],
					RefreshToken = responseValues["refresh_token"],
					PublishableKey = responseValues["stripe_publishable_key"]
				};

				// Clean up the streams and the response.
				reader.Close();
				response.Close();

				// Store Stripe Tokens
				bool HasUpdated = _paymentManager.SetStripeTokensAsync(stripeTokensDto).Result;

				if (HasUpdated)
				{
					return View("_StripeConfirmed");
				}
				else
				{
					//send back an error view
					return View("_StripeConfirmed");
				}
			}
			catch (Exception ex)
			{


				Abp.Web.Mvc.Models.ErrorViewModel error = new Abp.Web.Mvc.Models.ErrorViewModel();
				error.ErrorInfo = new Abp.Web.Models.ErrorInfo();
				error.ErrorInfo.Message = ex.Message;
				return View("../Error", error);
			}
		}

		public ActionResult Error(string message)
		{

			//send back an error view
			return View("_Error", message);
		}

		//[AbpMvcAuthorize(AppPermissions.Pages_Records_Create, AppPermissions.Pages_Records_Edit)]
		//[Authorize(Policy = "EditById")]
		public async Task<PartialViewResult> PaymentsPartial(string Type, Guid? OriginalId, string Version)
		{
			var model = new PaymentsPartialViewModel();
			switch (Type)
			{
				case "User":
					GetUserForEditOutput _User = await _userAppService.GetUserForEdit(new NullableIdDto<long>() { Id = AbpSession.UserId });
					if (_User != null)
					{
						model.ForView = "User";
						model.HasPaymentConfigured = _User.User.HasPaymentConfigured;
						model.PaymentCurrency = _User.User.PaymentCurrency;
					}
					break;
				case "Form":
					GetFormForEditOutput _Form = await _formsAppService.GetFormForEdit(new GetFormForViewDto() { OriginalId = OriginalId, Version = Version });
					if (_Form != null)
					{
						model.ForView = "Form";
						model.HasPaymentConfigured = _Form.Form.PaymentPublishableToken != null ? true : false;
						model.IsPaymentEnabled = _Form.Form.PaymentEnabled;
						model.PaymentAmount = _Form.Form.PaymentAmount;
						model.PaymentCurrency = _Form.Form.PaymentCurrency;
						model.PaymentProcess = _Form.Form.PaymentProcess;
					}
					break;
				case "Tenant":

					break;
			}
			return PartialView("_PaymentsPartial", model);
		}
	}
}