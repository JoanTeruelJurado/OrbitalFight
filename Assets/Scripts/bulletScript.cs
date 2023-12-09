using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public float rotationSpeed;
    public bool miraDerecha;
    public float altura;
    public string equipedGun;
    private float tiempoVidaAct;
    private float damageHit;
    private Vector3 center;

    void Start() {
        float tiempoVidaMax = 7f;
        if(equipedGun == "Fusil") {
            tiempoVidaMax = 2f;
            damageHit = 3f;
        }
        else if(equipedGun == "Pistola") {
            tiempoVidaMax = 0.3f;
            damageHit = 7f;
        }

        //rotationSpeed = 100f;
        Destroy(gameObject, tiempoVidaMax);
        center = new Vector3(0f,transform.position.y,0f);
    }

    void FixedUpdate()
    {
        float angle = rotationSpeed * Time.deltaTime;
        if(miraDerecha) angle *= -1f;
        transform.RotateAround(center, Vector3.up, angle);

        //rotando la bala como toca
        transform.LookAt(new Vector3(0,transform.position.y,0));
        Quaternion rotacionActual = transform.rotation;
        Quaternion nuevaRotacion = Quaternion.Euler(rotacionActual.eulerAngles + new Vector3(0, 0, 90f));
        transform.rotation = nuevaRotacion;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destruye la bala cuando colisiona con otro objeto
        if(collision.gameObject.tag != "Player") Destroy(gameObject);
    }

//     // Calcula la nueva posición de la bala
    //     float anglePerStep = rotationSpeed * Time.deltaTime;
    //     if(miraDerecha) anglePerStep = -anglePerStep;
    //     Vector3 center = new Vector3(0, 3 * altura, 0);
    //     Vector3 direction = transform.position - center;
    //     Vector3 target = center + Quaternion.AngleAxis(anglePerStep, Vector3.up) * direction;

    //     // Mueve la bala hacia la nueva posición
    //     transform.position = Vector3.MoveTowards(transform.position, target, rotationSpeed * Time.deltaTime);

    //     // Rotación
    //     transform.LookAt(new Vector3(0, transform.position.y, 0));

    //     // Verificar tiempo de vida
    //     tiempoVidaAct += Time.deltaTime;
    //     if (tiempoVidaAct >= tiempoVidaMax)
    //     {
    //         Destroy(gameObject);
    //     }
}
