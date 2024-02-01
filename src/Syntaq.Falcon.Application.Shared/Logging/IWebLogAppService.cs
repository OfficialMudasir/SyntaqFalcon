using Abp.Application.Services;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Logging.Dto;

namespace Syntaq.Falcon.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
