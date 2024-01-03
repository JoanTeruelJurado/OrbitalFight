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
    private float lasthittime = 0.0f;
    private bool beenhit = false;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        target = GameObject.Find("Player");
    }
    // Update is called once per frame
    void Update()
    {
        if(mismaAltura()) {
            ActualTime -= Time.deltaTime;
            retSound -= Time.deltaTime;
            if (ActualTime <= 0)
            {
                _audioSource.Play(0);
                _animator.SetTrigger("Throw");
            }
            if (retSound <= 0) {  _animator.ResetTrigger("Throw");_audioSource.Play(0); retSound = 8.5f; ActualTime = tiempoReset; }
            if (beenhit) lasthittime += Time.deltaTime;
            if (lasthittime >= 6.0f) {lasthittime = 0.0f; beenhit=false; }
        }
    }

   /* private void OnTriggerEnter(Collider other)
    {
        //print("HI");
        //print(ActualTime);
        if (ActualTime <= 0.0f && lasthittime <= 0.0f) { // ACTIVE TRAP

            if (other.gameObject.tag == "Player") {
                other.GetComponent<MovePlayer>().lessLive(50);
                beenhit = true;
            }
        }
       } 
    }*/

    private void OnTriggerStay(Collider other)
    {
        //print("HI");
        //print(ActualTime);
        if (ActualTime <= 0.0f && lasthittime <= 0.0f) { // ACTIVE TRAP
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<MovePlayer>().lessLive(50);
                beenhit = true;
            }
        }
    }

    private bool mismaAltura() {
        return Mathf.Abs(transform.position.y - target.transform.position.y) < 2f;
    }
}
