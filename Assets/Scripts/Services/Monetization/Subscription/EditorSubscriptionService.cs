using UnityEngine;

namespace Services.Monetization.Subscription
{
    public class EditorSubscriptionService : MonoBehaviour, ISubscriptionService
    {
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
            }
            else
            {
                Debug.Log($"Подписка не оформлена!");
            }
        }
    }
}