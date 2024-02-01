using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class GetAllFormsFoldersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}