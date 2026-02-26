using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CatHelper : Singleton<CatHelper>
{
    public Text text;
    public AudioSource audioSource;
    public float animationDuaration = 0.5f;
    public CanvasGroup[] clouds;
    public string defaultText;
    public AudioClip defaultAudio;

    private IEnumerator showTextRoutine;
    private IEnumerator audioRoutine;

    public void ShowDefaultText()
    {
        ShowText(defaultText, 3f);
    }

    public void PlayDefaultAudio()
    {
        if (defaultAudio) audioSource.PlayOneShot(defaultAudio);
    }

    public void ShowText(string text, float hideAfter = 3)
    {
        if (!string.IsNullOrEmpty(text)) this.text.text = text;
        if (showTextRoutine != null) StopCoroutine(showTextRoutine);
        showTextRoutine = _ShowText(text, animationDuaration, hideAfter);
        StartCoroutine(showTextRoutine);
    }

    public void HideText(float duration = 0.1f)
    {
        if (clouds[0].alpha == 0) return;
        if (showTextRoutine != null) StopCoroutine(showTextRoutine);
        showTextRoutine = _HideText(duration);
        StartCoroutine(showTextRoutine);
    }

    public void PlayAudio(object clip, float delay = 0)
    {
        if (clip.GetType().IsArrayOf<AudioClip>())
        {
            object[] temp = clip as object[];
            if (temp != null)
            {
                AudioClip[] audioArray = temp.OfType<AudioClip>().ToArray();
                Global.RenewCoroutine(this, ref audioRoutine, DelayedPlay(audioArray.Random(), delay));
            }
        }
        else
        {
            Global.RenewCoroutine(this, ref audioRoutine, DelayedPlay(clip as AudioClip, delay));
        }
    }

    private IEnumerator _ShowText(string text, float duration, float hideAfter = -1f)
    {
        for (int i = 0; i < clouds.Length; i++)
        {
            clouds[i].interactable = true;
            clouds[i].blocksRaycasts = true;
        }
        float stepDuration = duration / 3f;
        //float delay = duration / 10f * 1f;
        float t = 0f;
        float progress;
        if (clouds[0].alpha > 0)
        {
            while (t < stepDuration) // скрыть
            {
                t += Time.deltaTime;
                progress = t / stepDuration;
                for (int i = 0; i < clouds.Length; i++)
                {
                    clouds[i].alpha = Mathf.Lerp(1f, 0f, progress);
                }
            }
        }

        for (int i = 0; i < clouds.Length; i++)
        {
            t = 0;
            while (t < stepDuration) // 1 облако
            {
                t += Time.deltaTime;
                progress = t / stepDuration;
                clouds[i].alpha = Mathf.Lerp(0f, 1f, progress);
                yield return null;
            }
            //yield return new WaitForSeconds(delay);
        }
        if (hideAfter > 0)
        {
            yield return new WaitForSeconds(hideAfter);
            HideText();
        }
    }

    private IEnumerator _HideText(float duration)
    {
        for (int i = 0; i < clouds.Length; i++)
        {
            clouds[i].interactable = false;
            clouds[i].blocksRaycasts = false;
        }
        float t = 0;
        float progress;
        while (t < duration) // скрыть
        {
            t += Time.deltaTime;
            progress = t / duration;
            for (int i = 0; i < clouds.Length; i++)
            {
                clouds[i].alpha = Mathf.Lerp(1f, 0f, progress);
            }
            yield return null;
        }
    }

    private IEnumerator DelayedPlay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (SoundMaster.Instance) SoundMaster.Instance.Stop();
        Stop();
        audioSource.PlayOneShot(clip);
    }

    public void StopDelayedAudio()
    {
        if (audioRoutine != null) StopCoroutine(audioRoutine);
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
