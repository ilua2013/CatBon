using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SubscribeWindow : MonoBehaviour
    {
        [SerializeField] private Button arrangeButton;
        
        public void Show()
        {
            arrangeButton.onClick.AddListener(ClickArrangeButton);
        }

        public void Hide()
        {
            arrangeButton.onClick.RemoveListener(ClickArrangeButton);
        }

        private void ClickArrangeButton()
        {
            
        }
    }
}