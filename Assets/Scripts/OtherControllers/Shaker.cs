using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    public RectTransform objectToShake;

    public float defaultDuration = 1f;
    public float defaultIntensity = 1f;
    public float defaultSpeed = 5f;
    public bool playAtStart = false;

    public Vector3 defaultPosition;

    private IEnumerator routine;

    private void Start()
    {
        if (!objectToShake) objectToShake = transform as RectTransform;
        defaultPosition = objectToShake.position;
        if (playAtStart) Play();
    }

    public void Play(float duration = -1, float intensity = -1, float speed = -1, System.Action callback = null)
    {
        Global.RenewCoroutine(this, ref routine, Shake
            (
                duration != -1 ? duration : defaultDuration,
                intensity != -1 ? intensity : defaultIntensity,
                speed != -1 ? speed : defaultSpeed,
                callback
            ));
    }
    public void Stop()
    {
        StopCoroutine(routine);
        objectToShake.position = defaultPosition;
    }

    private IEnumerator Shake(float duration, float intensity, float speed, System.Action callback)
    {
        float t = 0;
        Vector3 newPosition;
        while (t < duration)
        {
            newPosition = defaultPosition + Random.insideUnitSphere * intensity;
            while(objectToShake.position != newPosition && t < duration)
            {
                objectToShake.position = Vector3.MoveTowards(objectToShake.position, newPosition, speed);
                t += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
        objectToShake.position = defaultPosition;
        if (callback != null) callback();
        yield break;
    }
}
