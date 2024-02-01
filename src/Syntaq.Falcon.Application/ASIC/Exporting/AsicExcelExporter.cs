using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.ASIC.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.ASIC.Exporting
{
    public class AsicExcelExporter : NpoiExcelExporterBase, IAsicExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AsicExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

       
    }
}