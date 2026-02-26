using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BaseController<T> : Singleton<T> where T : MonoBehaviour
{
    public abstract void Restart();
    public virtual void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
    protected IEnumerator AutoExit(float delay = -1)
    {
        if(delay == -1) yield return new WaitForSeconds(Global.AUTO_EXIT_DELAY);
        else yield return new WaitForSeconds(delay);
        Exit();
    }
}
