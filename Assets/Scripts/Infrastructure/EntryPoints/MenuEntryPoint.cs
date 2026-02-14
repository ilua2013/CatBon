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
            var subscriptionService = GetSubscriptionService();
            mainMenu.Constructor(subscriptionService);

            if (!subscriptionService.IsThereSubscription())
                mainMenu.ShowSubscribeWindow();
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