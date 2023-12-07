using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float rotationSpeed, jumpSpeed, gravity, radius;
    const float radioInterior = 3.5f;
    const float radioExterior = 6.77f;
    private bool miraDerecha = true;

    Vector3 startDirection;
    float speedY;

    //dash
    private bool canDash;
    private bool isDashing;
    public float dashingPower;
    private float dashingTime;
    private float dashingCooldown;
    //public TrailRenderer tr;

    //plataformas
    float tiempoPulsandoJ = 0.0f;
    float tiempoRequeridoJ = 1.2f;

    //shooting
    public GameObject balaPistola;
    public GameObject balaFusil;
    private bool pistolaDesbloqueada = false;
    private bool fusilDesbloqueado = false;
    private enum Armas {Ninguna,Fusil,Pistola};
    private Armas armaEquipada = Armas.Ninguna;
    private float tiempoEntreDisparosMinFusil = 0.3f;
    private float tiempoEntreDisparosMinPistola = 0.7f;
    private float tiempoEntreDisparos = 0f;

    //tema anillos
    private bool ringExterior = true;
    private float altura;

    

    // Start is called before the first frame update
    void Start()
    {
        if(isDashing) {
            return;
        }
        // Store starting direction of the player with respect to the axis of the level
        Vector3 center = new Vector3(0,transform.position.y,0);
        startDirection = transform.position - center;
        startDirection.y = 0.0f;
        startDirection.Normalize();

        speedY = 0;
        radius = radioExterior;
        altura = 0f;

        canDash = true;
        isDashing = false;
        //dashingPower = 100f;
        dashingTime = 0.5f;
        dashingCooldown = 0.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CharacterController charControl = GetComponent<CharacterController>();
        Vector3 position;

        if(isDashing) {
            return;
        }
        if (Input.GetKey(KeyCode.E) && canDash) {
            StartCoroutine(Dash());
        }

        // Left-right movement
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            float angle;
            Vector3 direction, target;

            position = transform.position;
            angle = rotationSpeed * Time.deltaTime;
            Vector3 center = new Vector3(0,transform.position.y,0);
            direction = position - center;

            if (Input.GetKey(KeyCode.D)) {
                angle = -angle;
                miraDerecha = true;
            }
            else miraDerecha = false;
            
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
            if (speedY < 0.0f) speedY = 0.0f;
            if (Input.GetKey(KeyCode.W)) {
                speedY = jumpSpeed;
            }
        }
        else speedY -= gravity * Time.deltaTime;
        
        

        //Disparar
        tiempoEntreDisparos += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.T)) // 0 representa el botón izquierdo del ratón
        {
            if(armaEquipada == Armas.Fusil && tiempoEntreDisparos > tiempoEntreDisparosMinFusil) {
                Disparar();
                tiempoEntreDisparos = 0f;
            }
            else if(armaEquipada == Armas.Pistola && tiempoEntreDisparos > tiempoEntreDisparosMinPistola) {
                Disparar();
                tiempoEntreDisparos = 0f;
            }
        }

        if(Input.GetKey(KeyCode.Z)) {
            armaEquipada = Armas.Fusil;
            tiempoEntreDisparos = 0f;
        }
        if(Input.GetKey(KeyCode.X)) {
            armaEquipada = Armas.Pistola;
            tiempoEntreDisparos = 0f;
        }
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
    }

    private IEnumerator Dash() {
        canDash = false;
        isDashing = true;

        CharacterController charControl = GetComponent<CharacterController>();
        Vector3 position;
        float anglePerStep = dashingPower * Time.deltaTime;
        if(miraDerecha) anglePerStep = -anglePerStep;
        Vector3 center = new Vector3(0,transform.position.y,0);
        Vector3 direction;

        float elapsedTime = 0;
        while (elapsedTime < dashingTime) {
            position = transform.position;
            direction = position - center;
            Vector3 target = center + Quaternion.AngleAxis(anglePerStep, Vector3.up) * direction;
            if (charControl.Move(target - position) != CollisionFlags.None) {
                transform.position = position;
                Physics.SyncTransforms();
                elapsedTime = dashingTime;
            }
            else elapsedTime += Time.deltaTime;
            transform.LookAt(new Vector3(0,transform.position.y,0));

            //codigo caida
            position = transform.position;
            if (charControl.Move(speedY * Time.deltaTime * Vector3.up) != CollisionFlags.None)
            {
                transform.position = position;
                Physics.SyncTransforms();
            }
            if (charControl.isGrounded)
            {
                if (speedY < 0.0f) speedY = 0.0f;
            }
            else speedY -= gravity * Time.deltaTime;
            /////
            
            yield return null; // espera hasta el próximo frame
        }

        isDashing = false;
        yield return new WaitForSecondsRealtime(dashingCooldown);
        canDash = true;
    }

    void Disparar()
    {
        // Instancia una nueva bala en el centro del jugador
        if(armaEquipada != Armas.Ninguna) {
            GameObject nuevaBalaObject;
            if(armaEquipada == Armas.Fusil) nuevaBalaObject = Instantiate(balaFusil, transform.position, Quaternion.identity);
            else nuevaBalaObject = Instantiate(balaPistola, transform.position, Quaternion.identity);
            // 'miraDerecha' es un atributo del componente 'bulletScript'
            bulletScript balita = nuevaBalaObject.GetComponent<bulletScript>();
            balita.miraDerecha = miraDerecha;
            balita.altura = altura;
            if(armaEquipada == Armas.Fusil) balita.equipedGun = "Fusil";
            else balita.equipedGun = "Pistola";
        }
    }
}





