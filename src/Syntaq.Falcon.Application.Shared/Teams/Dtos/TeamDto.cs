
using System;
using System.Numerics;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class TeamDto : EntityDto<Guid>
    {
        public new BigInteger Id { get; set; }
        public string DisplayName { get; set; }
    }
}