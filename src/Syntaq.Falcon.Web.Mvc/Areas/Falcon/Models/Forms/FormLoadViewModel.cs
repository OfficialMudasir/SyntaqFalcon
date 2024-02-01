﻿using Abp.AutoMapper;
using Syntaq.Falcon.Forms.Dtos;
using System;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Forms
{
    [AutoMapFrom(typeof(GetFormForLoad))]
    public class FormLoadViewModel
	{
		public string AuthToken { get; set; }
		public string AnonAuthToken { get; set; }
		public string TenantName { get; set; }
		public int? TenantId { get; set; }
		public Guid FormId { get; set; }
		public string FormData { get; set; }
		public string FormName { get; set; }
		public Guid? RecordId { get; set; }
		public Guid? RecordMatterId { get; set; }
		public Guid? ProjectId { get; set; }
        public Guid? ReleaseId { get; set; }
        public string ProjectName { get; set; }
		//public Guid SubmissionId { get; set; }
	}
}
