using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ColorCardRow
{
    public Color color;
    public ColorCard header;
    public List<ColorCard> cards;
    public bool[] hasCard;

    public void SetColor(Color color)
    {
        header.SetColor(color);
        this.color = color;
    }
}
public class ColorLoto_1_3_Controller : BaseController<ColorLoto_1_3_Controller>
{
    public List<ColorCardRow> colorRows;
    public ColorCard[] colorCards;
    public RectTransform[] colorCardsStartPoint;
    public float maxRandomDistance = 10f;
    public float maxRandomRotation = 20f;
    public ColorLotoFrameController frameController;

    public List<Color> rowsColors;

    public UnityEvent falseColor;
    public UnityEvent trueColor;
    public UnityEvent win;
    public float goToPositionDuration = 0.3f;

    [Header("Cat phrases")]
    public string wrongColorText;
    public string trueColorText;
    public string rulesText;
    public string winText;
    public string falseSlotText;
    public string falseEnimalText;

    [Header("Animals")]
    public Sprite[] animals;
    public float sizeDenominator = 1.5f;

    [Header("Audio")]
    public AudioClip[] startAudio;

    public int maxLevels = 3;
    private int _currLevel = 0;


    #region GAME
    private void Start()
    {
        DragAndDropManager.Instance.onDragStop.AddListener(OnCardDrop);
        DragAndDropManager.Instance.onDragStart.AddListener(OnStartDrag);
        Restart();
        CatHelper.Instance.defaultText = rulesText;
        CatHelper.Instance.defaultAudio = startAudio.Random();
        
    }
    public override void Restart()
    {
        rowsColors = Global.GetRandomColors(4, 7);
        GenerateCards();
        GenerateField();
        if (_currLevel == 0)
        {
            CatHelper.Instance.PlayAudio(startAudio);
            CatHelper.Instance.ShowText(rulesText, 3f);
        }
    }
    public void OnCardDrop(DragNDropCard card)
    {
        bool goBack = true;
        bool falseColor = false;
        bool falseEnimal = false;
        for (int i = 0; i < colorRows.Count; i++)
        {
            for (int j = 0; j < colorRows[i].cards.Count; j++)
            {
                if (!colorRows[i].hasCard[j] && colorRows[i].cards[j].rectTransform.ContainsPointScalable(card.rectTransform.position)) // попали на незанятый слот
                {
                    if (colorRows[i].cards[j].id2 == card.GetComponent<ColorCard>().id2)
                    {
                        if (colorRows[i].color == card.GetComponent<ColorCard>().currentColor) // цвета совпадают
                        {
                            goBack = false;
                            falseColor = false;
                            colorRows[i].hasCard[j] = true;
                            card.GoToPosition(colorRows[i].cards[j].rectTransform.position, null, goToPositionDuration);
                            card.SetScale(colorRows[i].cards[j].rectTransform.localScale);
                            card.draggable = false;
                            trueColor.Invoke();

                            if (CheckRules())
                            {
                                NextLevel();
                            }
                            else
                            {
                                CatHelper.Instance.ShowText(trueColorText, 3f);
                                if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                            }
                            break;
                        }
                        else
                        {
                            //CatHelper.Instance.ShowText(wrongColorText, 3f);
                            falseColor = true;
                            break;
                        }
                    }
                    else
                    {

                        falseEnimal = true;
                        break;
                    }
                }
                else
                {

                }
            }

        }
        if (goBack)
        {
            card.SetAllDefault();
            //if (!falseColor && !falseEnimal) CatHelper.Instance.ShowText(falseSlotText, 3f);
            if (falseEnimal || falseColor)
            {
                CatHelper.Instance.ShowText(falseEnimalText, 3f);
                if(SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            }

        }
        if (falseColor)
        {
            this.falseColor.Invoke();
        }
        frameController.Hide();
    }
    public void OnStartDrag(DragNDropCard card)
    {
        frameController.SetColumn(card.GetComponent<ColorCard>().id2);
    }
    public bool CheckRules()
    {
        bool result = true;
        for (int i = 0; i < colorRows.Count; i++)
        {
            foreach (var item in colorRows[i].hasCard)
            {
                //print(item);
                result = result && item;
            }
        }
        return result;
    }
    #endregion

    #region GENERATION
    public void GenerateField()
    {
        for (int i = 0; i < colorRows.Count; i++)
        {
            colorRows[i].SetColor(rowsColors[i]);
            colorRows[i].hasCard = new bool[3];
            for (int j = 0; j < colorRows[i].hasCard.Length; j++)
            {
                colorRows[i].hasCard[j] = false;
            }
        }
    }
    public void GenerateCards()
    {
        FillWithColors(colorCards, rowsColors, 3);
        for (int i = 0; i < colorCards.Length; i++)
        {
            Global.RandomizePositionAndRotation(ref colorCards[i].rectTransform, colorCardsStartPoint[i], maxRandomDistance, maxRandomRotation);
            DragNDropCard temp = colorCards[i].GetComponent<DragNDropCard>();
            temp.Start();
            temp.draggable = true;
        }
        FillWithAnimals(colorCards, rowsColors, animals);
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
                int index = Random.Range(0, tempCards.Count);
                tempCards[index].SetColor(availableColors[i]);
                tempCards.RemoveAt(index);
            }
        }
    }
    public void FillWithAnimals(ColorCard[] cards, List<Color> availableColors, Sprite[] animals)
    {
        List<List<ColorCard>> c = new List<List<ColorCard>>();
        for (int i = 0; i < availableColors.Count; i++)
        {
            c.Add(new List<ColorCard>());
            foreach (var item in cards)
            {
                if (item.currentColor == availableColors[i])
                {
                    c.Last().Add(item);
                }
            }
        }
        for (int i = 0; i < c.Count; i++)
        {
            for (int j = 0; j < c[i].Count; j++)
            {
                c[i][j].SetColorImage(0, animals[j]);
                c[i][j].id2 = j;
                c[i][j].colorImages[0].rectTransform.sizeDelta = new Vector2(animals[j].rect.width, animals[j].rect.height) / sizeDenominator;
            }
        }
    }
    #endregion

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
        if(_currLevel == maxLevels)
        {
            Win();
        }
        else
        {
            if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
            CatHelper.Instance.ShowText("Молодец! Идём дальше.");
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
