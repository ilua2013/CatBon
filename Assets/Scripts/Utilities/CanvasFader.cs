using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasFader : MonoBehaviour
{
    public float inSpeed = 0.01f;
    public float outSpeed = 0.01f;
    public CanvasGroup canvasGroup;

    #region THREADS MAGIC
    public CancellationTokenSource source1 = new CancellationTokenSource();
    public CancellationToken RenewCT(ref CancellationTokenSource source)
    {
        source.Cancel(false);
        source = new CancellationTokenSource();
        return source.Token;
    }
    #endregion

    public void In(System.Action callback = null)
    {
        CG_FadeInOut(RenewCT(ref source1), 1f, inSpeed, canvasGroup, callback);
    }
    public void Out(System.Action callback = null)
    {
        CG_FadeInOut(RenewCT(ref source1), 0f, outSpeed, canvasGroup, callback);
    }
    private async void CG_FadeInOut(CancellationToken token, float target, float speed, CanvasGroup canvasGroup, System.Action callback = null)
    {
        while (Mathf.Abs(canvasGroup.alpha - target) > float.Epsilon && !token.IsCancellationRequested)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, speed);
            await Task.Delay(16, token).ContinueWith(_ => { });
        }
        canvasGroup.alpha = target;
        if (callback != null) callback();
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(CanvasFader))]
public class CanvasFaderEditor : Editor
{
    CanvasFader _targetFader;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _targetFader = target as CanvasFader;
        if (GUILayout.Button("In"))
        {
            _targetFader.In();
        }
        if (GUILayout.Button("Out"))
        {
            _targetFader.Out();
        }
        if (GUILayout.Button("Stop"))
        {
            _targetFader.source1.Cancel();
            _targetFader.RenewCT(ref _targetFader.source1);
        }
    }
}
#endif