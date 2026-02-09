using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButterflyController : MonoBehaviour
{
    public float animationDuration = 0.5f;
    public float movingSpeed = 10f;
    public float randomFactor = 0.1f;
    public float showhideAnimationDuration = 0.3f;
    public bool isFlying = true;
    public bool isAnimating = true;
    public bool autoShow = true;

    public RectTransform borders;
    public Image body;
    public Sprite[] animationSprites;
    public RectTransform rectTransform;
    public Button button;

    public FloatingAlpha floatingAlpha;

    private Vector3 _currPoint;
    private IEnumerator showHideRoutine;

    [HideInInspector] public int id1 = -1;
    [HideInInspector] public int id2 = -1;

    private float _animTime = 0;
    private int _currFrame = 0;

    void Start()
    {
        body.rectTransform.localScale = Vector3.zero;
        movingSpeed += Random.Range(movingSpeed * randomFactor, movingSpeed * randomFactor);
        animationDuration += Random.Range(-animationDuration * randomFactor/10, animationDuration * randomFactor/10);
        body.sprite = animationSprites[0];
        GenerateNextPoint();
        if (autoShow) Show();
    }

    
    void Update()
    {
        if (isAnimating)
        {
            _animTime += Time.deltaTime;
            if (_animTime >= animationDuration)
            {
                _animTime = 0f;
                _currFrame++;
                if (_currFrame >= animationSprites.Length)
                {
                    _currFrame = 0;
                }
                body.sprite = animationSprites[_currFrame];
            }
        }
        if (isFlying)
        {
            transform.position = Vector3.MoveTowards(transform.position, _currPoint, movingSpeed * Time.deltaTime);
            if (Vector3.SqrMagnitude(transform.position - _currPoint) < 1f)
            {
                GenerateNextPoint();
            }
        }
    }


    public void SetColor(Color color)
    {
        body.color = color;
    }

    private void GenerateNextPoint()
    {
        Vector3 temp = new Vector3
            (
                Random.Range(borders.position.x - borders.rect.width * borders.lossyScale.x / 2f, borders.position.x + borders.rect.width * borders.lossyScale.x / 2f),
                Random.Range(borders.position.y - borders.rect.height * borders.lossyScale.x / 2f, borders.position.y + borders.rect.height * borders.lossyScale.x / 2f),
                transform.position.z
            );
        SetDirection(temp.x > transform.position.x);
        _currPoint = temp;
       
    }
    private void SetDirection(bool right)
    {
        if(right)
        {
            body.rectTransform.localRotation = Quaternion.Euler(Vector3.forward * -20f);
        }
        else
        {
            body.rectTransform.localRotation = Quaternion.Euler(Vector3.forward * 20f);
        }
    }

    public void Show()
    {
        Global.RenewCoroutine(this, ref showHideRoutine, ShowHide(true));
        isAnimating = true;
        isFlying = true;
    }
    public void Hide(float delay = 0)
    {
        Global.RenewCoroutine(this, ref showHideRoutine, ShowHide(false, delay));
    }
    public void Stop()
    {
        isFlying = false;
    }
    public void Go()
    {
        isFlying = true;
    }

    private IEnumerator ShowHide(bool show, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        if(show)
        {
            isAnimating = true;
            isFlying = true;
        }
        Vector3 target = show ? Vector3.one : Vector3.forward;
        Vector3 start = body.rectTransform.localScale;
        float t = 0;
        float progress;
        while (t < showhideAnimationDuration)
        {
            t += Time.deltaTime;
            progress = t / showhideAnimationDuration;
            body.rectTransform.localScale = Vector3.Lerp(start, target, progress);
            yield return null;
        }
        if(!show)
        {
            isAnimating = false;
            isFlying = false;
        }
        yield break;
    }

    public void Select()
    {
        floatingAlpha.StartFloating(0f);
    }
    public void Deselect()
    {
        floatingAlpha.StopFloating();
    }
}
