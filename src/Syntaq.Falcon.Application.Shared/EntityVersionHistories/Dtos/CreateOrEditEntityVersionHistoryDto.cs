using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.EntityVersionHistories.Dtos
{
    public class CreateOrEditEntityVersionHistoryDto : EntityDto<Guid?>
    {

        [Required]
        [StringLength(EntityVersionHistoryConsts.MaxNameLength, MinimumLength = EntityVersionHistoryConsts.MinNameLength)]
        public string Name { get; set; }

        public string VersionName { get; set; }

        [StringLength(EntityVersionHistoryConsts.MaxDescriptionLength, MinimumLength = EntityVersionHistoryConsts.MinDescriptionLength)]
        public string Description { get; set; }

        public int Version { get; set; }

        public int PreviousVersion { get; set; }

        public Guid EntityId { get; set; }

        public string Data { get; set; }

        [Required]
        public string Type { get; set; }

        public long? UserId { get; set; }

    }
}