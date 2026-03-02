using System;
using UnityEngine;
using UnityEngine.UI;

public class TrialHasNotActivatedPopup : MonoBehaviour
{
    public event Action SubscribeButtonClicked;
    
    [SerializeField] private Button showSubscribeButton;
    
    public void Show()
    {
        gameObject.SetActive(true);
        showSubscribeButton.onClick.AddListener(ShowSubscribeButtonClick);
    }

    private void ShowSubscribeButtonClick()
    {
        SubscribeButtonClicked?.Invoke();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        showSubscribeButton.onClick.RemoveListener(ShowSubscribeButtonClick);
    }
}
