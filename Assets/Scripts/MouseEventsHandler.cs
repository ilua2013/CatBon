using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void IntDelegate(int a);

public class MouseEventsHandler : MonoBehaviour
{
    public int id1;
    public IntDelegate onMouseEnter = (_) => { };
    public IntDelegate onMouseExit = (_) => { };
    public IntDelegate onMouseHover = (_) => { };
    public IntDelegate onMouseDown = (_) => { };
    public IntDelegate onMouseDrag = (_) => { };

    private void OnMouseDown()
    {
        onMouseEnter(id1);
    }
    private void OnMouseEnter()
    {
        onMouseEnter(id1);
    }
    private void OnMouseExit()
    {
        onMouseExit(id1);
    }
    private void OnMouseDrag()
    {
        onMouseDrag(id1);
    }
    private void OnMouseOver()
    {
        onMouseHover(id1);
    }

}
