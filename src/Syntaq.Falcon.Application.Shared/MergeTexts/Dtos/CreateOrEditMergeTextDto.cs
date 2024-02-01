using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
	public class CreateOrEditMergeTextDto : EntityDto<long?>
	{
		public string MergeTextEntityType { get; set; } //Mtxt Entity Type
		public string MergeTextEntityKey { get; set; } //Mtxt Entity Key
		//public long MergeTextItemId { get; set; } //Mtxt Item Id
		//public string Name { get; set; } //Mtxt Item Name
		//public List<CreateOrEditMergeTextItemValueDto> MTextList { get; set; }  //Mtxt Item Value list

		//[Required]
		//[StringLength(MergeTextConsts.MaxEntityTypeLength, MinimumLength = MergeTextConsts.MinEntityTypeLength)]
		//public string EntityType { get; set; }	

		//[Required]
		//[StringLength(MergeTextConsts.MaxEntityKeyLength, MinimumLength = MergeTextConsts.MinEntityKeyLength)]
		//public string EntityKey { get; set; }		

		//public long? MergeTextItemId { get; set; }
	}
}