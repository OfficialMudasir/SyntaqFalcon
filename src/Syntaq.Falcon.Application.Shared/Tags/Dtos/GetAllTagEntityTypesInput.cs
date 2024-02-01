using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetAllTagEntityTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int? EntityTypeFilter { get; set; }

        public string TagNameFilter { get; set; }

    }
}