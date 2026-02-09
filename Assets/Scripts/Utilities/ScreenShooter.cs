using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Сохранение скриншотов из приложения или из окна Game.
/// </summary>
public class ScreenShooter : MonoBehaviour
{
    public KeyCode captureScreenshot = KeyCode.Insert;
    public int counter = 0;
    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }
#if UNITY_EDITOR || UNITY_STANDALONE
    void Update()
    {
        if(Input.GetKeyDown(captureScreenshot))
        {
            TakeScreenshot();
        }
    }
#endif
    public void TakeScreenshot()
	{
        string path = Application.dataPath;
        path = path.Remove(path.LastIndexOf("/") + 1) + "ScreenCaptures";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string finalPath = path + $"/{Application.productName}_{Screen.width}x{Screen.height}_{counter}.png";
        while (File.Exists(finalPath))
        {
            counter++;
            finalPath = path + $"/{Application.productName}_{Screen.width}x{Screen.height}_{counter}.png";
        }
        ScreenCapture.CaptureScreenshot(finalPath);
        counter++;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScreenShooter))]
public class ScreenShooterEditor : Editor
{
    private ScreenShooter _target;
	private void OnEnable()
	{
        _target = target as ScreenShooter;
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
        if(GUILayout.Button("Take screenshot"))
		{
            _target.TakeScreenshot();
		}
	}
}

#endif
