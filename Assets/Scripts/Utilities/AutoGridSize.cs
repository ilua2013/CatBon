using UnityEngine;
using UnityEngine.UI;

public class AutoGridSize : MonoBehaviour
{
    public GridLayoutGroup gLg;
    public int rows;
    public int columns;


    private void Start()
    {
        Refresh();
        //Global.DelayedAction(300, Refresh);
    }
    public void Refresh()
    {
        RectTransform rt = gLg.transform as RectTransform;
        float width = (rt.rect.width - gLg.spacing.x * columns - gLg.padding.left - gLg.padding.right) / (float)columns;
        float height = (rt.rect.height - gLg.spacing.y * rows - gLg.padding.top - gLg.padding.bottom) / (float)rows;
        Canvas.ForceUpdateCanvases();
        gLg.cellSize = new Vector2(width, height);
        gLg.gameObject.SetActive(!gLg.gameObject.activeSelf);
        gLg.gameObject.SetActive(!gLg.gameObject.activeSelf);

    }

}
