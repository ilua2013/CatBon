using System;
using UnityEngine;

namespace Services.Monetization.Subscription
{
    public class EditorSubscriptionService : MonoBehaviour, ISubscriptionService
    {
        public event Action SubscriptionActivated;
        public event Action SubscriptionDeactivated;

        [field: SerializeField]
        public bool ISubscriptionIsActive { get; private set; }

        [SerializeField] private bool isThereAPurchaseOption;
        
        public void Initialize()
        {
            Debug.Log($"Инициализация сервиса покупок подписки");
        }

        public void BuySubscription(int value)
        {
            if (isThereAPurchaseOption)
            {
                Debug.Log($"Покупка оформлена!!! Подписка на {(value == 0 ? "Месяц" : "Год")}");
                SubscriptionActivated?.Invoke();
                ISubscriptionIsActive = true;
            }
            else
            {
                Debug.Log($"Подписка не оформлена!");
                ISubscriptionIsActive = false;
                SubscriptionDeactivated?.Invoke();
            }
        }
    }
}