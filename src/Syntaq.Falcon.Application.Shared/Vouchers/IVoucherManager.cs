using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.VoucherUsages.Dtos;
using Syntaq.Falcon.Vouchers;

namespace Syntaq.Falcon.Vouchers
{
    public interface IVoucherManager : IApplicationService 
    {

        Task<VoucherValidityDto> GetVoucherDetailsByKey(GetVoucherDetailsByKeyInput input);


        //VoucherValidityDto CheckVoucherValidityOnEntities(string entityId, string voucherKey);
    }
}