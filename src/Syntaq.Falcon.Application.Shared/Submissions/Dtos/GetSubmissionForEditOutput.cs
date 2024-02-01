using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Submissions.Dtos
{
	public class GetSubmissionForEditOutput
	{
		public CreateOrEditSubmissionDto Submission { get; set; }
		public string RecordRecordName { get; set;}
		public string RecordMatterRecordMatterName { get; set;}
		public string UserName { get; set;}
		public string AppJobName { get; set;}
	}
}