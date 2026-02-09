using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzles_5_1_Controller : BaseController<Puzzles_5_1_Controller>
{
    public Sprite[] availableImages;
    public GameObject cardPrefab;
    public int columns = 3;
    public int rows = 3;
    public Sprite sprite;
    public RectTransform fieldParent;
    private List<DragNDropCard> cards;
    public Transform test;
    public RectTransform fieldMaxRect;
    public Image backImage;

    [Header("Cat phrases")]

    public string rulesText;
    public string winText;
    public string nextLevelText;


    public int currentLevel = 0;

    [Header("Audio")]
    public AudioClip[] startAudio;

    public void NextLevel()
    {
        DisableDragging();
        currentLevel++;
        if (currentLevel == 1)
        {
            rows = 3;
            columns = 2;
            SoundMaster.Instance.PlayNextLevel();
        }
        else if (currentLevel == 2)
        {
            rows = 3;
            columns = 3;
            SoundMaster.Instance.PlayNextLevel();
        }
        else
        {
            Win();
            return;
        }
        CatHelper.Instance.ShowText(nextLevelText, 3f);
        StartCoroutine(DelayedRestart());
    }
    public void Win()
    {
        CatHelper.Instance.ShowText(winText);
        SoundMaster.Instance.PlayWin();
        StartCoroutine(AutoExit());
    }
    public override void Restart()
    {
        DragNDropCard[] temp = FindObjectsOfType<DragNDropCard>();
        foreach (var item in temp)
        {
            Destroy(item.gameObject);
        }
        sprite = availableImages.Random();
        StartCoroutine(GenerateField());
    }
    void Start()
    {
        Input.multiTouchEnabled = false;
        CatHelper.Instance.defaultText = rulesText;
        CatHelper.Instance.ShowText(rulesText);
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.audioSource.clip = startAudio.Random();
        CatHelper.Instance.audioSource.Play();
        Restart();
        DragAndDropManager.Instance.onDragStart.AddListener(OnStartDrag);
        DragAndDropManager.Instance.onDragStop.AddListener(OnStopDrag);
    }


    public void OnStartDrag(DragNDropCard dragNDropCard)
    {
        dragNDropCard.rectTransform.SetAsLastSibling();
    }
    public void OnStopDrag(DragNDropCard dragNDropCard)
    {
        foreach (var item in cards)
        {
            if (item == dragNDropCard) continue;
            if (item.rectTransform.ContainsPointScalable(dragNDropCard.rectTransform.position))
            {
                item.rectTransform.SetAsLastSibling();
                dragNDropCard.rectTransform.SetAsLastSibling();
                DisableDragging();
                int d1i1 = item.id1;
                int d1i2 = item.id2;
                item.id1 = dragNDropCard.id1;
                item.id2 = dragNDropCard.id2;
                dragNDropCard.id1 = d1i1;
                dragNDropCard.id2 = d1i2;
                item.GoToPosition(dragNDropCard.tempPosition, () => { item.SetDefaultScale(); item.tempPosition = item.rectTransform.position; });
                dragNDropCard.GoToPosition(item.tempPosition, () => 
                { 
                    dragNDropCard.SetDefaultScale(); 
                    dragNDropCard.tempPosition = dragNDropCard.rectTransform.position; 
                    if (CheckRules()) NextLevel(); 
                    else EnableDragging(); 
                });
                
                return;
            }
        }
        dragNDropCard.GoToPosition(dragNDropCard.tempPosition, () => { dragNDropCard.SetDefaultScale(); });
    }

    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(1.5f);
        Restart();
    }

    private IEnumerator GenerateField()
    {
        fieldParent.localScale = Vector3.one;
        //backImage.enabled = true;
        //backImage.sprite = sprite;
        yield return null;
        cards = new List<DragNDropCard>(columns * rows);
        Sprite[,] sprites = TextureSlicer.SliceSprite(sprite, columns, rows);
        ResizeField(sprite.texture.width, sprite.texture.height);
        float sWidth = sprite.texture.width / columns;
        float sHeight = sprite.texture.height / rows;
        Vector3 startPoint = fieldParent.position + new Vector3(-sWidth * (columns - 1) / 2f, sHeight * (rows - 1) / 2f);
        test.position = startPoint;
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                DragNDropCard temp = Instantiate(cardPrefab).GetComponent<DragNDropCard>();
                temp.id1 = temp.id3 = i;
                temp.id2 = temp.id4 = j;
                temp.rectTransform.SetParent(fieldParent, true);
                temp.rectTransform.position = Vector3.one * 5000f;
                temp.rectTransform.ForceUpdateRectTransforms();
                //yield return null;
                temp.rectTransform.position = startPoint + new Vector3(sWidth * i, -sHeight * j);
                temp.rectTransform.sizeDelta = new Vector2(sWidth, sHeight);
                temp.defaultScale = temp.rectTransform.localScale;
                temp.scaleOnStartDrag = temp.defaultScale * 1.1f;
                temp.defaultAnchoredPosition = temp.rectTransform.localPosition;

                //temp.rectTransform.localScale = Vector3.zero;
                //temp.SetDefaultScale();
                temp.GetComponent<ColorCard>().SetColorImage(0, sprites[i, j]);
                cards.Add(temp);
                temp.draggable = false;
                //yield return null;
                //yield return new WaitForSeconds(3f / (columns * rows));
            }
        }
        Vector3 p1 = new Vector3(
            cards[0].rectTransform.position.x - cards[0].rectTransform.rect.width / 2f * cards[0].rectTransform.lossyScale.x,
            cards[0].rectTransform.position.y + cards[0].rectTransform.rect.height / 2f * cards[0].rectTransform.lossyScale.y
        );
        Vector3 p2 = new Vector3(
            cards[cards.Count - 1].rectTransform.position.x + cards[cards.Count - 1].rectTransform.rect.width / 2f * cards[cards.Count - 1].rectTransform.lossyScale.x,
            cards[cards.Count - 1].rectTransform.position.y - cards[cards.Count - 1].rectTransform.rect.height / 2f * cards[cards.Count - 1].rectTransform.lossyScale.y
        );

        backImage.rectTransform.sizeDelta =
            new Vector2(Mathf.Abs(p1.x - p2.x) + 20f, Mathf.Abs(p1.y - p2.y) + 20f);
        //backImage.enabled = false;
        fieldParent.localScale /= cards[0].transform.localScale.x;
        yield return new WaitForSeconds(3f);
        //backImage.enabled = false;
        List<DragNDropCard> tcards = new List<DragNDropCard>(cards);
        int count = cards.Count / 2;
        for (int i = 0; i < count; i++)
        {
            int first = Random.Range(0, tcards.Count);
            DragNDropCard dnd1 = tcards[first];
            dnd1.tempPosition = dnd1.rectTransform.position;
            Vector3 f = tcards[first].rectTransform.position;
            tcards.RemoveAt(first);

            int second = Random.Range(0, tcards.Count);
            DragNDropCard dnd2 = tcards[second];
            dnd2.tempPosition = dnd2.rectTransform.position;
            Vector3 s = tcards[second].rectTransform.position;
            tcards.RemoveAt(second);

            int d1i1 = dnd1.id1;
            int d1i2 = dnd1.id2;
            dnd1.id1 = dnd2.id1;
            dnd1.id2 = dnd2.id2;
            dnd2.id1 = d1i1;
            dnd2.id2 = d1i2;
            //print($"first: {first}");
            //print($"second: {second}");
            dnd1.rectTransform.SetAsLastSibling();
            dnd2.rectTransform.SetAsLastSibling();

            dnd1.GoToPosition(s, () => { dnd1.tempPosition = dnd1.rectTransform.position; });
            dnd2.GoToPosition(f, () => { dnd2.tempPosition = dnd2.rectTransform.position; });

            yield return new WaitForSeconds(3f / (cards.Count / 2));
        }

        foreach (var item in tcards)
        {
            int first = Random.Range(0, tcards.Count);
            DragNDropCard dnd1 = item;
            dnd1.tempPosition = dnd1.rectTransform.position;
            Vector3 f = item.rectTransform.position;

            int second = Random.Range(0, cards.Count);
            DragNDropCard dnd2 = cards[second];
            dnd2.tempPosition = dnd2.rectTransform.position;
            Vector3 s = cards[second].rectTransform.position;
            dnd1.rectTransform.SetAsLastSibling();
            dnd2.rectTransform.SetAsLastSibling();
            dnd1.GoToPosition(s, () => { dnd1.tempPosition = dnd1.rectTransform.position; });
            dnd2.GoToPosition(f, () => { dnd2.tempPosition = dnd2.rectTransform.position; });
            int d1i1 = dnd1.id1;
            int d1i2 = dnd1.id2;
            dnd1.id1 = dnd2.id1;
            dnd1.id2 = dnd2.id2;
            dnd2.id1 = d1i1;
            dnd2.id2 = d1i2;
        }

        EnableDragging();
        yield break;
    }

    public void ResizeField(float textureWidth, float textureHeight)
    {
        if (fieldMaxRect.sizeDelta.x < textureWidth)
        {
            float multiplier = fieldMaxRect.rect.width / textureWidth;
            fieldParent.localScale *= multiplier;
            textureHeight *= multiplier;
        }
        if (fieldMaxRect.sizeDelta.y < textureHeight)
        {
            float multiplier = fieldMaxRect.rect.height / textureHeight;
            fieldParent.localScale *= multiplier;
        }
    }

    public void EnableDragging()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].draggable = true;
        }
    }
    public void DisableDragging()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].draggable = false;
        }
    }

    public bool CheckRules()
    {
        bool result = true;
        for (int i = 0; i < cards.Count; i++)
        {
            result = result && cards[i].id1 == cards[i].id3 && cards[i].id2 == cards[i].id4;
        }
        return result;
    }
}
