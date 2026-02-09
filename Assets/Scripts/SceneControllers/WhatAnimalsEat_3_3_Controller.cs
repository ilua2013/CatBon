using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AnimalFoodContainer
{
    public string name;
    public AudioClip[] trueFood;
    public AudioClip[] falseFood;
    public AudioClip[] questionFood;
    public string trueFoodText;
    public string falseFoodText;
    public string questionFoodText;
    public Sprite animal;
    public Sprite food;
}


public class WhatAnimalsEat_3_3_Controller : BaseController<WhatAnimalsEat_3_3_Controller>
{
    public HorizontalScroll horizontalScroll;
    public Image animalImage;


    public AnimalFoodContainer[] availableAnimals;
    private  static int _currentAnimal = 0;
    public string Scene;
    public bool Fin;
    public  Sprite[] _availableFood;

    [Header("Cat phrases")]
    public string rulesText;
    public string winText;
    public string nextLevelText;
    public static int nextLevel;
    public string wrongAnswerText;
    public bool s;

    [Header("Audio")]
    public AudioClip[] startAudio;


    private void Start()
    {
        if( _currentAnimal==0)
        {
             _currentAnimal=Random.Range(0,10);
        }
         Debug.Log(_currentAnimal);
        _availableFood = new Sprite[availableAnimals.Length];
        for (int i = 0; i < availableAnimals.Length; i++)
        {
            _availableFood[i] = availableAnimals[i].food;
        }
        if(s==true)
        {
        CatHelper.Instance.ShowText(rulesText, 8f);
        CatHelper.Instance.defaultText = rulesText;
        
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.audioSource.clip = startAudio.Random();
        CatHelper.Instance.audioSource.Play();
        }
        //availableAnimals.Shuffle();
        Restart();
    }

    public void OnCardPush(int id)
    {
        if (id == 1)
        {
            foreach (var item in horizontalScroll.availableCards[0].cards)
            {
                if (item.id1 == 1)
                {
                    DragNDropCard dnd = item.GetComponent<DragNDropCard>();
                    dnd.enabled = true;
                    dnd.rectTransform.localScale = Vector3.one;
                    dnd.SetScale(Vector3.one * 1.2f);
                    dnd.GoToPosition(animalImage.rectTransform.position,
                        () =>
                        {
                            dnd.SetScale(Vector3.zero,
                                () =>
                                {
                                    dnd.rectTransform.localPosition = Vector3.zero;
                                    //dnd.rectTransform.localScale = Vector3.one;
                                    dnd.enabled = false;
                                });

                        }, 0.25f);
                    break;
                }
            }
            NextLevel();
        }
        else
        {
            CatHelper.Instance.ShowText(availableAnimals[_currentAnimal].falseFoodText, 5f);
            CatHelper.Instance.PlayAudio(availableAnimals[_currentAnimal].falseFood);
        }
    }
    public void NextLevel()
    {
  
        SetButtonsInteractive(false);
        nextLevel++;
        _currentAnimal++;
        if(_currentAnimal>10)
        {
          _currentAnimal=0;
        }
        if (nextLevel >10)
        {
            Win();
        }
        else
        {
            CatHelper.Instance.ShowText(nextLevelText, 4f);

            if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
            SoundMaster.Instance.PlayNextLevel();
            StartCoroutine(DelayedRestart());
        }
    }
    public void Win()
    {
        CatHelper.Instance.ShowText(winText);
        SoundMaster.Instance.PlayWin();
        _currentAnimal=0;
         nextLevel=0;
        StartCoroutine(AutoExit());
    }
    public override void Restart()
    {
        ClearButtonEvents();
        GenerateAnimal();
        GenerateFood();
        SetButtonsInteractive(true);
    }


    private void GenerateAnimal()
    {

        animalImage.sprite = availableAnimals[_currentAnimal].animal;
        animalImage.rectTransform.sizeDelta
            = new Vector2(availableAnimals[_currentAnimal].animal.rect.width, availableAnimals[_currentAnimal].animal.rect.height);
        if(availableAnimals[_currentAnimal].name == "кот")
        {
            animalImage.rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else
        {
            animalImage.rectTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }
    }
    private void GenerateFood()
    {
        Sprite[] tempFood = new Sprite[horizontalScroll.slots.Length];
        tempFood[0] = availableAnimals[_currentAnimal].food;

        tempFood[1] = _availableFood.Random();
        while (tempFood[1] == tempFood[0])
        {
            tempFood[1] = _availableFood.Random();
        }

        tempFood[2] = _availableFood.Random();
        while (tempFood[2] == tempFood[0] || tempFood[2] == tempFood[1])
        {
            tempFood[2] = _availableFood.Random();
        }

        tempFood[3] = _availableFood.Random();
        while (tempFood[3] == tempFood[0] || tempFood[3] == tempFood[1] || tempFood[3] == tempFood[2])
        {
            tempFood[3] = _availableFood.Random();
        }

        tempFood.Shuffle();
        for (int i = 0; i < tempFood.Length; i++)
        {
            ColorCard t = horizontalScroll.availableCards[0].cards[i];
            t.SetColorImage(0, tempFood[i]);
            t.id1 = tempFood[i] == availableAnimals[_currentAnimal].food ? 1 : 0;
            horizontalScroll.availableCards[0].cards[i].GetComponent<Button>()
                .onClick.AddListener(
                new UnityEngine.Events.UnityAction(() => { OnCardPush(t.id1); })
                );
            t.GetComponent<AspectRatioFitter>().aspectRatio = tempFood[i].rect.width / tempFood[i].rect.height;
        }
        foreach (var item in horizontalScroll.availableCards[0].cards)
        {
            item.rectTransform.localScale = Vector3.one;
        }
    }

    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(2f);
        Application.LoadLevel(Scene);
    }
    void SetButtonsInteractive(bool state)
    {
        foreach (var item in horizontalScroll.availableCards[0].cards)
        {
            item.GetComponent<Button>().interactable = state;
        }
    }
    void ClearButtonEvents()
    {
        foreach (var item in horizontalScroll.availableCards[0].cards)
        {
            item.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }


}
