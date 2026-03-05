using Services.Monetization.Subscription;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneMusicContainer
{
    public string sceneId;
    public int musicId;
}

public class MainMenu2 : MonoBehaviour
{
    public GameObject backButton;
    public GameObject mainMenu;
    public GameObject[] screens;
    public string[] scenes;
    public SceneMusicContainer[] sceneThemes;
    public AudioClip[] helloSound;
    
    public static MainMenu2 Instance;

    [SerializeField] private WelcomeWindow welcomeWindow;
        //[SerializeField] private TrialHasNotActivatedPopup trialHasNotActivatedPopup;
    [SerializeField] private TrialHasCanceledOrCompletePopup trialHasCanceledOrCompletePopup;
    [SerializeField] private SubscribeWindow subscribeWindow;
    
    private ISubscriptionService subscriptionService;

    public void Constructor(ISubscriptionService subscriptionService)
    {
        this.subscriptionService = subscriptionService;
        
        subscribeWindow.Hide();
        
        if (subscriptionService.ISubscriptionIsActive)
        {
            trialHasCanceledOrCompletePopup.Hide();
        }
        else
        {
            trialHasCanceledOrCompletePopup.Show();
        }
        subscriptionService.SubscriptionEnded += trialHasCanceledOrCompletePopup.Show;
        subscriptionService.SubscriptionActivated += trialHasCanceledOrCompletePopup.Hide;

        trialHasCanceledOrCompletePopup.SubscribeButtonClicked += ShowSubscribeWindow;
    }

    private void ShowSubscribeWindow()
    {
        subscribeWindow.Show(subscriptionService);
    }
    
    private void Awake()
    {
        Instance = this;
        
        HideWelcomWindow();
    }

    private void Start() 
    {
        ResetScreen();
        SoundMaster.Instance.PlayMusic(0);
    }

    public void ShowWelcomWindow()
    {
        welcomeWindow.gameObject.SetActive(true);
        welcomeWindow.Show(subscriptionService);
    }

    public void HideWelcomWindow()
    {
        welcomeWindow.gameObject.SetActive(false);
        welcomeWindow.Hide();
    }

    public void PlayHello()
    {
        SoundMaster.Instance.PlayAudio(helloSound, 0.2f);
    }

    private void OnEnable()
    {
        if (SoundMaster.Instance) SoundMaster.Instance.PlayMusic(0);
    }

    public void ResetScreen()
    {
        if (Global.lastScreen == -1)
        {
            ShowMainMenu();
        }
        else
        {
            ShowScreen(Global.lastScreen);
        }
    }

    public void ShowScreen(int id)
    {
        mainMenu.SetActive(false);
        backButton.SetActive(true);
        for (int i = 0; i < screens.Length; i++)
        {
            if (id == i) screens[i].SetActive(true);
            else screens[i].SetActive(false);
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void StartGame(string id)
    {
        for (var i = 0; i < scenes.Length; i++)
        {
            if (!scenes[i].Contains(id + "_"))
                continue;
            
            /*if(!subscriptionService.IsThereSubscription())
            {
                Debug.Log($"Купите подписку!!!");
                return;
            }*/
            
            SoundMaster.Instance.PlayMusic(sceneThemes[i].musicId);
            Global.lastScreen = int.Parse(id.Substring(0, id.IndexOf("_"))) - 1;
            LoadScene(scenes[i]);
        }
    }

    public void ShowMainMenu()
    {
        backButton.SetActive(false);
        Global.lastScreen = -1;
        mainMenu.SetActive(true);
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (subscriptionService == null)
            return;
        
        subscriptionService.SubscriptionEnded -= trialHasCanceledOrCompletePopup.Show;
        subscriptionService.SubscriptionActivated -= trialHasCanceledOrCompletePopup.Hide;
        trialHasCanceledOrCompletePopup.SubscribeButtonClicked -= ShowSubscribeWindow;
    }
}