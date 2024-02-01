using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.VoucherEntitites;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.VoucherUsages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Vouchers
{
    public class VoucherManager : IVoucherManager
    {
        private readonly IRepository<Voucher, Guid> _voucherRepository;
        private readonly IRepository<VoucherEntity, Guid> _voucherEntityRepository;
        private readonly IRepository<VoucherUsage, Guid> _voucherUsageRepository;

        public VoucherManager(
            IRepository<Voucher,
            Guid> voucherRepository,
            IRepository<VoucherEntity, Guid> voucherEntityRepository,
            IRepository<VoucherUsage, Guid> voucherUsageRepository           
        )
        {
            _voucherRepository = voucherRepository;
            _voucherEntityRepository = voucherEntityRepository;
            _voucherUsageRepository = voucherUsageRepository;
        }

        public async Task<VoucherValidityDto> GetVoucherDetailsByKey(GetVoucherDetailsByKeyInput input)
        {   //checking voucher validity for usage
            // this is only checking form entities currently, but will adapt to include switch case for document entities

            Voucher voucher = await _voucherRepository.FirstOrDefaultAsync(i => i.Key == input.VoucherKey);
            if (voucher != null)
            {
                //check balance is not lower than voucher value!!
                //if (Convert.ToDecimal (input.Balance) > voucher.Value)
                //{
                    if (voucher.Expiry > DateTime.Now)
                    { // voucher has not expired, valid
                        if (voucher.NoOfUses == null) //unlimited usage avail
                        {
                            return CheckVoucherValidityOnEntities((Guid)input.EntityId, voucher.Key);
                        }
                        else /*if (voucher.NoOfUses != null)*/
                        {  //return count of vUsages with that VoucherId
                            var vUsages = _voucherUsageRepository.GetAll().Where(i => i.VoucherFk.Id == voucher.Id);
                            var usageCount = await vUsages.CountAsync();

                            if (usageCount < voucher.NoOfUses)
                            {
                                return CheckVoucherValidityOnEntities((Guid)input.EntityId, voucher.Key);
                            }
                            else /*if (usageCount >= voucher.NoOfUses)*/
                            { //usage limit has already been reached 
                                var message = "The voucher is not valid for use; no usage available";
                                return InvalidVoucherResponse(message);
                            };
                        };
                    }
                    else
                    {
                        var message = "Expired voucher";
                        return InvalidVoucherResponse(message);
                    };
                //}
                //else
                //{
                //    var message = "Voucher value cannot be greater than the payment value";
                //    return InvalidVoucherResponse(message);
                //}
            }
            else
            {
                var message = "There is no voucher for that key";
                return InvalidVoucherResponse(message);
            };
        }

        private VoucherValidityDto CheckVoucherValidityOnEntities(Guid entityId, string voucherKey)
        {
            var voucher = _voucherRepository.FirstOrDefault(i => i.Key == voucherKey);

            var vEntityList = voucher.VoucherEntities;
            if (vEntityList.Any())
            {
                if (vEntityList.Count() != 0 && vEntityList.Any(i => i.EntityKey ==  entityId ))
                {
                    var message = "Success. Voucher has been applied";
                    return ValidVoucherResponse(voucher, message);
                }
                else if (vEntityList.Count() != 0 && !vEntityList.Any(i => i.EntityKey ==  entityId ))
                {
                    var message = "voucherKey is valid, but cannot be used for that entity";
                    return InvalidVoucherResponse(message);
                }
                else
                { // there are no entities attached to that voucher
                    var message = "there are no entities attached to that voucher";
                    return InvalidVoucherResponse(message);
                }
            }

            return ValidVoucherResponse(voucher, "Success. Voucher has been applied");
        }

        private static VoucherValidityDto InvalidVoucherResponse(string message)
        {
            return new VoucherValidityDto()
            {
                VoucherId = null,
                VoucherValid = false,
                ValidityMessage = message,
                NoOfUses = null,
                VoucherValue = null,
                DiscountType = ""
            };
        }

        private static VoucherValidityDto ValidVoucherResponse(Voucher voucher, string message)
        {
            return new VoucherValidityDto()
            {
                VoucherId = voucher.Id,
                VoucherValid = true,
                ValidityMessage = message,
                NoOfUses = voucher.NoOfUses,
                VoucherValue = voucher.Value,
                DiscountType = voucher.DiscountType
            };
        }

        public Task<bool> CheckVoucherKeyNotExisting(string vKey)
        {
            var v = _voucherRepository.GetAll().Where(i => i.Key == vKey);
            if (v.Count() != 0)
            {
                return Task.FromResult(false);
            }
            else
            {
                return Task.FromResult(true);
            }
        }


    }
}
