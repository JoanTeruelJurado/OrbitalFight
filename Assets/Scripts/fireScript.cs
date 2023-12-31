using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class fireScript : MonoBehaviour
{
    float tiempoVidaMax = 1f;
    public bool direccionDerecha;
    public float rotationSpeed = 60;
    private Vector3 center;
    
    // Start is called before the first frame update
    void Start()
    {
        center = new Vector3(0f,transform.position.y,0f);
        Destroy(gameObject, tiempoVidaMax);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(1,1,1) * Time.deltaTime;
        float angle = rotationSpeed * Time.deltaTime;
        if(direccionDerecha) angle *= -1f;
        transform.RotateAround(center, Vector3.up, angle);

        //rotando la bala como toca
        transform.LookAt(new Vector3(0,transform.position.y,0));
        Quaternion rotacionActual = transform.rotation;
        Quaternion nuevaRotacion = Quaternion.Euler(rotacionActual.eulerAngles + new Vector3(0, 0, 90f));
        transform.rotation = nuevaRotacion;
    }
}
