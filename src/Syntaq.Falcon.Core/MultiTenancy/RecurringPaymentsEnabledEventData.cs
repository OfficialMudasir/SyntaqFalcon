using Abp.Events.Bus;

namespace Syntaq.Falcon.MultiTenancy
{
    public class RecurringPaymentsEnabledEventData : EventData
    {
        public int TenantId { get; set; }
    }
}