using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Records.Dtos;
using System;
using System.Collections.Generic;

namespace Syntaq.Falcon.Forms.Dtos
{
	public class GetFormForView
	{
		public string AnonAuthToken { get; set; }
        public string UserACLPermission { get; set; }
        public bool hasDocuments { get; set; } = false;
		public FormDto Form { get; set; }
		public ProjectDto Project { get; set; }
		public GetRecordMatterContributorForViewDto Contributor {get;set;}	
		public List<GetDocumentForView> Documents { get; set; }
		public CreateOrEditRecordDto Record { get; set; }
		public CreateOrEditRecordMatterDto RecordMatter { get; set; }
		public CreateOrEditRecordMatterItemDto RecordMatterItem { get; set; }

		public Guid? ReleaseId { get; set; }
	}

	public class GetDocumentForView
    {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Format { get; set; }
	}
	
}