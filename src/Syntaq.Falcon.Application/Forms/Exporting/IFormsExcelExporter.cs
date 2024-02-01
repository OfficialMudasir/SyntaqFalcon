using System.Collections.Generic;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Forms.Exporting
{
    public interface IFormsExcelExporter
    {
        FileDto ExportToFile(List<GetFormForView> forms);
    }
}