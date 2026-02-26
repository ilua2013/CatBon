using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningNumbers_2_2_Controller : BaseController<LearningNumbers_2_2_Controller>
{
    public ColorCard[] cards;
    public RectTransform[] cardsPoints;
    public float maxRandomDistance = 10f;
    public float maxRandomRotation = 20f;
    public HorizontalScroll horizontalScroll;
    public float sizeMultiplier = 1.1f;
    private int _currLevel = 0;
    public int maxLevels = 2;

    [Header("Cat phrases")]
    public string rulesText;
    public string winText;

    [Header("Audio")]
    public AudioClip[] startAudio;

    private void Start()
    {
        Restart();
        DragAndDropManager.Instance.onDragStop.AddListener(OnCardDrop);
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.defaultText = rulesText;
    }

    public override void Restart()
    {
        if(_currLevel == 0)
        {
            CatHelper.Instance.ShowText(rulesText);
            CatHelper.Instance.PlayAudio(startAudio);
        }
        
        GenerateCards();
        for (int i = 0; i < horizontalScroll.availableCards.Count; i++)
        {
            foreach (var item in horizontalScroll.availableCards[i].cards)
            {
                item.rectTransform.localScale = Vector3.one;
                item.GetComponent<DragNDropCard>().id4 = item.id4 = -1;
                item.gameObject.SetActive(false);
            }
        }
        horizontalScroll.ShowCardsGroup(0);
    }


    public void OnCardDrop(DragNDropCard dragNDropCard)
    {
        CheckCardRules(dragNDropCard);
        
    }

    public void GenerateCards()
    {
        List<RectTransform> tpoints = new List<RectTransform>(cardsPoints);
        for (int i = 0; i < cards.Length; i++)
        {
            int t = Random.Range(0, tpoints.Count);
            Global.RandomizePositionAndRotation(ref cards[i].rectTransform, tpoints[t], maxRandomDistance, maxRandomRotation);
            tpoints.RemoveAt(t);
        }
    }

    public void CheckCardRules(DragNDropCard card)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if(cards[i].rectTransform.ContainsPointScalable(card.rectTransform.position)) // попали на какую-то цифру
            {
                if(cards[i].id1 == card.id1) // попали на правильную цифру
                {
                    card.GetComponent<ColorCard>().id4 = card.id4 = 9;
                    card.GoToPosition(cards[i].rectTransform.position);
                    card.rectTransform.SetParent(cards[i].rectTransform);
                    card.draggable = false;
                    card.SetDefaultScale();
                    card.SetRotation(cards[i].rectTransform.localRotation);
                    card.SetScale(cards[i].rectTransform.GetChild(0).localScale);
                    card.rectTransform.SetAsFirstSibling();
                    
                    CatHelper.Instance.ShowText("Верно!", 2f); 
                    if (CheckWinRules())
                    {
                        NextLevel();
                    }
                    else
                    {
                        SoundMaster.Instance.PlayTrueAnswer();
                    }
                }
                else
                {
                    card.GoToDefaultPosition();
                    SoundMaster.Instance.PlayWrongAnswer();
                    CatHelper.Instance.ShowText("Неверно.", 2f);
                }
                return;
            }
        }
        card.GoToDefaultPosition();
    }
    public bool CheckWinRules()
    {
        bool result = true;
        for (int i = 0; i < horizontalScroll.availableCards.Count; i++)
        {
            foreach (var item in horizontalScroll.availableCards[i].cards)
            {
                result = result && item.GetComponent<DragNDropCard>().id4 == 9;
            }
        }
        if(horizontalScroll.currentCardGroup == 0) // на первом паке карточек
        {
            bool allCardsUsed = true;
            foreach (var item in horizontalScroll.availableCards[0].cards)
            {
                allCardsUsed = allCardsUsed && item.GetComponent<DragNDropCard>().id4 == 9;
            }
            if(allCardsUsed)
            {
                bool needToChange = false;
                foreach (var item in horizontalScroll.availableCards[1].cards)
                {
                    needToChange = needToChange || item.GetComponent<DragNDropCard>().id4 != 9;
                }
                if(needToChange)
                {
                    horizontalScroll.ShowCardsGroup(1);
                }
            }
        }
        return result;
    }

    private void Win()
    {
        CatHelper.Instance.ShowText(winText, 5f);
        SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit(5.5f));
    }
    private void NextLevel()
    {
        _currLevel++;
        if (_currLevel == maxLevels)
        {
            Win();
        }
        else
        {
            if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
            CatHelper.Instance.ShowText("Молодец! Идём дальше.", 3f);
            StartCoroutine(DelayedRestart());
        }
    }
    private IEnumerator DelayedRestart()
    {
        if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
        yield return new WaitForSeconds(3f);
        Restart();
    }
}
