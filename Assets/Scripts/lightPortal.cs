using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightPortal : MonoBehaviour
{
    private float alturaMaxima;
    private float alturaMinima;
    private float velocidadAscenso = 1.2f;
    private bool subiendo = true;

    private void Start() {
        alturaMaxima = transform.position.y+0.8f;
        alturaMinima = transform.position.y;
    }

    private void Update()
    {
        if(transform.position.y >= alturaMaxima) subiendo = false;
        else if(transform.position.y <= alturaMinima) subiendo = true;

        if(subiendo) {
            float nuevaAltura = transform.position.y + Time.deltaTime * velocidadAscenso;
            transform.position = new Vector3(transform.position.x, nuevaAltura, transform.position.z);
        }
        else {
            float nuevaAltura = transform.position.y - Time.deltaTime * velocidadAscenso;
            transform.position = new Vector3(transform.position.x, nuevaAltura, transform.position.z);
        }
        // // Calcula la posición vertical utilizando Mathf.PingPong para oscilar entre 0 y alturaMaxima
        // float nuevaAltura = Mathf.PingPong(Time.time * velocidadAscenso, alturaMaxima);

        // // Establece la posición del objeto
        // transform.position = new Vector3(transform.position.x, nuevaAltura, transform.position.z);
    }
}
