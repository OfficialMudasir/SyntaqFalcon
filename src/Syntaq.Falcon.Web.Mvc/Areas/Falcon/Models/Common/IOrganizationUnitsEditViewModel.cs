using System.Collections.Generic;
using Syntaq.Falcon.Organizations.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Common
{
    public interface IOrganizationUnitsEditViewModel
    {
        List<OrganizationUnitDto> AllOrganizationUnits { get; set; }

        List<string> MemberedOrganizationUnits { get; set; }
    }
}