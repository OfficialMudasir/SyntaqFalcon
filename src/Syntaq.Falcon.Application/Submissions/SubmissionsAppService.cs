using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Organizations;
using Abp.UI;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Submissions.Exporting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Submissions
{
    //[AbpAuthorize(AppPermissions.Pages_Submissions)]
    [EnableCors("AllowAll")]
    public class SubmissionsAppService : FalconAppServiceBase, ISubmissionsAppService
	{
		private readonly ACLManager _ACLManager;
		private readonly SubmissionManager _submissionManager;
		private readonly IRepository<Submission, Guid> _submissionRepository;
		private readonly ISubmissionsExcelExporter _submissionsExcelExporter;
		private readonly IRepository<Record,Guid> _lookup_recordRepository;
		private readonly IRepository<RecordMatterItem,Guid> _recordMatterItemRepository;
		private readonly IRepository<User,long> _lookup_userRepository;
		private readonly IRepository<AppJob,Guid> _lookup_appJobRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
		private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;

		public SubmissionsAppService(
			ACLManager aclManager,
			SubmissionManager submissionManager, 
            IRepository<Submission, Guid> submissionRepository, 
            ISubmissionsExcelExporter submissionsExcelExporter , 
            IRepository<Record, Guid> lookup_recordRepository, 
            IRepository<RecordMatterItem, Guid> recordMatterItemRepository, 
            IRepository<User, long> lookup_userRepository, 
            IRepository<AppJob, Guid> lookup_appJobRepository, 
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
			IRepository<RecordMatter, Guid> recordMatterRepository,
			IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository) 
		{
			_ACLManager = aclManager;
			_submissionManager = submissionManager;
			_submissionRepository = submissionRepository;
			_submissionsExcelExporter = submissionsExcelExporter;
			_lookup_recordRepository = lookup_recordRepository;
			_recordMatterItemRepository = recordMatterItemRepository;
			_lookup_userRepository = lookup_userRepository;
			_lookup_appJobRepository = lookup_appJobRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
			_recordMatterRepository = recordMatterRepository;
		}

        [AbpAuthorize(AppPermissions.Pages_Submissions_Admin)]
        public async Task<PagedResultDto<GetSubmissionForViewDto>> GetAll(GetAllSubmissionsInput input)
		{
			input.Filter = input.Filter?.Trim();

			var filteredSubmissions = _submissionRepository.GetAll()
				.Include( e => e.RecordFk)
				.Include( e => e.RecordMatterFk)
				.Include( e => e.UserFk)
				.Include( e => e.AppJobFk)
				.Include( e => e.FormFk)
				.Include( e => e.AppFk)
				.Where(e => (e.LastModificationTime ?? e.CreationTime) >= input.StartDateFilter && (e.LastModificationTime ?? e.CreationTime) <= input.EndDateFilter)
				.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.FormFk != null && e.FormFk.Name.ToLower().Contains(input.Filter) || e.RecordFk != null && e.RecordFk.RecordName.ToLower().Contains(input.Filter) || e.RecordMatterFk != null && e.RecordMatterFk.RecordMatterName.ToLower().Contains(input.Filter) || e.UserFk != null && e.UserFk.Name.ToLower().Contains(input.Filter) || e.AppJobFk != null && e.AppJobFk.Name.ToLower().Contains(input.Filter))
				.WhereIf(input.RequiresPaymentFilter > -1,  e => Convert.ToInt32(e.RequiresPayment) == input.RequiresPaymentFilter )
				.WhereIf(!string.IsNullOrWhiteSpace(input.PaymentStatusFilter),  e => e.PaymentStatus.ToLower() == input.PaymentStatusFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.ChargeIdFilter),  e => e.ChargeId.ToLower() == input.ChargeIdFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.SubmissionStatusFilter),  e => e.SubmissionStatus.ToLower() == input.SubmissionStatusFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.TypeFilter),  e => e.Type.ToLower() == input.TypeFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.FormNameFilter), e => e.FormFk != null && e.FormFk.Name.ToLower() == input.FormNameFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.RecordNameFilter), e => e.RecordFk != null && e.RecordFk.RecordName.ToLower() == input.RecordNameFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.RecordMatterNameFilter), e => e.RecordMatterFk != null && e.RecordMatterFk.RecordMatterName.ToLower() == input.RecordMatterNameFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
				//.WhereIf(!string.IsNullOrWhiteSpace(input.AppJobNameFilter), e => e.AppJobFk != null && e.AppJobFk.Name.ToLower() == input.AppJobNameFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.AppNameFilter), e => e.AppFk != null && e.AppFk.Name.ToLower() == input.AppNameFilter.ToLower().Trim())
				.WhereIf(input.ExcludeOwnAccountFilter == true, e => e.UserId != AbpSession.UserId);

            var pagedAndFilteredSubmissions = filteredSubmissions
			.OrderByDescending(i => i.CreationTime)
			//.OrderBy(input.Sorting ?? "id asc")
			.PageBy(input);

		var SubmissionsOutput = new List<GetSubmissionForViewDto>();

		pagedAndFilteredSubmissions.ToList().ForEach(i =>
		{
			GetSubmissionForViewDto output = new GetSubmissionForViewDto
			{
				Submission = new SubmissionDto()
				{
					Id = i.Id,
					Date = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime,
					AccessToken = i.AccessToken,
					RequiresPayment = i.RequiresPayment,
					PaymentCurrency = i.FormFk?.PaymentCurrency ?? "",
					PaymentStatus = i.PaymentStatus,
					PaymentAmount = i.PaymentAmount,
					VoucherAmount = i.VoucherAmount,
					AmountPaid = i.AmountPaid,
					ChargeId = i.ChargeId,
					SubmissionStatus = i.SubmissionStatus,
					Type = i.Type,
					RecordId = i.RecordId,
					RecordMatterId = i.RecordMatterId,
					UserId = i.UserId,
					AppJobId = i.AppJobId
				},
				FormName = i.FormFk?.Name,
				AppName = i.AppFk?.Name,
				RecordName = i.RecordFk?.RecordName,
				RecordMatterName = i.RecordMatterFk?.RecordMatterName,
				RecordMatterItems = new List<RecordMatterItemDto>(),
				UserName = i.UserFk?.UserName ?? "",
				UserEmail = i.UserFk?.EmailAddress
			};
			SubmissionsOutput.Add(output);
		});

		SubmissionsOutput.ForEach(i =>
		{
			List<RecordMatterItem> _RecordMatterItems = _recordMatterItemRepository.GetAllList(j => j.SubmissionId == i.Submission.Id && j.RecordMatterId == i.Submission.RecordMatterId && j.Status == "Complete");
			i.RecordMatterItems = new List<RecordMatterItemDto>();
			if (_RecordMatterItems.Count() >= 1)
			{
				i.Submission.HasDocuments = true;
				_RecordMatterItems.ForEach(j =>
				{
					RecordMatterItemDto _RecordMatterItem = new RecordMatterItemDto()
					{
						Id = j.Id,
						FormId = j.FormId,
						Document = j.Status == "Complete" ? true : false,
						DocumentName = j.DocumentName,
						CreationTime = j.CreationTime,
						//AllowWord = j.AllowedFormats.Contains("W") ? true : false,
						//AllowPdf = j.AllowedFormats.Contains("P") ? true : false,
						//AllowHTML = j.AllowedFormats.Contains("H") ? true : false,

                        AllowWord = FileFormatAllowed(j.AllowWordAssignees, j.AllowedFormats, "W").Result,
                        AllowPdf = FileFormatAllowed(j.AllowPdfAssignees, j.AllowedFormats, "P").Result,
                        AllowHTML = FileFormatAllowed(j.AllowHtmlAssignees, j.AllowedFormats, "H").Result,

                        ErrorDetails = j.ErrorDetails,
						LastModified = j.LastModificationTime == null ? j.CreationTime : (DateTime)j.LastModificationTime
					};
					i.RecordMatterItems.Add(_RecordMatterItem);
				});
			}
		});

		var totalCount = await filteredSubmissions.CountAsync();

		return new PagedResultDto<GetSubmissionForViewDto>(
			totalCount,
			SubmissionsOutput
		);
		}

        //[AbpAuthorize(AppPermissions.Pages_Submissions_Edit)] //Unused?
        //public async Task<GetSubmissionForEditOutput> GetSubmissionForEdit(EntityDto<Guid> input)
        //{
        //var submission = await _submissionRepository.FirstOrDefaultAsync(input.Id);

        //var output = new GetSubmissionForEditOutput {Submission = ObjectMapper.Map<CreateOrEditSubmissionDto>(submission)};

        //if (output.Submission.RecordId != null)
        //{
        //	var _lookupRecord = await _lookup_recordRepository.FirstOrDefaultAsync((Guid)output.Submission.RecordId);
        //	output.RecordRecordName = _lookupRecord.RecordName.ToString();
        //}

        //if (output.Submission.UserId != null)
        //{
        //	var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Submission.UserId);
        //	output.UserName = _lookupUser.Name.ToString();
        //}

        //if (output.Submission.AppJobId != null)
        //{
        //	var _lookupAppJob = await _lookup_appJobRepository.FirstOrDefaultAsync((Guid)output.Submission.AppJobId);
        //	output.AppJobName = _lookupAppJob.Name.ToString();
        //}

        //return output;
        //}

        ////[AbpAuthorize(AppPermissions.Pages_Submissions)]
        //// Anonymous required
        //public async Task<GetSubmissionForViewOutput> GetSubmissionForView(EntityDto<Guid> input)
        //{

        //          _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
        //          GetSubmissionForViewOutput output = null;

        //          using (var unitOfWork = _unitOfWorkManager.Begin())
        //          {

        //              Submission submission = _submissionRepository.GetAll()
        //                   .Include(i => i.RecordFk)
        //                   .Include(i => i.RecordMatterFk)
        //                   .Include(i => i.UserFk)
        //                   .FirstOrDefault(i => i.Id == input.Id);

        //              if (submission != null)
        //              {
        //                  output = new GetSubmissionForViewOutput
        //                  {
        //                      Submission = new CreateOrEditSubmissionDto()
        //                      {
        //                          Id = submission.Id,
        //                          CreationTime = submission.CreationTime,
        //                          CreatorUserId = submission.CreatorUserId,
        //                          LastModificationTime = submission.LastModificationTime,
        //                          LastModifierUserId = submission.LastModifierUserId,
        //                          IsDeleted = submission.IsDeleted,
        //                          DeleterUserId = submission.DeleterUserId,
        //                          DeletionTime = submission.DeletionTime,
        //                          TenantId = submission.TenantId,
        //                          AccessToken = submission.AccessToken,
        //                          RequiresPayment = submission.RequiresPayment,
        //                          PaymentCurrency = submission.FormFk?.PaymentCurrency ?? "",
        //                          PaymentStatus = submission.PaymentStatus,
        //                          PaymentAmount = submission.PaymentAmount,
        //                          VoucherAmount = submission.VoucherAmount,
        //                          AmountPaid = submission.AmountPaid,
        //                          ChargeId = submission.ChargeId,
        //                          SubmissionStatus = submission.SubmissionStatus,
        //                          Type = submission.Type,
        //                          RecordId = submission.RecordId,
        //                          RecordMatterId = submission.RecordMatterId,
        //                          UserId = submission.UserId,
        //                          AppId = submission.AppId,
        //                          AppJobId = submission.AppJobId,
        //                          FormId = submission.FormId
        //                      },
        //                      RecordName = submission.RecordFk?.RecordName,
        //                      RecordMatterName = submission.RecordMatterFk?.RecordMatterName,
        //				HasFiles = submission.RecordMatterFk?.HasFiles,
        //				RecordMatterItems = new List<RecordMatterItemDto>(),
        //                      UserName = submission.UserFk?.UserName,
        //                      UserEmail = submission.UserFk?.EmailAddress
        //                  };

        //			output.RecordMatterItems = new List<RecordMatterItemDto>();

        //                  if (submission.SubmissionStatus == "Complete")
        //                  {
        //				List<RecordMatterItem> RecordMatterItems =
        //					_recordMatterItemRepository.GetAll().Where(
        //						i => i.SubmissionId == submission.Id &&
        //						i.RecordMatterId == submission.RecordMatterId
        //						&&i.HasDocument==true
        //						//&&  i.Status == "Complete"
        //						//i.Document.Length  > 0
        //					).OrderBy("Order asc")
        //					.ThenBy("CreationTime desc")
        //					.ToList();


        //				RecordMatterItems.ForEach(i => {
        //					output.RecordMatterItems.Add(new RecordMatterItemDto()
        //					{
        //						Id = i.Id,
        //						FormId = i.FormId,
        //						GroupId= i.GroupId,
        //						Document = true, // preselected with document
        //						DocumentName = i.DocumentName,
        //						CreationTime = i.CreationTime,

        //						AllowWord = FileFormatAllowed(i.AllowWordAssignees, i.AllowedFormats, "W").Result,
        //						AllowPdf = FileFormatAllowed(i.AllowPdfAssignees, i.AllowedFormats, "P").Result,
        //						AllowHTML = FileFormatAllowed(i.AllowHtmlAssignees, i.AllowedFormats, "H").Result,

        //						ErrorDetails = i.ErrorDetails,
        //						LastModified = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime
        //					});
        //				});

        //                  }


        //                  if (output.Submission.UserId != null)
        //                  {
        //                      User _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Submission.UserId);
        //                      output.UserName = _lookupUser.Name.ToString();
        //                  }
        //                  unitOfWork.Complete();

        //                  return output;
        //              }
        //              else
        //              {
        //                  unitOfWork.Complete();
        //                  return null;
        //              }

        //          }

        //}

        public async Task<GetSubmissionForViewOutput> GetSubmissionForView(EntityDto<Guid> input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            GetSubmissionForViewOutput output = null;

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {

                Submission submission = _submissionRepository.GetAll()
                     .Include(i => i.RecordFk)
                     .Include(i => i.RecordMatterFk)
                     .Include(i => i.UserFk)
                     .FirstOrDefault(i => i.Id == input.Id);

                if (submission != null)
                {
                    output = new GetSubmissionForViewOutput
                    {
                        Submission = new CreateOrEditSubmissionDto()
                        {
                            Id = submission.Id,
                            CreationTime = submission.CreationTime,
                            CreatorUserId = submission.CreatorUserId,
                            LastModificationTime = submission.LastModificationTime,
                            LastModifierUserId = submission.LastModifierUserId,
                            IsDeleted = submission.IsDeleted,
                            DeleterUserId = submission.DeleterUserId,
                            DeletionTime = submission.DeletionTime,
                            TenantId = submission.TenantId,
                            AccessToken = submission.AccessToken,
                            RequiresPayment = submission.RequiresPayment,
                            PaymentCurrency = submission.FormFk?.PaymentCurrency ?? "",
                            PaymentStatus = submission.PaymentStatus,
                            PaymentAmount = submission.PaymentAmount,
                            VoucherAmount = submission.VoucherAmount,
                            AmountPaid = submission.AmountPaid,
                            ChargeId = submission.ChargeId,
                            SubmissionStatus = submission.SubmissionStatus,
                            Type = submission.Type,
                            RecordId = submission.RecordId,
                            RecordMatterId = submission.RecordMatterId,
                            UserId = submission.UserId,
                            AppId = submission.AppId,
                            AppJobId = submission.AppJobId,
                            FormId = submission.FormId
                        },
                        RecordName = submission.RecordFk?.RecordName,
                        RecordMatterName = submission.RecordMatterFk?.RecordMatterName,
                        HasFiles = submission.RecordMatterFk?.HasFiles,
                        RecordMatterItems = new List<RecordMatterItemDto>(),
                        UserName = submission.UserFk?.UserName,
                        UserEmail = submission.UserFk?.EmailAddress
                    };

                    output.RecordMatterItems = new List<RecordMatterItemDto>();

                    if (submission.SubmissionStatus == "Complete")
                    {
                        List<RecordMatterItem> RecordMatterItems =
                            _recordMatterItemRepository.GetAll().Where(
                                i => i.SubmissionId == submission.Id &&
                                i.RecordMatterId == submission.RecordMatterId
                                && i.HasDocument == true
                            //&&  i.Status == "Complete"
                            //i.Document.Length  > 0
                            ).OrderBy("Order asc")
                            .ThenBy("CreationTime desc")
                            .ToList();


                        RecordMatterItems.ForEach(i => {
                            output.RecordMatterItems.Add(new RecordMatterItemDto()
                            {
                                Id = i.Id,
                                FormId = i.FormId,
                                GroupId = i.GroupId,
                                Document = true, // preselected with document
                                DocumentName = i.DocumentName,
                                CreationTime = i.CreationTime,

                                AllowWord = FileFormatAllowed(i.AllowWordAssignees, i.AllowedFormats, "W").Result,
                                AllowPdf = FileFormatAllowed(i.AllowPdfAssignees, i.AllowedFormats, "P").Result,
                                AllowHTML = FileFormatAllowed(i.AllowHtmlAssignees, i.AllowedFormats, "H").Result,

                                ErrorDetails = i.ErrorDetails,
                                LastModified = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime
                            });
                        });

                    }


                    if (output.Submission.UserId != null)
                    {
                        User _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Submission.UserId);
                        output.UserName = _lookupUser.Name.ToString();
                    }
                    unitOfWork.Complete();

                    return output;
                }
                else
                {
                    unitOfWork.Complete();
                    return null;
                }

            }

        }

        // Support old basic format allowed also or else formats will appear // for all users
        private async Task<bool> FileFormatAllowed(string assignees, string allowedformats = "P", string type = "P")
        {

            // default none set then true
            bool result = true;

            if (string.IsNullOrEmpty(assignees) || assignees == "[]" || assignees == "null")
            {
                result = allowedformats.Contains(type) ? true : false;
            }
            else
            {
                List<GrantACLDto> obj = new List<GrantACLDto>();
                result = false;

                obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GrantACLDto>>(assignees);

                if (obj != null)
                {
                    var user = await UserManager.GetUserByIdAsync((long)AbpSession.UserId);
                   
                    if(user != null)
                    {
                        if (obj.Any(i => i.value == user.UserName)) return true;
                            _userOrganizationUnitRepository.GetAll().Where(i => i.UserId == user.Id).ToList().ForEach(i => {
                                if(obj.Any(n => n.Type == "Team" && n.Id == i.OrganizationUnitId))
                                {
                                    result = true;
                                }
                            });                    
                    }
                }

            }

            return result;
        }

        public Guid? GetIdByRecordMatterItemId(EntityDto<Guid> input, string AuthToken)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = AuthToken, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				Guid? SubmissionId = _recordMatterItemRepository.FirstOrDefault(input.Id)?.Id;
				return SubmissionId;
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}


		}

		//     public async Task CreateOrEdit(CreateOrEditSubmissionDto input)
		//{
		//if(input.Id == null){
		//	await Create(input);
		//}
		//else{
		//	await Update(input);
		//}
		//}

		//[AbpAuthorize(AppPermissions.Pages_Submissions_Create)]
		//private async Task Create(CreateOrEditSubmissionDto input)
		//{
		//var submission = ObjectMapper.Map<Submission>(input);


		//if (AbpSession.TenantId != null)
		//{
		//	submission.TenantId = (int?) AbpSession.TenantId;
		//}


		//await _submissionRepository.InsertAsync(submission);
		//}

		//[AbpAuthorize(AppPermissions.Pages_Submissions_Edit)]
		//private async Task Update(CreateOrEditSubmissionDto input)
		//{
		//var submission = await _submissionRepository.FirstOrDefaultAsync((Guid)input.Id);
		//ObjectMapper.Map(input, submission);
		//}

		//[AbpAuthorize(AppPermissions.Pages_Submissions_Delete)]
		//public async Task Delete(EntityDto<Guid> input)
		//{
		//await _submissionRepository.DeleteAsync(input.Id);
		//} 


		[AbpAuthorize(AppPermissions.Pages_Submissions_Admin)]
		public async Task<FileDto> GetSubmissionsToExcel(GetAllSubmissionsForExcelInput input)
		{
			var filteredSubmissions = _submissionRepository.GetAll()
				.Include(e => e.RecordFk)
				.Include(e => e.RecordMatterFk)
				.Include(e => e.UserFk)
				.Include(e => e.AppJobFk)
				.Include(e => e.FormFk)
				.Where(e => (e.LastModificationTime ?? e.CreationTime) >= input.StartDateFilter && (e.LastModificationTime ?? e.CreationTime) <= input.EndDateFilter)
				.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FormFk != null && e.FormFk.Name.ToLower().Contains(input.Filter) || e.RecordFk != null && e.RecordFk.RecordName.ToLower().Contains(input.Filter) || e.RecordMatterFk != null && e.RecordMatterFk.RecordMatterName.ToLower().Contains(input.Filter) || e.UserFk != null && e.UserFk.Name.ToLower().Contains(input.Filter) || e.AppJobFk != null && e.AppJobFk.Name.ToLower().Contains(input.Filter))
				.WhereIf(input.RequiresPaymentFilter > -1, e => Convert.ToInt32(e.RequiresPayment) == input.RequiresPaymentFilter)
				.WhereIf(!string.IsNullOrWhiteSpace(input.PaymentStatusFilter), e => e.PaymentStatus.ToLower() == input.PaymentStatusFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.ChargeIdFilter), e => e.ChargeId.ToLower() == input.ChargeIdFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.SubmissionStatusFilter), e => e.SubmissionStatus.ToLower() == input.SubmissionStatusFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.TypeFilter), e => e.Type.ToLower() == input.TypeFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.FormNameFilter), e => e.FormFk != null && e.FormFk.Name.ToLower() == input.FormNameFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.RecordNameFilter), e => e.RecordFk != null && e.RecordFk.RecordName.ToLower() == input.RecordNameFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.RecordMatterNameFilter), e => e.RecordMatterFk != null && e.RecordMatterFk.RecordMatterName.ToLower() == input.RecordMatterNameFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
				.WhereIf(!string.IsNullOrWhiteSpace(input.AppJobNameFilter), e => e.AppJobFk != null && e.AppJobFk.Name.ToLower() == input.AppJobNameFilter.ToLower().Trim())
				.WhereIf(input.ExcludeOwnAccountFilter == true, e => e.UserId != AbpSession.UserId);

			var OrderedSubmissions = filteredSubmissions.OrderByDescending(i => i.CreationTime);

			var SubmissionsOutput = new List<GetSubmissionForViewDto>();

			OrderedSubmissions.ToList().ForEach(i =>
			{
				GetSubmissionForViewDto output = new GetSubmissionForViewDto
				{
					Submission = new SubmissionDto()
					{
						Id = i.Id,
						Date = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime,
						AccessToken = i.AccessToken,
						RequiresPayment = i.RequiresPayment,
						PaymentCurrency = i.FormFk?.PaymentCurrency ?? "",
						PaymentStatus = i.PaymentStatus,
						PaymentAmount = i.PaymentAmount,
						VoucherAmount = i.VoucherAmount,
						AmountPaid = i.AmountPaid,
						ChargeId = i.ChargeId,
						SubmissionStatus = i.SubmissionStatus,
						Type = i.Type,
						RecordId = i.RecordId,
						RecordMatterId = i.RecordMatterId,
						UserId = i.UserId,
						AppJobId = i.AppJobId
					},
					AppJobName = i.AppJobFk?.Name,
					FormName = i.FormFk?.Name,
					RecordName = i.RecordFk?.RecordName,
					RecordMatterName = i.RecordMatterFk?.RecordMatterName,
					RecordMatterItemCount = _recordMatterItemRepository.GetAllList(j => j.SubmissionId == i.Id && j.RecordMatterId == i.RecordMatterId && j.Status == "Complete").Count(),
					UserName = i.UserFk?.UserName ?? "",
					UserEmail = i.UserFk?.EmailAddress
				};
				SubmissionsOutput.Add(output);
			});

			return _submissionsExcelExporter.ExportToFile(SubmissionsOutput);
		 }

		//[AbpAuthorize(AppPermissions.Pages_Submissions)]
		// public async Task<PagedResultDto<SubmissionRecordLookupTableDto>> GetAllRecordForLookupTable(GetAllForLookupTableInput input)
		// {
		//	 var query = _lookup_recordRepository.GetAll().WhereIf(
		//			!string.IsNullOrWhiteSpace(input.Filter),
		//		   e=> e.RecordName.ToString().Contains(input.Filter)
		//		);

		//	var totalCount = await query.CountAsync();

		//	var recordList = await query
		//		.PageBy(input)
		//		.ToListAsync();

		//	var lookupTableDtoList = new List<SubmissionRecordLookupTableDto>();
		//	foreach(var record in recordList){
		//		lookupTableDtoList.Add(new SubmissionRecordLookupTableDto
		//		{
		//			Id = record.Id.ToString(),
		//			DisplayName = record.RecordName?.ToString()
		//		});
		//	}

		//	return new PagedResultDto<SubmissionRecordLookupTableDto>(
		//		totalCount,
		//		lookupTableDtoList
		//	);
		// }

		//[AbpAuthorize(AppPermissions.Pages_Submissions)]
		// public async Task<PagedResultDto<SubmissionRecordMatterLookupTableDto>> GetAllRecordMatterForLookupTable(GetAllForLookupTableInput input)
		// {
		//	 var query = _lookup_recordMatterItemRepository.GetAll().WhereIf(
		//			!string.IsNullOrWhiteSpace(input.Filter),
		//		   e=> e.DocumentName.ToString().Contains(input.Filter)
		//		);

		//	var totalCount = await query.CountAsync();

		//	var recordMatterItemList = await query
		//		.PageBy(input)
		//		.ToListAsync();

		//	var lookupTableDtoList = new List<SubmissionRecordMatterLookupTableDto>();
		//	foreach(var recordMatterItem in recordMatterItemList)
		//	{
		//		lookupTableDtoList.Add(new SubmissionRecordMatterLookupTableDto
		//		{
		//			Id = recordMatterItem.Id.ToString(),
		//			DisplayName = recordMatterItem.DocumentName?.ToString()
		//		});
		//	}

		//	return new PagedResultDto<SubmissionRecordMatterLookupTableDto>(
		//		totalCount,
		//		lookupTableDtoList
		//	);
		// }

		//[AbpAuthorize(AppPermissions.Pages_Submissions)]
		// public async Task<PagedResultDto<SubmissionUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
		// {
		//	 var query = _lookup_userRepository.GetAll().WhereIf(
		//			!string.IsNullOrWhiteSpace(input.Filter),
		//		   e=> e.Name.ToString().Contains(input.Filter)
		//		);

		//	var totalCount = await query.CountAsync();

		//	var userList = await query
		//		.PageBy(input)
		//		.ToListAsync();

		//	var lookupTableDtoList = new List<SubmissionUserLookupTableDto>();
		//	foreach(var user in userList){
		//		lookupTableDtoList.Add(new SubmissionUserLookupTableDto
		//		{
		//			Id = user.Id,
		//			DisplayName = user.Name?.ToString()
		//		});
		//	}

		//	return new PagedResultDto<SubmissionUserLookupTableDto>(
		//		totalCount,
		//		lookupTableDtoList
		//	);
		// }

		//[AbpAuthorize(AppPermissions.Pages_Submissions)]
		// public async Task<PagedResultDto<SubmissionAppJobLookupTableDto>> GetAllAppJobForLookupTable(GetAllForLookupTableInput input)
		// {
		//	 var query = _lookup_appJobRepository.GetAll().WhereIf(
		//			!string.IsNullOrWhiteSpace(input.Filter),
		//		   e=> e.Name.ToString().Contains(input.Filter)
		//		);

		//	var totalCount = await query.CountAsync();

		//	var appJobList = await query
		//		.PageBy(input)
		//		.ToListAsync();

		//	var lookupTableDtoList = new List<SubmissionAppJobLookupTableDto>();
		//	foreach(var appJob in appJobList){
		//		lookupTableDtoList.Add(new SubmissionAppJobLookupTableDto
		//		{
		//			Id = appJob.Id.ToString(),
		//			DisplayName = appJob.Name?.ToString()
		//		});
		//	}

		//	return new PagedResultDto<SubmissionAppJobLookupTableDto>(
		//		totalCount,
		//		lookupTableDtoList
		//	);
		// }
	}
}