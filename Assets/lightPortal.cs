using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightPortal : MonoBehaviour
{
    private float alturaMaxima;
    private float velocidadAscenso = 1.2f;

    private void Start() {
        alturaMaxima = transform.position.y+0.8f;
    }

    private void Update()
    {
        // Calcula la posición vertical utilizando Mathf.PingPong para oscilar entre 0 y alturaMaxima
        float nuevaAltura = Mathf.PingPong(Time.time * velocidadAscenso, alturaMaxima);

        // Establece la posición del objeto
        transform.position = new Vector3(transform.position.x, nuevaAltura, transform.position.z);
    }
}
