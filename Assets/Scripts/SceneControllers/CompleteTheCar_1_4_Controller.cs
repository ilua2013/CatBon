using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompleteTheCar_1_4_Controller : BaseController<CompleteTheCar_1_4_Controller>
{
    [Header("Settings")]
    public float goToPointDuration = 0.1f;

    [Header("Components")]
    public HorizontalScroll horizontalScroll;
    public CarHighlighter carHighlighter;
    public Camera cam;

    [Header("Parts IDs")]
    public int doorId;
    public int wheelsId;
    public int windowsId;
    public int frontLightId;
    public int backLightId;

    [Header("Parts")]
    public RectTransform[] cars;
    [Space]
    public RectTransform currentCar;
    public Image carBody;
    public RectTransform doorsArea;
    public RectTransform doorsPoint;
    public RectTransform windowsArea;
    public RectTransform windowsPoint;
    public RectTransform wheelsArea;
    public RectTransform[] wheelsPoints;
    public RectTransform frontLightArea;
    public RectTransform frontLightPoint;
    public RectTransform backLightsArea;
    public RectTransform backLightsPoint;


    [Header("Attached parts")]
    public bool hasDoors = false;
    public bool hasWindows = false;
    public bool hasWheels = false;
    public bool hasFrontLight = false;
    public bool hasBackLight = false;

    public Color[] colors;
    private Color mainColor;
    private int currentLevel = 0;

    [Header("Cat phrases")]
    public string rulesText;
    public string trueColorText;
    public string wrongColorText;
    public string wrongPartText;
    public string needTheDoorsFirstText;
    public string winText;
    public string nextLevelText;
    
    [Header("Audio")]
    public AudioClip[] startAudio;
    public AudioClip[] doorsAudio;
    public AudioClip[] windowsAudio;
    public AudioClip[] wheelsAudio;
    public AudioClip[] headlightAudio;
    public AudioClip[] taillightAudio;

    public float speed = 10f;

    #region EVENTS
    public void OnStopDragging(DragNDropCard dragNDropCard)
    {
        CheckPartRules(dragNDropCard);
        carHighlighter.StopAll();
        if (CheckCarIsCompleted())
        {
            NextLevel();
        }
    }

    public void OnStartDragging(DragNDropCard dragNDropCard)
    {
        ColorCard card = dragNDropCard.GetComponent<ColorCard>();
        if(card.id1 == doorId)
        {
            carHighlighter.HighlightPart(CarParts.Doors);
        }
        else if (card.id1 == windowsId)
        {
            carHighlighter.HighlightPart(CarParts.Windows);
        }
        if (card.id1 == wheelsId)
        {
            carHighlighter.HighlightPart(CarParts.Wheels);
        }
        if (card.id1 == frontLightId)
        {
            carHighlighter.HighlightPart(CarParts.Headlight);
        }
        if (card.id1 == backLightId)
        {
            carHighlighter.HighlightPart(CarParts.Taillight);
        }
    }
    private void Start()
    {
        Vector3 camPos = cam.transform.position;
        camPos.x = Screen.width / 2f;
        camPos.y = Screen.height / 2f;
        cam.transform.position = camPos;
        cam.orthographicSize = Screen.height / 2f;
        DragAndDropManager.Instance.onDragStart.AddListener(OnStartDragging);
        CatHelper.Instance.defaultText = rulesText;
        CatHelper.Instance.ShowText(rulesText);
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.PlayDefaultAudio();
        GenerateColors();
        Restart();
    }
    #endregion


    public override void Restart()
    {
        //GenerateColors();
        ResetCar();
        SelectCarBody();
        SelectMainColor();
        ColorizeCar();
        ResetAllParts();
        ColorizeAllParts();
        ActivatePartColliders(0);
        horizontalScroll.ShowCardsGroup(0);
        CatHelper.Instance.PlayAudio(doorsAudio, 1.8f);
    }
    public void NextLevel() // следующий уровень
    {
        CatHelper.Instance.ShowText(nextLevelText);
        
        currentLevel++;
        if (currentLevel > colors.Length - 1)
        {
            Win();
        }
        else
        {
            if (SoundMaster.Instance) SoundMaster.Instance.PlayNextLevel();
            StartCoroutine(_NextLevel());
        }
    }
    public void Win() // все машины собраны, конец игры
    {
        foreach (var item in horizontalScroll.availableCards)
        {
            foreach (var item2 in item.cards)
            {
                item2.GetComponent<DragNDropCard>().draggable = false;
            }
        }
        CatHelper.Instance.ShowText(winText);
        if (SoundMaster.Instance) SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit());
    }

    private IEnumerator _NextLevel()
    {
        yield return new WaitForSeconds(1f);
        while (currentCar.localPosition != Vector3.down * 2000)
        {
            currentCar.localPosition = Vector3.MoveTowards(currentCar.localPosition, Vector3.down * 2000, speed * Time.deltaTime);
            yield return null;
        }
        Restart();
        currentCar.localPosition = Vector3.down * 2000;
        while (currentCar.localPosition != Vector3.zero)
        {
            currentCar.localPosition = Vector3.MoveTowards(currentCar.localPosition, Vector3.zero, speed * Time.deltaTime);
            yield return null;
        }
        yield break;
    }
    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(2f);
        Restart();
    }


    private void GenerateColors() // генерация 9 рандомно расположенных цветов
    {
        colors = Global.GetRandomColors(9, 9).ToArray();
    }
    private void ResetCar() //сброс собранной машины
    {
        hasDoors = false;
        hasWheels = false;
        hasWindows = false;
        hasFrontLight = false;
        hasBackLight = false;
        if (wheelsArea != null)
        {
            Transform t = wheelsArea.GetChild(3);
            if (t) Destroy(t.gameObject);
        }
    }
    private void SelectCarBody() //выбор кузова машины и всех ее частей
    {
        if (currentCar) currentCar.gameObject.SetActive(false);
        currentCar = cars.Random();
        carBody = currentCar.Find("Body").GetComponent<Image>();
        doorsArea = currentCar.Find("Doors") as RectTransform;
        doorsPoint = doorsArea.GetChild(0) as RectTransform;
        wheelsArea = currentCar.Find("Wheels") as RectTransform;
        wheelsPoints = new RectTransform[] { wheelsArea.GetChild(0) as RectTransform, wheelsArea.GetChild(1) as RectTransform };
        frontLightArea = currentCar.Find("FrontLight") as RectTransform;
        frontLightPoint = frontLightArea.GetChild(0) as RectTransform;
        backLightsArea = currentCar.Find("BackLight") as RectTransform;
        backLightsPoint = backLightsArea.GetChild(0) as RectTransform;
        windowsArea = currentCar.Find("Windows") as RectTransform;
        windowsPoint = windowsArea.GetChild(0) as RectTransform;
        currentCar.gameObject.SetActive(true);
        carHighlighter = currentCar.Find("Selection").GetComponent<CarHighlighter>();
    }
    private void SelectMainColor() // выбор цвета машины
    {
        mainColor = colors[currentLevel];
        //carHighlighter.SetColor(new Color(1f - mainColor.r, 1f - mainColor.g, 1f - mainColor.b, 0f));
    }
    private void ColorizeCar() // покраска машины
    {
        carBody.color = mainColor;
    }
    private void ResetAllParts() // сбор всех частей машины
    {
        for (int i = 0; i < horizontalScroll.availableCards.Count; i++)
        {
            foreach (var item in horizontalScroll.availableCards[i].cards)
            {
                item.id4 = -1;
                item.GetComponent<DragNDropCard>().id4 = -1;
                item.gameObject.SetActive(false);
            }
        }
    }
    private void ColorizeAllParts() // покраска всех частей (один цвет - всегда цвет машины)
    {
        List<ColorCard> tCards;
        List<Color> tColors = new List<Color>(colors);
        tColors.Remove(mainColor);
        for (int i = 0; i < horizontalScroll.availableCards.Count; i++)
        {
            tCards = horizontalScroll.availableCards[i].cards;
            int randomMainColorIndex = Random.Range(0, tCards.Count);
            //print("tCards.Count " + tCards.Count);
            //print("colors.Count " + colors.Length);
            Global.FillWithColors(tCards.ToArray(), tColors);
            tCards[randomMainColorIndex].SetColor(mainColor);
        }
    }
    private RectTransform CheckPartRules(DragNDropCard dragNDropCard) // проверка правильности установки части
    {
        RectTransform result = null;
        ColorCard card = dragNDropCard.GetComponent<ColorCard>();
        if(HasColliderUnderFinger(dragNDropCard)) // попали в коллайдер
        {
            if(card.id1 == horizontalScroll.currentCardGroup) // попали правильной деталью
            {
                if (card.id1 == windowsId) // правильный предмет
                {
                    if (hasDoors) // двери есть, можно ставить окна
                    {
                        if (card.currentColor == mainColor) // правильный цвет
                        {
                            CatHelper.Instance.ShowText(trueColorText, 3f);
                            card.id4 = dragNDropCard.id4 = 9;
                            dragNDropCard.rectTransform.SetParent(windowsArea);
                            dragNDropCard.GoToPosition(windowsPoint.position, () => { dragNDropCard.SetScale(windowsPoint.localScale); }, goToPointDuration);
                            dragNDropCard.draggable = false;
                            hasWindows = true;
                            result = windowsPoint;
                            NextPartsPack();
                            if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                        }
                        else // неправильный цвет
                        {
                            CatHelper.Instance.ShowText(wrongColorText, 3f);
                            dragNDropCard.GoToDefaultPosition();
                            if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                        }
                    }
                    else // дверей нет, окна ставить нельзя
                    {
                        CatHelper.Instance.ShowText(needTheDoorsFirstText);
                        dragNDropCard.GoToDefaultPosition();
                        if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                    }
                }
                else if (card.id1 == doorId) // правильный предмет
                {
                    if (card.currentColor == mainColor) // правильный цвет
                    {
                        CatHelper.Instance.ShowText(trueColorText, 3f);
                        card.id4 = dragNDropCard.id4 = 9;
                        dragNDropCard.rectTransform.SetParent(doorsArea);
                        dragNDropCard.GoToPosition(doorsPoint.position, () => { dragNDropCard.SetScale(doorsPoint.localScale); }, goToPointDuration);
                        hasDoors = true;
                        result = doorsPoint;
                        dragNDropCard.draggable = false;
                        NextPartsPack();
                        if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                    }
                    else // неправильный цвет
                    {
                        CatHelper.Instance.ShowText(wrongColorText, 3f);
                        dragNDropCard.GoToDefaultPosition();
                        if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                    }
                }
                else if (card.id1 == wheelsId) // правильный предмет
                {
                    if (card.currentColor == mainColor) // правильный цвет
                    {
                        CatHelper.Instance.ShowText(trueColorText, 3f);
                        card.id4 = dragNDropCard.id4 = 9;
                        dragNDropCard.rectTransform.SetParent(wheelsArea);
                        dragNDropCard.GoToPosition(wheelsPoints[0].position, () => { dragNDropCard.SetScale(wheelsPoints[0].localScale); }, goToPointDuration);
                        DragNDropCard temp = Instantiate(dragNDropCard.gameObject).GetComponent<DragNDropCard>();
                        temp.rectTransform.SetParent(wheelsArea);
                        temp.rectTransform.position = wheelsPoints[1].position;
                        temp.SetScale(wheelsPoints[1].localScale);
                        temp.draggable = false;
                        hasWheels = true;
                        result = wheelsPoints[0];
                        dragNDropCard.draggable = false;
                        NextPartsPack();
                        if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                    }
                    else // неправильный цвет
                    {
                        CatHelper.Instance.ShowText(wrongColorText, 3f);
                        dragNDropCard.GoToDefaultPosition();
                        if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                    }
                }
                else if (card.id1 == backLightId) // правильный объект
                {
                    if (card.currentColor == mainColor) // правильный цвет
                    {
                        CatHelper.Instance.ShowText(trueColorText, 3f);
                        card.id4 = dragNDropCard.id4 = 9;
                        dragNDropCard.rectTransform.SetParent(backLightsArea);
                        dragNDropCard.GoToPosition(backLightsPoint.position, () => { dragNDropCard.SetScale(backLightsPoint.localScale); }, goToPointDuration);
                        hasBackLight = true;
                        result = backLightsPoint;
                        dragNDropCard.draggable = false;
                        NextPartsPack();
                        if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                    }
                    else // неправильный цвет
                    {
                        CatHelper.Instance.ShowText(wrongColorText, 3f);
                        dragNDropCard.GoToDefaultPosition();
                        if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                    }
                }
                else if (card.id1 == frontLightId) // правильный предмет
                {
                    if (card.currentColor == mainColor) // правильный цвет
                    {
                        CatHelper.Instance.ShowText(trueColorText, 3f);
                        card.id4 = dragNDropCard.id4 = 9;
                        dragNDropCard.rectTransform.SetParent(frontLightArea);
                        dragNDropCard.GoToPosition(frontLightPoint.position, () => { dragNDropCard.SetScale(frontLightPoint.localScale); }, goToPointDuration);
                        hasFrontLight = true;
                        result = frontLightPoint;
                        dragNDropCard.draggable = false;
                        NextPartsPack();
                        if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                    }
                    else // неправильный цвет
                    {
                        CatHelper.Instance.ShowText(wrongColorText, 3f);
                        dragNDropCard.GoToDefaultPosition();
                        if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                    }
                }
            }
            else // не тот предмет
            {
                CatHelper.Instance.ShowText(wrongPartText);
                dragNDropCard.GoToDefaultPosition();
                if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            }
        }
        else // не тот предмет
        {
            CatHelper.Instance.ShowText(wrongPartText);
            dragNDropCard.GoToDefaultPosition();
            if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
        }
        #region OLD METHOD
        /*
        if (windowsArea.ContainsPointScalable(dragNDropCard.rectTransform.position)) // попали на окна
        {
            if (card.id1 == windowsId) // правильный предмет
            {
                if (hasDoors) // двери есть, можно ставить окна
                {
                    if (card.currentColor == mainColor) // правильный цвет
                    {
                        CatHelper.Instance.ShowText(trueColorText, 3f);
                        card.id4 = dragNDropCard.id4 = 9;
                        dragNDropCard.rectTransform.SetParent(windowsArea);
                        dragNDropCard.GoToPosition(windowsPoint.position, () => { dragNDropCard.SetScale(windowsPoint.localScale); }, goToPointDuration);
                        dragNDropCard.draggable = false;
                        hasWindows = true;
                        result = windowsPoint;
                        NextPartsPack();
                        if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                    }
                    else // неправильный цвет
                    {
                        CatHelper.Instance.ShowText(wrongColorText, 3f);
                        dragNDropCard.GoToDefaultPosition();
                        if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                    }
                }
                else // дверей нет, окна ставить нельзя
                {
                    CatHelper.Instance.ShowText(needTheDoorsFirstText);
                    dragNDropCard.GoToDefaultPosition();
                    if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                }
            }
            else // не тот предмет
            {
                CatHelper.Instance.ShowText(wrongPartText);
                dragNDropCard.GoToDefaultPosition();
                if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            }
        }
        else if (doorsArea.ContainsPointScalable(dragNDropCard.rectTransform.position)) // попали на дверь
        {
            if (card.id1 == doorId) // правильный предмет
            {
                if (card.currentColor == mainColor) // правильный цвет
                {
                    CatHelper.Instance.ShowText(trueColorText, 3f);
                    card.id4 = dragNDropCard.id4 = 9;
                    dragNDropCard.rectTransform.SetParent(doorsArea);
                    dragNDropCard.GoToPosition(doorsPoint.position, () => { dragNDropCard.SetScale(doorsPoint.localScale); }, goToPointDuration);
                    hasDoors = true;
                    result = doorsPoint;
                    dragNDropCard.draggable = false;
                    NextPartsPack();
                    if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                }
                else // неправильный цвет
                {
                    CatHelper.Instance.ShowText(wrongColorText, 3f);
                    dragNDropCard.GoToDefaultPosition();
                    if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                }
            }
            else // не тот предмет
            {
                CatHelper.Instance.ShowText(wrongPartText);
                dragNDropCard.GoToDefaultPosition();
                if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            }
        }
        else if (wheelsArea.ContainsPointScalable(dragNDropCard.rectTransform.position)) // попали на колеса
        {
            if (card.id1 == wheelsId) // правильный предмет
            {
                if (card.currentColor == mainColor) // правильный цвет
                {
                    CatHelper.Instance.ShowText(trueColorText, 3f);
                    card.id4 = dragNDropCard.id4 = 9;
                    dragNDropCard.rectTransform.SetParent(wheelsArea);
                    dragNDropCard.GoToPosition(wheelsPoints[0].position, () => { dragNDropCard.SetScale(wheelsPoints[0].localScale); }, goToPointDuration);
                    DragNDropCard temp = Instantiate(dragNDropCard.gameObject).GetComponent<DragNDropCard>();
                    temp.rectTransform.SetParent(wheelsArea);
                    temp.rectTransform.position = wheelsPoints[1].position;
                    temp.SetScale(wheelsPoints[1].localScale);
                    temp.draggable = false;
                    hasWheels = true;
                    result = wheelsPoints[0];
                    dragNDropCard.draggable = false;
                    NextPartsPack();
                    if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                }
                else // неправильный цвет
                {
                    CatHelper.Instance.ShowText(wrongColorText, 3f);
                    dragNDropCard.GoToDefaultPosition();
                    if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                }
            }
            else // не тот предмет
            {
                CatHelper.Instance.ShowText(wrongPartText);
                dragNDropCard.GoToDefaultPosition();
                if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            }
        }
        else if (backLightsArea.ContainsPointScalable(dragNDropCard.rectTransform.position)) // попали на задние фонари
        {
            if (card.id1 == backLightId) // правильный объект
            {
                if (card.currentColor == mainColor) // правильный цвет
                {
                    CatHelper.Instance.ShowText(trueColorText, 3f);
                    card.id4 = dragNDropCard.id4 = 9;
                    dragNDropCard.rectTransform.SetParent(backLightsArea);
                    dragNDropCard.GoToPosition(backLightsPoint.position, () => { dragNDropCard.SetScale(backLightsPoint.localScale); }, goToPointDuration);
                    hasBackLight = true;
                    result = backLightsPoint;
                    dragNDropCard.draggable = false;
                    NextPartsPack();
                    if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                }
                else // неправильный цвет
                {
                    CatHelper.Instance.ShowText(wrongColorText, 3f);
                    dragNDropCard.GoToDefaultPosition();
                    if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                }
            }
            else // не тот предмет
            {
                CatHelper.Instance.ShowText(wrongPartText);
                dragNDropCard.GoToDefaultPosition();
                if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            }
        }
        else if (frontLightArea.ContainsPointScalable(dragNDropCard.rectTransform.position)) // попали на переднии фонари
        {
            if (card.id1 == frontLightId) // правильный предмет
            {
                if (card.currentColor == mainColor) // правильный цвет
                {
                    CatHelper.Instance.ShowText(trueColorText, 3f);
                    card.id4 = dragNDropCard.id4 = 9;
                    dragNDropCard.rectTransform.SetParent(frontLightArea);
                    dragNDropCard.GoToPosition(frontLightPoint.position, () => { dragNDropCard.SetScale(frontLightPoint.localScale); }, goToPointDuration);
                    hasFrontLight = true;
                    result = frontLightPoint;
                    dragNDropCard.draggable = false;
                    NextPartsPack();
                    if (!CheckCarIsCompleted()) if (SoundMaster.Instance) SoundMaster.Instance.PlayTrueAnswer();
                }
                else // неправильный цвет
                {
                    CatHelper.Instance.ShowText(wrongColorText, 3f);
                    dragNDropCard.GoToDefaultPosition();
                    if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
                }
            }
            else // не тот предмет
            {
                CatHelper.Instance.ShowText(wrongPartText);
                dragNDropCard.GoToDefaultPosition();
                if (SoundMaster.Instance) SoundMaster.Instance.PlayWrongAnswer();
            }
        }
        else // никуда не попали
        {
            dragNDropCard.GoToDefaultPosition();
        }
        */
        #endregion
        return result;
    }
    private bool CheckCarIsCompleted() // проверка завершенности сборки машины
    {
        return hasBackLight && hasDoors && hasFrontLight && hasWheels && hasWindows;
    }

    private void NextPartsPack()
    {
        if (horizontalScroll.currentCardGroup < horizontalScroll.availableCards.Count - 1)
        {
            horizontalScroll.ShowCardsGroup(horizontalScroll.currentCardGroup + 1);
            switch (horizontalScroll.currentCardGroup)
            {
                case 0:
                    CatHelper.Instance.PlayAudio(doorsAudio, 1.5f);
                    break;
                case 1:
                    CatHelper.Instance.PlayAudio(windowsAudio, 1.5f);
                    break;
                case 2:
                    CatHelper.Instance.PlayAudio(wheelsAudio, 1.5f);
                    break;
                case 3:
                    CatHelper.Instance.PlayAudio(headlightAudio, 1.5f);
                    break;
                case 4:
                    CatHelper.Instance.PlayAudio(taillightAudio, 1.5f);
                    break;
                default:
                    break;
            }
            ActivatePartColliders(horizontalScroll.currentCardGroup);
        }
    }
    private void ActivatePartColliders(int partsGroup)
    {
        doorsArea.GetComponent<Collider2D>().enabled = partsGroup == 0;
        windowsArea.GetComponent<Collider2D>().enabled = partsGroup == 1;
        foreach (var item in wheelsArea.GetComponents<Collider2D>())
        {
            item.enabled = partsGroup == 2;
        }
        frontLightArea.GetComponent<Collider2D>().enabled = partsGroup == 3;
        backLightsArea.GetComponent<Collider2D>().enabled = partsGroup == 4;
    }
    private bool HasColliderUnderFinger(DragNDropCard dragNDropCard)
    {
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(dragNDropCard.transform.position), Vector2.zero);
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }
}
