using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountTheDolls_2_4_Controller : BaseController<CountTheDolls_2_4_Controller>
{
    //public Sprite[] dollsVariants;
    //public Sprite[] clochesVariants;



    public ColorCard[] cards;
    public RectTransform[] cardsPoints;
    public float maxRandomDistance = 10f;
    public float maxRandomRotation = 20f;
    public HorizontalScroll horizontalScroll;
    public AudioClip a_true;
    public AudioClip[] a_folse;
    int r;
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
        CatHelper.Instance.PlayAudio(startAudio2, 4f);
        Restart();
        CatHelper.Instance.ShowText(!useDolls ? rulesText : rulesDollsText);
    }

    public void Generate()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].otherImages[0].color = Global.colors9.Random();
            cards[i].gameObject.SetActive(false);
            //cards[i].colorImages[0].sprite = dollsVariants.Random();
            //cards[i].otherImages[0].sprite = clochesVariants.Random();
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

            tpoints.RemoveAt(t);
            tcards.RemoveAt(tt);
        }
    }

    public void OnCardPush(int id)
    {
        if (id == currentNumber)
        {
            CatHelper.Instance.PlayAudio(a_true);
            //CatHelper.Instance.ShowText("Правильно! Тут " + (!useDolls ? Global.animalsCountNames[id] : Global.dollsCountNames[id]) + ".");
            NextLevel();
        }
        else
        {
            CatHelper.Instance.ShowText("Неверно...");
            
            CatHelper.Instance.PlayAudio(a_folse[r], 0.01f);
            r=r+1;
            if(r>1)
            {
            r=0;
            }
            
            
            SoundMaster.Instance.PlayWrongAnswer();
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
            //SoundMaster.Instance.PlayNextLevel();
            CatHelper.Instance.ShowText("Молодец, идем дальше!");
            for (int i = 0; i < horizontalScroll.availableCards.Count; i++)
            {
                foreach (var item in horizontalScroll.availableCards[i].cards)
                {
                    item.GetComponent<Button>().interactable = true;
                }
            }
            yield return new WaitForSeconds(3.5f);
            Restart();
        }
    }

    public override void Restart()
    {
        //CatHelper.Instance.ShowText(!useDolls ? rulesText : rulesDollsText);
        Generate();
        horizontalScroll.ShowCardsGroup(0);
    }

    public void Win()
    {
        CatHelper.Instance.ShowText(!useDolls ? winText : winDollsText);
        SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit());
    }
}
