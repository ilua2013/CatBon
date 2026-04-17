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

    // Основное публичное свойство
    public bool ISubscriptionIsActive => m_MonthActive || m_YearActive;

    private StoreController m_StoreController;

    // Унифицированные ID (должны совпадать со сторами)
    public const string SUB_MONTH = "month";
    public string SUB_YEAR = "year_subscribe";

    private List<ProductDefinition> m_ProductDefinitions;

    // Отдельные флаги для каждой подписки (более точный контроль)
    private bool m_MonthActive = false;
    private bool m_YearActive = false;

    public async void Initialize()
    {
        SUB_YEAR = Application.platform == RuntimePlatform.Android ? "year-subscribe" : "year_subscribe";

        m_ProductDefinitions = new List<ProductDefinition>
        {
            new ProductDefinition(SUB_MONTH, ProductType.Subscription),
            new ProductDefinition(SUB_YEAR, ProductType.Subscription)
        };

        await InitializeIAPAsync();
        SROptions.Current.OnLockAllContent += UnlockPremiumContent;
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

            Debug.Log($"[{Time.time:F2}] IAP: Starting connection to store...");

            // 1. Подключение к стору
            await m_StoreController.Connect();
            Debug.Log($"[{Time.time:F2}] IAP: Connected to store");

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
        Debug.Log($"[{Time.time:F2}] IAP: Products fetched → {products.Count} items");

        // После продуктов сразу запрашиваем покупки и entitlement
        m_StoreController.FetchPurchases();
    }

    private void OnPurchasesFetched(Orders orders)
    {
        Debug.Log($"[{Time.time:F2}] IAP: Purchases fetched → Confirmed: {orders.ConfirmedOrders.Count}, " +
                  $"Pending: {orders.PendingOrders.Count}, Deferred: {orders.DeferredOrders.Count}");

        RefreshSubscriptionStatus();
    }

    /// <summary>
    /// Основной метод обновления статуса подписок (вызывать при старте, после restore и после покупки)
    /// </summary>
    public void RefreshSubscriptionStatus()
    {
        Debug.Log($"[{Time.time:F2}] IAP: Refreshing subscription status...");

        var monthProduct = m_StoreController.GetProductById(SUB_MONTH);
        if (monthProduct != null)
            m_StoreController.CheckEntitlement(monthProduct);

        var yearProduct = m_StoreController.GetProductById(SUB_YEAR);
        if (yearProduct != null)
            m_StoreController.CheckEntitlement(yearProduct);
    }

    private void OnCheckEntitlement(Entitlement entitlement)
    {
        string productId = entitlement.Product?.definition.id ?? "unknown";
        var status = entitlement.Status;

        Debug.Log($"[{Time.time:F2}] OnCheckEntitlement → {productId}: {status}");

        bool shouldBeActive = false;

        switch (status)
        {
            case EntitlementStatus.FullyEntitled:
                shouldBeActive = true;
                Debug.Log($"[{Time.time:F2}] FullyEntitled → подписка активна");
                break;

            case EntitlementStatus.EntitledButNotFinished:
                shouldBeActive = true;
                Debug.Log($"[{Time.time:F2}] EntitledButNotFinished → подтверждаем pending заказ");

                if (entitlement.Order is PendingOrder pendingOrder)
                {
                    m_StoreController.ConfirmPurchase(pendingOrder);
                }
                break;

            case EntitlementStatus.EntitledUntilConsumed:
                shouldBeActive = true;
                break;

            case EntitlementStatus.NotEntitled:
            case EntitlementStatus.Unknown:
                Debug.Log($"[{Time.time:F2}] Нет entitlement для {productId}");
                break;
        }

        // Обновляем флаги
        if (productId == SUB_MONTH)
            m_MonthActive = shouldBeActive;
        else if (productId == SUB_YEAR)
            m_YearActive = shouldBeActive;

        // Проверяем общее состояние
        UpdatePremiumState();
    }

    private void UpdatePremiumState()
    {
        bool newState = m_MonthActive || m_YearActive;

        if (newState && !ISubscriptionIsActive) // было false → стало true
        {
            Debug.Log($"[{Time.time:F2}] Premium активирован!");
            UnlockPremiumContent();
        }
        else if (!newState && ISubscriptionIsActive) // было true → стало false
        {
            Debug.Log($"[{Time.time:F2}] Premium деактивирован");
            LockPremiumContent();
        }
    }

    private void OnPurchaseConfirmed(Order order)
    {
        if (order is not ConfirmedOrder confirmedOrder || confirmedOrder.Info.PurchasedProductInfo.Count == 0)
            return;

        string storeId = confirmedOrder.Info.PurchasedProductInfo[0].productId;
        var product = m_StoreController.GetProductById(storeId);

        string unifiedId = product?.definition.id ?? storeId;

        Debug.Log($"[{Time.time:F2}] Purchase confirmed for {unifiedId} → проверяем entitlement");

        if (product != null && (unifiedId == SUB_MONTH || unifiedId == SUB_YEAR))
        {
            m_StoreController.CheckEntitlement(product);   // ← Ключевой вызов!
        }
    }

    private void OnPurchasePending(PendingOrder pendingOrder)
    {
        if (pendingOrder.Info.PurchasedProductInfo.Count > 0)
        {
            string storeId = pendingOrder.Info.PurchasedProductInfo[0].productId;
            Debug.Log($"[{Time.time:F2}] Purchase pending for {storeId}");
        }
        // Здесь можно показать индикатор "Обработка покупки..."
    }

    private void OnPurchaseFailed(FailedOrder failedOrder)
    {
        Debug.LogWarning($"[{Time.time:F2}] Purchase failed: {failedOrder.FailureReason}");
    }

    private void BuyProduct(string productId)
    {
        if (m_StoreController == null)
        {
            Debug.LogError("StoreController not initialized");
            return;
        }

        Debug.Log($"[{Time.time:F2}] Starting purchase: {productId}");
        m_StoreController.PurchaseProduct(productId);
    }

    public void RestorePurchases()
    {
        if (m_StoreController == null) return;

        Debug.Log($"[{Time.time:F2}] Starting Restore Purchases...");

        m_StoreController.RestoreTransactions((success, message) =>
        {
            Debug.Log($"[{Time.time:F2}] Restore completed: success={success}, message={message}");

            if (success)
            {
                m_StoreController.FetchPurchases(); // → OnPurchasesFetched → RefreshSubscriptionStatus
            }
        });
    }

    public void BuySubscription(int value) // 0 = month, 1 = year
    {
        string productId = value == 0 ? SUB_MONTH : SUB_YEAR;
        BuyProduct(productId);
    }

    private void UnlockPremiumContent()
    {
        Debug.Log($"[{Time.time:F2}] === UNLOCK PREMIUM CONTENT ===");
        SubscriptionActivated?.Invoke();
        // Здесь обновляй UI, активируй весь контент, сохраняй состояние и т.д.
        // Важно: обновление должно происходить синхронно и сразу видно ревьюеру
    }

    private void LockPremiumContent()
    {
        Debug.Log($"[{Time.time:F2}] === LOCK PREMIUM CONTENT ===");
        SubscriptionDeactivated?.Invoke();
        SubscriptionEnded?.Invoke();
        // Здесь прячь/блокируй контент
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
        
        SROptions.Current.OnLockAllContent -= UnlockPremiumContent;
    }
}