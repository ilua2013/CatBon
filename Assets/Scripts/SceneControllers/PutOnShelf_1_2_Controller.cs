using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PutOnShelf_1_2_Controller : BaseController<PutOnShelf_1_2_Controller>
{
    public List<ColorCardRow> colorRows;
    public ColorCard[] colorCards;
    public RectTransform[] colorCardsStartPoint;
    public float maxRandomDistance = 10f;
    public float maxRandomRotation = 20f;

    public List<Color> rowsColors;

    public UnityEvent falseColor;
    public UnityEvent trueColor;
    public UnityEvent win;
    public RectTransform defaultCarfsParent;

    [Header("Cat phrases")]
    public string wrongColorText;
    public string trueColorText;
    public string rulesText;
    public string winText;

    [Header("Audio")]
    public AudioClip[] startAudio;
    public AudioClip[] lastThree;
    public SoundContainer[] colorsClips;

    private int trueAnswersCount = 0;

    private int _currLevel = 0;
    public int maxLevels = 3;

    private void Start()
    {
        DragAndDropManager.Instance.onDragStop.AddListener(new UnityAction<DragNDropCard>(OnCardDrop));
        Restart();
    }

    public override void Restart()
    {
        trueAnswersCount = 0;
        if(_currLevel == 0)
        {
            CatHelper.Instance.defaultText = rulesText;
            CatHelper.Instance.defaultAudio = startAudio.Random();
            CatHelper.Instance.ShowText(rulesText);
            CatHelper.Instance.PlayAudio(startAudio);
        }
        rowsColors = Global.GetRandomColors(8, 7);
        GenerateCards();
        GenerateField();
    }

    public void GenerateField()
    {
        for (int i = 0; i < colorRows.Count; i++)
        {
            colorRows[i].SetColor(rowsColors[i]);
            colorRows[i].hasCard = new bool[1];
            for (int j = 0; j < colorRows[i].hasCard.Length; j++)
            {
                colorRows[i].hasCard[j] = false;
            }
        }
    }

    public void GenerateCards()
    {
        FillWithColors(colorCards, rowsColors, 1);
        for (int i = 0; i < colorCards.Length; i++)
        {
            colorCards[i].rectTransform.SetParent(defaultCarfsParent);
            Global.RandomizePositionAndRotation(ref colorCards[i].rectTransform, colorCardsStartPoint[i], maxRandomDistance, maxRandomRotation);
            DragNDropCard temp = colorCards[i].GetComponent<DragNDropCard>();
            temp.rectTransform.localScale = Vector3.one * 1.55f;
            temp.Start();
            temp.draggable = true;
        }
    }

    public void OnCardDrop(DragNDropCard card)
    {
        bool goBack = true;
        bool falseColor = false;
        for (int i = 0; i < colorRows.Count; i++)
        {
            for (int j = 0; j < colorRows[i].cards.Count; j++)
            {
                if (colorRows[i].cards[j].rectTransform.ContainsPointScalable(card.rectTransform.position)) // попали на незанятый слот
                {
                    if (colorRows[i].color == card.GetComponent<ColorCard>().currentColor) // цвета совпадают
                    {
                        goBack = false;
                        falseColor = false;
                        colorRows[i].hasCard[j] = true;
                        card.newParent = colorRows[i].header.GetComponentInChildren<HorizontalLayoutGroup>().GetComponent<RectTransform>();
                        card.GoToPosition(card.newParent.position, () =>
                        {
                            card.rectTransform.SetParent(card.newParent);
                            card.SetScale(Vector3.one);
                            Canvas.ForceUpdateCanvases();
                            LayoutRebuilder.ForceRebuildLayoutImmediate(card.newParent);
                            LayoutRebuilder.ForceRebuildLayoutImmediate(card.rectTransform);
                        }, 0.05f);
                        card.SetScale(card.defaultScale);
                        card.draggable = false;
                        int colorId = Array.IndexOf(Global.colors9, colorRows[i].color);
                        CatHelper.Instance.StopDelayedAudio();
                        CatHelper.Instance.ShowText(Global.colorsNames[colorId], 3f);
                        CatHelper.Instance.PlayAudio(colorsClips[colorId].audios.Random());

                        trueAnswersCount++;
                        trueColor.Invoke();
                        if (CheckRules())
                        {
                            NextLevel();
                            //CatHelper.Instance.ShowText(winText, 6f);
                            //SoundMaster.Instance.PlayWin(1.5f);
                            //StartCoroutine(AutoExit(7f));
                            //win.Invoke();
                        }
                        else
                        {
                            //if (trueAnswersCount != colorCards.Length - 3) SoundMaster.Instance.PlayTrueAnswer();
                        }
                    }
                    else
                    {
                        falseColor = true;
                        CatHelper.Instance.ShowText(wrongColorText, 3f);
                        if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                    }
                }
            }

        }
        if (goBack)
        {
            card.SetAllDefault();
        }
        if (falseColor)
        {
            this.falseColor.Invoke();
        }
    }

    /// <summary>
    /// Заполняет карточки цветами
    /// </summary>
    /// <param name="cards">Карточки</param>
    /// <param name="availableColors">Доступные цвета</param>
    public static void FillWithColors(ColorCard[] cards, List<Color> availableColors, int cardsPerColor)
    {
        List<ColorCard> tempCards = new List<ColorCard>(cards);
        for (int i = 0; i < availableColors.Count; i++)
        {
            for (int j = 0; j < cardsPerColor; j++)
            {
                int index = UnityEngine.Random.Range(0, tempCards.Count);
                tempCards[index].SetColor(availableColors[i]);
                tempCards.RemoveAt(index);
            }
        }
        for (int i = 0; i < tempCards.Count; i++)
        {
            tempCards[i].SetColor(availableColors.Random());
        }
    }

    public bool CheckRules()
    {
        if (trueAnswersCount == colorCards.Length - 3)
        {
            //SoundMaster.Instance.soundSource.Stop();
            //CatHelper.Instance.audioSource.Stop();
            //CatHelper.Instance.PlayAudio(lastThree);
        }
        return trueAnswersCount == colorCards.Length;
    }

    private void Win()
    {
        CatHelper.Instance.ShowText(winText, 5f);
        SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit(5.5f));
        win.Invoke();
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