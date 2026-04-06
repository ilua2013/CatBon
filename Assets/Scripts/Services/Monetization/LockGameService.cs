using Services.Monetization.Subscription;
using UnityEngine;

namespace Services.Monetization
{
    public class LockGameService : MonoBehaviour
    {
        [SerializeField] private GameObject[] locks;

        public void Initialize(ISubscriptionService subscriptionService)
        {
            subscriptionService.SubscriptionActivated += UnlockerGames;
            subscriptionService.SubscriptionDeactivated += LockGames;
            
            if(subscriptionService.ISubscriptionIsActive)
                UnlockerGames();
            else
                LockGames();
        }

        private void LockGames()
        {
            foreach (var game in locks)
            {
                game.SetActive(true);
            }
        }

        private void UnlockerGames()
        {
            foreach (var game in locks)
            {
                game.SetActive(false);
            }
        }
    }
}