using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Files.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;


namespace Syntaq.Falcon.Files
{
    public interface IFilesAppService : IApplicationService 
    {
        PagedResultDto<GetFileForViewDto> GetFiles(FilesDto filesDto);

        // MemoryStream DownloadFileById(FilesDto filesDto);
        FileStreamResult DownloadFileById(FilesDto filesDto);
        //HttpResponseMessage DownloadFileById(FilesDto filesDto);

        MemoryStream DownloadAllFilesByRecordMatterId(FilesDto filesDto);
    }
}