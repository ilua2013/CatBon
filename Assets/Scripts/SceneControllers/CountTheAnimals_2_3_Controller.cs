using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountTheAnimals_2_3_Controller : BaseController<CountTheAnimals_2_3_Controller>
{
    public ColorCard[] cards;
    public RectTransform[] cardsPoints;
    public float maxRandomDistance = 10f;
    public float maxRandomRotation = 20f;
    public HorizontalScroll horizontalScroll;

    public int currentNumber = 0;
    public int currentLevel = 0;
    public int maxLevels = 5;

    public bool useDolls = false;


    [Header("Cat phrases")]

    public string rulesDollsText;
    public string rulesText;
    public string winText;
    public string winDollsText;
    public string wrongNumberText;

    [Header("Audio")]
    public AudioClip[] startAudio1;
    public AudioClip[] startAudio2;


    private void Start()
    {
        for (int i = 0; i < horizontalScroll.availableCards.Count; i++)
        {
            foreach (var item in horizontalScroll.availableCards[i].cards)
            {
                item.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => { OnCardPush(item.id1); }));
            }
        }
        CatHelper.Instance.defaultAudio = startAudio1.Random();
        CatHelper.Instance.audioSource.clip = startAudio1.Random();
        CatHelper.Instance.audioSource.Play();
        CatHelper.Instance.PlayAudio(CatHelper.Instance.defaultAudio);
        CatHelper.Instance.PlayAudio(startAudio2, 4.5f);
        Restart();
        CatHelper.Instance.ShowText(!useDolls ? rulesText : rulesDollsText);
    }

    public void Generate()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].gameObject.SetActive(false);
        }
        currentNumber = Random.Range(1, 11);
        List<RectTransform> tpoints = new List<RectTransform>(cardsPoints);
        List<ColorCard> tcards = new List<ColorCard>(cards);
        for (int i = 0; i < currentNumber; i++)
        {
            int t = Random.Range(0, tpoints.Count);
            int tt = Random.Range(0, tcards.Count);
            tcards[tt].gameObject.SetActive(true);
            tcards[tt].rectTransform.SetParent(tpoints[t]);
            Global.RandomizePositionAndRotation(ref tcards[tt].rectTransform, tpoints[t], maxRandomDistance, maxRandomRotation);
            if (tpoints[t].localPosition.x > 0)
            {
                if (Random.Range(0, 100) < 80)
                {
                    tcards[tt].rectTransform.localScale = Vector3.one;
                }
                else
                {
                    tcards[tt].rectTransform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else
            {
                if (Random.Range(0, 100) < 80)
                {
                    tcards[tt].rectTransform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    tcards[tt].rectTransform.localScale = Vector3.one;
                }
            }

            tpoints.RemoveAt(t);
            tcards.RemoveAt(tt);

        }
    }

    public void OnCardPush(int id)
    {
        if (id == currentNumber)
        {
            //CatHelper.Instance.ShowText("Верно!");
            //SoundMaster.Instance.PlayTrueAnswer();
            NextLevel();
        }
        else
        {
            CatHelper.Instance.ShowText("Неверно!");
            if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        StartCoroutine(DelayedNextLevel(currentLevel >= maxLevels));
    }

    private IEnumerator DelayedNextLevel(bool win)
    {
        for (int i = 0; i < horizontalScroll.availableCards.Count; i++)
        {
            foreach (var item in horizontalScroll.availableCards[i].cards)
            {
                item.GetComponent<Button>().interactable = false;
            }
        }
        yield return new WaitForSeconds(0.3f);
        if (win)
        {
            Win();
        }
        else
        {
            if(SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
            CatHelper.Instance.ShowText("Молодец! Идём дальше.");
            for (int i = 0; i < horizontalScroll.availableCards.Count; i++)
            {
                foreach (var item in horizontalScroll.availableCards[i].cards)
                {
                    item.GetComponent<Button>().interactable = true;
                }
            }
            yield return new WaitForSeconds(3f);
            Restart();
        }
    }

    public override void Restart()
    {
        
        
        Generate();
        horizontalScroll.ShowCardsGroup(0);
    }

    public void Win()
    {
        StartCoroutine(AutoExit());
        CatHelper.Instance.ShowText(!useDolls ? winText : winDollsText);
        if (SoundMaster.Instance) SoundMaster.Instance.PlayWin();
    }
    
}
