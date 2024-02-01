﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Editions.Dto
{
    public class CreateEditionDto
    {
        [Required]
        public EditionCreateDto Edition { get; set; }

        [Required]
        public List<NameValueDto> FeatureValues { get; set; }
    }
}