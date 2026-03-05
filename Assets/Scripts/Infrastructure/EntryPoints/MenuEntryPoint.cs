using Services.Monetization;
using Services.Monetization.Subscription;
using UnityEngine;

namespace Infractructure.EntryPoints
{
    public class MenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private MainMenu2 mainMenu;
        [SerializeField] private EditorSubscriptionService editorSubscriptionService;
        [SerializeField] private LockGameService lockGameService;
        
        private async void Start()
        {
            //var subscriptionService = new AppStoreSubscriptionService(); //GetSubscriptionService();
            var subscriptionService = GetSubscriptionService();
            subscriptionService.Initialize();
            lockGameService.Initialize(subscriptionService);
            mainMenu.Constructor(subscriptionService);
        }

        private ISubscriptionService GetSubscriptionService() =>
            Application.platform switch
            {
                RuntimePlatform.Android => new IAPManager(),
                RuntimePlatform.IPhonePlayer => new IAPManager(),
                _ => editorSubscriptionService
            };
    }
}