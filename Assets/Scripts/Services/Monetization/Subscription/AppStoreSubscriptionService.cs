using Services.Monetization.Subscription;

namespace Services.Monetization
{
    public class AppStoreSubscriptionService : ISubscriptionService
    {
        public bool IsThereSubscription()
        {
            return false;
        }
    }
}