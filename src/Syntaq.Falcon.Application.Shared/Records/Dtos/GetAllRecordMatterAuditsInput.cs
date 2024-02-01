using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetAllRecordMatterAuditsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int? StatusFilter { get; set; }

        public string DataFilter { get; set; }

        public string UserNameFilter { get; set; }

        public string RecordMatterRecordMatterNameFilter { get; set; }

    }
}