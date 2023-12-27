using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PressingJ : MonoBehaviour
{

    [SerializeField] private Slider slider;


    public void hayQueGirar() {
        Vector3 escalaActual = transform.localScale;
        escalaActual.x *= -1;
        transform.localScale = escalaActual;
        
        transform.LookAt(new Vector3(0, transform.position.y, 0));

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
