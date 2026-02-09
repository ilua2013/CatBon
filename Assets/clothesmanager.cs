using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clothesmanager : MonoBehaviour
{
    public GameObject[] gm;
    
    // Start is called before the first frame update
    public void get(int i)
    {
    foreach(Transform child in transform) 
    {
    child.gameObject.SetActive(false);
   }
   gm[i].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
