using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GetColorBySpite : MonoBehaviour
{
    public Image img;
    public Sprite[] currentColorSprite;
    Color color;
    public bool Car;
    public ColorCard ColorCard;
    // Start is called before the first frame update
    void Start()
    {
        SetColor();
    }

 public void SetColor()
    {

             if(Car==true)
             {
             color=img.color;
             } 
             else
             {
              color=ColorCard.currentColor;
             }         
            if(color== new Color(254f/255f, 32f/255f, 43f/255f, 1f))
            {
            img.sprite = currentColorSprite[0];
            }
            if(color==  new Color(1f, 151f/255f, 32f/255f, 1f))
            {
            img.sprite = currentColorSprite[1];
            }
            if(color==new Color(1f, 244f/255f, 32f/255f, 1f))
            {
            img.sprite = currentColorSprite[2];
            }
            if(color==new Color(44f/255f, 243f/255f, 91f/255f, 1f))
            {
            img.sprite = currentColorSprite[3];
                      
            }
            if(color==new Color(0f, 1f, 241f/255f, 1f))
            {
            img.sprite = currentColorSprite[4];
                      
            }
            if(color==new Color(0f, 0f, 254f/255f, 1f))
            {
            img.sprite = currentColorSprite[5];
                    
            }
            if(color==new Color(189f/255f, 97f/255f, 255f/255f, 1f))
            {
            img.sprite = currentColorSprite[6];
                   
            }
             if(color==new Color(1f, 1f, 1f, 1f))
            {
            img.sprite = currentColorSprite[7];
                   
            }
             if(color== new Color(0.15f, 0.15f, 0.15f, 1f))
            {
            img.sprite = currentColorSprite[8];
                   
            }
           if(color==new Color(0.3019608f, 0.1333333f, 0.05490196f, 1f))
            {
            img.sprite = currentColorSprite[9];
                   
            }
            img.color=new Color(1f, 1f, 1f, 1f);
            
        
    
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.position.x!=0)
        {
            if(Car==false)
            {
            SetColor();
            }
        }
    }
}
