using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyV2 : MonoBehaviour
{
    private float speed;
    private Animator ani;
    private bool followingPlayer;
    private bool attacking = false;
    private bool direccionDerecha;
    private int shield = 100;
    private int shieldMax = 100;
    private int live = 50;
    private int liveMax = 50;
    private bool armorActive = true;

    private FloatingHealthBar healthBar;


    //V2
    public GameObject target;
    private float tiempoEntreDisparosMin = 2.5f;
    private float tiempoEntreDisparos = 10f;
    public GameObject bala;


    //sounds
    private AudioSource audioSource;
    public AudioClip armorHitSound;
    public AudioClip armorCrashSound;
    public AudioClip fleshHitSound;
    public AudioClip dieSound;


    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.updateHealthBar(shield, shieldMax);
        speed = 20f;
        direccionDerecha = Random.Range(0,2) == 0 ? false : true;

        audioSource = GetComponent<AudioSource>();

        //V2
        target = GameObject.Find("Player");
    }


    void Update()
    {
        if(Vector3.Distance(transform.position, target.transform.position) > 8f) {
            Vector3 center = new Vector3(0f,transform.position.y,0f);
            float angle = speed * Time.deltaTime;
            if(direccionDerecha) angle *= -1f;
            transform.RotateAround(center, Vector3.up, angle);
            attacking = false;
        }
        else {
            MovePlayer player = target.GetComponent<MovePlayer>();
            if(!attacking) {
                direccionDerecha = !player.miraDerecha;
            }
            attacking = true;
            tiempoEntreDisparos += Time.deltaTime;
            if(tiempoEntreDisparos > tiempoEntreDisparosMin) {
                Disparar();
                tiempoEntreDisparos = 0f;
            }
        }
        
    }

    private void Disparar() {
        GameObject nuevaBalaObject = Instantiate(bala, transform.position, Quaternion.identity);
        Physics.IgnoreCollision(nuevaBalaObject.GetComponent<Collider>(), GetComponent<Collider>());
        // 'miraDerecha' es un atributo del componente 'bulletScript'
        bulletScript balita = nuevaBalaObject.GetComponent<bulletScript>();
        balita.miraDerecha = direccionDerecha;
        balita.altura = transform.position.y;
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
        if(collision.gameObject.tag == "Entorno") {

            direccionDerecha = !direccionDerecha; // Cambia el signo de anglePerStep
            Vector3 escalaActual = transform.localScale;
            escalaActual.z *= -1;
            transform.localScale = escalaActual;

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
