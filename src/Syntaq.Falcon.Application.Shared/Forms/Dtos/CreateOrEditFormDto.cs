
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Ganss.Xss;
using System.Text.RegularExpressions;

namespace Syntaq.Falcon.Forms.Dtos
{

	public class GetFormForViewDto
	{
		public Guid? Id { get; set; }
		public Guid? OriginalId { get; set; }
		public string AccessToken { get; set; }
		public string VersionName { get; set; }
		public string Version { get; set; }
        public string UserACLPermission { get; set; }

        // Used to determine if the form should be locked
        public Guid? RecordMatterItemId { get; set; }
		public Guid? RecordMatterId { get; set; }

		public string TenantName { get; set; }
		public string TenantId { get; set; }

		public string ProjectName{get;set;}
        public Guid? ProjectId { get; set; }
        public Guid? ReleaseId { get; set; }

    }

	public class CreateOrEditFormDto : AuditedEntityDto<Guid?>
	{


        private string _name = String.Empty;
        [Required]
        [StringLength(FormConsts.MaxNameLength, MinimumLength = FormConsts.MinNameLength)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                value = value == null ? string.Empty : value;
                // Sanitize the Name property using HtmlSanitizer
                string pattern = @"<[^>]*>";
                _name = Regex.Replace(value, pattern, string.Empty);
            }
        }

        private string _description = String.Empty;
        [StringLength(FormConsts.MaxDescriptionLength, MinimumLength = FormConsts.MinDescriptionLength)]
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

 
		[StringLength(FormConsts.MaxNameLength, MinimumLength = FormConsts.MinNameLength)]
		public string VersionName { get; set; }

 
		public bool PaymentEnabled { get; set; }

        public bool LockOnBuild { get; set; } = false;

        public bool LockToTenant { get; set; } = false;
        public bool RequireAuth { get; set; } = false;

        public decimal PaymentAmount { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentProcess { get; set; }
		public string PaymentPublishableToken { get; set; }
		public int Version { get; set; }
		public int CurrentVersion { get; set; }
		public Guid? FolderId { get; set; }
		//public Guid? AppJobId { get; set; }
		public string Schema { get; set; }
		public string Script { get; set; }
		public string Rules { get; set; }
		public string RulesScript { get; set; }
		public Guid OriginalId { get; set; }
		public string IsIndex { get; set; } = "1";
		public bool IsEnabled { get; set; } = true;

        public string UserACLPermission { get; set; }

    }
}