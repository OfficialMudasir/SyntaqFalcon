using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class GetAsicForEditOutput
    {
        public CreateOrEditAsicDto Asic { get; set; }

    }
}