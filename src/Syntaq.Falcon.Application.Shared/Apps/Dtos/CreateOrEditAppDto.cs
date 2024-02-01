using Abp.Application.Services.Dto;
using Ganss.Xss;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Syntaq.Falcon.Apps.Dtos
{
	public class CreateOrEditAppDto : EntityDto<Guid?>
	{
        private string _name = String.Empty;
        [Required]
        [StringLength(AppConsts.MaxNameLength, MinimumLength = AppConsts.MinNameLength)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                // Sanitize the Name property using HtmlSanitizer
                string pattern = @"<[^>]*>";
                _name = Regex.Replace(value, pattern, string.Empty);
            }
        }

        private string _description = String.Empty;
        [StringLength(AppConsts.MaxDescriptionLength, MinimumLength = AppConsts.MinDescriptionLength)]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                // Sanitize the Name property using HtmlSanitizer
                string pattern = @"<[^>]*>";
                _description = Regex.Replace(value, pattern, string.Empty);
            }
        }
 
		[Required]
		public string Data { get; set; }
	}

	public class CreateOrEditAppJobDto : EntityDto<Guid?>
	{
		[Required]
		[StringLength(AppJobConsts.MaxNameLength, MinimumLength = AppJobConsts.MinNameLength)]
		public string Name { get; set; }
		public string Data { get; set; }
		public Guid? AppId { get; set; }
		public Guid? EntityId { get; set; }
		public Guid? FormId { get; set; }
		public Guid? ContributorId { get; set; }
		public String ContributorStatus { get; set; }
		public Guid? DocumentTemplateId { get; set; }
		public int? TenantId { get; set; }
		public CreateOrEditAppJobFormDto Form { get; set; }
		public List<CreateOrEditAppJobDocumentDto> Document { get; set; } = new List<CreateOrEditAppJobDocumentDto>();
		public bool? DeleteRecordsAfterAssembly { get; set; } = false;
		public List<CreateOrEditAppJobRecordMatterDto> RecordMatter { get; set; }
		public CreateOrEditAppJobUserDto User { get; set; }
		public CreateOrEditAppJobWorkFlowDto WorkFlow { get; set; }
		public List<System.Xml.Linq.XElement> XData { get; set; }
        public string SummaryTableHTML { get; set; }
		public bool? SendWebhooksAfterAssembly { get; set; } = false;
		public bool? IsAssemblyDebug { get; set; } = false;
	}

	public class CreateOrEditAppJobFormDto
	{
		public Guid? FormID { get; set; }
		public string FormName { get; set; }
		public string ProjectURL { get; set; }
		public decimal PaymentValue { get; set; }
		public decimal PaymentValueCents { get; set; }
		public string MergeTextUser { get; set; }
	}

	public class CreateOrEditAppJobUserDto
	{
		public long ID { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Permission { get; set; } = "E"; // Set to E
    }

	public class CreateOrEditAppJobTeamDto
	{
		public long ID { get; set; }
		public string Name { get; set; }
		public string Permission { get; set; } = "E"; // Set to E
	}

	public class CreateOrEditAppJobDocumentDto
	{
		public int DocumentId { get; set; }
		[StringLength(AppConsts.MaxDescriptionLength, MinimumLength = AppConsts.MinDescriptionLength)]
		public string DocumentName { get; set; }
		[StringLength(AppConsts.MaxDescriptionLength, MinimumLength = AppConsts.MinDescriptionLength)]
		public string DocumentTemplateURL { get; set; }
		[StringLength(AppConsts.MaxDescriptionLength, MinimumLength = AppConsts.MinDescriptionLength)]
		public string DocumentSummaryTemplateURL { get; set; }
		public string FilterRule { get; set; }
		public bool? SummaryDoc { get; set; }
		public bool? AllowWord { get; set; }
		public bool? AllowWordPaid { get; set; }
		public bool? AllowPdf { get; set; }
		public bool? AllowHTML { get; set; }

        public List<GrantACLDto> AllowWordAssignees { get; set; }
        public string AllowWordAssigneeJSON { get; set; }

        public List<GrantACLDto> AllowPdfAssignees { get; set; }
        public string AllowPdfAssigneeJSON { get; set; }

        public List<GrantACLDto> AllowHtmlAssignees { get; set; }
        public string AllowHtmlAssigneeJSON { get; set; }

        public string MText { get; set; }
	}

	public class CreateOrEditAppJobRecordMatterDto
	{
		public List<GrantACLDto> Assignees { get; set; }
		public string AssigneeJSON { get; set; }
		public int? UserId { get; set; } 
		public string UserIds { get; set; } //TO REMOVE
		public List<CreateOrEditAppJobUserDto> Users { get; set; }
		public string TeamIds { get; set; } //TO REMOVE
		public List<CreateOrEditAppJobTeamDto> Teams { get; set; }
		private long? _OrganizationId;
		public long? OrganizationId
		{
			get { return _OrganizationId; }
			set => _OrganizationId = value.ToString() == "null" ? null : value;
		}
		public int? TenantId { get; set; }
		public Guid? RecordId { get; set; }       
		public string RecordName { get; set; }
		public string FileDescription { get; set; }
		public Guid? RecordMatterId { get; set; }
		public string RecordMatterName { get; set; }
		public Guid? RecordMatterItemId { get; set; }
		public Guid? RecordMatterItemGroupId { get; set; }
		public Guid? SubmissionId { get; set; }
		public bool? GenerateInvoice { get; set; }
		public bool? SaveToFormOwner { get; set; }
		public string CustomSaveFolderId { get; set; }
		public Guid? FolderId { get; set; }
		public string FolderName { get; set; }
	}

	public class CreateOrEditAppJobWorkFlowDto
	{
		public List<CreateOrEditAppJobRestDto> BeforeAssembly { get; set; } = new List<CreateOrEditAppJobRestDto>();
		public List<CreateOrEditAppJobRestDto> AfterAssembly { get; set; } = new List<CreateOrEditAppJobRestDto>();
		public List<CreateOrEditAppJobEmailDto> Email { get; set; } = new List<CreateOrEditAppJobEmailDto>();
	}

	public class CreateOrEditAppJobRestDto
	{
		public string URL { get; set; }
		public string Rest { get; set; }
		public string BodyType { get; set; } 
		public string BodyContent { get; set; }
		public bool? Async { get; set; }
		public string Header1Key { get; set; }
		public string Header1Value { get; set; }
		public string Header1Description { get; set; }
		public string Header2Key { get; set; }
		public string Header2Value { get; set; }
		public string Header2Description { get; set; }
		public string Header3Key { get; set; }
		public string Header3Value { get; set; }
		public string Header3Description { get; set; }
		public List<CreateOrEditAppJobRestHeaderDto> Headers { get; set; }
	}

	public class CreateOrEditAppJobRestHeaderDto
	{
		public string Key { get; set; }
		public string Value { get; set; }
		public string Description { get; set; }
	}

	public class CreateOrEditAppJobEmailDto
	{
		public string EmailTemplateURL { get; set; }
		public string TemplatePartID { get; set; }
		public string DocumentEmailFrom { get; set; }
		public string DocumentEmailSubject { get; set; }
		public string DocumentEmailMessage { get; set; }
		public string DocumentEmailRecipients { get; set; }
		public string DocumentEmailCCRecipients { get; set; }
		public string DocumentEmailBCCRecipients { get; set; }
		public string UserEmail { get; set; }
		public string EmailType { get; set; }
		public string FilterRule { get; set; }
		public bool? EmailPublisher { get; set; }
		//public int[] DocumentIds { get; set; }
		public string DocumentAttachmentIds { get; set; } = string.Empty;
		public string EmailBodyDocumentIds { get; set; } = string.Empty;
		public bool? AttachFileUploads { get; set; }
		public bool? AttachAllRecordMatterItems { get; set; }
	}

	//public class CreateOrEditAppJobRedirectDto
	//{
	//    public string RedirectURL { get; set; }
	//}
}