using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Sprite_set : MonoBehaviour
{
    public SpriteRenderer spr;
    public Image img;
    public bool reverse;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  
        if(reverse==false)
        {
        img.sprite=spr.sprite;
        }
        else
        {
            spr.sprite=img.sprite;
        }
        
    }
}
