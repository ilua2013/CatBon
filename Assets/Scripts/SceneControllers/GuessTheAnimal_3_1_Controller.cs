using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuessTheAnimal_3_1_Controller : BaseController<GuessTheAnimal_3_1_Controller>
{
    public Animator horizontalScrollAnimator;
    public FarmController farmController;
    public Vector3 zoomFarmScale;
    public Shaker farmShaker;
    public HorizontalScroll horizontalScroll;
    
    int _stage = 0;
    int _currentAnimal = 0;
    List<SoundContainer> _currentAnimals = new List<SoundContainer>(3);
    float _preStartDelay = 0.5f;
    public bool _trueAnimal = false;

    [Header("Cat phrases")]
    public string rulesText;
    public string winText;
    public string nextLevelText;
    public string wrongText;
    bool NotZumed;

    [Header("Audio")]
    public AudioClip[] startAudio;
    public AudioClip[] trueAnimalSound;
    public SoundContainer[] availableAnimals;

    private void Start()
    {
        availableAnimals.Shuffle();
        StartCoroutine(Controller());
    }

    IEnumerator Controller()
    {
        
        if (_stage == 0)// показываем правила
        {
            CatHelper.Instance.defaultText = rulesText;
            CatHelper.Instance.ShowText(rulesText, 4f);
            CatHelper.Instance.defaultAudio = startAudio.Random();
            CatHelper.Instance.audioSource.clip = startAudio.Random();
            CatHelper.Instance.audioSource.Play();
            yield return new WaitForSeconds(2f);
            _stage = 1; 
        }
        _trueAnimal = false;
        yield return new WaitForSeconds(_preStartDelay);
        
        
        if (_stage == 1)// животное кричит, сарай трясется
        {
            GenerateAnimals();
            PlayCurrentAnimal();
            yield return new WaitForSeconds(2f);
            _stage = 2;
        }
        
        
        if (_stage == 2)// показываем нижнее меню, ждем правильного нажатия
        {
            SetButtonsInteractive(true);
            horizontalScrollAnimator.SetTrigger("in");
            //horizontalScroll.gameObject.SetActive(true);
            while (!_trueAnimal)
            {
                yield return null;
            }
            farmShaker.Stop();
            _stage = 3;
        }
        
        
        if (_stage == 3)// скрываем меню, показываем животное
        {
            horizontalScrollAnimator.SetTrigger("out");
            NotZumed=true;
            CatHelper.Instance.audioSource.clip = trueAnimalSound.Random();
            CatHelper.Instance.audioSource.Play();
            CatHelper.Instance.PlayAudio(availableAnimals[_currentAnimal].phrases, 2f);
            CatHelper.Instance.ShowText($"Правильно! Это {availableAnimals[_currentAnimal].name}.", 4f);
            //horizontalScroll.gameObject.SetActive(false);
            farmController.Show();
            yield return new WaitForSeconds(4f);
            CatHelper.Instance.PlayAudio(availableAnimals[_currentAnimal].audios);
            yield return new WaitForSeconds(2f);
            farmController.Hide();
        }


        if(_currentAnimal < availableAnimals.Length - 1)
        {
            NextLevel();
        }    
        else
        {
            Win();
        }


        yield break;
    }
    void GenerateAnimals()
    {
        List<SoundContainer> temp = new List<SoundContainer>(3);
        temp.Add(availableAnimals[_currentAnimal]);
        temp.Add(availableAnimals.Random());
        while (temp[1] == temp[0])
        {
            temp[1] = availableAnimals.Random();
        }
        temp.Add(availableAnimals.Random());
        while (temp[2] == temp[0] || temp[2] == temp[1])
        {
            temp[2] = availableAnimals.Random();
        }

        List<ColorCard> cards = new List<ColorCard>(horizontalScroll.availableCards[0].cards);
        for (int i = 0; i < 3; i++)
        {
            int tIndex = Random.Range(0, cards.Count);
            if(i == 0)
            {
                cards[tIndex].id1 = 1;
            }
            else
            {
                cards[tIndex].id1 = 0;
            }
            cards[tIndex].rectTransform.localScale = Vector3.one;
            cards[tIndex].SetColorImage(0, temp[i].sprite);
            ColorCard t = cards[tIndex];
            cards[tIndex].GetComponent<Button>().onClick.AddListener(() => { OnCardPush(t.id1); });
            cards[tIndex].GetComponent<AspectRatioFitter>().aspectRatio = temp[i].sprite.rect.width / temp[i].sprite.rect.height;
            cards.RemoveAt(tIndex);
        }
        horizontalScroll.ShowCardsGroup(0);
    }
    public void OnCardPush(int id)
    {
        SetButtonsInteractive(false);
        if (_stage != 2) return;
        if(id == 1)
        {
            _trueAnimal = true;
        }
        else
        {
            _trueAnimal = false;
            if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            CatHelper.Instance.ShowText(wrongText);
            StartCoroutine(DelayedAnimalSound(1.3f));
        }
    }
    private IEnumerator DelayedAnimalSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioClip tClip = availableAnimals[_currentAnimal].audios.Random();
        CatHelper.Instance.PlayAudio(tClip);
        farmShaker.Play(tClip.length);
        SetButtonsInteractive(true);
    }
    void NextLevel()
    {
        if(SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
        CatHelper.Instance.ShowText(nextLevelText);
        _currentAnimal++;
        _preStartDelay = 3f;
        _stage = 1;
        NotZumed=false;
        StartCoroutine(Controller());
    }
    void Win()
    {
        if (SoundMaster.Instance) SoundMaster.Instance.PlayWin();
        CatHelper.Instance.ShowText(winText);
        StartCoroutine(AutoExit());
    }
    public override void Restart()
    {
        
    }

    public void PlayCurrentAnimal()
    {
        if(NotZumed==false)
        {
        farmController.SetImage(availableAnimals[_currentAnimal].sprite);
        AudioClip currAnimalSound = availableAnimals[_currentAnimal].audios.Random();
        CatHelper.Instance.PlayAudio(currAnimalSound);
        farmShaker.Play(currAnimalSound.length);
        }
    }

    void SetButtonsInteractive(bool state)
    {
        foreach (var item in horizontalScroll.availableCards[0].cards)
        {
            item.GetComponent<Button>().interactable = state;
        }
    }
}
