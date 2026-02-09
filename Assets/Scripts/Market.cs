using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SceneButtonContainer
{
    public string id;
    public Button button;
    public Image lockImage;
}


public class Market : MonoBehaviour
{
    public SceneButtonContainer[] scenes;
    public static Market Instance;
    public bool m_PurchaseInProgress = false;

    private void Awake()
    {
        Instance = this;
        UnlockAllGames();
    }
    public void UnlockGame(string[] ids, bool autosave = true)
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            foreach (var item in ids)
            {
                if (scenes[i].id == item)
                {
                    scenes[i].button.interactable = true;
                    scenes[i].lockImage.gameObject.SetActive(false);
                }
            }
        }
        if (autosave) SaveUnlockedScenes();
    }
    public void LockAllGames()
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].lockImage.gameObject.SetActive(true);
            //scenes[i].button.interactable = false;
        }
        SaveUnlockedScenes();
    }
    public void UnlockAllGames()
    {
        foreach (var item in scenes)
        {
            item.button.interactable = true;
            item.lockImage.gameObject.SetActive(false);
        }
        SaveUnlockedScenes();
    }
    public void SaveUnlockedScenes()
    {
        PlayerPrefs.DeleteKey("usa");
        string result = "";
        for (int i = 0; i < scenes.Length; i++)
        {
            if (!scenes[i].lockImage.gameObject.activeSelf)
            {
                result += scenes[i].id + ':';
            }
        }
        if (result != "")
        {
            result = result.Remove(result.Length - 1);
        }
        PlayerPrefs.SetString("usa", result);
    }
    public void LoadUnlockedScenes()
    {
        string result = PlayerPrefs.GetString("usa");
        string[] arr = result.Split(':');
        UnlockGame(arr, false);
    }
    public bool IsGameUnlocked(string id)
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].id == id)
            {
                return !scenes[i].lockImage.gameObject.activeSelf;
            }
            if (i == scenes.Length - 1) return true;
        }
        return true;
    }

    public void ShowBuyWinow(string id)
    {

    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            UnlockGame(new string[1] { scenes.Random().id });
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            LockAllGames();
            SaveUnlockedScenes();
        }
    }
#endif
}

