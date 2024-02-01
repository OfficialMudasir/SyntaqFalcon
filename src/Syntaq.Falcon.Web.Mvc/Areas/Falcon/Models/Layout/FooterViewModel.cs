using Syntaq.Falcon.Sessions.Dto;
using Syntaq.Falcon.Users;
using Syntaq.Falcon.Users.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Layout
{
    public class FooterViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }

        public List<UserAcceptanceTypeDto> ActiveUserAcceptanceTypesList { get; set; }

        public string GetProductNameWithEdition()
        {
            const string productName = "Falcon";

            if (LoginInformations.Tenant?.Edition?.DisplayName == null)
            {
                return productName;
            }

            return productName + " " + LoginInformations.Tenant.Edition.DisplayName;
        }
    }

    public class SubheaderViewModel
    {
        public string Title { get; set; }
        
        public string Description { get; set; }
    }
}
