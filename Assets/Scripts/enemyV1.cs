using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyV1 : MonoBehaviour
{
    private float speed;
    private int rutina;
    private float timer;
    private Animator ani;
    private bool followingPlayer;
    private bool attacking;
    private int direccionDerecha;
    private int shield = 100;
    private int live = 50;

    private FloatingHealthBar healthBar;

    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.updateHealthBar(shield, 100);
        speed = 40f;
        rutina = 0;
        timer = 0f;
    }


    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 2) {
            timer = 0;
            if (rutina == 0) {
                direccionDerecha = Random.Range(0,2);
                //StartCoroutine(move());
                rutina = 1;
            }
        }
        Vector3 center = new Vector3(0f,transform.position.y,0f);
        float angle = speed * Time.deltaTime;
        if(direccionDerecha == 1) angle *= -1f;
        transform.RotateAround(center, Vector3.up, angle);
    }

    private IEnumerator move() {
        return null;
    }

    private void die() {
        Destroy(gameObject);
    }

    void lessLive(int damage) {
        shield -= damage;
        if(shield >= 0) {
            healthBar.updateHealthBar(shield, 100);
        }
        else {
            live += shield;
            shield = 0;
            healthBar.ShieldDestroyed();
            healthBar.updateHealthBar(live, 50);
            if(live < 0) { //muere
                live = 0;
                die();
            }
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Entorno") {
            direccionDerecha = 1 - direccionDerecha; // Cambia el signo de anglePerStep
        }
        else if(collision.gameObject.tag == "Bullet") {
            bulletScript scriptBullet = collision.gameObject.GetComponent<bulletScript>();
            if (scriptBullet != null){
                int damage = scriptBullet.damageHit;
                lessLive(damage);
            }
        }
    }



}

/*
//con rigidbody + capsule collider
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 position;
        float anglePerStep = speed * Time.deltaTime;
        if (direccionDerecha == 1) anglePerStep = -anglePerStep;
        Vector3 center = new Vector3(0, transform.position.y, 0);
        Vector3 direction;

        float elapsedTime = 0f;
        while (elapsedTime < 4f)
        {
            position = transform.position;
            direction = position - center;
            if (direccionDerecha == 1) anglePerStep = -anglePerStep;
            Vector3 target = center + Quaternion.AngleAxis(anglePerStep, Vector3.up) * direction;

            // Calcular la dirección del movimiento
            Vector3 movementDirection = (target - position).normalized;

            // Realizar el barrido de colisión
            RaycastHit hit;
            if (rb.SweepTest(movementDirection, out hit, speed * Time.deltaTime))
            {
                // Hay una colisión, ajustar la posición
                transform.position = hit.point;
                Physics.SyncTransforms();
            }
            else
            {
                // No hay colisión, moverse a la nueva posición
                rb.MovePosition(rb.position + movementDirection * speed * Time.deltaTime);
                Physics.SyncTransforms();
            }

            // Rotación
            transform.LookAt(new Vector3(0, transform.position.y, 0));

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rutina = 0;

    //con character controller
    CharacterController charControl = GetComponent<CharacterController>();
        Vector3 position;
        float anglePerStep = speed * Time.deltaTime;
        if (direccionDerecha == 1) anglePerStep = -anglePerStep;
        Vector3 center = new Vector3(0, transform.position.y, 0);
        Vector3 direction;

        float elapsedTime = 0f;
        while (elapsedTime < 4f)
        {
            position = transform.position;
            direction = position - center;
            if (direccionDerecha == 1) anglePerStep = -anglePerStep;
            Vector3 target = center + Quaternion.AngleAxis(anglePerStep, Vector3.up) * direction;

            if (charControl.Move(target - position) != CollisionFlags.None) {
                transform.position = position;
            }

            // Rotación
            transform.LookAt(new Vector3(0, transform.position.y, 0));

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rutina = 0;
*/
