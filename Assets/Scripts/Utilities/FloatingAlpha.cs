using UnityEngine;
using UnityEngine.UI;

public class FloatingAlpha : MonoBehaviour
{
    public float speed = 1f;
    public Image image;
    public bool isFloating = false;
    private float currOffset;
    private Color c;
    //public float startAlpha;
    private void Awake()
    {
        if (!image) image = gameObject.GetComponent<Image>();
    }
    private void Start()
    {
        
        c = image.color;
        //StartFloating(startAlpha);
    }
    public void StartFloating(float startAlpha)
    {
        currOffset = Mathf.Asin(Mathf.Lerp(-1f, 1f, startAlpha)) - (Time.time * speed);
        isFloating = true;
    }
    public void StopFloating()
    {
        isFloating = false;
        SetAlpha(0f);
    }

    private void Update()
    {
        if (isFloating)
        {
            c.a = (Mathf.Sin(Time.time * speed + currOffset) + 1f) / 2f;
            image.color = c;
        }
    }


    public void SetAlpha(float alpha)
    {
        c = image.color;
        c.a = alpha;
        image.color = c;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
}
