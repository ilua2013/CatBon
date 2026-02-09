using UnityEngine;

public class Floating : MonoBehaviour
{
    public Vector3 startPosition;
    public float amplitude;
    public float randomOffsetRange = 10f;
    public float speed = 1f;
    private Vector3 currentPosition;
    private float offset;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = currentPosition = transform.localPosition;
        offset = Random.Range(-randomOffsetRange, randomOffsetRange);
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition.y = startPosition.y + Mathf.Sin((Time.time + offset) * speed) * amplitude;
        transform.localPosition = currentPosition;
    }
}
