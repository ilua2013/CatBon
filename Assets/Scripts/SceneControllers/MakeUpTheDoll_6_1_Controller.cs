using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MakeUpTheDoll_6_1_Controller : BaseController<MakeUpTheDoll_6_1_Controller>
{
    public GameObject[] dollVariants;
    public int stage = 0;


    [Header("Parts")]
    public Image doll;
    public Image mud;
    public Image blush;
    public Image lips;
    public Image eyelash;
    [Header("Cosmetics")]
    public DragNDropCard cottonpad;
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

    public int maxLevels = 3;
    private int _currLevel = 0;


    public override void Restart()
    {
        RandomizeDoll();

        faceCleaningProgress = 0;
        Color c = mud.color;
        c.a = Mathf.Lerp(1f, 0f, faceCleaningProgress);
        mud.color = c;

        eyelashesProgress = 0.25f;
        c = eyelash.color;
        c.a = Mathf.Lerp(0f, 1f, eyelashesProgress);
        eyelash.color = c;

        creamProgress = 0;
        c = blush.color;
        c.a = Mathf.Lerp(1f, 0f, creamProgress);
        blush.color = c;

        lipstickProgress = 0;
        c = lips.color;
        c.a = Mathf.Lerp(0f, 1f, lipstickProgress);
        lips.color = c;

        stage = 0;
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
        CatHelper.Instance.Stop();
        if (SoundMaster.Instance) SoundMaster.Instance.Stop();
        CatHelper.Instance.defaultText = startRulesText;
        CatHelper.Instance.ShowText(startRulesText);
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.audioSource.clip = startAudio.Random();
        CatHelper.Instance.audioSource.Play();
        
        cottonpad.onDrag += OnDrag;
        lipstick.onDrag += OnDrag;
        mascara.onDrag += OnDrag;
        sponge.onDrag += OnDrag;
        dollVariants.Shuffle();
        Restart();
    }


    private IEnumerator _Controller()
    {
        yield return new WaitForSeconds(startRulesDelay);

        CatHelper.Instance.ShowText(faceCleaningText);
        stage = 1;
        cottonpadHighlighter.StartFloating(0f);
        while (faceCleaningProgress < 1f) yield return null;
        DragAndDropManager.Instance.lastCard.StopDrag();
        cottonpadHighlighter.StopFloating();

        CatHelper.Instance.ShowText(creamText);
        stage = 2;
        spongeHighlighter.StartFloating(0f);
        while (creamProgress < 1f) yield return null;
        DragAndDropManager.Instance.lastCard.StopDrag();
        spongeHighlighter.StopFloating();

        CatHelper.Instance.ShowText(eyelashesText);
        stage = 3;
        mascaraHighlighter.StartFloating(0f);
        while (eyelashesProgress < 1f) yield return null;
        DragAndDropManager.Instance.lastCard.StopDrag();
        mascaraHighlighter.StopFloating();

        CatHelper.Instance.ShowText(lipstickText);
        stage = 4;
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
        else
        {
            switch (stage)
            {
                case 1:
                    cottonpadHighlighter.StopFloating();
                    break;
                case 2:
                    spongeHighlighter.StopFloating();
                    break;
                case 3:
                    mascaraHighlighter.StopFloating();
                    break;
                case 4:
                    lipstickHighlighter.StopFloating();
                    break;
                default:
                    break;
            }
        }
    }
    public void OnDragStop(DragNDropCard card)
    {
        card.GoToDefaultPosition();
        switch (stage)
        {
            case 1:
                cottonpadHighlighter.StartFloating(0f);
                break;
            case 2:
                spongeHighlighter.StartFloating(0f);
                break;
            case 3:
                mascaraHighlighter.StartFloating(0f);
                break;
            case 4:
                lipstickHighlighter.StartFloating(0f);
                break;
            default:
                break;
        }
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
            if (eyelash.rectTransform.ContainsPointScalable(mascara.pointer.position))
            {
                AddEyelashesProgress();
            }
        }
        else
        {
            if (lips.rectTransform.ContainsPointScalable(lipstick.pointer.position))
            {
                AddLipstickProgress();
            }
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
        c.a = Mathf.Lerp(0f, 1f, eyelashesProgress);
        eyelash.color = c;
    }
    public void AddCreamProgress()
    {
        creamProgress = Mathf.Clamp(creamProgress + creamProgressSpeed, 0f, 1f);
        Color c = blush.color;
        c.a = Mathf.Lerp(1f, 0f, creamProgress);
        blush.color = c;
    }
    public void AddLipstickProgress()
    {
        lipstickProgress = Mathf.Clamp(lipstickProgress + lipstickProgressSpeed, 0f, 1f);
        Color c = lips.color;
        c.a = Mathf.Lerp(0f, 1f, lipstickProgress);
        lips.color = c;
    }


    private void Win()
    {
        CatHelper.Instance.ShowText(endText, 5f);
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
