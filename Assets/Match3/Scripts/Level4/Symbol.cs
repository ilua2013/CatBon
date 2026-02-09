using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Symbol : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    Board _board;
    const int typesCount = 5;
    public int type;
    public Vector2Int cellPosition;
    bool _touched=false;
    int _touchID;
    Vector2 _lastPosition;
    const float distanceZ = -5f;
    private Vector3 _screenPoint;
    private Vector3 _offset;

    private void Start()
    {
        _lastPosition = transform.position;
        _board = Board.instance;
    }
    public void DestrEffect()
    {
        StartCoroutine(DownSizing());
/*        if(board.destrEffect)
            Instantiate(board.destrEffect, transform.position, Quaternion.identity,board.transform);*/
    }
    IEnumerator DownSizing()
    {
        var bazeSize = transform.localScale;
        var startTime = 0.35f;
        var time = startTime;
        while (time>0)
        {
            time -= Time.deltaTime;
            transform.localScale = bazeSize * (time / startTime);
            yield return null;
        }
        Destroy(gameObject);
    }
    public void ChangePosition(Vector2Int v)
    {
        StartCoroutine(Slide(v));
    }
    public void FastChangePosition(Vector2Int v)
    {
        transform.position = new Vector2(_lastPosition.x + v.x * 1.2f, _lastPosition.y + v.y * 1.2f);
        _lastPosition = transform.position;
    }
    IEnumerator Slide(Vector2Int v) //для всех движений, непривязанных к мыши
    {
        float learpTime;
        if (v.y < 2) learpTime = 0.3f;
        else learpTime=0.3f * v.y;
        Vector2 startPosition = transform.position;
        Vector2 finalPosition = new Vector2(transform.position.x + v.x * 1.2f, transform.position.y + v.y*1.2f);
        for (float t = 0; t < learpTime; t += Time.deltaTime)
        {
            transform.position=Vector2.Lerp(startPosition, finalPosition, t/learpTime);
            yield return null;
        }
        transform.position = finalPosition;
        _lastPosition = finalPosition;
    }

    public void Shifting(Vector2 shiftVector)
    {
        transform.position = _lastPosition - shiftVector;
    }
    public void ResetPosition()
    {
        transform.position = _lastPosition;
    }
    #region Drag&Drop

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Board.moving || _board.touched) return;
        _touchID = eventData.pointerId;
        _touched = true;
        _board.touched = true;
        _screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, distanceZ));
    }


    Vector3 curScreenPoint;
    Vector3 curPosition;
    float tempShifting;
    Shift shift;
    public void OnDrag(PointerEventData eventData)
    {
        if (_touchID != eventData.pointerId) return;
        if (Board.moving)
        {
            return;
      /*      transform.position = _lastPosition;
            _touched = false;*/

        }
        
        curScreenPoint = new Vector3(eventData.position.x, eventData.position.y, distanceZ);
        curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;

        if (Mathf.Abs(_lastPosition.x - curPosition.x) > Mathf.Abs(_lastPosition.y - curPosition.y))
        {
            tempShifting = Mathf.Clamp(curPosition.x, _lastPosition.x - 1.2f, _lastPosition.x + 1.2f);
            transform.position = new Vector3(tempShifting, _lastPosition.y, distanceZ);
            if (_lastPosition.x - tempShifting < 0) shift = Shift.left;
            else shift = Shift.right;
            _board.LightChange(this, _lastPosition.x-tempShifting, shift);
        }
        else
        {
            tempShifting = Mathf.Clamp(curPosition.y, _lastPosition.y - 1.2f, _lastPosition.y + 1.2f);
            transform.position = new Vector3(_lastPosition.x, tempShifting, distanceZ);
            if (_lastPosition.y - tempShifting < 0) shift = Shift.down;
            else shift = Shift.up;
            _board.LightChange(this, _lastPosition.y - tempShifting, shift);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_touchID != eventData.pointerId) return;
        if (Board.moving)
        {
            return;
        /*    transform.position = _lastPosition;
            _touched = false;*/

        }
        if (_touched)
        {
            _board.touched = false;
            _touched = false;
            if (Mathf.Abs(_lastPosition.x - transform.position.x) < 0.6f && Mathf.Abs(_lastPosition.y - transform.position.y) < 0.6f)
            {
                transform.position = _lastPosition;
                _board.ResetLastSymbol();
                return;
            }
            Vector2Int v;
            if (Mathf.Abs(_lastPosition.x - transform.position.x) > Mathf.Abs(_lastPosition.y - transform.position.y))
            {
                if (_lastPosition.x > transform.position.x) v = new Vector2Int(-1, 0);
                else v = new Vector2Int(1, 0);
            }
            else
            {
                if (_lastPosition.y > transform.position.y) v = new Vector2Int(0, -1);
                else v = new Vector2Int(0, 1);
            }
            _board.ResetLastSymbol();
            if (_board.Change(this, v))
            {
                transform.position = _lastPosition + new Vector2(v.x * 1.2f, v.y * 1.2f);
                _lastPosition = transform.position;
                transform.position += new Vector3(0, 0, -1); //чтобы передвинутый символ не загораживался символом, с которым его поменяли местами
            }
            else
            {
                transform.position = _lastPosition;
            }

        }
    }
    #endregion
}
