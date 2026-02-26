using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class Global
{
    public const int DELAY_120FPS = 8;
    public const int DELAY_60FPS = 15;
    public const int DELAY_30FPS = 32;

    public static int lastScreen = -1;
    public const float AUTO_EXIT_DELAY = 6f;
    public static bool appFirstStart = true;

    public const float EPSILON = 0.0001f;

    public static readonly Color[] colors9 = new Color[] {
        new Color(254f/255f, 32f/255f, 43f/255f, 1f), // red
        new Color(1f, 151f/255f, 32f/255f, 1f), // orange
        new Color(1f, 244f/255f, 32f/255f, 1f), // yellow
        new Color(44f/255f, 243f/255f, 91f/255f, 1f), // green
        new Color(0f, 1f, 241f/255f, 1f), // light blue
        new Color(0f, 0f, 254f/255f, 1f), // blue
        new Color(189f/255f, 97f/255f, 255f/255f, 1f), // purple
        new Color(1f, 1f, 1f, 1f), // white
        new Color(0.15f, 0.15f, 0.15f, 1f), // black
    };

    public static readonly Color[] colors7 = new Color[] {
        new Color(254f/255f, 32f/255f, 43f/255f, 1f), // red
        new Color(1f, 151f/255f, 32f/255f, 1f), // orange
        new Color(1f, 244f/255f, 32f/255f, 1f), // yellow
        new Color(44f/255f, 243f/255f, 91f/255f, 1f), // green
        new Color(0f, 1f, 241f/255f, 1f), // light blue
        new Color(0f, 0f, 254f/255f, 1f), // blue
        new Color(189f/255f, 97f/255f, 255f/255f, 1f), // purple
    };

    public static readonly string[] colorsNamesGenitive = new string[]
    {
        "красного",
        "оранжевого",
        "жёлтого",
        "зеленого",
        "голубого",
        "синего",
        "фиолетового",
        "белого",
        "черного",
    };

    public static readonly string[] colorsNames = new string[]
    {
        "Красный",
        "Оранжевый",
        "Желтый",
        "Зеленый",
        "Голубой",
        "Синий",
        "Фиолетовый",
        "Белый",
        "Черный"
    };

    public static string GetColorName(Color color)
    {
        for (int i = 0; i < colors9.Length; i++)
        {
            if (colors9[i] == color) return colorsNames[i];
        }
        return "Неизвестный цвет";
    }

    public static readonly string[] numbersNames = new string[]
    {
        "Ноль",
        "Один",
        "Два",
        "Три",
        "Четыре",
        "Пять",
        "Шесть",
        "Семь",
        "Восемь",
        "Девять",
        "Десять"
    };

    public static readonly string[] animalsCountNames = new string[]
    {
        "нет животных",
        "одно животное",
        "два животных",
        "три животных",
        "четыре животных",
        "пять животных",
        "шесть животных",
        "семь животных",
        "восемь животных",
        "девять животных",
        "десять животных"
    };

    public static readonly string[] dollsCountNames = new string[]
    {
        "нет кукол",
        "одна кукла",
        "две куклы",
        "три куклы",
        "четыре куклы",
        "пять кукол",
        "шесть кукол",
        "семь кукол",
        "восемь кукол",
        "девять кукол",
        "десять кукол"
    };


    public static List<Color> GetRandomColors(int listLength, int colorsCount)
    {
        List<Color> result = new List<Color>(listLength);
        List<Color> availableColors = new List<Color>(colors9.SubArray(0, colorsCount));
        for (int i = 0; i < listLength; i++)
        {
            if (availableColors.Count == 0)
            {
                result.Add(colors9[UnityEngine.Random.Range(0, colorsCount)]);
            }
            else
            {
                int colorIndex = UnityEngine.Random.Range(0, availableColors.Count);
                result.Add(availableColors[colorIndex]);
                availableColors.RemoveAt(colorIndex);
            }
        }
        return result;
    }

    public static void RandomizePositionAndRotation(ref RectTransform rectTransform, RectTransform startPoint, float maxDistance, float maxAngle)
    {
        rectTransform.position = startPoint.position + new Vector3(UnityEngine.Random.Range(-maxDistance, maxDistance), UnityEngine.Random.Range(-maxDistance, maxDistance), 0f);
        rectTransform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-maxAngle, maxAngle));
    }

    /// <summary>
    /// Выполняет код с задержкой
    /// </summary>
    /// <param name="delayMS">Задержка, миллисекунды</param>
    /// <param name="action"></param>
    /// <param name="callback"></param>
    public static async void DelayedAction(int delayMS, System.Action action, System.Action callback = null)
    {
        await Task.Delay(delayMS);
        action();
        if (callback != null) callback();
    }
    /// <summary>
    /// Отправляет запрос на остановку и обновляет токен
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static CancellationToken RenewCT(ref CancellationTokenSource source)
    {
        //Debug.Log("RenewCT");
        source.Cancel(false);
        source = new CancellationTokenSource();
        return source.Token;
    }
    /// <summary>
    /// Delay, который не срет ошибками при остановке таска
    /// </summary>
    /// <param name="milliseconds">Время задержки</param>
    public static async void Delay(int milliseconds) => await Task.Delay(milliseconds).ContinueWith((_) => { });

    /// <summary>
    /// Подстраивает размеры картинки под указанную ширину/высоту
    /// </summary>
    /// <param name="image">Ссылка на изменяемое изображение</param>
    /// <param name="preferredSize"></param>
    /// <param name="width">true - изменять ширину, false - изменять высоту</param>
    public static void ImageAutoSize(ref Image image, float preferredSize, bool width = true)
    {
        if (width)
        {
            float currMultiplier = preferredSize / image.sprite.texture.height;
            image.rectTransform.sizeDelta = new Vector2(image.sprite.texture.width * currMultiplier, preferredSize);
        }
        else
        {
            float currMultiplier = preferredSize / image.sprite.texture.width;
            image.rectTransform.sizeDelta = new Vector2(preferredSize, image.sprite.texture.height * currMultiplier);
        }
    }
    /// <summary>
    /// Подстраивает размеры картинки по существующей верстке
    /// </summary>
    /// <param name="image">Ссылка на изменяемое изображение</param>
    /// <param name="width">true - изменять ширину, false - изменять высоту</param>
    public static void ImageAutoSize(ref Image image, bool width = true)
    {
        if (width)
        {
            float y1 = image.rectTransform.rect.height;
            float currMultiplier = y1 / image.sprite.texture.height;
            image.rectTransform.sizeDelta = new Vector2(image.sprite.texture.width * currMultiplier, y1);
        }
        else
        {
            float x1 = image.rectTransform.rect.width;
            float currMultiplier = x1 / image.sprite.texture.width;
            image.rectTransform.sizeDelta = new Vector2(x1, image.sprite.texture.height * currMultiplier);
        }
    }
    /// <summary>
    /// Подстраивает размеры изображения под указанные максимальные размеры
    /// </summary>
    /// <param name="image">Ссылка на изменяемое изображение</param>
    /// <param name="maxWidth">Максимальная ширина</param>
    /// <param name="maxHeight">Максимальная высота</param>
    public static void ImageAutoSize(ref Image image, float maxWidth, float maxHeight)
    {
        float currMultiplier;
        Vector2 calculatedizeDelta = Vector2.zero;

        currMultiplier = maxHeight / image.sprite.texture.height;
        calculatedizeDelta.x = image.sprite.texture.width * currMultiplier;
        calculatedizeDelta.y = maxHeight;
        if (calculatedizeDelta.x > maxWidth)
        {
            currMultiplier = maxWidth / calculatedizeDelta.x;
            calculatedizeDelta.x = maxWidth;
            calculatedizeDelta.y *= currMultiplier;
        }
        image.rectTransform.sizeDelta = calculatedizeDelta;
    }
    /// <summary>
    /// Подстраивает размеры изображения под указанные максимальные размеры
    /// </summary>
    /// <param name="image">Ссылка на изменяемое изображение</param>
    /// <param name="maxSize">Максимальные размеры</param>
    public static void ImageAutoSize(ref Image image, Vector2 maxSize)
    {
        ImageAutoSize(ref image, maxSize.x, maxSize.y);
    }
    /// <summary>
    /// Устанавливает SizeDelta в указанном диапазоне
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    public static void AutoSize(this RectTransform rectTransform, float maxWidth, float maxHeight)
    {
        float currMultiplier;
        Vector2 calculatedizeDelta = Vector2.zero;

        currMultiplier = maxHeight / rectTransform.sizeDelta.y;
        calculatedizeDelta.x = rectTransform.sizeDelta.x * currMultiplier;
        calculatedizeDelta.y = maxHeight;
        if (calculatedizeDelta.x > maxWidth)
        {
            currMultiplier = maxHeight / calculatedizeDelta.x;
            calculatedizeDelta.x = maxHeight;
            calculatedizeDelta.y *= currMultiplier;
        }
        rectTransform.sizeDelta = calculatedizeDelta;
    }
    /// <summary>
    /// Устанавливает SizeDelta в указанном диапазоне
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="maxSize"></param>
    public static void AutoSize(this RectTransform rectTransform, Vector2 maxSize)
    {
        AutoSize(rectTransform, maxSize.x, maxSize.y);
    }
    /// <summary>
    /// Вычисляет максимальный размер изображения, вписанного в указанный прямоугольник
    /// </summary>
    /// <param name="image">Изображение</param>
    /// <param name="maxSize">Максимальные размеры</param>
    /// <returns></returns>
    public static Vector2 CalculateMaxImageSize(Image image, Vector2 maxSize)
    {
        float currMultiplier;
        Vector2 calculatedizeDelta;
        currMultiplier = maxSize.y / image.sprite.texture.height;
        calculatedizeDelta.x = image.sprite.texture.width * currMultiplier;
        calculatedizeDelta.y = maxSize.y;
        if (calculatedizeDelta.x > maxSize.x)
        {
            currMultiplier = maxSize.x / calculatedizeDelta.x;
            calculatedizeDelta.x = maxSize.x;
            calculatedizeDelta.y *= currMultiplier;
        }
        return calculatedizeDelta;
    }

    public static void ZoomToPoint(this RectTransform rectTransform, Vector2 point, Vector2 baseSizeDelta, float zoomValue)
    {
        Vector2 delta = baseSizeDelta * zoomValue - rectTransform.sizeDelta;
        rectTransform.sizeDelta += delta;
        rectTransform.anchoredPosition += (rectTransform.anchoredPosition - point) * delta / rectTransform.sizeDelta;
    }

    public static bool ContainsPoint(this RectTransform rct, Vector2 point)
    {
        return
            point.x >= rct.position.x - rct.rect.width / 2f &&
            point.x <= rct.position.x + rct.rect.width / 2f &&
            point.y >= rct.position.y - rct.rect.height / 2f &&
            point.y <= rct.position.y + rct.rect.height / 2f;
    }
    public static bool ContainsPointScalable(this RectTransform rct, Vector2 point)
    {
        return
            point.x >= rct.position.x - rct.rect.width / 2f * rct.transform.lossyScale.x &&
            point.x <= rct.position.x + rct.rect.width / 2f * rct.transform.lossyScale.x &&
            point.y >= rct.position.y - rct.rect.height / 2f * rct.transform.lossyScale.y &&
            point.y <= rct.position.y + rct.rect.height / 2f * rct.transform.lossyScale.y;
    }

    public static void RenewCoroutine(this MonoBehaviour gameObject, ref IEnumerator routineContainer, IEnumerator method)
    {
        if (routineContainer != null) gameObject.StopCoroutine(routineContainer);
        routineContainer = method;
        if(gameObject.isActiveAndEnabled) gameObject.StartCoroutine(routineContainer);
    }

    /// <summary>
    /// Заполняет карточки цветами
    /// </summary>
    /// <param name="cards">Карточки</param>
    /// <param name="availableColors">Доступные цвета</param>
    public static void FillWithColors(ColorCard[] cards, List<Color> availableColors)
    {
        List<Color> tempColors = new List<Color>(availableColors);
        for (int i = 0; i < cards.Length; i++)
        {
            int t = UnityEngine.Random.Range(0, tempColors.Count);
            cards[i].SetColor(tempColors[t]);
            tempColors.RemoveAt(t);
        }
    }
}
