using System;
using System.Collections.Generic;
using System.Linq;
using Services.Monetization.Subscription;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Services.Monetization
{
    public class AppStoreSubscriptionService : ISubscriptionService
    {
        private const string SubscriptionMonthProductId = ""; 
        private const string SubscriptionYearProductId = ""; 
        
        private StoreController storeController;

        public event Action SubscriptionActivated;
        public event Action SubscriptionDeactivated;
        public event Action SubscriptionEnded;

        public bool ISubscriptionIsActive { get; }

        public async void Initialize()
        {
            storeController = UnityIAPServices.StoreController();

            storeController.OnPurchasePending += OnPurchasePending;

            await storeController.Connect();

            storeController.OnStoreDisconnected += OnStoreDisconnected;
            
            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnPurchasesFetched += OnPurchasesFetched;
            
            var initialProductsToFetch = new List<ProductDefinition>  
            {  
                new(SubscriptionMonthProductId, ProductType.Subscription),  
                new(SubscriptionYearProductId, ProductType.Subscription)
            };  
  
            storeController.FetchProducts(initialProductsToFetch);  
        }

        private void OnPurchasePending(PendingOrder pending)
        {
            var product = pending.CartOrdered.Items().First().Product;
            var isFreeTrial = IsProductFreeTrial(product);
            
            if (isFreeTrial)
            {
                Debug.Log("Free trial started!");
                // Здесь можно показать специальное сообщение или просто активировать контент
            }
            else
            {
                Debug.Log("Paid subscription started.");
            }
        }

        public void BuySubscription(int value)
        {
            if(storeController == null)
            {
                Debug.Log($"Сервис покупок не доступен");
                return;
            }
            
            storeController.PurchaseProduct(value == 0 ? SubscriptionMonthProductId: SubscriptionYearProductId);
        }
        
        private void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            Debug.Log($"Отключение от сервиса покупок");
        }

        private void OnPurchasesFetched(Orders obj)
        {
            
        }

        private void OnProductsFetched(List<Product> obj)
        {
        }
        
        private bool IsProductFreeTrial(Product product)
        {
            if (product.definition.type == ProductType.Subscription)
            {
                SubscriptionManager subscriptionManager = new SubscriptionManager(product, null);
                var info = subscriptionManager.getSubscriptionInfo();
                return info.isFreeTrial() == Result.True;
            }
            return false;
        }
    }
}