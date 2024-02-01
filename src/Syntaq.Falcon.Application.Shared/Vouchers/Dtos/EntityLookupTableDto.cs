using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Vouchers.Dtos
{
    public class EntityLookupTableDto : EntityDto<Guid>
    {
        //public string Id { get; set; } //inherited from EntityDto??

        public string DisplayName { get; set; }
    }
}
