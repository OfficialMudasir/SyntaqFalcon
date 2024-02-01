using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
    public class GetMergeTextItemForEditOutput
    {
		public CreateOrEditMergeTextItemDto MergeTextItem { get; set; }

		public string MergeTextItemValueKey { get; set;}


    }
}