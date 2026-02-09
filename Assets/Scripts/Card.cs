using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{


    public bool DO_NOT = false;

    public static int openCardsCount = 0;
    

private enum State
{
    FrontSide,
    BackSide
}

[SerializeField]
private int _state;
[SerializeField]
private int _cardValue;
[SerializeField]
private bool _initialized = false;

private Sprite _cardBack;
private Sprite _cardFace;

private GameObject _manager;

private void Start()
{
    _state = 1;
    _manager = GameObject.FindGameObjectWithTag("Manager");
}

public void setupGraphics()
{
    _cardBack = _manager.GetComponent<GameManager>().getCardBack();
    _cardFace = _manager.GetComponent<GameManager>().getCardFace(_cardValue);
    flipCard();
    openCardsCount=0;
}

public void flipCard()
{
    Debug.Log(openCardsCount );
    if (openCardsCount < 2)
    {
        if (_state == 0)
        {
            _state = 1;
        }
        else
        {
            _state = 0;
        }

        if (_state == 0)
        {
            GetComponent<Image>().sprite = _cardBack;
            GetComponent<Button>().enabled = true;
            openCardsCount--;
        }
        else if (_state == 1)
        {
            GetComponent<Image>().sprite = _cardFace;
            GetComponent<Button>().enabled = false;
            openCardsCount++;
        }
    }
}

public int cardValue
{
    get => _cardValue;
    set { _cardValue = value; }
}

public int state
{
    get => _state;
    set { _state = value; }
}

public bool initialized
{
    get => _initialized;
    set { _initialized = value; }
}

public void falseCheck()
{
    StartCoroutine(pause());
}

IEnumerator pause()
{
    yield return new WaitForSeconds(1.2f);
    if (_state == 0)
    {
        GetComponent<Image>().sprite = _cardBack;
    }
    else if (_state == 1)
    {
        GetComponent<Image>().sprite = _cardFace;
    }
    GetComponent<Button>().enabled = true;
    openCardsCount--;
}
}