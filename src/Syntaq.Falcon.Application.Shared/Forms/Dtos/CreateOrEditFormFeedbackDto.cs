
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class CreateOrEditFormFeedbackDto : EntityDto<Guid?>
    {

		 public Guid? FormId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public Guid FeedbackFormId { get; set; }

        public string FeedbackFormData { get; set; }

        public int? TenantId { get; set; } // Nullable for host and annon users feedback  

        //public int Rating { get; set; }

        //public string Description { get; set; }


    }
}