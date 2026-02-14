using System;
using Services.GameBlocker;
using Services.Monetization;
using Services.Monetization.Subscription;
using UnityEngine;

namespace Infractructure.EntryPoints
{
    public class MenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private GameBlockerService.Setting gameBlockerServiceSetting;
        
        private void Start()
        {
            var subscriptionService = GetSubscriptionService();
            var gameBlockerService = new GameBlockerService(gameBlockerServiceSetting);
            
            if(subscriptionService.IsThereSubscription())
                gameBlockerService.Block();
        }

        private ISubscriptionService GetSubscriptionService() =>
            Application.platform switch
            {
                RuntimePlatform.Android => new GooglePlaySubscriptionService(),
                RuntimePlatform.IPhonePlayer => new AppStoreSubscriptionService(),
                _ => new EditorSubscriptionService()
            };
    }
}