using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopController : MonoBehaviour
{
    public Animator animator;

    public void Show()
    {
        animator.ResetTrigger("Show");
        animator.SetTrigger("Show");
    }
}
