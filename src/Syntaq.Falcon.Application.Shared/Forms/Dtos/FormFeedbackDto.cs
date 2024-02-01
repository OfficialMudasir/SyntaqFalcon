
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class FormFeedbackDto : EntityDto<Guid>
    {

		 public Guid? FormId { get; set; }

        public DateTime CreationTime { get; set; }

    }
}