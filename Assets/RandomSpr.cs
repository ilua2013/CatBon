using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RandomSpr : MonoBehaviour
{
    public SpriteRenderer img;
    public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        img.sprite=sprites[Random.Range(0,2)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
