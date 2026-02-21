using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    public MainMenu2 mainMenu2;
    public Animator animator;
    public Button skipperButton;

    public Image background;
    public Image car;
    public Image logo;

    public Sprite dayBack;
    public Sprite nightBack;
    public Sprite dayCar;
    public Sprite nightCar;
    public Sprite dayLogo;
    public Sprite nightLogo;
    public CanvasFader canvasFader;

    private IEnumerator routine;
    // Start is called before the first frame update
    void Start()
    {
        if(Global.appFirstStart)
        {
            if (System.DateTime.Now.Hour > 20)
            {
                background.sprite = nightBack;
                car.sprite = nightCar;
                logo.sprite = nightLogo;
            }
            else
            {
                background.sprite = dayBack;
                car.sprite = dayCar;
                logo.sprite = dayLogo;
            }
            
            mainMenu2.HideSubscribeWindow();
            Global.RenewCoroutine(this, ref routine, DelayedSkip());
        }
        else
        {
            gameObject.SetActive(false);
        } 
    }

    public void Hide()
    {
        skipperButton.interactable = false;
        StopCoroutine(routine);
        canvasFader.Out(() => { gameObject.SetActive(false); });
        if (Global.appFirstStart) animator.SetTrigger("Out");
        Global.appFirstStart = false;
        mainMenu2.ShowSubscribeWindow();
        //mainMenu2.PlayHello();
    }
    private IEnumerator DelayedSkip()
    {
        yield return new WaitForSeconds(3f);
        Hide();
    }
}
