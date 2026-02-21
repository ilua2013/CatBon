using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Elements
{
    public class HoldProgressBarButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public event Action OnFilling;

        [SerializeField] private Image progressBar;
        [SerializeField] private float fillingSpeed;

        private Coroutine fillingRoutine;
        private bool isActivated;

        public void SetActivated(bool isActivated)
        {
            if(fillingRoutine != null)
                StopCoroutine(fillingRoutine);
                
            this.isActivated = isActivated;
        }

        public void SetFillAmount(float value) 
            => progressBar.fillAmount = value;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isActivated)
                return;
            
            if(fillingRoutine != null)
                StopCoroutine(fillingRoutine);
            
            fillingRoutine = StartCoroutine(Filling(fillingSpeed, 1f, () => OnFilling?.Invoke()));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isActivated)
                return;
            
            if(fillingRoutine != null)
                StopCoroutine(fillingRoutine);
            
            fillingRoutine = StartCoroutine(Filling(-fillingSpeed, 0f));
        }

        private IEnumerator Filling(float speed, float endValue, Action onCompleted = null)
        {
            while (!Mathf.Approximately(progressBar.fillAmount, endValue))
            {
                progressBar.fillAmount += speed * Time.deltaTime;
                yield return null;
            }
            
            onCompleted?.Invoke();
        }
    }
}