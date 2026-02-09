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
     public ColorCard  colorCardsM;
                  int colorId;
    public RectTransform[] colorCardsStartPoint;
    public float maxRandomDistance = 10f;
    public float maxRandomRotation = 20f;
    public bool monster;
    public List<Color> rowsColors;

    public UnityEvent falseColor;
    public UnityEvent trueColor;
    public UnityEvent win;
    public bool fin;
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
public int toys,maxtoys,cyrtoes;
    public int maxShielf;
    private int _currLevel = 0;
    public static int _currLevel4 = 0;
    public int maxLevels = 2,placetoys;

    private void Start()
    {
        DragAndDropManager.Instance.onDragStop.AddListener(new UnityAction<DragNDropCard>(OnCardDrop));
        Restart();
         rowsColors.Shuffle();
 
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
        rowsColors = Global.GetRandomColors(maxShielf, 12);
        if(monster)
        {
        rowsColors = Global.GetRandomColors(maxShielf, 12);
        rowsColors.Shuffle();
        }
        GenerateCards();
        GenerateField();
        Invoke("ChekColor",0.3f);
       
    }
public void ChekColor()
{

for (int i = 0; i < 8; i++)
{
  if(monster)
  {
if(colorCardsM.currentColor==colorCards[i].currentColor)
{
maxtoys++;
cyrtoes=maxtoys;
}
}
}
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
            temp.rectTransform.localScale = Vector3.one * 1.25f;
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
                            if(monster==false)
                            {
                            card.SetScale(Vector3.one);
                            }
                            else
                            {
                                 card.SetScale(Vector3.zero);
                                 colorRows[0].cards[0].GetComponent<MonsterController>().StartAnimation();
                                 Invoke("NextLevel",1f);
                                 if(_currLevel==3)
                                 {
                                   placetoys++;  
                                 }


                            }
                            Canvas.ForceUpdateCanvases();
                            LayoutRebuilder.ForceRebuildLayoutImmediate(card.newParent);
                            LayoutRebuilder.ForceRebuildLayoutImmediate(card.rectTransform);
                        }, 0.05f);
                        card.SetScale(card.defaultScale);
                        card.draggable = false;
                        if(monster)
                        {
                         colorId = Array.IndexOf(Global.colors9, colorRows[i].color);
                        }
                        else
                        {
                         colorId = Array.IndexOf(Global.colors9, colorRows[i].color);
                        }
                        
                        CatHelper.Instance.StopDelayedAudio();
                        CatHelper.Instance.ShowText(Global.colorsNames[colorId], 2.5f);
                        CatHelper.Instance.PlayAudio(colorsClips[colorId].audios.Random(),0.1f);

                        trueAnswersCount++;
                        trueColor.Invoke();
                        if (CheckRules())
                        {
                           
                            CatHelper.Instance.ShowText(Global.colorsNames[colorId], 1.5f);
                            Invoke("NextLevel",1f);
                           
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
                        CatHelper.Instance.ShowText(wrongColorText, 1.5f);
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
    public  void FillWithColors(ColorCard[] cards, List<Color> availableColors, int cardsPerColor)
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
           Color c=availableColors.Random();
            
          
            tempCards[i].SetColor(c);
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
        if(monster==false)
            {
        CatHelper.Instance.ShowText(winText, 1f);
          Debug.Log("d");
  
        StartCoroutine(AutoExit(5f));
        //win.Invoke();
            }
            else
{
        CatHelper.Instance.ShowText(winText, 3f);
    StartCoroutine(AutoExit(6f));
}
            

            
    }
    private void NextLevel()
    {
        
        if (_currLevel == maxLevels+1)
        {
            if(monster==false)
            {
            Win(); 
            
            SoundMaster.Instance.PlayWin();

            }
            else
            {
                if(cyrtoes==placetoys)
                {
                            Win(); 
                        SoundMaster.Instance.PlayWin();
                }
            }
            
            //SoundMaster.Instance.PlayWin();
        }
        else
        {
            if(monster==false)
            {
           if(_currLevel4<3)
           {
            if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
            CatHelper.Instance.ShowText("Молодец! Идём дальше.", 2f);
            
            StartCoroutine  (DelayedRestart());      
       
            _currLevel++;
             _currLevel4++;
           }
           else
           {
                  StartCoroutine(DelayFin());
           }
            }
            else
            {
            if(toys==maxtoys-1)
            {
            //SoundMaster.Instance.PlayNextLevel();
            CatHelper.Instance.ShowText("Молодец! Идём дальше.");
          
            StartCoroutine(DelayedRestart());
            if(_currLevel<=3)
            {
            toys=0;
            }
            maxtoys=0;
            maxShielf++;
            _currLevel++;
            }
            else
            toys++;

            }
        }
    }
    private IEnumerator DelayWin()
    {
    yield return new WaitForSeconds(1f);
    Application.LoadLevel("1_2_2_PutOnShelf");
    ///SoundMaster.Instance.PlayWin(3f);
    }
    private IEnumerator DelayFin()
    { 
    CatHelper.Instance.ShowText("Молодец! У тебя всё получилось. Попробуй другую игру...", 3f);
    _currLevel4=0;
    _currLevel=0;
SoundMaster.Instance.PlayWin(0.5f);
     StartCoroutine(AutoExit(5f));
    yield return new WaitForSeconds(5f);
    Application.LoadLevel("MainMenu");
      
   
    }
    private IEnumerator DelayedRestart()
    {
        if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
        yield return new WaitForSeconds(3.5f);
        if(monster==false)
        {
        if(fin==true)
        {
           
        StartCoroutine(DelayFin());
        }
        else
                StartCoroutine(DelayWin());
        }
        else
        {
             Restart();
        }
    }
}