using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float rotationSpeed, jumpSpeed, gravity, radius;
    const float radioInterior = 3.5f;
    const float radioExterior = 6.77f;

    Vector3 startDirection;
    float speedY;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    //public TrailRenderer tr;

    float tiempoPulsandoJ = 0.0f;
    float tiempoRequeridoJ = 1.2f;

    public GameObject balaPrefab;

    private bool ringExterior = true;
    private float altura;

    

    // Start is called before the first frame update
    void Start()
    {
        // if(isDashing) {
        //     return;
        // }
        // Store starting direction of the player with respect to the axis of the level
        Vector3 center = new Vector3(0,0,0);
        startDirection = transform.position - center;
        startDirection.y = 0.0f;
        startDirection.Normalize();

        speedY = 0;
        radius = radioExterior;
        altura = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CharacterController charControl = GetComponent<CharacterController>();
        Vector3 position;

        // if(isDashing) {
        //     return;
        // }
        // if (Input.GetKey(KeyCode.E) && canDash) {
        //     StartCoroutine(Dash());
        // }

        // if (saltoRing)
        // {
        //     if(ringExterior) ringExterior = false;
        //     else ringExterior = true;

        //     position = transform.position;
        //     float angle = rotationSpeed * Time.deltaTime;
        //     Vector3 direction = position - transform.parent.position;
        //     Vector3 target;
        //     float multiplicador;
            
        //     if (ringExterior) multiplicador = 1.2f;
        //     else multiplicador = 0.8f;

        //     target = transform.parent.position + Quaternion.AngleAxis(angle, Vector3.up) * direction * 0.8f;

        //     if (charControl.Move(target - position) != CollisionFlags.None)
        //     {
        //         transform.position = position;
        //         Physics.SyncTransforms();
        //     }

        //     // Asegúrate de establecer saltoRing en false para que no entre continuamente en este bloque de código
        //     saltoRing = false;
        // }


        // Left-right movement
        //if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            //solocuin chatgpt 05/ a las 13
            float angle;
            Vector3 direction, target;

            position = transform.position;
            angle = rotationSpeed * Time.deltaTime;
            Vector3 center = new Vector3(0,3*altura,0);
            direction = position - center;

            if (Input.GetKey(KeyCode.D)) 
            {
                angle = -angle;
            }

            target = center + Quaternion.AngleAxis(angle, Vector3.up) * direction;
            
            if (charControl.Move(target - position) != CollisionFlags.None)
            {
                transform.position = position;
                Physics.SyncTransforms();
            }
        }

        transform.LookAt(new Vector3(0,transform.position.y,0));
        // Correct orientation of player
        // Compute current direction
        // Vector3 currentDirection = transform.position - transform.parent.position;
        // currentDirection.y = 0.0f;
        // currentDirection.Normalize();
        // // Change orientation of player accordingly
        // Quaternion orientation;
        // if ((startDirection - currentDirection).magnitude < 1e-3)
        //     orientation = Quaternion.AngleAxis(0.0f, Vector3.up);
        // else if ((startDirection + currentDirection).magnitude < 1e-3)
        //     orientation = Quaternion.AngleAxis(180.0f, Vector3.up);
        // else
        //     orientation = Quaternion.FromToRotation(startDirection, currentDirection);
        // transform.rotation = orientation;


        // Apply up-down movement
        position = transform.position;
        if (charControl.Move(speedY * Time.deltaTime * Vector3.up) != CollisionFlags.None)
        {
            transform.position = position;
            Physics.SyncTransforms();
        }
        if (charControl.isGrounded)
        {
            if (speedY < 0.0f)
                speedY = 0.0f;
            if (Input.GetKey(KeyCode.W))
                speedY = jumpSpeed;
        }
        else speedY -= gravity * Time.deltaTime;

        //Disparar
        // if (Input.GetMouseButtonDown(0)) // 0 representa el botón izquierdo del ratón
        // {
        //     Disparar();
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
       tiempoPulsandoJ = 0.0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Jumper") {
            if (Input.GetKey(KeyCode.J))
            {
                tiempoPulsandoJ += Time.deltaTime;
                // Verifica si se ha estado pulsando la tecla durante el tiempo requerido
                if (tiempoPulsandoJ >= tiempoRequeridoJ)
                {
                    Vector3 position = transform.position;
                    position.y += 6.1f;
                    altura += 1f;
                    transform.position = position;
                }
            }
            else
            {
                // Resetea el tiempo si la tecla J no está siendo pulsada
                tiempoPulsandoJ = 0.0f;
            }
        }

        else if(other.gameObject.tag == "ChangerRing") {
            if (Input.GetKey(KeyCode.J))
            {
                tiempoPulsandoJ += Time.deltaTime;
                if (tiempoPulsandoJ >= tiempoRequeridoJ)
                {
                    ringExterior = !ringExterior;

                    Vector3 center = new Vector3(0f, transform.position.y, 0f);
                    float distanciaSalto = ringExterior ? 3f : -3f;
                    Vector3 direccion = (transform.position - center).normalized;
                    float distanciaActual = Vector3.Distance(transform.position, center);
                    float nuevaDistancia = Mathf.Clamp(distanciaActual + distanciaSalto, 0f, float.MaxValue);
                    Vector3 nuevaPosicion = center + direccion * nuevaDistancia;
                    CharacterController charControl = GetComponent<CharacterController>();
                    if (charControl.Move(nuevaPosicion - transform.position) != CollisionFlags.None)
                    {
                        transform.position = nuevaPosicion;
                        Physics.SyncTransforms();
                    }

                    tiempoPulsandoJ = 0.0f;
                }
            }
            else tiempoPulsandoJ = 0.0f;
        }
        
        
        // Verificar si la colisión es con un objeto etiquetado como suelo
        // if (other.gameObject.tag == "Jumper")
        // {
        //     if (Input.GetKey(KeyCode.J)) {
        //         Vector3 position = transform.position;
        //         position.y += 6;
        //         transform.position = position;
        //     }
        // }
    }

    // private IEnumerator Dash() {
    //     canDash = false;
    //     isDashing = true;
    //     GetComponent<Rigidbody>().velocity = new Vector2 (transform.localScale.x * dashingPower, 0f);
    //     tr.emitting = true;
    //     yield return new WaitForSeconds (dashingTime);
    //     tr.emitting = false;
    //     isDashing = false;
    //     yield return new WaitForSeconds (dashingCooldown); 
    //     canDash = true;
    // }

    void Disparar()
    {
        // Instancia una nueva bala en el centro del jugador
        GameObject nuevaBala = Instantiate(balaPrefab, transform.position, Quaternion.identity);

        // Obtiene la dirección hacia la que está mirando el jugador
        Vector3 direccionDisparo = transform.forward;

        // Asigna la dirección a la bala
        nuevaBala.transform.forward = direccionDisparo;
    }
}





