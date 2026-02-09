using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    public Camera cam;
    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown!!!!!!!!");
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("PHYSICS RAYCAST!!!!!");
            }
        }
    }
}
