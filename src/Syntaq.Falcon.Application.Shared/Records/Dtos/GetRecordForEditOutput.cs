using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetRecordForEditOutput
    {
		public CreateOrEditRecordDto Record { get; set; }

        public bool IsEditMode => Record.Id.HasValue;
    }
}