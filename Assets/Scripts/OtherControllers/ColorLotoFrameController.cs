using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLotoFrameController : MonoBehaviour
{
    public RectTransform[] columns;
    public FloatingAlpha floatingAlpha;
    public void SetColumn(int id)
    {
        transform.position = new Vector3(columns[id].position.x, transform.position.y, transform.position.z);
        Show();
    }

    public void Show()
    {
        floatingAlpha.StartFloating(0f);
    }
    public void Hide()
    {
        floatingAlpha.StopFloating();
    }
}
