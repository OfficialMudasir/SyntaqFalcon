using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
    public class GetMergeTextForEditOutput
    {
		public CreateOrEditMergeTextDto MergeText { get; set; }

		public string MergeTextItemName { get; set;}


    }
}