using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum Shift
{
    up,
    down,
    right,
    left
}
public class Board : MonoBehaviour
{
    public Text scoreText;
    public string level;
    public static Board instance;
    public Text scoresText;
    public Text newScoresText;
    public AudioSource rowAudio;
    public AudioSource stopAudio;
    public Animator newScoresAnim;
    public GameObject destrEffect;
    public bool touched;
    public Timer timerScript;
    public int max;
    public bool fin;
    public float maxTimer = 60;
    public float timer = 60f;
    public GameObject gameOverScreen;
    public const int n = 9;          //размер матрицы по иксу, можно изменять до любого нужного размера. В случае больших величин придется выставить более высокое значение size для главной камеры.
    const int m = 7;           //аналогично для игрека
    const float offsetX = -4.5f;
    const float offsetY = 1.2f;
    int scores = 0;
    Symbol[,] _board = new Symbol[n, m];
    public GameObject[] symbols;
    public List<Vector2Int> lineParts = new List<Vector2Int>();
    bool _canMove = false;
    public static bool moving = false;
    Transform _tr;
    public int lenght;

    public AudioClip[] startAudio;
    public string rulesText;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance);
            instance = this;
        }
        _tr = transform;
    }
    void Start()
    {
        CatHelper.Instance.ShowText(rulesText, 3f);
        CatHelper.Instance.defaultAudio = startAudio.Random();
        CatHelper.Instance.audioSource.clip = startAudio.Random();
        CatHelper.Instance.audioSource.Play();
        //StaticParameters.match3Score = 0;
        GameObject temp;
        moving = false;
        symbols.Shuffle();
        //заполняем игровую "доску" и массив
        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < m; y++)
            {
                temp = Instantiate(symbols[Random.Range(0, lenght)], new Vector2(-(n - 1) * 0.6f + 1.2f * x, -(m - 1) * 0.6f + 1.2f * y), Quaternion.identity, _tr);
                _board[x, y] = temp.GetComponent<Symbol>();
                _board[x, y].cellPosition = new Vector2Int(x, y);
            }
        }
        IfLines(false);
    }
    void IfLines(bool inGame, bool fast = false)        //проверяем, есть ли на доске собранные линии
    {
        int nDoubles;
        int lastSymbolType;
        // этот цикл ищет в столбцах
        for (int x = 0; x < n; x++)
        {
            nDoubles = 0;
            lastSymbolType = -1;
            for (int y = 0; y < m; y++)
            {
                if (_board[x, y].type == lastSymbolType)
                {
                    nDoubles++;

                    if (nDoubles == 2)
                    {
                        lineParts.Add(new Vector2Int(x, y));
                        lineParts.Add(new Vector2Int(x, y - 1));
                        lineParts.Add(new Vector2Int(x, y - 2));
                    }
                    else if (nDoubles > 2)
                    {
                        lineParts.Add(new Vector2Int(x, y));
                    }
                }
                else
                {
                    lastSymbolType = _board[x, y].type;
                    nDoubles = 0;
                }
            }
        }
        //а этот в строках
        for (int y = 0; y < m; y++)
        {
            nDoubles = 0;
            lastSymbolType = -1;
            for (int x = 0; x < n; x++)
            {
                if (_board[x, y].type == lastSymbolType)
                {
                    nDoubles++;
                    if (nDoubles == 2)
                    {
                        lineParts.Add(new Vector2Int(x, y));
                        lineParts.Add(new Vector2Int(x - 1, y));
                        lineParts.Add(new Vector2Int(x - 2, y));
                    }
                    else if (nDoubles > 2)
                    {
                        lineParts.Add(new Vector2Int(x, y));
                    }
                }
                else
                {
                    lastSymbolType = _board[x, y].type;
                    nDoubles = 0;
                }
            }
        }
        if (inGame)
        {
            if (lineParts.Count > 0)
            {
                _canMove = true;
                StartCoroutine(DestroyLines(fast));
            }

        }
        else
        {

            //поскольку собранные линии нашлись перед запуском игры, просто удаляем их и спавним новые случайные символы на их места
            if (lineParts.Count > 0)
            {
                GameObject temp;
                for (int i = 0; i < lineParts.Count; i++)
                {
                    Destroy(_board[lineParts[i].x, lineParts[i].y].gameObject);
                    temp = Instantiate(symbols[Random.Range(0, lenght)], new Vector2(-(n - 1) * 0.6f + 1.2f * lineParts[i].x, -(m - 1) * 0.6f + 1.2f * lineParts[i].y), Quaternion.identity, _tr);
                    _board[lineParts[i].x, lineParts[i].y] = temp.GetComponent<Symbol>();
                    _board[lineParts[i].x, lineParts[i].y].cellPosition = lineParts[i];
                }
                //подчищаем список и ищем линии заново
                lineParts.Clear();
                IfLines(false);
                return;
            }
            else IfTurns(false);

        }
    }
    void IfTurns(bool inGame) //проверяем, возможно ли сделать ход
    {
        bool isTurns = false;
        int nDoubles;
        int lastSymbolType;
        int temp;
        List<int> types = new List<int>();
        for (int x = 0; x < n; x++)
        {

            if (isTurns) break;
            for (int y = 0; y < m; y++)
            {
                if (y < m - 1)
                {
                    //проверяем что будет, если сдвинуть символ вверх
                    nDoubles = 0;
                    lastSymbolType = -1;
                    types.Clear();
                    //типы символов в столбце до сдвига
                    for (int i = 0; i < m; i++)
                    {
                        types.Add(_board[x, i].type);

                    }
                    //сдвиг
                    temp = types[y];
                    types[y] = types[y + 1];
                    types[y + 1] = temp;
                    //ищем, не образовался ли столбец одинаковых символов
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i] == lastSymbolType)
                        {
                            nDoubles++;
                            if (nDoubles >= 2) isTurns = true;
                        }
                        else
                        {
                            nDoubles = 0;
                            lastSymbolType = types[i];
                        }
                    }
                    //дальше то же самое, но для двух изменившихся строк
                    //нижняя
                    nDoubles = 0;
                    lastSymbolType = -1;
                    types.Clear();
                    for (int i = 0; i < n; i++)
                    {
                        types.Add(_board[i, y].type);
                    }

                    types[x] = _board[x, y + 1].type;
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i] == lastSymbolType)
                        {
                            nDoubles++;
                            if (nDoubles >= 2) isTurns = true;
                        }
                        else
                        {
                            nDoubles = 0;
                            lastSymbolType = types[i];
                        }
                    }
                    //верхняя
                    nDoubles = 0;
                    lastSymbolType = -1;
                    types.Clear();
                    for (int i = 0; i < n; i++)
                    {
                        types.Add(_board[i, y + 1].type);
                    }
                    types[x] = _board[x, y].type;
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i] == lastSymbolType)
                        {
                            nDoubles++;
                            if (nDoubles >= 2) isTurns = true;
                        }
                        else
                        {
                            nDoubles = 0;
                            lastSymbolType = types[i];
                        }
                    }

                }
                if (x < n - 1)
                {
                    //аналогично для сдвига элемента вправо
                    nDoubles = 0;
                    lastSymbolType = -1;
                    types.Clear();
                    for (int i = 0; i < n; i++)
                    {
                        types.Add(_board[i, y].type);
                    }
                    temp = types[x];
                    types[x] = types[x + 1];
                    types[x + 1] = temp;
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i] == lastSymbolType)
                        {
                            nDoubles++;
                            if (nDoubles >= 2) isTurns = true;
                        }
                        else
                        {
                            nDoubles = 0;
                            lastSymbolType = types[i];
                        }
                    }
                    //левый столбец
                    nDoubles = 0;
                    lastSymbolType = -1;
                    types.Clear();
                    for (int i = 0; i < m; i++)
                    {
                        types.Add(_board[x, i].type);
                    }

                    types[y] = _board[x + 1, y].type;
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i] == lastSymbolType)
                        {
                            nDoubles++;
                            if (nDoubles >= 2) isTurns = true;
                        }
                        else
                        {
                            lastSymbolType = types[i];
                            nDoubles = 0;
                        }
                    }
                    //правый столбец
                    nDoubles = 0;
                    lastSymbolType = -1;
                    types.Clear();
                    for (int i = 0; i < m; i++)
                    {
                        types.Add(_board[x + 1, i].type);
                    }
                    types[y] = _board[x, y].type;
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i] == lastSymbolType)
                        {
                            nDoubles++;
                            if (nDoubles >= 2) isTurns = true;
                        }
                        else
                        {
                            lastSymbolType = types[i];
                            nDoubles = 0;
                        }
                    }
                }
            }
        }
        if (!isTurns)
        {
            if (inGame)
            {
                //Game Over, даём игроку осознать произошедшее и перезапустить самостоятельно
                gameOverScreen.SetActive(true);
            }
            else
            {
                //заспавнилась доска без возможных ходов, перезапускаем
                ReStart();
            }
        }

    }
    public void ReStart()
    {
        gameOverScreen.SetActive(false);
        //StaticParameters.match3Score = scores;
        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < m; y++)
            {
                Destroy(_board[x, y].gameObject);
            }
        }
        Start();
    }
    bool _catHasBeenSpoken = false;
    IEnumerator DestroyLines(bool fast = false)
    {
        moving = true;
        //        if(!fast)

        int dropCells;
        int dropCellsOnTheLine;
        GameObject temp;
        var destroPoints = 0;
        var stopPoints = 0;
        for (int i = 0; i < lineParts.Count; i++)
        {
            if (_board[lineParts[i].x, lineParts[i].y] != null)
            {
                switch (_board[lineParts[i].x, lineParts[i].y].type)
                {
                    case 0:
                        destroPoints += 1;
                        break;
                    case 1:
                        destroPoints += 1;
                        break;
                    case 2:
                        destroPoints += 1;
                        break;
                    case 3:
                        destroPoints += 1;
                        break;
                    case 4:
                        destroPoints += 1;
                        break;
                    case 5:
                        destroPoints += 1;
                        break;
                    case 6:
                        destroPoints += 1;
                        break;
                    case 7:
                        destroPoints += 1;
                        break;
                    case 8:
                        destroPoints += 1;
                        break;
                    case 9:
                        destroPoints += 1;
                        break;

                }

                _board[lineParts[i].x, lineParts[i].y].DestrEffect();
                //                Destroy(_board[lineParts[i].x, lineParts[i].y].gameObject);
                //                _board[lineParts[i].x, lineParts[i].y] = null;
            }
        }
        if (destroPoints > 0)
        {
            newScoresText.text = "+" + destroPoints;
            newScoresAnim.SetTrigger("appear");
            //StaticParameters.match3Score += destroPoints;
            scores += destroPoints;
            scoreText.text = scores.ToString();
            newScoresText.color = new Color32(253, 196, 101, 255);
            //scoresText.text = StaticParameters.match3Score.ToString();
            if (scores >=max && !_catHasBeenSpoken)
            {
                _catHasBeenSpoken = true;
           
                if(fin)
                {
                CatHelper.Instance.ShowText("Молодец! У тебя всё получилось. Попробуй другую игру!", 6f);
               SoundMaster.Instance.PlayWin();
                }
                else
                {    
                //destrEffect.SetActive(true);
                CatHelper.Instance.ShowText("Молодец!Идем дальше", 6f);
                SoundMaster.Instance.PlayNextLevel();
                Invoke("LoadedLvel",6f);
            
                }
            }
            if (stopPoints == 0)
            {
                rowAudio.Play();
            }
        }
        if (stopPoints > 0)
        {
            timerScript.MinusTime(stopPoints);
            newScoresText.text = "Осторожно!";
            newScoresText.color = new Color32(230, 102, 102, 255);
            newScoresText.color = new Color32(230, 102, 102, 255);
            newScoresAnim.SetTrigger("appear");
            stopAudio.Play();
        }
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < lineParts.Count; i++)
        {
            if (_board[lineParts[i].x, lineParts[i].y] != null)
            {
                Destroy(_board[lineParts[i].x, lineParts[i].y].gameObject);
                _board[lineParts[i].x, lineParts[i].y] = null;
            }
        }
        //далее алгоритм спавна и падения новых элементов
        for (int x = 0; x < n; x++)
        {
            dropCells = 0;
            for (int y = 0; y < m; y++)
            {
                if (_board[x, y] == null)
                {
                    dropCells++;
                }
                else if (dropCells > 0)
                {
                    _board[x, y].ChangePosition(new Vector2Int(0, -dropCells));
                    _board[x, y - dropCells] = _board[x, y];
                    _board[x, y - dropCells].cellPosition = new Vector2Int(x, y - dropCells);
                    _board[x, y] = null;
                }
            }
            dropCellsOnTheLine = dropCells;
            while (dropCells > 0)
            {
                temp = Instantiate(symbols[Random.Range(0, lenght)], new Vector2(-(n - 1) * 0.6f + 1.2f * x, -(m - 1) * 0.6f + 1.2f * (m + dropCellsOnTheLine - dropCells)), Quaternion.identity, _tr);
                _board[x, m - dropCells] = temp.GetComponent<Symbol>();
                _board[x, m - dropCells].cellPosition = new Vector2Int(x, m - dropCells);
                _board[x, m - dropCells].ChangePosition(new Vector2Int(0, -dropCellsOnTheLine));
                dropCells--;

            }
        }
        lineParts.Clear();
        yield return new WaitForSeconds(0.4f);
        moving = false;
        IfLines(true);
        IfTurns(true);
    }
