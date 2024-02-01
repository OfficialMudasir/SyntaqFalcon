using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Records.Dtos
{
	public class RecordMatterDto : EntityDto<Guid>
	{
		public string Data { get; set; }

		public string AccessToken { get; set; }
		public string Comments { get; set; }

		public string RecordMatterName { get; set; }

		public Guid RecordId { get; set; }
		public Guid? RecordMatterItemId { get; set; }

		public DateTime CreationTime { get; set; }

        public DateTime LastModified { get; set; }

        //YICHAO FILE FLAG bool
        public bool HasFiles { get; set; }
		//

		[StringLength(RecordMatterConsts.MaxRulesSchemaLength, MinimumLength = RecordMatterConsts.MinRulesSchemaLength)]
		public string RulesSchema { get; set; }
 
		public RecordMatterConsts.RecordMatterStatus  Status { get; set; }

		public List<RecordMatterItemDto> RecordMatterItems { get; set; } = new List<RecordMatterItemDto>();

		public Guid? FormId { get; set; }
		public bool RequireApproval { get; set; }
		public bool RequireReview { get; set; }
        public int Order { get; set; }

        public List<RecordMatterAuditDto> RecordMatterAudits { get; set; } = new List<RecordMatterAuditDto>();

	}
}