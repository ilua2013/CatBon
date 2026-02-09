using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DreeRandom : MonoBehaviour
{
    public GameObject[] doll;
    public AudioSource [] a_s;
    public CatHelper[] CatHelper;
    int r;
    public static int n;
    void Start()
    {
        Invoke("Play",0.03f);
        Invoke("Play_Audio",3f);
   
          Debug.Log("N"+n);
    }
    // Start is called before the first frame update
    
   public void Play()
    {
        r=Random.Range(0,2);
       
        doll[r].SetActive(true);
        doll[3].SetActive(true);
        CatHelper[r].ShowTextT("Куклам пора на бал. Давай поможем им собраться!");
    }
    public void Play_Audio()
    {
         
       a_s[r].Play();

    }
    public void nn()
    {
        n=n+1;
    }
    public void LoadN()
    {
        
         SceneManager.LoadSceneAsync("4_2_MakeUpTheDoll"); 
          CancelInvoke(); 
    }
    public void LoadE()
    {    
        n=0;
        print($"LoadMenu {gameObject.name}. 1");
         SceneManager.LoadScene("MainMenu");
    }
    public void Next()
    {
        Debug.Log(n);
        if(n>2)
        {  
          
         CatHelper[4].ShowText("Молодец!Попробуй другую игру.");
         Invoke("LoadE",3f);
         a_s[4].Play();
              
         
      
        }
        else
        {  
        
       
        CatHelper[4].ShowText("Молодец! Идём дальше."); 
        Invoke("LoadN",2f);
        a_s[3].Play();
       
       
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
