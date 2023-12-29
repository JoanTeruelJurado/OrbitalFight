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
    private GameObject target;   
    private FloatingHealthBar healthBar;


    //sounds
    private AudioSource audioSource;
    public AudioClip armorHitSound;
    public AudioClip armorCrashSound;
    public AudioClip fleshHitSound;

    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.updateHealthBar(shield, shieldMax);
        speed = 20f;
        //direccionDerecha = Random.Range(0,2) == 0 ? false : true;
        direccionDerecha = false;

        audioSource = GetComponent<AudioSource>();
        target = GameObject.Find("Player");
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
        MovePlayer playerScript = target.GetComponent<MovePlayer>();
        playerScript.reproducirSonido("enemyDie");
        Destroy(gameObject);
    }

    public void lessLive(int damage) {
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