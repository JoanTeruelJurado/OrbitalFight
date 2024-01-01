using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cofre : MonoBehaviour
{
    [SerializeField] private int _tipodecofre;
    public Animator _animator;
    private bool _empty = false;
    [SerializeField] private AudioSource _source;

    /*
        switch(tipodecofre)
        case 0: //cofre de vida
        case 1: //cofre de munición
     
     */
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetEmpty() { return _empty; }

    public int GetTipus() {
        _empty = true;
        _animator.SetTrigger("isOpen");
        _source.Play(0);
        return _tipodecofre;
    }
}
