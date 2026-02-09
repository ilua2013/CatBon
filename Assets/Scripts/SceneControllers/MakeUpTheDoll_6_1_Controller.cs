using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MakeUpTheDoll_6_1_Controller : BaseController<MakeUpTheDoll_6_1_Controller>
{
    public GameObject[] dollVariants;
    public int stage = 2;

    public DreeRandom  DreeRandom ;
    [Header("Parts")]
    public Image doll;
    public bool dool2;
    public Image mud;
    public Image blush;
    public Image lips;
    public Image lips2;
    public Image eyelash;
    public Image eyelashbig;
    public GameObject S_G,M_G,L_G,Drees,Face,lp,el;
    [Header("Cosmetics")]
    public DragNDropCard cottonpad;
    public float lipstickProgressDuration = 3f; // Длительность изменения прозрачности
    private float lipstickProgressStartTime; // Время начала изменения прозрачности
    public DragNDropCard lipstick;
    public DragNDropCard mascara;
    public DragNDropCard sponge;
    [Header("Progression")]
    public float faceCleaningProgress;
    public float creamProgress;
    public float eyelashesProgress;
    public float lipstickProgress;
    [Header("Progression speed")]
    public float faceCleaningProgressSpeed;
    public float creamProgressSpeed;
    public float eyelashesProgressSpeed;
    public float lipstickProgressSpeed;
    [Header("Progression texts")]
    public string startRulesText;
    public string faceCleaningText;
    public string creamText;
    public string eyelashesText;
    public string lipstickText;
    public string endText;
    public string wrongItemText;
    [Header("Delays")]
    public float startRulesDelay;
    [Header("Audio")]
    public AudioClip[] startAudio;
    [Header("Highlighters")]
    public FloatingAlpha cottonpadHighlighter;
    public FloatingAlpha lipstickHighlighter;
    public FloatingAlpha mascaraHighlighter;
    public FloatingAlpha spongeHighlighter;
    Color c; 
    public int maxLevels = 3;
    private int _currLevel = 0;




    

  public void Choise(DragNDropCard sponges)
  {
      sponge= sponges;
      sponge.onDrag += OnDrag;
  }
  public void Choise_mascara(DragNDropCard mascaras)
  {
    mascara= mascaras;
      mascara.onDrag += OnDrag;
  }
    public void Choise_lipstick(DragNDropCard lipsticks)
  {
    lipstick= lipsticks;
      lipstick.onDrag += OnDrag;
  }
    public override void Restart()
    {
        RandomizeDoll();

        //faceCleaningProgress = 0;
        //olor c = mud.color;
        //c.a = Mathf.Lerp(1f, 0f, faceCleaningProgress);
        //mud.color = c;

        eyelashesProgress = 0f;
        c = eyelash.color;
        c.a = 0.1f;
        eyelash.color = c;

        creamProgress = 0;
        c = blush.color;
        c.a=0.1f;
        blush.color = c;

        lipstickProgress = 0;
        c = lips.color;
        c.a = Mathf.Lerp(0.01f, 1f, lipstickProgress);
        lips.color = c;

        stage = 2;
        if(_currLevel == 0)
        {

        }
        else
        {
            startRulesDelay = 0;
        }
        StartCoroutine(_Controller());
    }
    private void Start()
    {
        if(dool2==true)
        {
        //CatHelper.Instance.Stop();
        //if (SoundMaster.Instance) SoundMaster.Instance.Stop();
        CatHelper.Instance.defaultText = startRulesText;
        CatHelper.Instance.ShowText(startRulesText);
        ///CatHelper.Instance.defaultAudio = startAudio.Random();
        //CatHelper.Instance.audioSource.clip = startAudio.Random();
        //CatHelper.Instance.audioSource.Play();
        
        cottonpad.onDrag += OnDrag;
        //lipstick.onDrag += OnDrag;
       
      
        dollVariants.Shuffle();
        Restart();
        }
    }


    private IEnumerator _Controller()
    {
        yield return new WaitForSeconds(startRulesDelay);

        CatHelper.Instance.ShowText(faceCleaningText);
         ///stage = 2;
        cottonpadHighlighter.StartFloating(0f);
        while (faceCleaningProgress < 1f) yield return null;
        DragAndDropManager.Instance.lastCard.StopDrag();
        cottonpadHighlighter.StopFloating();

        CatHelper.Instance.ShowText(creamText);
        ///stage = 2;
        spongeHighlighter.StartFloating(0f);
        while (creamProgress < 0.6f) yield return null;
        DragAndDropManager.Instance.lastCard.StopDrag();
        spongeHighlighter.StopFloating();

        CatHelper.Instance.ShowText(eyelashesText);
       /// stage = 3;
        mascaraHighlighter.StartFloating(0f);
        while (eyelashesProgress < 1f) yield return null;
        DragAndDropManager.Instance.lastCard.StopDrag();
        mascaraHighlighter.StopFloating();

        CatHelper.Instance.ShowText(lipstickText);
        ///stage = 4;
        lipstickHighlighter.StartFloating(0f);
        while (lipstickProgress < 1f) yield return null;
        DragAndDropManager.Instance.lastCard.StopDrag();
        lipstickHighlighter.StopFloating();

        NextLevel();

        //CatHelper.Instance.ShowText(endText);
        //SoundMaster.Instance.PlayWin();
        //StartCoroutine(AutoExit());
        stage = 5;
        yield break;
    }


    public void OnDragStart(DragNDropCard card)
    {
        if (card.id1 != stage)
        {
            if (stage != 0) CatHelper.Instance.ShowText(wrongItemText, 3f);
            card.StopDrag();
        }
      
    }
    public void OnDragStop(DragNDropCard card)
    {
        card.GoToDefaultPosition();
        
    }

    public void OnDrag(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) == 0 && Mathf.Abs(delta.y) == 0) return;
        if (stage == 1)
        {
            if (mud.rectTransform.ContainsPointScalable(cottonpad.pointer.position))
            {
                AddFaceCleaningProgress();
            }
        }
        else if (stage == 2)
        {
            if (blush.rectTransform.ContainsPointScalable(sponge.pointer.position))
            {
                AddCreamProgress();
            }
        }
        else if (stage == 3)
        {
            if (eyelashbig.rectTransform.ContainsPointScalable(mascara.pointer.position))
            {
                AddEyelashesProgress();
               
            }
        }
        else
        {
            if (lips2.rectTransform.ContainsPointScalable(lipstick.pointer.position))
            {
                AddLipstickProgress();
            }
        }
    }

