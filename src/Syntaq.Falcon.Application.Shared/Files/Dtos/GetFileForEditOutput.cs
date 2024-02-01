using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Files.Dtos
{
    public class GetFileForEditOutput
    {
		public CreateOrEditFileDto File { get; set; }

		public string RecordRecordName { get; set;}

		public string RecordMatterRecordMatterName { get; set;}
    }
}