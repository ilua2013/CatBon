using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AllColors_1_1_Controller : BaseController<AllColors_1_1_Controller>
{
    public ColorCard[] cards = new ColorCard[9];
    public RectTransform[] cardsPoints = new RectTransform[9];

    public string[] helperTexts = new string[9];
    public float maxRandomDistance = 10f;
    public float maxRandomRotation = 20f;
    public Sprite[]  colors;

    public string startText = "Нажимай на цвета";
    [Header("Audio")]
    public AudioClip[] startAudio;
    public SoundContainer[] colorsSounds;

    private void Start()
    {
        Restart();
        StartCoroutine(ExitTimer());
    }


    public override void Restart()
    {
        //colors = Global.GetRandomColors(9, 12);
        GenerateCards();
        CatHelper.Instance.defaultText = startText;
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.ShowText(startText);
        CatHelper.Instance.PlayAudio(startAudio.Random());
    }

    public void GenerateCards()
    {
        
        for (int i = 0; i < cards.Length; i++)
        { 
            cards[i].colorImages[0].sprite=colors[i];
            Global.RandomizePositionAndRotation(ref cards[i].rectTransform, cardsPoints[i], maxRandomDistance, maxRandomRotation);
            SimpleCard temp = cards[i].GetComponent<SimpleCard>();
            temp.id = i;
            temp.button.onClick.AddListener(new UnityAction(() => { AllColors_1_1_Controller.Instance.OnCardPush(temp.id); }));
        }
    }
    public void OnCardPush(int id)
    {
        CatHelper.Instance.PlayAudio(colorsSounds[id].audios.Random());
        CatHelper.Instance.ShowText(helperTexts[id]);
    }

    private System.Collections.IEnumerator ExitTimer()
    {
        yield return new WaitForSeconds(90);
        SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit());
    }
}
