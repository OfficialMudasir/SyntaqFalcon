using System.Collections.Generic;
using Abp;
using Syntaq.Falcon.Chat.Dto;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages);
    }
}
