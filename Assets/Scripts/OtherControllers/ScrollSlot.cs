using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSlot : MonoBehaviour
{
    public Image mainImage;
    public Image frame;
    public RectTransform rectTransform;
    public RectTransform slotRectTransform;
    public ColorCard slotColorCard;
    public DragNDropCard slotDragNDropCard;

    public bool preserveCardScale = false;


    public float scaleDuration;
    public AnimationCurve scaleCurve;

    private IEnumerator scaleRoutine;

    public void SetCard(ColorCard newCard, bool withAnimation = true)
    {
        if (withAnimation)
        {
            if (slotDragNDropCard)
            {
                //slotDragNDropCard.StopDrag();
                slotDragNDropCard.draggable = false;
            }
            ScaleDown(() =>
            {
                if (slotColorCard)
                {
                    if (slotColorCard.id4 != 9) // если старая карточка не перемещена, выключаем ее
                    {
                        slotColorCard.gameObject.SetActive(false);
                    }
                }
                if (newCard.id4 != 9) // если новая карточка не использована, помещаем ее в слот
                {
                    slotColorCard = newCard;
                    slotDragNDropCard = newCard.GetComponent<DragNDropCard>();
                    slotColorCard.rectTransform.SetParent(slotRectTransform, preserveCardScale);
                    slotColorCard.rectTransform.localPosition = Vector3.zero;
                    slotColorCard.gameObject.SetActive(true);
                    slotDragNDropCard.defaultParent = slotRectTransform;
                    slotDragNDropCard.defaultAnchoredPosition = slotDragNDropCard.rectTransform.anchoredPosition;
                    slotDragNDropCard.rectTransform.localScale = slotDragNDropCard.defaultScale;
                }
                ScaleUp(() =>
                {
                    if (slotDragNDropCard) slotDragNDropCard.draggable = true;
                });
            });
        }
        else
        {
            if (slotColorCard.id4 == 9 || slotDragNDropCard.id4 == 9) // если старая карточка не перемещена, выключаем ее
            {
                slotColorCard.gameObject.SetActive(false);
            }
            if (newCard.id4 != 9) // если новая карточка не использована, помещаем ее в слот
            {
                slotColorCard = newCard;
                slotDragNDropCard = newCard.GetComponent<DragNDropCard>();
                slotColorCard.rectTransform.SetParent(slotRectTransform);
                slotColorCard.rectTransform.localPosition = Vector3.zero;
                slotColorCard.gameObject.SetActive(true);
            }
        }
    }

    public void ScaleDown(Action callback = null)
    {
        this.RenewCoroutine(ref scaleRoutine, _Scale(Vector3.zero, scaleCurve, scaleDuration, callback));
    }
    public void ScaleUp(Action callback = null)
    {
        this.RenewCoroutine(ref scaleRoutine, _Scale(Vector3.one, scaleCurve, scaleDuration, callback));
    }

    private IEnumerator _Scale(Vector3 target, AnimationCurve curve, float duration, Action callback = null)
    {
        float t = 0;
        float progress = 0;
        Vector3 startScale = transform.localScale;
        while (t < duration)
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0f, duration);
            progress = t / duration;
            transform.localScale = Vector3.Lerp(startScale, target, curve.Evaluate(progress));
            yield return null;
        }
        transform.localScale = target;
        callback();
        yield break;
    }
}
