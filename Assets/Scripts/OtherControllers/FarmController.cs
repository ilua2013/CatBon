using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmController : MonoBehaviour
{
    public Image image;
    public Animator doorsAnimator;
    public Vector3 zoomInScale = new Vector3(1.3f, 1.3f, 1.3f);
    public Vector3 zoomOutScale = new Vector3(1f, 1f, 1f);
    //public float imageScaleMultiplier;
    public float zoomDuration = 0.7f;

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
        image.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
    }
    public void Show()
    {
        StartCoroutine(Zoom(true));
    }
    public void Hide()
    {
        StartCoroutine(Zoom(false));
    }
    IEnumerator Zoom(bool show)
    {
        if(show)
        {
            float t = 0;
            float progress;
            while (t < zoomDuration)
            {
                t += Time.deltaTime;
                progress = t / zoomDuration;
                transform.localScale = Vector3.Lerp(zoomOutScale, zoomInScale, progress);
                yield return null;
            }
            doorsAnimator.SetTrigger("open");
        }
        else
        {
            doorsAnimator.SetTrigger("close");
            float t = 0;
            float progress;
            while (t < zoomDuration)
            {
                t += Time.deltaTime;
                progress = t / zoomDuration;
                transform.localScale = Vector3.Lerp(zoomInScale, zoomOutScale, progress);
                yield return null;
            }
        }
        yield break;
    }
}
