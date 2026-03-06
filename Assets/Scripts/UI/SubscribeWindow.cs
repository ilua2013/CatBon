using Services.Monetization.Subscription;
using TMPro;
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
        [SerializeField] private TMP_Text priceField;
        [SerializeField] private Button restoringPurchases;

        private int subscribeType;
        
        public void Show(ISubscriptionService subscriptionService)
        {
            this.subscriptionService = subscriptionService;
            gameObject.SetActive(true);
            
            closeButton.onClick.AddListener(Hide);
            subscribing.onClick.AddListener(Subscribing);
            restoringPurchases.onClick.AddListener(RestoringPurchases);
            monthToggle.onValueChanged.AddListener(MonthTextChange);
            yearToggle.onValueChanged.AddListener(YearTextChange);
        }

        private void RestoringPurchases()
        {
            Hide();
            subscriptionService.RestorePurchases();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            
            closeButton.onClick.RemoveListener(Hide);
            subscribing.onClick.RemoveListener(Subscribing);
            restoringPurchases.onClick.RemoveListener(RestoringPurchases);
            monthToggle.onValueChanged.RemoveListener(MonthTextChange);
            yearToggle.onValueChanged.RemoveListener(YearTextChange);
        }
        
        private void MonthTextChange(bool isOn)
        {
            if (isOn)
                priceField.text = "3 дня бесплатно, затем 290 руб/мес. \n" +
                                  "Автопродление. Отмена в любой момент.";
        }
        
        private void YearTextChange(bool isOn)
        {
            if (isOn)
                priceField.text = "3 дня бесплатно, затем 1790 руб/мес. \n" +
                                  "Автопродление. Отмена в любой момент.";
        }

        private void Subscribing()
        {
            var type = monthToggle.isOn ? 0 : 1;
            subscriptionService.BuySubscription(type);
            Hide();
        }
    }
}