using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.EntityVersionHistories.Dtos
{
    public class GetEntityVersionHistoryForEditOutput
    {
        public CreateOrEditEntityVersionHistoryDto EntityVersionHistory { get; set; }

        public string UserName { get; set; }

    }
}