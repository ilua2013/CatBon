using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColorCard : MonoBehaviour
{
    public Image[] colorImages;
    public Image[] otherImages;
    public Image img;
    public Color defaultColor;
    public Color currentColor;
    public Sprite[] currentColorSprite;
    public bool spr;
    public RectTransform rectTransform;

    public float animationDuration = 0.3f;
    public AnimationCurve animationCurve;

    public Vector3 defaultScale;

    private IEnumerator routine;
    private Button button;

    public int id1 = -1;
    public int id2 = -1;
    public int id3 = -1;
    public int id4 = -1;


    private void Awake()
    {
        if (!rectTransform) rectTransform = transform as RectTransform;
    }

    private void Start()
    {
        if (colorImages.Length > 0)
        {
            defaultColor = colorImages[0].color;
        }
        else
        {
            defaultColor = new Color(1f, 1f, 1f, 1f);
        }
        defaultScale = transform.localScale;
        button = GetComponent<Button>();
        if (button) button.onClick.AddListener(new UnityEngine.Events.UnityAction(PushAnimation));
    }
    /* 
        new Color(254f/255f, 32f/255f, 43f/255f, 1f), // red
        new Color(1f, 151f/255f, 32f/255f, 1f), // orange
        new Color(1f, 244f/255f, 32f/255f, 1f), // yellow
        new Color(44f/255f, 243f/255f, 91f/255f, 1f), // green
        new Color(0f, 1f, 241f/255f, 1f), // light blue
        new Color(0f, 0f, 254f/255f, 1f), // blue
        new Color(189f/255f, 97f/255f, 255f/255f, 1f), // purple
        new Color(1f, 1f, 1f, 1f), // white
        new Color(0.15f, 0.15f, 0.15f, 1f), // black
        */
    public void SetColor(Color color)
    {
        currentColor = color;
        for (int i = 0; i < colorImages.Length; i++)
        {    
            if(spr)
            {
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
                 
            if(color== new Color(0.9433962f,0.4049484f, 0.6929883f, 1f))
            {
            img.sprite = currentColorSprite[9];
                   
            }
            if(color== new Color(0.6603774f, 0.6510f, 0.6510f, 1f))
            {
            img.sprite = currentColorSprite[10];
                   
            }
            if(color==new Color(0.3019608f, 0.1333333f, 0.05490196f, 1f))
            {
               img.sprite = currentColorSprite[11]; 
            }
            
        }
            else
            {
            colorImages[i].color = color;
            }
        
    }
    }
    public void SetSprite(int i)
    {
        if(spr)
        {
        //for (int i = 0; i < colorImages.Length; i++)
        {
            img.sprite = currentColorSprite[i];
        }
        }
    }
    public void SetDefaultColor()
    {
        for (int i = 0; i < colorImages.Length; i++)
        {
            colorImages[i].color = defaultColor;
        }
    }

    public void SetColorImage(int id, Sprite sprite)
    {
        colorImages[id].sprite = sprite;
    }

    public void SetOtherImage(int id, Sprite sprite)
    {
        otherImages[id].sprite = sprite;
    }

    public void PushAnimation()
    {
        if (routine != null) StopCoroutine(routine);
        routine = _PushAnimation();
        StartCoroutine(routine);
    }

    private IEnumerator _PushAnimation()
    {
        float t = 0;
        while (t < animationDuration)
        {
            t += Time.deltaTime;
            transform.localScale = defaultScale * animationCurve.Evaluate(t / animationDuration);
            yield return null;
        }
    }


}
