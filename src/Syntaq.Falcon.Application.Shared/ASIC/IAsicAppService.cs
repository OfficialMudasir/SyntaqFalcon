using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.ASIC.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.ASIC
{
    public interface IAsicAppService : IApplicationService
    {
        //Task<PagedResultDto<GetAsicForViewDto>> GetAll(GetAllAsicInput input);

        //Task<GetAsicForViewDto> GetAsicForView(Guid id);

        //Task<GetAsicForEditOutput> GetAsicForEdit(EntityDto<Guid> input);

        //Task CreateOrEdit(CreateOrEditAsicDto input);

        //Task Delete(EntityDto<Guid> input);

        //Task<FileDto> GetAsicToExcel(GetAllAsicForExcelInput input);
        Task<JToken> CheckName(string companyName);
        Task<Byte[]>Get201Certificate(int requestId);
        Task<string> Check201Status(int requestId);
        Task<string> Check201Logs(int requestId);      
        MemoryStream DownloadAllDocumentsByRequestId(int tracenumber);
        Task<GetDocumentResultDto> GetAllDocumentsByRequestId(int requestId);
        Task<byte[]> GetAsic201Documents(int requestId);

    }
}