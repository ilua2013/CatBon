using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Sprite[] cardFace;
    public AudioClip[] startAudio;
    public Sprite cardBack;
    public AudioClip a_true, a_folse, a_win;
    public string level;
    public static int opencard;
    public GameObject[] cards;
    public Text matchText;
    public bool fin;
    public bool _init = false;
    private int _matches = 0;
    public int max;
    public bool s;
    public string winText;

    public string rulesText;
    // Update is called once per frame
    void Update()
    {
        if (!_init)
            initializeCards();

        if (Input.GetMouseButtonUp(0))
            checkCards();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }

    }
    void Start()
    {
        if (s)
        {
            CatHelper.Instance.ShowText("Давай потренируем память. Найди все парные карточки!");
        }


    }
    void initializeCards()
    {
        Debug.Log("initializingCards() has been called");
        Debug.Log("cards size " + cards.Length);
        for (int id = 0; id < 2; id++)
        {


            for (int i = 1; i <= (cards.Length / 2); i++)
            {

                bool test = false;
                int choice = 0;
                while (!test)
                {
                    choice = Random.Range(0, cards.Length);
                    test = !(cards[choice].GetComponent<Card>().initialized);
                }
                cards[choice].GetComponent<Card>().cardValue = i;
                cards[choice].GetComponent<Card>().initialized = true;
            }
        }
        foreach (GameObject c in cards)
            c.GetComponent<Card>().setupGraphics();

        if (!_init)
            _init = true;
    }

    public Sprite getCardBack()
    {
        return cardBack;
    }
    public Sprite getCardFace(int i)
    {
        return cardFace[i - 1];
    }

    void checkCards()
    {
        List<int> c = new List<int>();
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].GetComponent<Card>().state == 1)
                c.Add(i);
        }
        if (c.Count == 2)
            cardComparison(c);
    }
    public void exit()
    {
        print($"LoadMenu {gameObject.name}. 4");
        SceneManager.LoadScene("MainMenu");
        //_init = false;
        //Card.DO_NOT = true;
    }
    void Load()
    {
        SceneManager.LoadScene(level);
    }
    void cardComparison(List<int> c)
    {
        //Card.DO_NOT = true;
        int x = 0;
        if (cards[c[0]].GetComponent<Card>().cardValue == cards[c[1]].GetComponent<Card>().cardValue)
        {
            x = 2;
            _matches++;
            matchText.text = "Number of Matches; " + _matches;

            //making the found cards non-interactive
            //cards[c[0]].SetActive(false);
            //cards[c[1]].SetActive(false);
            // Making the found cards invisible

            CatHelper.Instance.ShowText("Верно!");
            CatHelper.Instance.PlayAudio(a_true, 0.1f);
            ///cards[c[0]].transform.localScale = new Vector3(0, 0, 0);
            //cards[c[1]].transform.localScale = new Vector3(0, 0, 0);

            if (_matches == 0)
                SceneManager.LoadScene("Menu");
        }
        else
        {
            CatHelper.Instance.ShowText("Неверно...");
            CatHelper.Instance.PlayAudio(a_folse, 0.1f);
        }

        for (int i = 0; i < c.Count; i++)
        {
            cards[c[i]].GetComponent<Card>().state = x;
            cards[c[i]].GetComponent<Card>().falseCheck();
        }
        if (max == _matches)
        {
            if (fin == false)
            {
                CatHelper.Instance.ShowText("Молодец! Идём дальше.");
                CatHelper.Instance.PlayAudio(a_win);

                Invoke("Load", 5f);
            }
            else
            {
                CatHelper.Instance.ShowText("Молодец!Попробуй другую игру.");
                CatHelper.Instance.PlayAudio(a_win, 0.1f);
                Invoke("Load", 5f);

            }
        }
    }
}