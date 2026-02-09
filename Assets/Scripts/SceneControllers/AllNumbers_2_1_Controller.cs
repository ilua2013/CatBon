using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AllNumbers_2_1_Controller : BaseController<AllNumbers_2_1_Controller>
{
    public ColorCard[] cards;
    public RectTransform[] cardsPoints;
    public float maxRandomDistance = 10f;
    public float maxRandomRotation = 20f;

    public string startText = "Нажимай на цифры";
    private FloatingAlpha lastFloatingAlpha;
    
    [Header("Audio")]
    public AudioClip[] startAudio;
    public SoundContainer[] numbersSounds;

    private void Start()
    {
        Restart();
        StartCoroutine(ExitTimer());
    }

    public override void Restart()
    {
        GenerateCards();
        CatHelper.Instance.ShowText(startText);
        CatHelper.Instance.PlayAudio(startAudio);
    }

    public void GenerateCards()
    {
        List<RectTransform> tpoints = new List<RectTransform>(cardsPoints);
        for (int i = 0; i < cards.Length; i++)
        {
            //int t = Random.Range(0, tpoints.Count);
            Global.RandomizePositionAndRotation(ref cards[i].rectTransform, tpoints[i], maxRandomDistance, 0f);
            //tpoints.RemoveAt(t);
            SimpleCard temp = cards[i].GetComponent<SimpleCard>();
            temp.button.onClick.AddListener(new UnityAction(() =>
            {
                AllNumbers_2_1_Controller.Instance.OnCardPush(temp.id);
                AllNumbers_2_1_Controller.Instance.lastFloatingAlpha = temp.GetComponent<FloatingAlpha>();
                AllNumbers_2_1_Controller.Instance.lastFloatingAlpha.StartFloating(0.5f);
            }));
        }
    }
    public void OnCardPush(int id)
    {
        CatHelper.Instance.PlayAudio(numbersSounds[id].audios);
        CatHelper.Instance.ShowText(Global.numbersNames[id]);
        if (lastFloatingAlpha) lastFloatingAlpha.StopFloating();
    }

    private System.Collections.IEnumerator ExitTimer()
    {
        yield return new WaitForSeconds(90);
        SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit());
    }
}
