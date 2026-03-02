using Services.Monetization;
using Services.Monetization.Subscription;
using UnityEngine;

namespace Infractructure.EntryPoints
{
    public class MenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private MainMenu2 mainMenu;
        
        private void Start()
        {
            var subscriptionService = new AppStoreSubscriptionService(); //GetSubscriptionService();
            subscriptionService.Initialize();
            mainMenu.Constructor(subscriptionService);
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