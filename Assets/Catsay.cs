using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catsay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CatHelper.Instance.ShowText("Выбери, кого будешь наряжать,маличка или девочку?");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
