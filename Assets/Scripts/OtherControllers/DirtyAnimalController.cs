using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirtyAnimalController : MonoBehaviour
{
    public int id1;
    public Image mud;
    public Image foam;
    public ParticleSystem sparks;
    public float sparklesDuration = 1f;
    public RectTransform rectTransform;

    public float _mudProgress = 0;
    public float _foamProgress = 0;
    private Color _mudColor;
    private Color _foamColor;

    public IntDelegate onMouseEnter = (_) => { };
    public IntDelegate onMouseExit = (_) => { };
    public IntDelegate onMouseHover = (_) => { };

    private void Start()
    {
        sparks.gameObject.SetActive(false);
    }

    public float MudProgress
    {
        get
        {
            return _mudProgress;
        }
        set
        {
            _mudProgress = Mathf.Clamp(value, 0f, 1f);
            //print("_mudProgress:" + _mudProgress);
            //print("1f / _mudProgress:" + (1f - _mudProgress));
            _mudColor = mud.color;
            _mudColor.a = _mudProgress == 0 ? 0 : 1 - _mudProgress;
            mud.color = _mudColor;
        }
    }
    public float FoamProgress
    {
        get
        {
            return _foamProgress;
        }
        set
        {
            _foamProgress = Mathf.Clamp(value, 0f, 1f);
            _foamColor = foam.color;
            _foamColor.a = _foamProgress;
            foam.color = _foamColor;
        }
    }

    public void ShowSparkles()
    {
        StartCoroutine(Sparkles());
    }

    private IEnumerator Sparkles()
    {
        sparks.gameObject.SetActive(true);
        sparks.emissionRate = 10f;
        yield return new WaitForSeconds(sparklesDuration);
        sparks.emissionRate = 0;
        yield return new WaitForSeconds(sparks.startLifetime);
        sparks.gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        onMouseEnter(id1);
    }
    private void OnMouseExit()
    {
        onMouseExit(id1);
    }
    private void OnMouseOver()
    {
        onMouseHover(id1);
    }
}
