using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetAllTagEntitiesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public Guid? EntityIdFilter { get; set; }

        public int? EntityTypeFilter { get; set; }

        public string TagValueValueFilter { get; set; }

    }
}