using System;

namespace Services.Monetization.Subscription
{
    public class GooglePlaySubscriptionService : ISubscriptionService
    {
        public bool IsThereSubscription()
        {
            return false;
        }

        public event Action SubscriptionActivated;
        public event Action SubscriptionDeactivated;
        public event Action SubscriptionEnded;

        public bool ISubscriptionIsActive { get; }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void BuySubscription(int value)
        {
            throw new System.NotImplementedException();
        }
    }
}
