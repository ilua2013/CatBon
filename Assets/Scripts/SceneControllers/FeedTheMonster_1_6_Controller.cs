using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FeedTheMonster_1_6_Controller : BaseController<FeedTheMonster_1_6_Controller>
{
    public int currentLevel = 0;
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
    public string secondRoundText;
    public string thirdRoundText;
    public string winText;

    [Header("Audio")]
    public AudioClip[] startAudio;

    private void Start()
    {
        CatHelper.Instance.defaultText = rulesText;
        DragAndDropManager.Instance.onDragStop.AddListener(new UnityAction<DragNDropCard>(OnCardDrop));
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.PlayAudio(startAudio);
        Restart();
    }
    public override void Restart()
    {
        currentLevel = 0;
        CatHelper.Instance.ShowDefaultText();
        rowsColors = Global.GetRandomColors(8, colorRows.Count);
        GenerateCards();
        GenerateField();
        for (int i = 0; i < colorCards.Length; i++)
        {
            colorCards[i].rectTransform.SetParent(defaultCarfsParent);
            DragNDropCard temp = colorCards[i].GetComponent<DragNDropCard>();
            temp.defaultScale = Vector3.one * 1.6f;
            temp.newParent = temp.defaultParent as RectTransform;
            temp.SetScale(Vector3.one * 1.6f);
            temp.id1 = -1;
        }
    }
    private IEnumerator NextLevel()
    {
        currentLevel++;
        CatHelper.Instance.ShowText("Молодец! Идем дальше.");
        if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
        rowsColors = Global.GetRandomColors(8, colorRows.Count);
        yield return new WaitForSeconds(1f);
        foreach (var item in colorRows)
        {
            item.cards[0].GetComponent<MonsterController>().StopAnimation();
        }
        GenerateCards();
        GenerateField();
        for (int i = 0; i < colorCards.Length; i++)
        {
            colorCards[i].rectTransform.SetParent(defaultCarfsParent);
            DragNDropCard temp = colorCards[i].GetComponent<DragNDropCard>();
            temp.defaultScale = Vector3.one * 1.6f;
            temp.newParent = temp.defaultParent as RectTransform;
            temp.SetScale(Vector3.one * 1.6f);
            temp.id1 = -1;
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
                        card.draggable = false;
                        goBack = false;
                        falseColor = false;
                        //colorRows[i].hasCard[j] = true;

                        card.id1 = 1;
                        CatHelper.Instance.ShowText("Верно!", 1.5f);
                        colorRows[i].cards[0].GetComponent<MonsterController>().StartAnimation();
                        trueColor.Invoke();
                        if (CheckRules())
                        {
                            card.SetScale(Vector3.zero);
                            if (currentLevel == 2)
                            {
                                CatHelper.Instance.ShowText(winText, 5f);
                                if (SoundMaster.Instance) SoundMaster.Instance.PlayWin();
                                card.newParent = colorRows[i].cards[0].colorImages[0].rectTransform.Find("Mouth") as RectTransform;
                                card.GoToPosition(card.newParent.position, () =>
                                {
                                    card.rectTransform.SetParent(card.newParent);
                                    Canvas.ForceUpdateCanvases();
                                    LayoutRebuilder.ForceRebuildLayoutImmediate(card.newParent);
                                    LayoutRebuilder.ForceRebuildLayoutImmediate(card.rectTransform);
                                }, 0.05f);
                                StartCoroutine(AutoExit());

                                win.Invoke();
                            }
                            else
                            {
                                StartCoroutine(NextLevel());
                            }

                        }
                        else
                        {
                            card.newParent = colorRows[i].cards[0].colorImages[0].rectTransform.Find("Mouth") as RectTransform;
                            card.GoToPosition(card.newParent.position, () =>
                            {
                                card.rectTransform.SetParent(card.newParent);
                                Canvas.ForceUpdateCanvases();
                                LayoutRebuilder.ForceRebuildLayoutImmediate(card.newParent);
                                LayoutRebuilder.ForceRebuildLayoutImmediate(card.rectTransform);
                            }, 0.05f);
                            card.SetScale(Vector3.zero);
                            card.draggable = false;
                            if(SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                        }
                    }
                    else
                    {
                        falseColor = true;
                        CatHelper.Instance.ShowText("Неверно", 3f);
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
    public bool CheckRules()
    {
        bool result = true;
        for (int i = 0; i < colorCards.Length; i++)
        {
            result = result && colorCards[i].GetComponent<DragNDropCard>().id1 == 1;
        }
        //print(result);
        return result;
    }

    #region GENERATION
    public void GenerateField()
    {
        for (int i = 0; i < colorRows.Count; i++)
        {
            colorRows[i].SetColor(rowsColors[i]);
            colorRows[i].hasCard = new bool[colorRows[i].cards.Count];
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
            temp.Start();
            temp.draggable = true;
        }
    }
    public static void FillWithColors(ColorCard[] cards, List<Color> availableColors, int cardsPerColor)
    {
        List<ColorCard> tempCards = new List<ColorCard>(cards);
        for (int i = 0; i < availableColors.Count; i++)
        {
            for (int j = 0; j < cardsPerColor; j++)
            {
                int index = Random.Range(0, tempCards.Count);
                tempCards[index].SetColor(availableColors[i]);
                tempCards.RemoveAt(index);
            }
        }
        for (int i = 0; i < tempCards.Count; i++)
        {
            tempCards[i].SetColor(availableColors.Random());
        }
    }
    #endregion
}
