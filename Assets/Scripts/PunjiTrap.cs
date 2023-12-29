using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunjiTrap : MonoBehaviour
{
    //time loop for trap to work
    private float tiempoReset = 7.5f;
    private float ActualTime = 7.5f;
    private float retSound = 8.5f;
    private Animator _animator;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start() {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        ActualTime -= Time.deltaTime;
        retSound -= Time.deltaTime;
        if (ActualTime <= 0) { 
            ActualTime = tiempoReset;
            _animator.SetTrigger("Throw");
        }
        if (retSound <= 0) { _audioSource.Play(0); retSound = 8.5f; }

    }
}
