using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Documents.Dtos
{
	public class GetAllDocumentsInput : PagedAndSortedResultRequestDto
	{
		public string Filter { get; set; }



	}
}