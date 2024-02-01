using System.Threading.Tasks;
using Abp.Dependency;

namespace Syntaq.Falcon.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}