using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectrandon : MonoBehaviour
{
    public bool select;
    public GameObject fath;
    public GameObject s_r;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void selected()
    {
for (int i = 0; i< fath.transform.childCount; i++)
{
    if(fath.transform.GetChild(i).gameObject.activeSelf==true)
    {
        select= true;
    }

    
}

if(select==false)
{
s_r.SetActive(true);
}
}
}
