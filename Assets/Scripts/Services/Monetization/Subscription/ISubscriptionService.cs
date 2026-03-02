namespace Services.Monetization.Subscription
{
    public interface ISubscriptionService
    {
        void Initialize();
        void BuySubscription(int value);
    }
}