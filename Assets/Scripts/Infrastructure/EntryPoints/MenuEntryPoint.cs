using Services.Monetization;
using Services.Monetization.Subscription;
using UnityEngine;
using UnityEngine.Serialization;

namespace Infractructure.EntryPoints
{
    public class MenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private MainMenu2 mainMenu;
        [SerializeField] private EditorSubscriptionService editorSubscriptionService; 
        
        private void Start()
        {
            //var subscriptionService = new AppStoreSubscriptionService(); //GetSubscriptionService();
            var subscriptionService = GetSubscriptionService();
            subscriptionService.Initialize();
            mainMenu.Constructor(subscriptionService);
        }

        private ISubscriptionService GetSubscriptionService() =>
            Application.platform switch
            {
                RuntimePlatform.Android => new GooglePlaySubscriptionService(),
                RuntimePlatform.IPhonePlayer => new AppStoreSubscriptionService(),
                _ => editorSubscriptionService
            };
    }
}