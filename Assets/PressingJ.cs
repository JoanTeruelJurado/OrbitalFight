using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PressingJ : MonoBehaviour
{

    [SerializeField] private Slider slider;
    public bool hayQueGirar = false;

    void Update()
    {
        if(hayQueGirar) {
            Vector3 escalaActual = transform.localScale;
            escalaActual.x *= -1;
            transform.localScale = escalaActual;
            hayQueGirar = false;
        }
        transform.LookAt(new Vector3(0,transform.position.y,0));
    }

    public void pintarBarraPressingJ() {
        gameObject.SetActive(true);
    }
    public void esconderBarraPressingJ() {
        gameObject.SetActive(false);
    }

    public void updateHealthBar(float currentValue, float maxValue) {
        slider.value = currentValue/maxValue;
    }
}
