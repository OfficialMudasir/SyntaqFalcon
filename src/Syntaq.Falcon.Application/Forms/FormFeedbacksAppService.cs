using Syntaq.Falcon.Forms;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.Forms.Exporting;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Abp.UI;
using Abp.Domain.Uow;
using Syntaq.Falcon.Authorization.Users;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.AccessControlList;

namespace Syntaq.Falcon.Forms
{
    //TODO: Need to desgin what AppPermissions it need.
    [EnableCors("AllowAll")]
    //[AbpAuthorize(AppPermissions.Pages_FormFeedbacks)]
    public class FormFeedbacksAppService : FalconAppServiceBase, IFormFeedbacksAppService
    {
		 private readonly IRepository<FormFeedback, Guid> _formFeedbackRepository;
		 private readonly IFormFeedbacksExcelExporter _formFeedbacksExcelExporter;
		 private readonly IRepository<Form,Guid> _lookup_formRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<User, long> _userRepository;

        private readonly ACLManager _ACLManager;

        public FormFeedbacksAppService(
            ACLManager aclManager,
            IRepository<FormFeedback, Guid> formFeedbackRepository, 
            IFormFeedbacksExcelExporter formFeedbacksExcelExporter , 
            IRepository<Form, Guid> lookup_formRepository,
            IRepository<User, long> userRepository,
            IUnitOfWorkManager unitOfWorkManager) 
		  {
            _ACLManager = aclManager;
            _formFeedbackRepository = formFeedbackRepository;
			_formFeedbacksExcelExporter = formFeedbacksExcelExporter;
			_lookup_formRepository = lookup_formRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;

        }

        [AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task<PagedResultDto<GetFormFeedbackForViewDto>> GetAll(GetAllFormFeedbacksInput input)
         {
            // Feedback form anonymously user
            // _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            // Show per Tenant and only for administrators

            input.Filter = input.Filter?.Trim();

            // Admin feature 
            var filteredFormFeedbacks = _formFeedbackRepository.GetAll() //.Where(f => f.CreatorUserId == AbpSession.UserId)
                .Join(_lookup_formRepository.GetAll(),
                FormFeedback => FormFeedback.FormId,
                Form => Form.Id,
                (FormFeedback, Form) => new FormFeedback
                {
                    Id = FormFeedback.Id,
                    TenantId = FormFeedback.TenantId,
                    UserName = FormFeedback.UserName,
                    Email = FormFeedback.Email,
                    FeedbackFormId = FormFeedback.FeedbackFormId,
                    FeedbackFormData = FormFeedback.FeedbackFormData,
                    Rating = FormFeedback.Rating,
                    Description = FormFeedback.Description,
                    FormId = FormFeedback.FormId,
                    FormFk = Form,
                    CreationTime = FormFeedback.CreationTime,
                    Comment = FormFeedback.Comment
                })
                //.Include(e => e.FormFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.FeedbackFormData.Contains(input.Filter) || e.Description.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.FormNameFilter), e => e.FormFk != null && e.FormFk.Name == input.FormNameFilter);




            var pagedAndFilteredFormFeedbacks = filteredFormFeedbacks
                .OrderBy(input.Sorting ?? "creationTime desc")
                .PageBy(input);

			var formFeedbacks = from o in pagedAndFilteredFormFeedbacks
                         join o1 in _lookup_formRepository.GetAll() on o.FormId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetFormFeedbackForViewDto() {
							FormFeedback = new FormFeedbackDto
							{
                                Id = o.Id,
                                FormId = o.FormId,
                                CreationTime = o.CreationTime
							},
                         	FormName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                            UserName = o.UserName??"",
                            Email = o.Email??"",
                            Rating = o.Rating,
                            Comment = o.Comment
						};

            var totalCount = await filteredFormFeedbacks.CountAsync();

            return new PagedResultDto<GetFormFeedbackForViewDto>(
                totalCount,
                await formFeedbacks.ToListAsync()
            );
         }
		 
		 public async Task<GetFormFeedbackForViewDto> GetFormFeedbackForView(Guid id)
         {
            // Feedback form anonymously user
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                var formFeedback = await _formFeedbackRepository.GetAsync(id);
                var feedbackForm = await _lookup_formRepository.GetAsync(formFeedback.FeedbackFormId);

                var output = new GetFormFeedbackForViewDto { 
                    FormFeedback = ObjectMapper.Map<FormFeedbackDto>(formFeedback),
                    UserName = formFeedback.UserName,
                    Email = formFeedback.Email,
                    Rating = formFeedback.Rating,
                    Comment = formFeedback.Comment,
                    FeedbackFormSchema = feedbackForm.Schema,
                    FeedbackFormData = formFeedback.FeedbackFormData
                };

		        if (output.FormFeedback.FormId != null)
                {
                    var _lookupForm = await _lookup_formRepository.FirstOrDefaultAsync((Guid)output.FormFeedback.FormId);
                    output.FormName = _lookupForm?.Name?.ToString();
                }
			
                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

         }
		 
		 [AbpAuthorize(AppPermissions.Pages_FormFeedbacks_Edit)]
		 public async Task<GetFormFeedbackForEditOutput> GetFormFeedbackForEdit(EntityDto<Guid> input)
         {


            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                var formFeedback = await _formFeedbackRepository.FirstOrDefaultAsync(input.Id);           
		        var output = new GetFormFeedbackForEditOutput {FormFeedback = ObjectMapper.Map<CreateOrEditFormFeedbackDto>(formFeedback)};

		        if (output.FormFeedback.FormId != null)
                {
                    var _lookupForm = await _lookup_formRepository.FirstOrDefaultAsync((Guid)output.FormFeedback.FormId);
                    output.FormName = _lookupForm?.Name?.ToString();
                }
			
                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

         }

		 public async Task CreateOrEdit(CreateOrEditFormFeedbackDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{

                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = (Guid)input.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
                if (ACLResult.IsAuthed)
                {
                    await Update(input);
                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }

			}
         }

		 [AbpAuthorize(AppPermissions.Pages_FormFeedbacks_Create)]
		 protected virtual async Task Create(CreateOrEditFormFeedbackDto input)
         {
            var formFeedback = ObjectMapper.Map<FormFeedback>(input);

			
			if (AbpSession.TenantId != null)
			{
				formFeedback.TenantId = (int?) AbpSession.TenantId;
			}

            await _formFeedbackRepository.InsertAsync(formFeedback);
         }

		 [AbpAuthorize(AppPermissions.Pages_FormFeedbacks_Edit)]
		 protected virtual async Task Update(CreateOrEditFormFeedbackDto input)
         {
            var formFeedback = await _formFeedbackRepository.FirstOrDefaultAsync((Guid)input.Id);
             ObjectMapper.Map(input, formFeedback);
         }

		 [AbpAuthorize(AppPermissions.Pages_FormFeedbacks_Delete)]
         public async Task Delete(EntityDto<Guid> input)
         {
            await _formFeedbackRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetFormFeedbacksToExcel(GetAllFormFeedbacksForExcelInput input)
         {
			
			var filteredFormFeedbacks = _formFeedbackRepository.GetAll()
						.Include( e => e.FormFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.UserName.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.FeedbackFormData.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.FormNameFilter), e => e.FormFk != null && e.FormFk.Name == input.FormNameFilter);

			var query = (from o in filteredFormFeedbacks
                         join o1 in _lookup_formRepository.GetAll() on o.FormId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         select new GetFormFeedbackForViewDto()
                         {
                             FormFeedback = new FormFeedbackDto
                             {
                                 Id = o.Id,
                                 FormId = o.FormId,
                                 CreationTime = o.CreationTime
                             },
                             FormName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                             UserName = o.UserName ?? "",
                             Email = o.Email ?? "",
                             Rating = o.Rating,
                             Comment = o.Comment
                         });
            //                  select new GetFormFeedbackForViewDto() { 
            //FormFeedback = new FormFeedbackDto
            //{
            //                         Id = o.Id
            //},
            //                  	FormName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
            //});





            var formFeedbackListDtos = await query.ToListAsync();

            return _formFeedbacksExcelExporter.ExportToFile(formFeedbackListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_FormFeedbacks)]
         public async Task<PagedResultDto<FormFeedbackFormLookupTableDto>> GetAllFormForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_formRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var formList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<FormFeedbackFormLookupTableDto>();
			foreach(var form in formList){
				lookupTableDtoList.Add(new FormFeedbackFormLookupTableDto
				{
					Id = form.Id.ToString(),
					DisplayName = form.Name?.ToString()
				});
			}

            return new PagedResultDto<FormFeedbackFormLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }


