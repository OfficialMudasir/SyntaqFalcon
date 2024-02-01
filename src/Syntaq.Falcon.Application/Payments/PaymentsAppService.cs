using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using RestSharp;
using Stripe;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Payments.Dto;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Vouchers;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.Web;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Payments
{

#if STQ_PRODUCTION

    [EnableCors("AllowAll")]
    public class PaymentsAppService : FalconAppServiceBase, IPaymentsAppService
	{
		private readonly ACLManager _ACLManager;
		private readonly IRepository<ACL> _aclRepository;
		private readonly SubmissionManager _submissionManager;
		private readonly IRepository<Form, Guid> _formRepository;
		private readonly IRepository<User, long> _userRepository;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
        //private readonly IRepository<Tenant, long> _tenantRepository; //not working currently
        private readonly IVoucherManager _voucherManager;

        private readonly IOptions<StGeorgeConfig> _stGeorgeConfig;

        private readonly IRepository<Syntaq.Falcon.Apps.App, Guid> _appRepository;

        private readonly IRepository<Syntaq.Falcon.Submissions.Submission, Guid> _submissionRepository;

        public PaymentsAppService(
            ACLManager aclManager,
            IRepository<ACL> aclRepository, 
            SubmissionManager submissionManager, 
            IRepository<Form, Guid> formRepository, 
            IRepository<User, long> userRepository, 
            IUnitOfWorkManager unitOfWorkManager,
            IVoucherManager voucherManager,
            IRepository<AppJob, Guid> appJobRepository,
            IRepository<Syntaq.Falcon.Apps.App, Guid> appRepository,
            IRepository<Syntaq.Falcon.Submissions.Submission, Guid> submissionRepository,
            IOptions<StGeorgeConfig> stGeorgeConfig
        )
		{
			_ACLManager = aclManager;
			_aclRepository = aclRepository;
			_submissionManager = submissionManager;
			_formRepository = formRepository;
			_userRepository = userRepository;
			_unitOfWorkManager = unitOfWorkManager;
            _voucherManager = voucherManager;
            _appRepository = appRepository;
            _stGeorgeConfig = stGeorgeConfig;
            _submissionRepository = submissionRepository;
        }

		public async Task UpdatePaymentSettingsAsync(UpdatePaymentSettingsDto input)
		{
			switch (input.EntityType)
			{
				case "User":
					User _User = await GetCurrentUserAsync();
					if (_User != null)
					{
						_User.HasPaymentConfigured = input.HasPaymentConfigured;
						_User.PaymentCurrency = input.PaymentCurrency;
						_User.PaymentProvider = input.PaymentProvider;
					}
					await UserManager.UpdateAsync(_User);
                    //CheckErrors(await UserManager.UpdateAsync(_User));
                    break;
				case "Form":
					Form _Form = _formRepository.FirstOrDefault(Guid.Parse(input.EntityId));
					if (_Form != null)
					{
						_Form.PaymentEnabled = input.IsPaymentEnabled;
						_Form.PaymentAmount = input.PaymentAmount;
						_Form.PaymentCurrency = input.PaymentCurrency;
						_Form.PaymentProcess = input.PaymentProcess;
					}
					_formRepository.Update(_Form);
					break;
				case "Tenant":

					break;
			}
		}

		public async Task<PaymentChargeResponseDto> ChargeCardStripe(StripeChargeDto stripeCharge)
		{
			//Syntaq Secret Test Key StripeConfiguration.SetApiKey("sk_test_GQs911aBSBbTR0RsXgCdNVb5");
			User user = null;
			Form form = null;
			//Lookup form from formid and fetch payment amount, payment currency, payment access token, 
			using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
			{
				if (!string.IsNullOrEmpty(stripeCharge.FormId.ToString()))
				{
					form = _formRepository.Get(stripeCharge.FormId);
					if (string.IsNullOrEmpty(form.PaymentAccessToken))
					{
						IQueryable<ACL> ACLs = _aclRepository.GetAll().Where(i => i.EntityID == stripeCharge.FormId && i.Role == "O");

						user = _userRepository.Get((long)ACLs.First().UserId);
						if (string.IsNullOrEmpty(user.PaymentAccessToken))
						{
							//Do Tenant Payment Details lookup here or error
						}
						else
						{
                            //Depreciated - figure out replacement
							//StripeConfiguration.SetApiKey(user.PaymentAccessToken);
                            StripeConfiguration.SetApiKey("sk_test_GQs911aBSBbTR0RsXgCdNVb5");
                        }
					}
					else
					{
                        //Depreciated - figure out replacement
                        StripeConfiguration.SetApiKey(form.PaymentAccessToken);
					}
					CurrentUnitOfWork.SaveChanges();
				}
			}

            //var frm = _formRepository.FirstOrDefault(e => e.Id == stripeCharge.FormId);
            

            //if(frm != null)
            //{
            //    PaymentAmount = (long) frm.PaymentAmount;
            //}


            decimal? voucheramount = 0;
            long paymentAmount = 0;

            // Check Payment Amout through Schema Check
            if (stripeCharge.FormData != null)
            {
                JObject SubData = JsonConvert.DeserializeObject<JObject>(stripeCharge.FormData);
                //if (!ValidatePaymentAmount(SubData, stripeCharge.FormId))
                //{
                //    throw new UserFriendlyException("Payment or Submission is invalid");
                //}

                if (SubData["PaidAmount"] != null)
                {
                    paymentAmount = Convert.ToInt64(SubData["PaidAmount"]);
                }

                if (SubData["VoucherAmount"] != null)
                {
                    voucheramount = Convert.ToDecimal(SubData["VoucherAmount"]);
                }
 
            }

            // Check Voucher
            // Not checking the Voucher Server side
            //if (! string.IsNullOrEmpty(stripeCharge.VoucherKey))
            //{
            //    VoucherValidityDto vouchercheck =  _voucherManager.GetVoucherDetailsByKey(
            //    new GetVoucherDetailsByKeyInput()
            //    {
            //        Balance = Convert.ToString(PaymentAmount),
            //        EntityId =  form.Id , 
            //        VoucherKey = stripeCharge.VoucherKey
            //    }).Result;

            //    if (vouchercheck != null)
            //    {
            //        if (vouchercheck.VoucherValid)
            //        {
            //            PaymentAmount = PaymentAmount - (long)vouchercheck.VoucherValue;
            //            voucheramount = vouchercheck.VoucherValue;

            //            if (vouchercheck.DiscountType == "Percentage")
            //            {
            //                PaymentAmount = PaymentAmount - (PaymentAmount * ((long)vouchercheck.VoucherValue / 100)); ;
            //                voucheramount =  PaymentAmount * ((long)vouchercheck.VoucherValue / 100) ;
            //            }

                        
            //        }
            //    }
            //}

            //decimal AmountPaid = form.PaymentAmount;
			var Options = new ChargeCreateOptions
			{
				Amount = form.PaymentCurrency != "JPY" ? paymentAmount * 100 : paymentAmount,
				Currency = form.PaymentCurrency,
                Source = stripeCharge.Token
			};

			Charge Charge = new Charge();

			try
			{
				var chargeService = new ChargeService();
				Charge = chargeService.Create(Options);
			}
			catch(StripeException Ex)
			{
				throw new UserFriendlyException(Ex.Message);
			}

			Guid SubmissionId = stripeCharge.SubmissionId.ToString() == "00000000-0000-0000-0000-000000000000" ? Guid.NewGuid() : (Guid)stripeCharge.SubmissionId;
			Submission NewSubmission = await _submissionManager.CreateAndOrFetchSubmission(new CreateOrEditSubmissionDto()
			{
				Id = stripeCharge.SubmissionId,
				FormId = stripeCharge.FormId
			});

			_submissionManager.UpdateSubmission(new CreateOrEditSubmissionDto()
			{
				Id = stripeCharge.SubmissionId,
				TenantId = NewSubmission.TenantId,
				AccessToken = NewSubmission.AccessToken,
				RequiresPayment = form.PaymentEnabled,
				PaymentCurrency = form.PaymentCurrency,
				PaymentStatus = Charge.Status == "succeeded" ? "Paid" : "Unpaid",
				PaymentAmount = form.PaymentAmount,
				VoucherAmount = voucheramount,
				AmountPaid = paymentAmount,
				ChargeId = Charge.Id,
				SubmissionStatus = Charge.Status == "succeeded" ? NewSubmission.SubmissionStatus == "Awaiting Payment" ? "Started" : NewSubmission.SubmissionStatus : "Awaiting Payment",
				RecordId = NewSubmission.RecordId,
				RecordMatterId = NewSubmission.RecordMatterId,
				UserId = NewSubmission.UserId ?? user?.Id,
				AppId = NewSubmission.AppId,
				AppJobId = NewSubmission.AppJobId,
				FormId = stripeCharge.FormId,
			});

			//process successful and unsuccessful charge amounts
			if (Charge.Status == "succeeded")
			{
				PaymentChargeResponseDto succeededResponseDto = new PaymentChargeResponseDto()
				{
					PaymentSuccess = true,
					PaymentProcess = form.PaymentProcess
				};
				return succeededResponseDto;
			}
			else
			{
				PaymentChargeResponseDto failedResponseDto = new PaymentChargeResponseDto()
				{
					PaymentSuccess = false,
					PaymentProcess = ""
				};
				return failedResponseDto;
			}
		}

        public async Task<PaymentChargeResponseDto> ChargeCard(StGeorgeChargeDto stGeorgeCharge)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            PaymentChargeResponseDto resultDto = new PaymentChargeResponseDto()
            {
                PaymentSuccess = false
            };

            User user = null;
            Form form = null;

            //Lookup form from formid and fetch payment amount, payment currency, payment access token,                  
            if (!string.IsNullOrEmpty(Convert.ToString(stGeorgeCharge.FormId)))
            {
                form = _formRepository.Get( new Guid(stGeorgeCharge.FormId));
            }

            JObject SubData = JsonConvert.DeserializeObject<JObject>(stGeorgeCharge.FormData );
            if(!ValidatePaymentAmount(SubData, new Guid(stGeorgeCharge.FormId)))
            {
                throw new UserFriendlyException("Payment or Submission is invalid");
            }

            long paymentAmount = 0;
            decimal? voucheramount = 0; 

            // Check Payment Amout through Schema Check
            if (stGeorgeCharge.FormData != null)
            {

                //JObject SubData = JsonConvert.DeserializeObject<JObject>(stripeCharge.FormData);
                //if (!ValidatePaymentAmount(SubData, stripeCharge.FormId))
                //{
                //    throw new UserFriendlyException("Payment or Submission is invalid");
                //}

                if (SubData["PaidAmount"] != null)
                {
                    paymentAmount = Convert.ToInt64(SubData["PaidAmount"]);
                }

                if (SubData["VoucherAmount"] != null)
                {
                    voucheramount = Convert.ToDecimal(SubData["VoucherAmount"]);
                }

            }

            // Take payment amount from Client but validate from schema
            //long amountpaid = (long)form.PaymentAmount;

            // Check Voucher
            //VoucherValidityDto vouchercheck = new VoucherValidityDto();
            //if (!string.IsNullOrEmpty(stGeorgeCharge.VoucherKey))
            //{
            //    vouchercheck = _voucherManager.GetVoucherDetailsByKey(
            //    new GetVoucherDetailsByKeyInput()
            //    {
            //        Balance = Convert.ToString(amountpaid),
            //        EntityId =  form.Id ,
            //        VoucherKey = stGeorgeCharge.VoucherKey
            //    }).Result;

            //    if (vouchercheck != null)
            //    {
            //        if (vouchercheck.VoucherValid)
            //        {
            //            // PaymentAmount = PaymentAmount - (long)vouchercheck.VoucherValue;

            //            var docamount = amountpaid - (amountpaid / 11); // 10% GST

            //            // Take the voucher value of the document Pre GST
            //            if (vouchercheck.DiscountType == "Percentage")
            //            {
            //                vouchercheck.VoucherValue = docamount * ((long)vouchercheck.VoucherValue / 100);

            //                docamount = (long)(docamount - vouchercheck.VoucherValue); 
            //                docamount = docamount < 0 ? 0 : docamount;
            //                amountpaid = (long)(docamount * 1.1);
            //            }

            //            if (vouchercheck.DiscountType == "Fixed")
            //            {
            //                docamount = docamount - (long) vouchercheck.VoucherValue;
            //                docamount = docamount < 0 ? 0 : docamount;
            //                amountpaid = (long)(docamount * 1.1);
            //            }

            //        }
            //    }
            //}

            try
            {

                var client = new RestClient(_stGeorgeConfig.Value.APIEndPoint);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("accept", "application/json");
                request.AddHeader("content-type", "application/json");

                var chargeamount = String.Format("{0:0.00}", paymentAmount);

                StGeorgeCharge charge = new StGeorgeCharge()
                {
                    authenticationtoken = _stGeorgeConfig.Value.AccessToken,
                    clientid = _stGeorgeConfig.Value.ClientID,
                    carddata = stGeorgeCharge.CardNumber.Replace(" ", ""),
                    cardexpirydate = stGeorgeCharge.CardMonth + stGeorgeCharge.CardYear,
                    cvc2 = stGeorgeCharge.CardCVC2,
                    totalamount = chargeamount
                };

                request.AddJsonBody(charge);

                IRestResponse response = client.Execute(request);

                resultDto.PaymentMessage = response.StatusDescription;
                resultDto.PaymentProcess = form.PaymentProcess;

                if (response.IsSuccessful)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        Newtonsoft.Json.Linq.JObject jresponse = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                        var responsecode = Convert.ToString(jresponse["responsecode"]);
                        var resonsemessage = Convert.ToString(jresponse["responsetext"]);
                        var resonseeerror = Convert.ToString(jresponse["error"]);

                        resultDto.PaymentMessage = resonsemessage + ": " + resonseeerror;

                        if (responsecode == "00" || responsecode == "08")
                        {
                            resultDto.PaymentSuccess = true;

                            Guid SubmissionId = stGeorgeCharge.SubmissionId.ToString() == "00000000-0000-0000-0000-000000000000" ? Guid.NewGuid() : new Guid(stGeorgeCharge.SubmissionId);

                            // Submission manager not commiting correctly so using repository
                            Submission Submission;
                            if (!_submissionRepository.GetAll().Any(i => i.Id == SubmissionId))
                            {                                
                                Submission = new Submission();

                                Submission.Id = SubmissionId;
                                Submission.FormId = form.Id;
                                Submission.RequiresPayment = form.PaymentEnabled;
                                Submission.PaymentStatus = "Paid";
                                Submission.PaymentAmount = (long)form.PaymentAmount;
                                Submission.VoucherAmount = voucheramount;
                                Submission.AmountPaid = paymentAmount;
                                Submission.ChargeId = Convert.ToString(jresponse["txnreference"]);
                                Submission.SubmissionStatus = "Paid";

                                await _submissionRepository.InsertAsync(Submission);
                            }
                            else
                            {

                                Submission = _submissionRepository.Get((Guid)SubmissionId);

                                Submission.RequiresPayment = form.PaymentEnabled;
                                Submission.PaymentStatus = "Paid";
                                Submission.PaymentAmount = (long)form.PaymentAmount;
                                Submission.VoucherAmount = voucheramount;
                                Submission.AmountPaid = paymentAmount;
                                Submission.ChargeId = Convert.ToString(jresponse["txnreference"]);
                                Submission.SubmissionStatus = "Paid";

                                _submissionRepository.Update(Submission);
                            }

                            _unitOfWorkManager.Current.SaveChanges();

                            //Submission NewSubmission = await _submissionManager.CreateAndOrFetchSubmission(new CreateOrEditSubmissionDto()
                            //{
                            //    Id = new Guid(stGeorgeCharge.SubmissionId),
                            //    FormId = new Guid(stGeorgeCharge.FormId), 
                            //    TenantId = form.TenantId,
                            //    RequiresPayment = form.PaymentEnabled,
                            //    PaymentCurrency = form.PaymentCurrency,
                            //    PaymentStatus = resultDto.PaymentSuccess ? "Paid" : "Unpaid",
                            //    PaymentAmount = (long)form.PaymentAmount,
                            //    VoucherAmount = vouchercheck.VoucherValue,
                            //    AmountPaid = amountpaid, 
                            //    ChargeId = Convert.ToString(jresponse["txnreference"]), // Todo
                            //    SubmissionStatus = "Paid",
                            //});


                            //_submissionManager.UpdateSubmission(new CreateOrEditSubmissionDto()
                            //{
                            //    Id = new Guid(stGeorgeCharge.SubmissionId),
                            //    TenantId = NewSubmission.TenantId,
                            //    AccessToken = NewSubmission.AccessToken,
                            //    RequiresPayment = form.PaymentEnabled,
                            //    PaymentCurrency = form.PaymentCurrency,
                            //    PaymentStatus = resultDto.PaymentSuccess ? "Paid" : "Unpaid",

                            //    PaymentAmount = (long)form.PaymentAmount,
                            //    VoucherAmount = vouchercheck.VoucherValue,
                            //    AmountPaid = amountpaid, // Convert.ToDecimal(stGeorgeCharge.CardAmount),

                            //    ChargeId = Convert.ToString(jresponse["txnreference"]), // Todo
                            //    SubmissionStatus = resultDto.PaymentSuccess ? NewSubmission.SubmissionStatus == "Awaiting Payment" ? "Started" : NewSubmission.SubmissionStatus : "Awaiting Payment",
                            //    RecordId = NewSubmission.RecordId,
                            //    RecordMatterId = NewSubmission.RecordMatterId,
                            //    UserId = NewSubmission.UserId ?? user?.Id,
                            //    AppId = NewSubmission.AppId,
                            //    AppJobId = NewSubmission.AppJobId,
                            //    FormId = new Guid(stGeorgeCharge.FormId),
                            //});


                        }
                        //else
                        //{
                        //    resultDto.PaymentSuccess = false;

                        //    Guid SubmissionId = stGeorgeCharge.SubmissionId.ToString() == "00000000-0000-0000-0000-000000000000" ? Guid.NewGuid() : new Guid(stGeorgeCharge.SubmissionId);
                        //    Submission NewSubmission = await _submissionManager.CreateAndOrFetchSubmission(new CreateOrEditSubmissionDto()
                        //    {
                        //        Id = new Guid(stGeorgeCharge.SubmissionId),
                        //        FormId = new Guid(stGeorgeCharge.FormId),

                        //        TenantId = form.TenantId,
                        //        RequiresPayment = form.PaymentEnabled,
                        //        PaymentCurrency = form.PaymentCurrency,
                        //        PaymentStatus = "Payment Failed",
                        //        PaymentAmount = (long)form.PaymentAmount,
                        //        VoucherAmount = vouchercheck.VoucherValue,
                        //        AmountPaid = amountpaid,  
                        //        ChargeId = Convert.ToString(jresponse["txnreference"]), // Todo
                        //        SubmissionStatus = resultDto.PaymentMessage, 

                        //    });

                        //}

                    }

                }


            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

 
            return resultDto;

        }
 
        private Boolean ValidatePaymentAmount(JObject submission, Guid? entityId)
        {

            var result = true;

            string schemaJson = string.Empty;

            // Example Payment Schema
            //schemaJson = @"{
            //  'properties': {
            //    'PaymentAmount': {'type': 'string'}
            //  },
            //  'if': {
	           // 'properties': { 'rate': { 'const': 1 } }
            //   },
            //  'then': {
	           // 'properties': { 'PaymentAmount': { 'const': '120.00'}}
            //  },
            //  'else': {
	           // 'properties': { 'PaymentAmount': { 'const': '100.00'}}
            //  }
            //}";

            if (entityId != null)
            {
                var form = _formRepository.FirstOrDefault(e => e.Id == entityId);
                if (form != null)
                    schemaJson = form.RulesSchema;

                var app = _appRepository.FirstOrDefault(e => e.Id == entityId);
                if (app != null)
                    schemaJson = app.RulesSchema;
            }

            if (!string.IsNullOrEmpty(schemaJson))
            {
                try
                {
                    JSchema rulesSchema = JSchema.Parse(schemaJson);
                    result = submission.IsValid(rulesSchema);
                }
                catch
                {
                    // If Error reject test
                    result = false;
                }
            }

            return result;

        }

        // POC 
        // Query String Value Key Pair
        private long getPaymentAmount(Form form, string formdata)
        {

            var paymentamount = (long)form.PaymentAmount;


            JObject jobj = JObject.Parse(formdata);

            //JToken acme = jobj.SelectToken("$.Manufacturers[?(@.Name == 'Acme Co')]");
            JToken acme = jobj.SelectToken("$.[?(@.Name == 'Bruce')]");



            return paymentamount;

        }

    }

#endif

}