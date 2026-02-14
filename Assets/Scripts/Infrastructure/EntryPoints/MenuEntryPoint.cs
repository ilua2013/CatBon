using System;
using Services.Monetization;
using Services.Monetization.Subscription;
using UnityEngine;

namespace Infractructure.EntryPoints
{
    public class MenuEntryPoint : MonoBehaviour
    {
        private void Start()
        {
            var subscriptionService = GetSubscriptionService();
            
            
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