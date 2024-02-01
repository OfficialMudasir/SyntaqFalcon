using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Vouchers.Dtos
{
    public class VoucherUsageLookupDto : EntityDto<Guid>
    {
        //public string Id { get; set; } //inherited from EntityDto ??

        public Guid? EntityId { get; set; }

        public long UserID { get; set; }

        public DateTime DateRedeemed { get; set; }

        public string EntityName { get; set; }

        public string EntityType { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        //public Guid? VoucherIdFk { get; set; }
    }
}
