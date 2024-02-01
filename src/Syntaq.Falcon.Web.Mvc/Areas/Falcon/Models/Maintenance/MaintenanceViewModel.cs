using System.Collections.Generic;
using Syntaq.Falcon.Caching.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Maintenance
{
    public class MaintenanceViewModel
    {
        public IReadOnlyList<CacheDto> Caches { get; set; }
    }
}