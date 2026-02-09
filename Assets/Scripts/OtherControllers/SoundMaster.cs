using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SoundContainer
{
    public string name;
    public Sprite sprite;
    public int id = -1;
    public float param1 = -1;
    public AudioClip[] audios;
    public AudioClip[] phrases;
}
public class SoundMaster : MonoBehaviour
{
    public float sleepTimer = 60f;

    public AudioSource soundSource;
    public AudioSource musicSource;
    [Space]
    public AudioClip menuTheme;
    public AudioClip garageTheme;
    public AudioClip roomTheme;
    public AudioClip farmTheme;
    [Space]
    public AudioClip[] trueAnswerSound;
    public AudioClip[] wrongAnswerSound;
    public AudioClip[] winSound;
    public AudioClip[] nextLevelSound;
    public AudioClip[] sleepSound;

    private IEnumerator themeRoutine;

    public static SoundMaster Instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private float t = 0;
    private void Update()
    {
        t += Time.deltaTime;
        if (Input.touchCount > 0 || Input.anyKey)
        {
            t = 0;
        }
        if (t >= sleepTimer)
        {
            t = 0;
            soundSource.PlayOneShot(sleepSound.Random());
        }
    }

    public void PlayWin(float delay = 0)
    {
        StartCoroutine(DelayedPlay(winSound.Random(), delay));
    }
    public void PlayTrueAnswer(float delay = 0)
    {
        StartCoroutine(DelayedPlay(trueAnswerSound.Random(), delay));
    }
    public void PlayWrongAnswer(float delay = 0)
    {
        StartCoroutine(DelayedPlay(wrongAnswerSound.Random(), delay));
    }
    public void PlayNextLevel(float delay = 0)
    {
        StartCoroutine(DelayedPlay(nextLevelSound.Random(), delay));
    }

    public void PlayAudio(object clip, float delay = 0)
    {
        if (clip.GetType().IsArrayOf<AudioClip>())
        {
            object[] temp = clip as object[];
            if (temp != null)
            {
                AudioClip[] audioArray = temp.OfType<AudioClip>().ToArray();
                soundSource.PlayOneShot(audioArray.Random());
            }
        }
        else
        {
            soundSource.PlayOneShot(clip as AudioClip);
        }
    }

    public void Stop()
    {
        soundSource.Stop();
    }


    private IEnumerator DelayedPlay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (CatHelper.Instance) CatHelper.Instance.Stop();
        Stop();
        soundSource.PlayOneShot(clip);
    }

    public void PlayMusic(int id)
    {
        switch (id)
        {
            case 0:
                Global.RenewCoroutine(this, ref themeRoutine, SwitchTheme(menuTheme, 8f));
                break;
            case 1:
                Global.RenewCoroutine(this, ref themeRoutine, SwitchTheme(garageTheme, 0.5f));
                break;
            case 2:
                Global.RenewCoroutine(this, ref themeRoutine, SwitchTheme(roomTheme, 0.5f));
                break;
            case 3:
                Global.RenewCoroutine(this, ref themeRoutine, SwitchTheme(farmTheme, 0.5f));
                break;
            default:
                break;
        }
    }

    private IEnumerator SwitchTheme(AudioClip audioClip, float volume)
    {
        while (musicSource.volume > 0)
        {
            musicSource.volume = Mathf.MoveTowards(musicSource.volume, 0f, 0.03f);
            yield return null;
        }
        musicSource.Stop();
        musicSource.clip = audioClip;
        musicSource.Play();
        while (musicSource.volume < volume)
        {
            musicSource.volume = Mathf.MoveTowards(musicSource.volume, volume, 0.03f);
            yield return null;
        }
    }
}