        public async Task<bool> SendFeedback(CreateOrEditFormFeedbackDto input)
        {

            var formFeedback = ObjectMapper.Map<FormFeedback>(input);

            JObject jfeedbackForm = JObject.Parse(input.FeedbackFormData);

            if (AbpSession.UserId != null)
            {
                var sender = _userRepository.FirstOrDefault((long)AbpSession.UserId);
                formFeedback.UserName = sender?.UserName;
                formFeedback.Email = sender?.EmailAddress;
            }

            if (AbpSession.TenantId != null)
            {
                formFeedback.TenantId = (int?)AbpSession.TenantId;

            }
            else {
                //if (string.IsNullOrWhiteSpace(formFeedback.UserName)) return false;
                //if (!IsValidEmail(formFeedback.Email)) return false;
            }

            //Map rating and comment
            if (jfeedbackForm.ContainsKey("Rating_cho"))
            {
                if (Int32.TryParse(jfeedbackForm.GetValue("Rating_cho").ToString(), out int rating))
                {
                    formFeedback.Rating = rating;
                }
                else
                {
                    var j = jfeedbackForm.GetValue("Rating_cho");
                    if (Int32.TryParse(j["Rating_cho"].ToString(), out int ratings))
                        formFeedback.Rating = ratings;
                }
            }

            if (jfeedbackForm.ContainsKey("Comment_txt")) {
                formFeedback.Comment = jfeedbackForm.GetValue("Comment_txt").ToString();
            }

            await _formFeedbackRepository.InsertAsync(formFeedback);


            //TODO more validation
            return true;
        }
        private bool IsValidEmail(string emailString)
        {
            // Return true if emailString is in valid e-mail format.
            return Regex.IsMatch(emailString, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}