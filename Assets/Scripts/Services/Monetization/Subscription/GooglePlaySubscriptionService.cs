namespace Services.Monetization.Subscription
{
    public class GooglePlaySubscriptionService : ISubscriptionService
    {
        public bool IsThereSubscription()
        {
            return false;
        }

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
