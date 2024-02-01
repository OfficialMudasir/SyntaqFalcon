using Abp.AutoMapper;
using Abp.Organizations;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.OrganizationUnits
{
    [AutoMapFrom(typeof(OrganizationUnit))]
    public class EditOrganizationUnitModalViewModel
    {
        public long? Id { get; set; }

        public string DisplayName { get; set; }
    }
}