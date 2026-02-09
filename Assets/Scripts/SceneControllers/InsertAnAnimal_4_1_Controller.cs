using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertAnAnimal_4_1_Controller : BaseController<InsertAnAnimal_4_1_Controller>
{
    public ColorCard[] currentAnimals;
    public ColorCard[] availableAnimals;
    public RectTransform[] animalsSlots;
    public RectTransform[] shadowsSlots;
    public RectTransform cardsParent;

    [Header("Cat phrases")]
    public string rulesText;
    public string trueAnimalText;
    public string wrongAnimalText;
    public string winText;
    public string nextLevelText;


    private int currStartIdex = 0;
    private int currLevel = 0;
    private int trueAnswers = 0;
    private List<ColorCard> shadows = new List<ColorCard>();

    [Header("Audio")]
    public AudioClip[] startAudio;

    public override void Restart()
    {
        HideAnimals();
        SelectAnimals();
        PlaceAnimals();
        PlaceShadows();
    }
    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(2f);
        Restart();
    }
    private void Start()
    {
        CatHelper.Instance.ShowText(rulesText);
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.defaultText = rulesText;
        CatHelper.Instance.audioSource.clip = startAudio.Random();
        CatHelper.Instance.audioSource.Play();
        availableAnimals.Shuffle();
        DragAndDropManager.Instance.onDragStop.AddListener(OnDragStop);
        Restart();
    }
    private void Win()
    {
        CatHelper.Instance.ShowText(winText);
        if (SoundMaster.Instance) SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit());
    }
    public void NextLevel()
    {
        currLevel++;
        if(currLevel == 6)
        {
            Win();
        }
        else
        {
            CatHelper.Instance.ShowText(nextLevelText);
            if(SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
            currStartIdex += 3;
            StartCoroutine(DelayedRestart());
        }
    }

    public void HideAnimals()
    {
        for (int i = 0; i < availableAnimals.Length; i++)
        {
            int tId1 = availableAnimals[i].id1;
            foreach (var item in shadows)
            {
                if(item.id1 == tId1)
                {
                    item.rectTransform.SetParent(availableAnimals[i].rectTransform);
                }
            }
            availableAnimals[i].gameObject.SetActive(false);
        }
    }
    public void SelectAnimals()
    {
        currentAnimals = new ColorCard[Random.Range(3,8)];
        availableAnimals.Shuffle();
        List<ColorCard> tempAnimals = new List<ColorCard>(availableAnimals);
        for (int i = 0; i < currentAnimals.Length; i++)
        {
            int t = Random.Range(0, tempAnimals.Count);
            currentAnimals[i] = tempAnimals[t];
            tempAnimals.RemoveAt(t);
        }
    }
    public void PlaceAnimals()
    {
        List<RectTransform> tempSlots = new List<RectTransform>(animalsSlots);

        for (int i = 0; i < currentAnimals.Length; i++)
        {
            int tIndex = Random.Range(0, tempSlots.Count);
            currentAnimals[i].gameObject.SetActive(true);
            currentAnimals[i].rectTransform.SetParent(animalsSlots[i]);
            currentAnimals[i].rectTransform.localPosition = Vector3.zero;
            DragNDropCard t = currentAnimals[i].GetComponent<DragNDropCard>();
            t.defaultAnchoredPosition = currentAnimals[i].rectTransform.position;
            t.rectTransform.SetParent(cardsParent, true);
            t.defaultParent = cardsParent;
            t.draggable = true;
            t.id1 = currentAnimals[i].id1;
            t.defaultScale = currentAnimals[i].defaultScale = currentAnimals[i].rectTransform.localScale;

            tempSlots.RemoveAt(tIndex);
        }
    }
    public void PlaceShadows()
    {
        shadows.Clear();
        List<RectTransform> tShadowSlots = new List<RectTransform>(shadowsSlots);

        for (int i = 0; i < currentAnimals.Length; i++)
        {
            int j = Random.Range(0, tShadowSlots.Count);
            RectTransform t = currentAnimals[i].rectTransform.Find("Shadow") as RectTransform;
            t.GetComponent<ColorCard>().id1 = currentAnimals[i].id1;
            shadows.Add(t.GetComponent<ColorCard>());
            t.SetParent(tShadowSlots[j]);
            t.position = tShadowSlots[j].position;

            tShadowSlots.RemoveAt(j);
        }
    }

    public void OnDragStop(DragNDropCard dragNDropCard)
    {
        for (int i = 0; i < shadows.Count; i++)
        {
            if(shadows[i].rectTransform.ContainsPointScalable(dragNDropCard.rectTransform.position)) // попали на тень
            {
                if(shadows[i].id1 == dragNDropCard.id1) // попали на нужную тень
                {
                    trueAnswers++;
                    CatHelper.Instance.ShowText(trueAnimalText);
                    
                    dragNDropCard.draggable = false;
                    dragNDropCard.GoToPosition(shadows[i].rectTransform.position, () => { dragNDropCard.SetDefaultScale(); }, 0.05f);
                    dragNDropCard.rectTransform.SetSiblingIndex(shadows[i].rectTransform.parent.GetSiblingIndex() + 1);
                    if(CheckRules())
                    {

                    }
                    else
                    {
                        if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                    }
                    return;
                }
                else
                {
                    CatHelper.Instance.ShowText(wrongAnimalText);
                    if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                }
            }
        }
        dragNDropCard.GoToPosition(dragNDropCard.defaultAnchoredPosition, () => { dragNDropCard.SetDefaultScale(); });
    }
    public bool CheckRules()
    {
        bool result = false;
        if(trueAnswers == currentAnimals.Length)
        {
            result = true;
            trueAnswers = 0;
            NextLevel();
        }
        return result;
    }
}
