using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalContainer
{
    public string name;
    public SoundContainer[] countClips;
    public GameObject objectToClone;
    
}


public class CarryAnimals_2_5_Controller : BaseController<CarryAnimals_2_5_Controller>
{
    public CustomNumber customNumber;
    public AudioClip nev;
    public RectTransform rightArea;
    public RectTransform leftArea;
    public RectTransform[] leftSlots;
    public RectTransform[] rightSlots;
    public AnimalContainer[] availableAnimals;
    public int currentAnimalType;
    public int animalsLeft;
    public int currentLevel = 0;
    public RectTransform animalsParent;
    public SoundContainer[] numbers;

   public bool fin;
     int t; 
    private int _animalsCarried = 0;
    private DragNDropCard[] _currentAnimals;
    public List<RectTransform> _availableRightSots;
    public List<RectTransform> _availableLeftSots;

    private void Start()
    {
        DragAndDropManager.Instance.onDragStop.AddListener(OnCardDrop);
        DragAndDropManager.Instance.onDragStart.AddListener(OnCardStartDrag);
        Restart();
    }

    public void GenerateNumber()
    {
        int prevAnimalType = currentAnimalType;
        while (prevAnimalType == currentAnimalType)
        {
            currentAnimalType = Random.Range(0, availableAnimals.Length);
        }
        animalsLeft = Random.Range(3, 11);
        customNumber.SetNumber(animalsLeft);
        CatHelper.Instance.ShowText($"Перенеси {availableAnimals[currentAnimalType].countClips[animalsLeft - 1].name} на другой луг.", 4f);
        if(availableAnimals[currentAnimalType].countClips[animalsLeft - 1].phrases.Length > 0)
        {
            CatHelper.Instance.PlayAudio(availableAnimals[currentAnimalType].countClips[animalsLeft - 1].phrases.Random());
        }
        else
        {
            CatHelper.Instance.PlayAudio(availableAnimals[currentAnimalType].countClips[animalsLeft - 1].audios.Random());
        }
    }
    public void GenerateAnimals()
    {
        _currentAnimals = new DragNDropCard[leftSlots.Length];
        _availableLeftSots = new List<RectTransform>();
        for (int i = 0; i < leftSlots.Length; i++)
        {
            _availableLeftSots.Add(leftSlots[i]);
        }
        int count = leftSlots.Length;
        for (int i = 0; i < count; i++)
        {
            int t = Random.Range(0, _availableLeftSots.Count);
            _currentAnimals[i] = Instantiate(availableAnimals[currentAnimalType].objectToClone, _availableLeftSots[t]).GetComponent<DragNDropCard>();
            _currentAnimals[i].defaultParent = _availableLeftSots[t];
            _currentAnimals[i].rectTransform.position = _availableLeftSots[t].position;
            _currentAnimals[i].draggable = true;
            _currentAnimals[i].defaultAnchoredPosition = _currentAnimals[i].rectTransform.position;
            _availableLeftSots.RemoveAt(t);
        }
    }
    public void Win()
    {
        CatHelper.Instance.ShowText(winText);
        SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit(6f));
    }

    [Header("Cat phrases")]
    public string rulesText;
    public string winText;
    public string nextLevelText;

    public void NextLevel()
    {
        //DeactivateAnimals();
        currentLevel++;
        if(currentLevel > 4)
        {
            Invoke("Win",4f);
        }
        else
        {
            _animalsCarried = 0;
            
            StartCoroutine(DelayedRestart());
        }
    }

    public bool CheckRules()
    {
        if(animalsLeft == 0)
        {
            NextLevel();
            fin=true;
            return true;
        }
        return false;
    }
    public void OnCardStartDrag(DragNDropCard dragNDropCard)
    {
        dragNDropCard.rectTransform.SetParent(animalsParent);
        dragNDropCard.rectTransform.SetAsLastSibling();
         customNumber.SetScleM();
        

    }
    public void OnCardDrop(DragNDropCard dragNDropCard)
    {
        if (rightArea.ContainsPointScalable(dragNDropCard.rectTransform.position))
        {   
          
            t = Random.Range(0, _availableRightSots.Count);
            dragNDropCard.rectTransform.SetParent(_availableRightSots[t]);
              customNumber.SetScleN();
            if(dragNDropCard.C_Parent==null)
            {
            dragNDropCard.GoToPosition(_availableRightSots[t].position, () => { dragNDropCard.SetDefaultScale(); });
            dragNDropCard.C_Parent=_availableRightSots[t]; 
            }
            else
            {
            dragNDropCard.rectTransform.SetParent(dragNDropCard.C_Parent);
            dragNDropCard.GoToPosition(dragNDropCard.C_Parent.position, () => { dragNDropCard.SetDefaultScale(); });
            }
           
            if(dragNDropCard.name!="lefted")
            {
            
            animalsLeft--;
            _animalsCarried++;
            dragNDropCard.name="lefted"; 
            _availableRightSots.RemoveAt(t);
            }
            if(animalsLeft >= 0)
            {
            CatHelper.Instance.PlayAudio(numbers[_animalsCarried].audios);
            CatHelper.Instance.ShowText(numbers[_animalsCarried].name, 1f);
            //customNumber.SetNumber(_animalsCarried);
            CheckRules();
            }
            else
            {
                  CatHelper.Instance.ShowText("Неверно...", 1f);
                           CatHelper.Instance.PlayAudio(nev);
                StopAllCoroutines();
            }
        }
        else
        {
            dragNDropCard.rectTransform.SetParent(dragNDropCard.defaultParent);
            dragNDropCard.GoToPosition(dragNDropCard.defaultAnchoredPosition, () => { dragNDropCard.SetDefaultScale(); });
        }


        if (leftArea.ContainsPointScalable(dragNDropCard.rectTransform.position))
        {
            //int t = Random.Range(0, _availableRightSots.Count);
            //dragNDropCard.rectTransform.SetParent(_availableRightSots[t]);
            //dragNDropCard.GoToPosition(_availableRightSots[t].position, () => { dragNDropCard.SetDefaultScale(); });
            //dragNDropCard.draggable = false;
           // _availableRightSots.RemoveAt(t);
           if(animalsLeft >= 0)
            {
                if(fin==true)
                {
                CatHelper.Instance.ShowText("Неверно", 1f);
                         CatHelper.Instance.PlayAudio(nev);
                StopAllCoroutines();
                }
            }

           if(dragNDropCard.name=="lefted")
            {
               animalsLeft++;
               dragNDropCard.name="right"; 
              _animalsCarried--;
              _availableRightSots.Add(dragNDropCard.C_Parent);
              dragNDropCard.C_Parent=null;
            }
            CheckRules();
          
        }
    }

    public override void Restart()
    {
        if(_currentAnimals != null && _currentAnimals.Length > 0)
        {
            foreach (var item in _currentAnimals)
            {
                Destroy(item.gameObject);
            }
            _currentAnimals = null;
        }
        _animalsCarried = 0;
        //customNumber.SetNumber(_animalsCarried);
        GenerateNumber();
        GenerateAnimals();
        _availableRightSots = new List<RectTransform>();
        for (int i = 0; i < rightSlots.Length; i++)
        {
            _availableRightSots.Add(rightSlots[i]);
        }
    }

    private void DeactivateAnimals()
    {
        foreach (var item in _currentAnimals)
        {
            item.draggable = false;
        }
    }

    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(2.5f);
        CatHelper.Instance.ShowText(nextLevelText);
        SoundMaster.Instance.PlayNextLevel();
        yield return new WaitForSeconds(2.5f);
        Restart();
    }
}
