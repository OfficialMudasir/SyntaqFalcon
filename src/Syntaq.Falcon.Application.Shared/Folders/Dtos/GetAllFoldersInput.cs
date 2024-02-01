using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Folders.Dtos
{
    public class GetAllFoldersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}