using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Monetization.Subscription;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : ISubscriptionService
{
    public event Action SubscriptionActivated;
    public event Action SubscriptionDeactivated;
    public event Action SubscriptionEnded;
    
    public bool ISubscriptionIsActive { get; }
    
    private StoreController m_StoreController;

    // Унифицированные ID (должны совпадать со сторами)
    public const string SUB_MONTH = "month";
    public string SUB_YEAR = "year_subscribe";

    private List<ProductDefinition> m_ProductDefinitions;

    private bool m_IsPremiumUnlocked = false;

    public async void Initialize()
    {
        SUB_YEAR = Application.platform == RuntimePlatform.Android ? "year-subscribe" : "year_subscribe";
        
        m_ProductDefinitions = new()
        {
            new ProductDefinition(SUB_MONTH, ProductType.Subscription),
            new ProductDefinition(SUB_YEAR, ProductType.Subscription)
        };
        
        await InitializeIAPAsync();
    }

    private async Task InitializeIAPAsync()
    {
        try
        {
            m_StoreController = UnityIAPServices.StoreController();

            // Подписка на события
            m_StoreController.OnPurchasePending += OnPurchasePending;
            m_StoreController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            m_StoreController.OnPurchaseFailed += OnPurchaseFailed;
            m_StoreController.OnProductsFetched += OnProductsFetched;
            m_StoreController.OnPurchasesFetched += OnPurchasesFetched;
            m_StoreController.OnCheckEntitlement += OnCheckEntitlement;

            // 1. Подключение к стору
            await m_StoreController.Connect();
            Debug.Log("IAP: Connected to store");

            // 2. Запрос продуктов
            m_StoreController.FetchProducts(m_ProductDefinitions);
        }
        catch (Exception ex)
        {
            Debug.LogError($"IAP init failed: {ex.Message}");
        }
    }

    private void OnProductsFetched(List<Product> products)
    {
        Debug.Log($"IAP: Products fetched → {products.Count} items");

        // После продуктов — запрос покупок
        m_StoreController.FetchPurchases();
    }

    private void OnPurchasesFetched(Orders orders)
    {
        Debug.Log(
            $"IAP: Purchases fetched → Confirmed: {orders.ConfirmedOrders.Count}, Pending: {orders.PendingOrders.Count}, Deferred: {orders.DeferredOrders.Count}");

        // Проверяем entitlement для подписок
        CheckEntitlementForSubscriptions();
    }

    private void CheckEntitlementForSubscriptions()
    {
        Product? monthProduct = m_StoreController.GetProductById(SUB_MONTH);
        if (monthProduct != null)
            m_StoreController.CheckEntitlement(monthProduct);

        Product? yearProduct = m_StoreController.GetProductById(SUB_YEAR);
        if (yearProduct != null)
            m_StoreController.CheckEntitlement(yearProduct);
    }

    private void OnCheckEntitlement(Entitlement entitlement)
    {
        string productId = entitlement.Product?.definition.id ?? "unknown";
        var status = entitlement.Status;

        Debug.Log($"OnCheckEntitlement для {productId}: статус = {status}");

        bool isEntitled = false;

        switch (status)
        {
            case EntitlementStatus.FullyEntitled:
                isEntitled = true;
                Debug.Log($"FullyEntitled → подписка полностью активна");
                break;

            case EntitlementStatus.EntitledButNotFinished:
                isEntitled = true; // entitlement уже дано, но транзакция pending → unlock можно
                Debug.Log(
                    $"EntitledButNotFinished → entitlement есть, но нужно завершить транзакцию (ConfirmPurchase если pending order)");

                // Если entitlement.Order != null и это PendingOrder — можно подтвердить вручную:
                // if (entitlement.Order is PendingOrder pending) m_StoreController.ConfirmPurchase(pending);
                // Но для подписок обычно не требуется — стор сам обработает
                break;

            case EntitlementStatus.EntitledUntilConsumed:
                isEntitled = true; // для consumables; для подписок редко, но unlock если пришло
                Debug.Log($"EntitledUntilConsumed → entitlement до потребления (часто consumables)");
                break;

            case EntitlementStatus.NotEntitled:
                Debug.Log($"NotEntitled → нет доступа");
                break;

            case EntitlementStatus.Unknown:
                Debug.Log($"Unknown → статус неизвестен (проверь entitlement.ErrorMessage если есть)");
                break;

            default:
                Debug.Log($"Неизвестный статус entitlement: {status}");
                break;
        }

        if (isEntitled && (productId == SUB_MONTH || productId == SUB_YEAR))
        {
            m_IsPremiumUnlocked = true;
            UnlockPremiumContent();
        }
        else
        {
            m_IsPremiumUnlocked = false;
            LockPremiumContent();
            // Для точного lock: добавь флаги m_MonthEntitled / m_YearEntitled
            // и lock только если оба false
            // Пока: unlock если хотя бы одна подписка entitled
        }
    }

    private void OnPurchasePending(PendingOrder pendingOrder)
    {
        // ID продукта из info (store-specific ID)
        if (pendingOrder.Info.PurchasedProductInfo.Count > 0)
        {
            string storeId = pendingOrder.Info.PurchasedProductInfo[0].productId;
            Debug.Log($"Purchase pending: {storeId}");
        }
        // Показать UI "Обработка..."
    }

    private void OnPurchaseConfirmed(Order order)
    {
        if (order is ConfirmedOrder confirmedOrder)
        {
            if (confirmedOrder.Info.PurchasedProductInfo.Count > 0)
            {
                string storeId = confirmedOrder.Info.PurchasedProductInfo[0].productId;
                Product? product = m_StoreController.GetProductById(storeId); // Или match по definition.storeSpecificId
                string unifiedId = product?.definition.id ?? storeId;

                if (unifiedId == SUB_MONTH || unifiedId == SUB_YEAR)
                {
                    Debug.Log($"Purchase confirmed: {unifiedId}");
                    m_IsPremiumUnlocked = true;
                    UnlockPremiumContent();
                }
                else
                {
                    m_IsPremiumUnlocked = false;
                    LockPremiumContent();
                }
            }
        }
    }

    private void OnPurchaseFailed(FailedOrder failedOrder)
    {
        Debug.LogWarning($"Purchase failed: {failedOrder.FailureReason}");
    }

    private void BuyProduct(string productId)
    {
        if (m_StoreController == null)
        {
            Debug.LogError("StoreController not initialized");
            return;
        }

        m_StoreController.PurchaseProduct(productId);
    }

    public void RestorePurchases()
    {
        if (m_StoreController == null) return;

        m_StoreController.RestoreTransactions((success, message) =>
        {
            Debug.Log($"Restore: success={success}, message={message}");
            if (success)
                m_StoreController.FetchPurchases(); // Обновить статус
        });
    }

    public void BuySubscription(int value)
    {
        BuyProduct(value == 0 ? SUB_MONTH : SUB_YEAR);
    }

    private void UnlockPremiumContent()
    {
        Debug.Log("Premium unlocked!");
        m_IsPremiumUnlocked = true;
        SubscriptionActivated?.Invoke();
    }

    private void LockPremiumContent()
    {
        Debug.Log("Premium locked");
        m_IsPremiumUnlocked = false;
        SubscriptionDeactivated?.Invoke();
        SubscriptionEnded?.Invoke();
    }

    public void OnDestroy()
    {
        if (m_StoreController == null) return;

        m_StoreController.OnPurchasePending -= OnPurchasePending;
        m_StoreController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
        m_StoreController.OnPurchaseFailed -= OnPurchaseFailed;
        m_StoreController.OnProductsFetched -= OnProductsFetched;
        m_StoreController.OnPurchasesFetched -= OnPurchasesFetched;
        m_StoreController.OnCheckEntitlement -= OnCheckEntitlement;
    }
}