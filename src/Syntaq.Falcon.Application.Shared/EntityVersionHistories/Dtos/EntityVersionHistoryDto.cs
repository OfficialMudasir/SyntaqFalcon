using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.EntityVersionHistories.Dtos
{
    public class EntityVersionHistoryDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string VersionName { get; set; }

        public string Description { get; set; }

        public int Version { get; set; }

        public int PreviousVersion { get; set; }

        public string Type { get; set; }

        public long? UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public string Data { get; set; }

        public virtual string PreviousData { get; set; }
        public virtual string NewData { get; set; }

    }
}