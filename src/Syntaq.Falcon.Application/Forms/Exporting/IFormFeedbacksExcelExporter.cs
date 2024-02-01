using System.Collections.Generic;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Forms.Exporting
{
    public interface IFormFeedbacksExcelExporter
    {
        FileDto ExportToFile(List<GetFormFeedbackForViewDto> formFeedbacks);
    }
}