using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public float velocidad = 10f;
    public Vector3 directionIzq = new Vector3(-1f,0f,-1f);
    public float rotationSpeed;
    Vector3 startDirection;
    float speedY;

    void Update()
    {   
        float angle;
        Vector3 target;
        CharacterController charControl = GetComponent<CharacterController>();
        Vector3 position = transform.position;
        angle = rotationSpeed * Time.deltaTime;

        //if (Input.GetKey(KeyCode.D)) angle = -angle;

        // Dirección inicial de la bala (puede ser ajustada según tus necesidades)
        Vector3 directionIzq = new Vector3(0f, 0f, 1f);

        // Calcula la nueva posición de la bala en una trayectoria circular
        target = position + Quaternion.AngleAxis(angle, Vector3.up) * directionIzq;

        // Mueve la bala hacia la nueva posición
        charControl.Move(target - position);

        // Actualiza la posición del transform para que coincida con la posición del CharacterController
        transform.position = charControl.transform.position;

        // Destruye la bala después de cierto tiempo o distancia (ajusta según tus necesidades)
        Destroy(gameObject, 2f);
    }

}
