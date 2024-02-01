using System.Collections.Generic;
using Syntaq.Falcon.Editions.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Tenants
{
    public class TenantIndexViewModel
    {
        public List<SubscribableEditionComboboxItemDto> EditionItems { get; set; }
    }
}