public void LoadedLvel()
{
Application.LoadLevel(level);
}
public void LoadedM()
{
Application.LoadLevel("MainMenu");
}
 
    Vector2Int cellPosition;
    public bool Change(Symbol symbol, Vector2Int v)
    {
        lastChangingSymbol = null;
        cellPosition = symbol.cellPosition;
        if (cellPosition.x + v.x >= 0 && cellPosition.x + v.x < n && cellPosition.y + v.y >= 0 && cellPosition.y + v.y < m)
        {
            //меняем элементы местами
            _board[cellPosition.x, cellPosition.y] = _board[cellPosition.x + v.x, cellPosition.y + v.y];
            _board[cellPosition.x + v.x, cellPosition.y + v.y] = symbol;
            _board[cellPosition.x, cellPosition.y].cellPosition = cellPosition;
            _board[cellPosition.x + v.x, cellPosition.y + v.y].cellPosition = new Vector2Int(cellPosition.x + v.x, cellPosition.y + v.y);
            //отправляем символ на его новую позицию
            _canMove = false;
            IfLines(true, true);
            if (!_canMove)
            {
                _board[cellPosition.x + v.x, cellPosition.y + v.y] = _board[cellPosition.x, cellPosition.y];
                _board[cellPosition.x, cellPosition.y] = symbol;
                _board[cellPosition.x, cellPosition.y].cellPosition = cellPosition;
                _board[cellPosition.x + v.x, cellPosition.y + v.y].cellPosition = new Vector2Int(cellPosition.x + v.x, cellPosition.y + v.y);
                return false;
            }
            //canMove = false;
            _board[cellPosition.x, cellPosition.y].FastChangePosition(new Vector2Int(v.x * (-1), v.y * (-1)));
            return true;
        }
        else return false;
    }

    Vector2Int nextSymbol;
    Vector2 nextSymbolPosition;
    int k;
    Symbol lastChangingSymbol;
    public void LightChange(Symbol symbol, float distance, Shift shift)
    {
        switch (shift)
        {
            case Shift.down:
                nextSymbol = new Vector2Int(0, -1);
                k = 1;
                break;
            case Shift.up:
                nextSymbol = new Vector2Int(0, 1);
                k = -1;
                break;
            case Shift.left:
                nextSymbol = new Vector2Int(-1, 0);
                k = 1;
                break;
            case Shift.right:
                nextSymbol = new Vector2Int(1, 0);
                k = -1;
                break;
        }
        nextSymbolPosition = new Vector2(k * nextSymbol.x * distance, k * nextSymbol.y * distance);
        //        Debug.Log(nextSymbolPosition);
        nextSymbol = symbol.cellPosition - nextSymbol;
        if (nextSymbol.x >= 0 && nextSymbol.x < n && nextSymbol.y >= 0 && nextSymbol.y < m)
        {
            if (lastChangingSymbol)
            {
                if (_board[nextSymbol.x, nextSymbol.y] != lastChangingSymbol)
                {
                    lastChangingSymbol.ResetPosition();
                    lastChangingSymbol = _board[nextSymbol.x, nextSymbol.y];
                }
            }
            else
            {

                lastChangingSymbol = _board[nextSymbol.x, nextSymbol.y];
            }

            _board[nextSymbol.x, nextSymbol.y].Shifting(nextSymbolPosition);
        }
    }
    public void ResetLastSymbol()
    {
        if (lastChangingSymbol)
            lastChangingSymbol.ResetPosition();
    }

    public void Exit()
    {
        print($"LoadMenu {gameObject.name}. 2");
        SceneManager.LoadScene("MainMenu");
    }
}