using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    public Sprite[] sprites;
    public Image image;
    public float animationDelay = 0.3f;
    public int framesCount = 6;

    private IEnumerator animationRoutine;
    public void StartAnimation()
    {
        Global.RenewCoroutine(this, ref animationRoutine, Animation());
    }
    public void StopAnimation()
    {
        StartCoroutine(animationRoutine);
        SetSprite(sprites[0]);
    }


    private IEnumerator Animation()
    {
        bool open = true;
        for (int i = 0; i < framesCount; i++)
        {
            SetSprite(open ? sprites[1] : sprites[0]);
            open = !open;
            yield return new WaitForSeconds(animationDelay);
        }
        SetSprite(sprites[0]);
    }

    private void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
        image.rectTransform.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);
    }
}
