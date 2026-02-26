using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoapController : MonoBehaviour
{
    public int id;
    public SpriteRenderer image;
    public RectTransform rectTransform;
    public Camera camera1;
    public ParticleSystem particles;
    Vector3 t;
    ParticleSystem.EmissionModule emissionModule;
    public float baseEmission = 1f;
    public bool draggable = true;
    public Collider2D collider2;
    public float hideSpeed = 0.02f;

    private bool _hided = false;
    public IntDelegate onMouseDown = (_) => { };
    public IntDelegate onMouseUp = (_) => { };

    public bool isDragging = false;

    private void Update()
    {
        if (_hided) return;
        if(Input.GetMouseButtonUp(0))
        {
            if(isDragging)
            {
                onMouseUp(id);
                collider2.enabled = true;
            }
            isDragging = false;
        }
        if(isDragging)
        {
            if (draggable)
            {
                t = camera1.ScreenToWorldPoint(Input.mousePosition);
                t.z = -1f;
                transform.position = t;
            }
        }
    }

    private void Awake()
    {
        emissionModule = particles.emission;
        emissionModule.rateOverDistanceMultiplier = baseEmission;
    }
    

    public void SetEmissionMultiplier(float multiplier)
    {
        emissionModule.rateOverDistanceMultiplier = multiplier * baseEmission;
    }

    private void OnMouseDown()
    {
        collider2.enabled = false;
        isDragging = true;
        onMouseDown(id);
    }

    public void Show()
    {
        _hided = false;
        Color c = image.color;
        c.a = 1f;
        image.color = c;
        collider2.enabled = true;
    }

    public void Hide()
    {
        _hided = true;
        collider2.enabled = false;
        StartCoroutine(_Hide());
    }

    private IEnumerator _Hide()
    {
        Color c = image.color;
        while (c.a > 0)
        {
            c.a -= hideSpeed;
            image.color = c;
            yield return null;
        }
        yield break;
    }
}
