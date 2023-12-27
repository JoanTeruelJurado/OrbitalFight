using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyV1 : MonoBehaviour
{
    private float speed;
    private Animator ani;
    private bool followingPlayer;
    private bool attacking;
    private bool direccionDerecha;
    private int shield = 100;
    private int shieldMax = 100;
    private int live = 50;
    private int liveMax = 50;
    private bool armorActive = true;
    private bool hayQueGirar = false; 

    private FloatingHealthBar healthBar;


    //sounds
    private AudioSource audioSource;
    public AudioClip armorHitSound;
    public AudioClip armorCrashSound;
    public AudioClip fleshHitSound;
    public AudioClip dieSound;

    //Animator
    //public Animator animator;

    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.updateHealthBar(shield, shieldMax);
        speed = 20f;
        //direccionDerecha = Random.Range(0,2) == 0 ? false : true;
        direccionDerecha = false;

        audioSource = GetComponent<AudioSource>();
        //animator.SetFloat("Speed", speed);
    }


    void Update()
    {
        Vector3 center = new Vector3(0f,transform.position.y,0f);
        float angle = speed * Time.deltaTime;
        if(direccionDerecha) angle *= -1f;
        transform.RotateAround(center, Vector3.up, angle);

        if(hayQueGirar) {
            Vector3 escalaActual = transform.localScale;
            escalaActual.z *= -1;
            transform.localScale = escalaActual;
            hayQueGirar = false;
        }
    }

    private IEnumerator move() {
        return null;
    }

    private void die() {
        audioSource.PlayOneShot(dieSound);
        Destroy(gameObject);
    }

    void lessLive(int damage) {
        shield -= damage;
        if(shield > 0) {
            healthBar.updateHealthBar(shield, shieldMax);
            audioSource.PlayOneShot(armorHitSound);
        }
        if(shield <= 0 && armorActive) {
            audioSource.PlayOneShot(armorCrashSound);
            //AudioSource.PlayClipAtPoint(armorCrashSound, transform.position, 0.7f);
        }
        if(shield <= 0) {
            armorActive = false;
            live += shield;
            shield = 0;
            healthBar.ShieldDestroyed();
            healthBar.updateHealthBar(live, liveMax);
            audioSource.PlayOneShot(fleshHitSound);
            if(live < 0) { //muere
                live = 0;
                die();
            }
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Entorno" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy") {
            direccionDerecha = !direccionDerecha; // Cambia el signo de anglePerStep
            hayQueGirar = true;
        }
        
    }

    void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.tag == "BulletPlayer") {
            bulletScript scriptBullet = collider.gameObject.GetComponent<bulletScript>();
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
