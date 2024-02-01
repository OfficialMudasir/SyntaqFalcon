using Abp.Domain.Services;

namespace Syntaq.Falcon
{
    public abstract class FalconDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected FalconDomainServiceBase()
        {
            LocalizationSourceName = FalconConsts.LocalizationSourceName;
        }
    }
}
