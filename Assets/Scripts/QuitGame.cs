using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    private int firstEnter;
    public void OnApplicationQuit()
    {
        Application.Quit();
    }

   
}
