using System.Collections.Generic;
using Syntaq.Falcon.Authorization.Delegation;
using Syntaq.Falcon.Authorization.Users.Delegation.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Layout
{
    public class ActiveUserDelegationsComboboxViewModel
    {
        public IUserDelegationConfiguration UserDelegationConfiguration { get; set; }
        
        public List<UserDelegationDto> UserDelegations { get; set; }

        public string CssClass { get; set; }
    }
}
