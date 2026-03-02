using Services.Monetization.Subscription;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SubscribeWindow : MonoBehaviour
    {
        private ISubscriptionService subscriptionService;

        [SerializeField] private Button subscribing;
        [SerializeField] private Button closeButton;
        [SerializeField] private Toggle monthToggle;
        [SerializeField] private Toggle yearToggle;
        
        public void Show(ISubscriptionService subscriptionService)
        {
            this.subscriptionService = subscriptionService;
            gameObject.SetActive(true);
            
            closeButton.onClick.AddListener(Hide);
            subscribing.onClick.AddListener(Subscribing);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            
            closeButton.onClick.RemoveListener(Hide);
            subscribing.onClick.RemoveListener(Subscribing);
        }

        private void Subscribing()
        {
            var type = monthToggle.isOn ? 0 : 1;
            subscriptionService.BuySubscription(type);
            Hide();
        }
    }
}