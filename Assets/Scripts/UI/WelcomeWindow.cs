using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WelcomeWindow : MonoBehaviour
    {
        [SerializeField] private GameObject frame1;
        [SerializeField] private Button nextFrame2;
        
        [Space]
        [SerializeField] private GameObject frame2;
        [SerializeField] private Button nextFrame3;
        
        [Space]
        [SerializeField] private GameObject frame3;
        [SerializeField] private Button nextFrame4;
        
        [Space]
        [SerializeField] private GameObject frame4;
        [SerializeField] private Button nextFrame5;
        
        [Space]
        [SerializeField] private GameObject frame5;
        [SerializeField] private Button subscribing;

        public void Show()
        {
            frame1.SetActive(false);
            frame2.SetActive(false);
            frame3.SetActive(false);
            frame4.SetActive(false);
            frame5.SetActive(false);

            ShowFrame1();
        }

        public void Hide()
        {
            
        }

        private void ShowFrame1()
        {
            frame1.SetActive(true);
            nextFrame2.onClick.AddListener(ShowFrame2);
        }

        private void ShowFrame2()
        {
            frame1.SetActive(false);
            nextFrame2.onClick.RemoveListener(ShowFrame2);
            
            frame2.SetActive(true);
            nextFrame3.onClick.AddListener(ShowFrame3);
        }

        private void ShowFrame3()
        {
            frame2.SetActive(false);
            nextFrame3.onClick.RemoveListener(ShowFrame3);
            
            frame3.SetActive(true);
            nextFrame4.onClick.AddListener(ShowFrame4);
        }
        
        private void ShowFrame4()
        {
            frame3.SetActive(false);
            nextFrame3.onClick.RemoveListener(ShowFrame4);
            
            frame4.SetActive(true);
            nextFrame5.onClick.AddListener(ShowFrame5);
        }
        
        private void ShowFrame5()
        {
            frame4.SetActive(false);
            nextFrame3.onClick.RemoveListener(ShowFrame5);

            frame5.SetActive(true);
            subscribing.onClick.AddListener(Subscribing);
        }

        private void Subscribing()
        {
            frame5.SetActive(false);
            Debug.Log($"Подписка оформлена!");
        }
    }
}