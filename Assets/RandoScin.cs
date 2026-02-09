using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RandoScin : MonoBehaviour
{
    public Image sr;
    public Sprite[] srm;
    // Start is called before the first frame update
    void Start()
    {
       sr.sprite=srm[Random.Range(0,3)];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
