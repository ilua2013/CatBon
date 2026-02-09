using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WashTheAnimal_4_3_Controller : BaseController<WashTheAnimal_4_3_Controller>
{
    public int maxLevels = 3;
    public RectTransform tableSlot;
    public RectTransform animalSlot;
    public SoapController soap;
    public SoapController brush;
    public ShowerController shower;
    public DirtyAnimalController[] animals;

    private int _currLevel = 0;
    private DirtyAnimalController _currAnimal;
    private int _stage = 0;

    public float soapProgressSpeed = 0.01f;
    public float brushProgressSpeed = 0.01f;
    public float showerProgressSpeed = 0.01f;
    private bool _soapInHand = false;
    private bool _brushInHand = false;
    private bool _showerInHand = false;
    private Vector2 _prevMousePosition;
    private float _mouseSpeed = 0;
    private float _preStartDelay = 0f;

    [Header("Cat phrases")]
    public string rulesText;
    public string winText;
    public string nextLevelText;
    public string brushText;
    public string showerText;

    [Header("Audio")]
    public AudioClip[] startAudio1;
    public AudioClip[] brushSound;
    public AudioClip[] showerSound;


    private void Start()
    {
        animals.Shuffle();
        _prevMousePosition = Input.mousePosition;
        soap.onMouseDown = OnSoapDown;
        soap.onMouseUp = OnSoapUp;
        brush.onMouseDown = OnBrushDown;
        brush.onMouseUp = OnBrushUp;
        shower.onMouseDown = OnShowerDown;
        shower.onMouseUp = OnShowerUp;
        StartCoroutine(Controller());
    }

    private void Update()
    {
        _mouseSpeed = Vector2.Distance(Input.mousePosition, _prevMousePosition);
        _prevMousePosition = Input.mousePosition;
    }

    private IEnumerator Controller()
    {
        if (_stage == 0) // показываем правила, генерим всё что нужно
        {
            CatHelper.Instance.ShowText(rulesText, 5f);
            CatHelper.Instance.PlayAudio(startAudio1);
            _stage = 1;
        }
        
        yield return new WaitForSeconds(_preStartDelay);
        GenerateAnimal();
        if (_stage == 1) // намылить мылом
        {
            soap.SetEmissionMultiplier(0);
            brush.SetEmissionMultiplier(0);
            soap.gameObject.SetActive(true);
            soap.Show();
            soap.rectTransform.SetParent(tableSlot, false);
            soap.rectTransform.localPosition = Vector3.zero;
            while (_currAnimal.FoamProgress < 1)
            {
                yield return null;
            }
            _soapInHand = false;
            _stage = 2;
        }


        if (_stage == 2) // почистить щёткой
        {
            CatHelper.Instance.ShowText(brushText, 4f);
            CatHelper.Instance.PlayAudio(brushSound);
            soap.Hide();
            brush.gameObject.SetActive(true);
            brush.Show();
            brush.rectTransform.SetParent(tableSlot, false);
            brush.rectTransform.localPosition = Vector3.zero;
            while (_currAnimal.MudProgress < 1)
            {
                yield return null;
            }
            _brushInHand = false;
            _stage = 3;
        }


        if (_stage == 3) // смыть душем
        {
            CatHelper.Instance.ShowText(showerText, 3f);
            CatHelper.Instance.PlayAudio(showerSound);
            brush.Hide();
            shower.gameObject.SetActive(true);
            shower.In(); 
            Invoke("OffWash",8f);
            while (_currAnimal.FoamProgress > 0)
            {
                if (_showerInHand)
                {
                    //print("_showerInHand");
                    ///_currAnimal.FoamProgress -= showerProgressSpeed;
                }
                yield return null;
            }
            _currAnimal.ShowSparkles();
           
            shower.Out();
            _stage = 4;
        }
        NextLevel();
        yield break;
    }
public void OffWash()
{
    _currAnimal.FoamProgress=-0.1f;
           _currAnimal.ShowSparkles();
    Debug.Log("+++++");
     shower.Out();
            _stage = 4;
}
    public void GenerateAnimal()
    {
        if (_currAnimal) _currAnimal.gameObject.SetActive(false);
        _currAnimal = animals[_currLevel];
        _currAnimal.gameObject.SetActive(true);
        _currAnimal.rectTransform.SetParent(animalSlot);
        _currAnimal.rectTransform.localPosition = Vector3.zero;
        _currAnimal.onMouseEnter = OnMouseEnterAnimal;
        _currAnimal.onMouseExit = OnMouseLeaveAnimal;
        _currAnimal.onMouseHover = OnMouseOverAnimal;
    }

    private void Win()
    {
        CatHelper.Instance.ShowText(winText);
        if (SoundMaster.Instance) SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit());
    }
    private void NextLevel()
    {
        _currLevel++;
        if(_currLevel == maxLevels)
        {
            Win();
        }
        else
        {
            CatHelper.Instance.ShowText(nextLevelText);
            if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
            _preStartDelay = 3f;
            Restart();
        }
    }
    public override void Restart()
    {
        _stage = 1;
        brush.isDragging = false;
        soap.isDragging = false;
        StartCoroutine(Controller());
    }

    public void OnShowerMouseDown(int id)
    {
        _showerInHand = true;
    }
    public void OnShowerMouseUp(int id)
    {
        _showerInHand = false;
    }

    public void OnMouseOverAnimal(int id)
    {
        if (_stage == 1)
        {
            if (_soapInHand)
            {
                _currAnimal.FoamProgress += soapProgressSpeed * _mouseSpeed;
            }
        }
        else if (_stage == 2)
        {
            if (_brushInHand)
            {
                _currAnimal.MudProgress += brushProgressSpeed;
            }
        }
    }
    public void OnMouseEnterAnimal(int id)
    {
        if (_soapInHand)
        {
            soap.SetEmissionMultiplier(1);
        }
        else if (_brushInHand)
        {
            //print("_brushInHand");
            brush.SetEmissionMultiplier(1);
        }
    }
    public void OnMouseLeaveAnimal(int id)
    {
        soap.SetEmissionMultiplier(0);
        brush.SetEmissionMultiplier(0);
    }

    public void OnSoapDown(int id)
    {
        _soapInHand = true;
    }
    public void OnSoapUp(int id)
    {
        _soapInHand = false;
    }
    public void OnBrushDown(int id)
    {
        _brushInHand = true;
    }
    public void OnBrushUp(int id)
    {
        _brushInHand = false;
    }
    public void OnShowerDown(int id)
    {
        _showerInHand = true;
    }
    public void OnShowerUp(int id)
    {
        _showerInHand = false;
    }
}
