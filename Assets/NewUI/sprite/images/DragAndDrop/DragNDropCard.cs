using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void Vect2Delegate(Vector2 delta);
public class DragNDropCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool setDefaultPosOnStart = true;
    public bool draggable = true;
public RectTransform C_Parent;
    public float goBackDuration = 1f;
    public float scaleDuration = 1f;
    public float rotationDuration = 1f;
    public Quaternion defaultRotation;
    private Vector2 _defaultAnchoredPosition;
    public Vector2 defaultAnchoredPosition
    {
        get
        {
            return _defaultAnchoredPosition;
        }
        set
        {
            //Debug.Log("_defaultAnchoredPosition", gameObject);
            _defaultAnchoredPosition = value;
        }
    }
    public Vector3 defaultScale;
    public Vector3 tempPosition;
    public Transform defaultParent;
    public RectTransform newParent;
    public RectTransform rectTransform;
    private bool _isDragging = false;
    public bool resetSiblingOnDefaultPosition = true;

    public bool isDragging
    {
        get { return _isDragging; }
        set
        {
            _isDragging = value;
        }
    }
    public Vect2Delegate onDrag = (_) => { };
    public RectTransform pointer;

    private int siblingIndex = 0;
    private CancellationTokenSource source = new CancellationTokenSource();
    private CancellationTokenSource source1 = new CancellationTokenSource();
    private CancellationTokenSource source2 = new CancellationTokenSource();
    private Vector3 prevPos;

    public int id1 = -1;
    public int id2 = -1;
    public int id3 = -1;
    public int id4 = -1;

    public bool zoomOnStartDrag = true;
    public Vector3 scaleOnStartDrag = new Vector3(1.25f, 1.25f, 1.25f);


    private void Awake()
    {
        if (!rectTransform) rectTransform = transform as RectTransform;
    }



    public void GoToDefaultPosition()
    {
        _GoToDefaultPoint(Global.RenewCT(ref source));
    }
    public void GoToPosition(Vector2 position, System.Action callback = null, float duration = -1)
    {
        //Debug.Log("GoToPosition");
        _GoToPosition(Global.RenewCT(ref source), position, callback, duration);
    }
    public void SetScale(Vector3 newScale, System.Action callback = null)
    {
        _SetScale(Global.RenewCT(ref source1), newScale, callback);
    }
    public void SetDefaultScale(Action callback = null)
    {
        SetScale(defaultScale, callback);
    }
    public void SetRotation(Quaternion newRotation)
    {
        _SetRotation(Global.RenewCT(ref source2), newRotation);
    }
    public void SetDefaultRotation()
    {
        SetRotation(defaultRotation);
    }

    public void SetAllDefault()
    {
        SetDefaultRotation();
        SetDefaultScale();
        GoToDefaultPosition();
    }


    private async void _GoToDefaultPoint(CancellationToken ct) //+
    {
        //Debug.Log("_GoToDefaultPoint");
        float t = 0;
        float progress = 0;
        Vector2 startPosition = rectTransform.anchoredPosition;
        while (progress < 1f && !ct.IsCancellationRequested && !isDragging)
        {
            t += Global.DELAY_120FPS / 1000f;
            progress = t / goBackDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, defaultAnchoredPosition, progress);
            if (progress >= 1f)
            {
                
                OnArrivedToDefaultPosition();
            }
            await Task.Delay(Global.DELAY_120FPS, ct).ContinueWith(_ => { });
        }
    }
    private async void _GoToPosition(CancellationToken ct, Vector3 position, System.Action callback, float duration = -1) //+
    {
        float t = 0;
        float progress = 0;
        float d = duration > 0 ? duration : goBackDuration;
        Vector3 startPosition = rectTransform.position;
        while (progress < 1f && !ct.IsCancellationRequested && !isDragging)
        {
            t += Global.DELAY_120FPS / 1000f;
            progress = t / d;
            rectTransform.position = Vector3.Lerp(startPosition, position, progress);
            await Task.Delay(Global.DELAY_120FPS, ct).ContinueWith(_ => { });
        }
        if (!ct.IsCancellationRequested)
        {

            rectTransform.position = position;
            if (callback != null) callback.Invoke();
        }
    }
    private async void _SetScale(CancellationToken ct, Vector3 newScale, System.Action callback)
    {
        //Debug.Log("_SetScale");
        float t = 0;
        float progress = 0;
        while (progress < 1f && !ct.IsCancellationRequested)
        {
            t += Global.DELAY_120FPS / 1000f;
            progress = t / scaleDuration;
            rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, newScale, progress);
            await Task.Delay(Global.DELAY_120FPS, ct).ContinueWith(_ => { });
        }
        if (callback != null && !ct.IsCancellationRequested)
        {
            rectTransform.localScale = newScale;
            callback();
        }
    }
    private async void _SetRotation(CancellationToken ct, Quaternion newRotation)//+
    {
        //Debug.Log("_SetRotation");
        float t = 0;
        float progress = 0;
        while (progress < 1f && !ct.IsCancellationRequested)
        {
            t += Global.DELAY_120FPS / 1000f;
            progress = t / rotationDuration;
            rectTransform.rotation = Quaternion.Slerp(rectTransform.rotation, newRotation, progress);
            await Task.Delay(Global.DELAY_120FPS, ct).ContinueWith(_ => { });
        }
        //Debug.Log("CALLBACK 2 ");
        if (!ct.IsCancellationRequested)
        {
            //Debug.Log("CALLBACK");
            rectTransform.rotation = newRotation;
        }
    }

    private void OnArrivedToDefaultPosition()
    {
        if(resetSiblingOnDefaultPosition) rectTransform.SetSiblingIndex(siblingIndex);
        SetScale(defaultScale);
        SetRotation(defaultRotation);
    }
    private void StartDrag()
    {
        if (!draggable) return;
        //print("StartDrag");
        transform.SetParent(defaultParent);
        isDragging = true;
        DragAndDropManager.Instance.OnDragStarted(this);
        rectTransform.SetAsLastSibling();
        if (zoomOnStartDrag) SetScale(scaleOnStartDrag);
        SetRotation(Quaternion.identity);
    }
    public void StopDrag()
    {
        //print("StopDrag");
        if (!draggable) return;
        isDragging = false;
        DragAndDropManager.Instance.OnDragEnded(this);
        //GoToDefaultPosition();
    }
    private IEnumerator WaitForFirstFrame(System.Action action)
    {
        yield return new WaitForEndOfFrame();
        action();
    }

    #region EVENTS
    public void Start()
    {
        siblingIndex = rectTransform.GetSiblingIndex();
        if(setDefaultPosOnStart) defaultAnchoredPosition = rectTransform.anchoredPosition;
        defaultScale = rectTransform.localScale;
        defaultRotation = rectTransform.rotation;
        defaultParent = transform.parent;
        prevPos = transform.position;
    }
    void Update()
    {
        if (isDragging)
        {
            //print("isDragging");
            transform.position = Input.mousePosition;
            onDrag(transform.position - prevPos);
            prevPos = transform.position;
        }
    }
    private void OnDisable()
    {
        Global.RenewCT(ref source);
        Global.RenewCT(ref source1);
        Global.RenewCT(ref source2);
    }
    private void OnMouseDown()
    {
        StartDrag();
    }
    private void OnMouseUp()
    {
        StopDrag();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        StartDrag();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        StopDrag();
    }
    #endregion
}
