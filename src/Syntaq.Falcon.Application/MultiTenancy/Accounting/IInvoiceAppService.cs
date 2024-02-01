using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.MultiTenancy.Accounting.Dto;

namespace Syntaq.Falcon.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
