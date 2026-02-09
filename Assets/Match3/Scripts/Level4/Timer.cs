using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float maxTimer = 60f;
    float timer = 60f;
    float k = 1f;
    Board board;
    public RectTransform timerBar;
    public GameObject stopScreen;
//    public Transform ;
    // Start is called before the first frame update
    void Start()
    {
        board = Board.instance;
        board.timerScript = this;
    }
    public void MinusTime(int stop)
    {
        timer -= k * stop;
    }
    void Update()
    {
        timer -= Time.deltaTime;
    //    if (timer < 0) timer = maxTimer;
        timerBar.localScale = new Vector3(Mathf.Clamp01(timer/ maxTimer),1f,1f);
        if (timer < 0)
            stopScreen.SetActive(true);

//      Debug.Log(timerBar.localScale);
    }
}
