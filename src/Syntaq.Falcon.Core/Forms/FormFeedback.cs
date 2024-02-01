using Syntaq.Falcon.Forms;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Forms
{
	[Table("SfaFormFeedBacks")]
    public class FormFeedback : AuditedEntity<Guid> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[StringLength(FormFeedbackConsts.MaxUserNameLength, MinimumLength = FormFeedbackConsts.MinUserNameLength)]
		public virtual string UserName { get; set; }
		
		[StringLength(FormFeedbackConsts.MaxEmailLength, MinimumLength = FormFeedbackConsts.MinEmailLength)]
		public virtual string Email { get; set; }
		
		public virtual Guid FeedbackFormId { get; set; }
		
		public virtual string FeedbackFormData { get; set; }
		
		public virtual int Rating { get; set; }

        [StringLength(FormFeedbackConsts.MaxCommentLength, MinimumLength = FormFeedbackConsts.MinCommentLength)]
        public virtual string Comment { get; set; }

        [StringLength(FormFeedbackConsts.MaxDescriptionLength, MinimumLength = FormFeedbackConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
		

		public virtual Guid? FormId { get; set; }
		
        [ForeignKey("FormId")]
		public Form FormFk { get; set; }
		
    }
}