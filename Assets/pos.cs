using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pos : MonoBehaviour
{
    public Transform pos_girl;
    // Start is called before the first frame update
    public void Get()
    {
        pos_girl.position=new Vector2(0,pos_girl.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
