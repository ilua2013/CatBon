using System;

namespace Services.Monetization.Subscription
{
    public interface ISubscriptionService
    {
        event Action SubscriptionActivated;
        event Action SubscriptionDeactivated;
        bool ISubscriptionIsActive { get; }
        
        void Initialize();
        void BuySubscription(int value);
    }
}