void FixedUpate()
{
   if (lips2.rectTransform.ContainsPointScalable(lipstick.pointer.position))
            {
                AddLipstickProgress();
            } 
}
    public void RandomizeDoll()
    {
        foreach (var item in dollVariants)
        {
            item.SetActive(false);
        }
        dollVariants[_currLevel].SetActive(true);
        //doll.rectTransform.sizeDelta = doll.sprite.rect.size / 1.5f;
    }

    public void AddFaceCleaningProgress()
    {
        faceCleaningProgress = Mathf.Clamp(faceCleaningProgress + faceCleaningProgressSpeed, 0f, 1f);
        Color c = mud.color;
        c.a = Mathf.Lerp(1f, 0f, faceCleaningProgress);
        mud.color = c;
    }
    public void AddEyelashesProgress()
    {
        eyelashesProgress = Mathf.Clamp(eyelashesProgress + eyelashesProgressSpeed, 0f, 1f);
        Color c = eyelash.color;
        c.a = Mathf.Lerp(0.1f, 1f, eyelashesProgress);
        eyelash.color = c;

          if(eyelashesProgress>0.95f)
          {
            stage=4;
            L_G.SetActive(true);
            M_G.SetActive(false);
          }
    }
    public void AddCreamProgress()
    {
        creamProgress = Mathf.Clamp(creamProgress + creamProgressSpeed/2, 0f, 1f);
        Color c = blush.color;
        c.a = Mathf.Lerp(0.1f, 1f, creamProgress);
        blush.color = c;
        
        if(creamProgress>0.85f)
        {
            stage=3;
             el.SetActive(true);
            S_G.SetActive(false);
            M_G.SetActive(true);
        }
    }
    public void Reset()
    {
     
        Color c = lips.color;  
        c.a = 0;
        lips.color = c;
        lipstickProgress=0;
    }
    public void AddLipstickProgress()
    {
    if (lips2.rectTransform.ContainsPointScalable(lipstick.pointer.position))
    {
    if (lipstickProgressStartTime == 0f)
    {
        // Начало изменения прозрачности
        lipstickProgressStartTime = Time.time;
    }

    // Прошедшее время с начала изменения прозрачности
   float elapsedTime = Time.fixedTime - lipstickProgressStartTime;

    // Вычисляем прогресс изменения прозрачности в диапазоне от 0 до 1
    float progress = Mathf.Clamp01(elapsedTime / lipstickProgressDuration);
     lipstickProgress=progress;
    // Вычисляем прозрачность с помощью линейной интерполяции
    Color c = lips.color;
    c.a = Mathf.Lerp(0.1f, 1f, progress);
    lips.color = c;
}
else
{
    // Прямоугольники не пересекаются, прекращаем изменение прозрачности
    lipstickProgressStartTime = 0f;
}
Debug.Log(lipstickProgress+"d");
if (lipstickProgress > 0.95f)
{
    Drees.SetActive(true);
    Face.SetActive(false);
    lp.SetActive(true);
    DreeRandom.Next();
}
}


    private void Win()
    {
        CatHelper.Instance.ShowText(endText, 5f);
        SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit(5.5f));
    }
     public  void NextLevel()
    {
        _currLevel++;
        if (_currLevel == maxLevels)
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
