using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColorCard : MonoBehaviour
{
    public Image[] colorImages;
    public Image[] otherImages;

    public Color defaultColor;
    public Color currentColor;

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

    public void SetColor(Color color)
    {
        currentColor = color;
        for (int i = 0; i < colorImages.Length; i++)
        {
            colorImages[i].color = color;
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
