using System.Threading.Tasks;
using UI.Elements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class WelcomeWindow : MonoBehaviour
    {
        [FormerlySerializedAs("showHideTime")] [SerializeField] private float showHideSpeed;
        
        [Space]
        [SerializeField] private CanvasGroup frame1;
        [SerializeField] private Button nextFrame2;
        
        [Space]
        [SerializeField] private CanvasGroup frame2;
        [SerializeField] private Button nextFrame3;
        
        [Space]
        [SerializeField] private CanvasGroup frame3;
        [SerializeField] private Button nextFrame4;
        
        [Space]
        [SerializeField] private CanvasGroup frame4;
        [SerializeField] private Button backFrame3;
        [SerializeField] private HoldProgressBarButton nextFrame5;
        
        [Space]
        [SerializeField] private CanvasGroup frame5;
        [SerializeField] private Button subscribing;

        public void Show()
        {
            frame1.gameObject.SetActive(false);
            frame2.gameObject.SetActive(false);
            frame3.gameObject.SetActive(false);
            frame4.gameObject.SetActive(false);
            frame5.gameObject.SetActive(false);

            ShowFrame1();
        }

        public void Hide()
        {
            frame1.gameObject.SetActive(false);
            frame2.gameObject.SetActive(false);
            frame3.gameObject.SetActive(false);
            frame4.gameObject.SetActive(false);
            frame5.gameObject.SetActive(false);
            
            nextFrame2.onClick.RemoveListener(ShowFrame2);
            nextFrame3.onClick.RemoveListener(ShowFrame3);
            nextFrame4.onClick.RemoveListener(ShowFrame4);
            nextFrame5.OnFilling -= ShowFrame5;
            backFrame3.onClick.RemoveListener(BackToFrame3);
            subscribing.onClick.RemoveListener(Subscribing);
            
            gameObject.SetActive(false);
        }

        private async Task ShowFrame(CanvasGroup frame)
        {
            frame.alpha = 0f;
            frame.gameObject.SetActive(true);
            while (!Mathf.Approximately(frame.alpha, 1f))
            {
                await Task.Delay((int)(Time.deltaTime * 1000));
                frame.alpha += showHideSpeed * Time.deltaTime;
            }
        }
        
        private async Task HideFrame(CanvasGroup frame)
        {
            frame.alpha = 1f;
            
            while (!Mathf.Approximately(frame.alpha, 0f))
            {
                await Task.Delay((int)(Time.deltaTime * 1000));
                frame.alpha -= showHideSpeed * Time.deltaTime;
            }
            
            frame.gameObject.SetActive(false);
        }
        
        private async void ShowFrame1()
        {
            nextFrame2.onClick.AddListener(ShowFrame2);
            await ShowFrame(frame1);
        }
        
        private async void ShowFrame2()
        {
            nextFrame2.onClick.RemoveListener(ShowFrame2);
            
            await HideFrame(frame1);
            await ShowFrame(frame2);
            
            nextFrame3.onClick.AddListener(ShowFrame3);
        }

        private async void ShowFrame3()
        {
            nextFrame3.onClick.RemoveListener(ShowFrame3);
            await HideFrame(frame2);
            await ShowFrame(frame3);
            nextFrame4.onClick.AddListener(ShowFrame4);
        }
        
        private async void ShowFrame4()
        {
            nextFrame4.onClick.RemoveListener(ShowFrame4);
            await HideFrame(frame3);
            await ShowFrame(frame4);
            backFrame3.onClick.AddListener(BackToFrame3);
            nextFrame5.OnFilling += ShowFrame5;
            nextFrame5.SetFillAmount(0f);
            nextFrame5.SetActivated(true);
        }

        private async void BackToFrame3()
        {
            nextFrame5.OnFilling -= ShowFrame5;
            nextFrame5.SetActivated(false);
            nextFrame5.SetFillAmount(0f);
            backFrame3.onClick.RemoveListener(BackToFrame3);
            await HideFrame(frame4);
            await ShowFrame(frame3);
            nextFrame4.onClick.AddListener(ShowFrame4);
        }
        
        private async void ShowFrame5()
        {
            nextFrame5.OnFilling -= ShowFrame5;
            nextFrame5.SetActivated(false);
            nextFrame5.SetFillAmount(1f);
            await HideFrame(frame4);
            await ShowFrame(frame5);
            subscribing.onClick.AddListener(Subscribing);
        }

        private async void Subscribing()
        {
            await HideFrame(frame5);
            Debug.Log($"Подписка оформлена!");
            Hide();
        }
    }
}