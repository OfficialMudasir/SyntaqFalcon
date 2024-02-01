using Abp.AutoMapper;
using Syntaq.Falcon.Users.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.UserAcceptances
{
	[AutoMapFrom(typeof(GetUserAcceptanceForViewDto))]
	public class UserAcceptanceViewModel : GetUserAcceptanceForViewDto
	{

	}
}
