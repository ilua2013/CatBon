using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowerController : MonoBehaviour
{
    public Animator animator;
    public int id;
    public ParticleSystem pSystem;
    public AudioSource A_S;
    private ParticleSystem.EmissionModule emissionModule;

    private float _defaultEmissionRate;

    public IntDelegate onMouseDown = (_) => { };
    public IntDelegate onMouseUp = (_) => { };

    private void Awake()
    {
        emissionModule = pSystem.emission;
        _defaultEmissionRate = emissionModule.rateOverTimeMultiplier;
        emissionModule.rateOverTimeMultiplier = 0;
    }


    public void StartShower()
    {
        emissionModule.rateOverTimeMultiplier = _defaultEmissionRate;
    }

    public void StopShower()
    {
        emissionModule.rateOverTimeMultiplier = 0;
    }

    private void OnMouseDown()
    {
        StartShower();
         A_S.Play();
        onMouseDown(id);
    }
    private void OnMouseUp()
    {
        StopShower();
        onMouseUp(id);
    }

    public void In()
    {
        animator.ResetTrigger("out");
        animator.SetTrigger("in");
       
    }
    public void Out()
    {
        StopShower();
        A_S.Stop(); 
        animator.ResetTrigger("in");
        animator.SetTrigger("out");
    }

}
