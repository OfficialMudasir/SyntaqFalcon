using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class GetFormFeedbackForEditOutput
    {
		public CreateOrEditFormFeedbackDto FormFeedback { get; set; }

		public string FormName { get; set;}


    }
}