using System;

namespace Services.Monetization.Subscription
{
    public interface ISubscriptionService
    {
        event Action SubscriptionActivated;
        event Action SubscriptionDeactivated;
        event Action SubscriptionEnded;
        bool ISubscriptionIsActive { get; }
        
        void Initialize();
        void BuySubscription(int value);
    }
}