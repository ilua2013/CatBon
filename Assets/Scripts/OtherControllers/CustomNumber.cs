using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomNumber : MonoBehaviour
{
    public Sprite[] numbers;
    public ColorCard numberCard;
    public ColorCard backgroundCard;

    private void Start()
    {
        SetNumber(5);
    }
 public void SetScleM()
    {
        //numberCard.rectTransform.sizeDelta = new Vector2(150f, 200f);
    
    }
     public void SetScleN()
    {
        //numberCard.rectTransform.sizeDelta = new Vector2(130f, 194f);
    
    }

    public void SetNumber(int n)
    {
        numberCard.SetColorImage(0, numbers[n]);
        numberCard.rectTransform.sizeDelta = new Vector2(numbers[n].rect.width, numbers[n].rect.height);
        numberCard.PushAnimation();
        backgroundCard.PushAnimation();
    }
}
