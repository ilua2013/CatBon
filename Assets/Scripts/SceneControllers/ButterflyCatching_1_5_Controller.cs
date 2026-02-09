using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyCatching_1_5_Controller : BaseController<ButterflyCatching_1_5_Controller>
{
    public ScoopController scoop;
    public GameObject butterflyPrefab;
    public Color mainColor;
    public int butterfliesPerLevel = 8;
    public int trueColorsPerLevel = 3;
    public int levels = 4;
    public RectTransform butterfliesParent;
    public RectTransform bounds;
    public GameObject redImage;

    private IEnumerator redRoutine;

    [Header("Cat phrases")]
    public string rulesText;
    public string winText;
    public string nextLevelText;
    public string wrongText;

    private int _currLevel = 0;
    private List<ButterflyController> _butterflies;
    private int _colorsLeft = 0;
    private bool _win = false;
    private int colorId;
    private IEnumerator repeaterRoutine;

    [Header("Audio")]
    public SoundContainer[] colorSounds;

    private void Start()
    {
        Restart();
    }

    private void Win()
    {
        _win = true;
        CatHelper.Instance.ShowText(winText, 5f);
        if(SoundMaster.Instance) SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit());
    }
    private void NextLevel()
    {
        //StopCoroutine(repeaterRoutine);
        SetBatterflyInteractible(false);
        _currLevel++;
        if(_currLevel >= levels)
        {
            Win();
            return;
        }
        CatHelper.Instance.ShowText("Молодец! Идём дальше.");
        SoundMaster.Instance.PlayNextLevel();
        StartCoroutine(DelayedRestart());
    }
    private IEnumerator DelayedRestart()
    {
        
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < _butterflies.Count; i++)
        {
            _butterflies[i].Hide();
        }
        yield return new WaitForSeconds(2);
        Restart();
        SetBatterflyInteractible(true);
        //CatHelper.Instance.ShowText(nextLevelText + $" {Global.GetColorName(mainColor)} цвет.");
    }
    public override void Restart()
    {
        GenerateMainColor();
        ClearButterflies();
        GenerateButterflies();
        ColorizeButterflies();
        colorId = Array.IndexOf(Global.colors9, mainColor);
        print(colorId);
        CatHelper.Instance.ShowText(rulesText + Global.colorsNamesGenitive[colorId] + " цвета.");
        CatHelper.Instance.PlayAudio(colorSounds[colorId].audios.Random());
        //Global.RenewCoroutine(this, ref repeaterRoutine, CatchRepeating());
    }
    public void plays()
    {
        CatHelper.Instance.PlayAudio(colorSounds[colorId].audios.Random());
    }

    private void GenerateMainColor()
    {
        Color temp = mainColor;
        while (temp == mainColor)
        {
            mainColor = Global.colors7.Random();
        }
    }
    private void ClearButterflies()
    {
        if (_butterflies == null) return;
        foreach (var item in _butterflies)
        {
            Destroy(item.gameObject);
        }
        _butterflies.Clear();
    }
    private void GenerateButterflies()
    {
        _butterflies = new List<ButterflyController>(butterfliesPerLevel);
        for (int i = 0; i < butterfliesPerLevel; i++)
        {
            ButterflyController temp = Instantiate(butterflyPrefab, butterfliesParent).GetComponent<ButterflyController>();
            temp.rectTransform.SetParent(butterfliesParent, false);
            temp.transform.localPosition = Vector3.zero;
            _butterflies.Add(temp);
            temp.borders = bounds;
            temp.id1 = i;
            temp.button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { OnButterflyTap(temp.id1); }));
        }
    }
    private void ColorizeButterflies()
    {
        _colorsLeft = trueColorsPerLevel;
        List<ButterflyController> temp = new List<ButterflyController>(_butterflies);
        for (int i = 0; i < trueColorsPerLevel; i++)
        {
            int j = UnityEngine.Random.Range(0, temp.Count);
            temp[j].SetColor(mainColor);
            temp[j].Select();
            temp[j].id2 = 1;
            temp.RemoveAt(j);
        }
        foreach (var item in temp)
        {
            Color rndColor = Global.colors9.Random();
            while (rndColor == mainColor)
            {
                rndColor = Global.colors9.Random();
            }
            item.SetColor(rndColor);
            item.id2 = 0;
        }
        temp.Clear();
    }

    public void OnButterflyTap(int id)
    {
        if(_butterflies[id].id2 == 1)
        {
            CatchTheButterfly(_butterflies[id]);
        }
        else
        {
            CatHelper.Instance.ShowText(wrongText);
            if(SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            Global.RenewCoroutine(this, ref redRoutine, RedSpark());
        }
    }
    private void CatchTheButterfly(ButterflyController butterfly)
    {
        
        CatHelper.Instance.ShowText("Верно!", 1.5f);
        butterfly.button.interactable = false;
        butterfly.Deselect();
        butterfly.Stop();
        butterfly.Hide(0.1f);
        scoop.transform.parent.position = butterfly.transform.position;
        scoop.Show();
        _colorsLeft--;
        if(_colorsLeft == 0)
        {
            NextLevel();
        }
        else
        {
            if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
        }
    }

    private IEnumerator AutoExit()
    {
        yield return new WaitForSeconds(Global.AUTO_EXIT_DELAY);
        Exit();
    }
    private IEnumerator RedSpark()
    {
        redImage.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        redImage.SetActive(false);
    }

    private void SetBatterflyInteractible(bool state)
    {
        foreach (var item in _butterflies)
        {
            item.button.interactable = state;
        }
    }

    private IEnumerator CatchRepeating()
    {
        while (!_win)
        {
            yield return new WaitForSeconds(10f);
            CatHelper.Instance.ShowText(rulesText + Global.colorsNamesGenitive[colorId] + " цвета.");
            CatHelper.Instance.PlayAudio(colorSounds[colorId].audios.Random());
        }
    }
}
