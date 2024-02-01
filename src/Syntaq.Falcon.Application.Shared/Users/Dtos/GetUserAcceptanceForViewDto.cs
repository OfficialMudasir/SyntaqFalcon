namespace Syntaq.Falcon.Users.Dtos
{
    public class GetUserAcceptanceForViewDto
    {
		public UserAcceptanceDto UserAcceptance { get; set; }

		public string UserAcceptanceTypeName { get; set;}
		public string UserName { get; set;}
		public string UserFirstName { get; set; }
		public string UserSurname { get; set; }
		public string UserEmailAddress { get; set; }
		public string RecordMatterContributorName { get; set;}

		public int? TenantId { get; set; }
		public string TenantName { get; set; }
	}
}