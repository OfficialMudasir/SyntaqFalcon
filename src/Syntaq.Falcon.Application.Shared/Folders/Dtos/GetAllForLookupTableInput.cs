using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Folders.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

		public string Filter { get; set; }
    }
}