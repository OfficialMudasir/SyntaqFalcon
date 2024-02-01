using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Folders.Dtos
{
    public class GetFolderForEditOutput
    {
		public CreateOrEditFolderDto Folder { get; set; }


    }
}