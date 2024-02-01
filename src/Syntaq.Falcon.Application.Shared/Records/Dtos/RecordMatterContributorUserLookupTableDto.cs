using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Records.Dtos
{
    public class RecordMatterContributorUserLookupTableDto
    {
		public long Id { get; set; }

		public string DisplayName { get; set; }
        public string Surname { get; set; }

        public string Email { get; set; }

        public string Entity { get; set; }
    }
}