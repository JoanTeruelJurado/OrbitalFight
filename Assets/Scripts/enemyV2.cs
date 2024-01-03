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
    private bool hayQueGirar = false;
    [SerializeField] private GameObject _explosion;
    private FloatingHealthBar healthBar;


    //V2
    private GameObject target;
    private float tiempoEntreDisparosMin = 2.5f;
    private float tiempoEntreDisparos = 10f;
    public GameObject bala;


    //sounds
    private AudioSource audioSource;
    public AudioClip armorHitSound;
    public AudioClip armorCrashSound;
    public AudioClip fleshHitSound;

    //calcular distancia player
    public GameObject delante;
    public GameObject detras;


    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.updateHealthBar(shield, shieldMax);
        speed = 30f;
        //direccionDerecha = Random.Range(0,2) == 0 ? false : true;
        direccionDerecha = false;

        audioSource = GetComponent<AudioSource>();

        //V2
        target = GameObject.Find("Player");
    }


    void Update()
    {
        if(Vector3.Distance(transform.position, target.transform.position) > 9f) tiempoEntreDisparos = 99f;

        if(Vector3.Distance(transform.position, target.transform.position) > 8f) { //anda
            Vector3 center = new Vector3(0f,transform.position.y,0f);
            float angle = speed * Time.deltaTime;
            if(direccionDerecha) angle *= -1f;
            transform.RotateAround(center, Vector3.up, angle);
            attacking = false;
        }
        else { //dispara
            MovePlayer player = target.GetComponent<MovePlayer>();
            
            bool aux = direccionDerecha;
            if(Vector3.Distance(detras.transform.position, target.transform.position) < Vector3.Distance(delante.transform.position, target.transform.position)) {
                direccionDerecha = !direccionDerecha;
            }
            if(aux != direccionDerecha) { //se gira si lo detecta en el otro sentido
                girar();
            }
            attacking = true;
            tiempoEntreDisparos += Time.deltaTime;
            if(tiempoEntreDisparos > tiempoEntreDisparosMin) {
                Disparar();
                tiempoEntreDisparos = 0f;
            }
        }

        if(hayQueGirar) {
            girar();
            hayQueGirar = false;
        }

        destruirSiMuyAbajo();
    }

    private void Disparar() {
        if(mismaAltura()) {
            GameObject nuevaBalaObject = Instantiate(bala, transform.position, Quaternion.identity);
            Physics.IgnoreCollision(nuevaBalaObject.GetComponent<Collider>(), GetComponent<Collider>());
            bulletScript balita = nuevaBalaObject.GetComponent<bulletScript>();
            balita.miraDerecha = direccionDerecha;
            balita.altura = transform.position.y;
        }
    }

    private bool mismaAltura() {
        return Mathf.Abs(transform.position.y - target.transform.position.y) < 2f;
    }

    private void die() {
        MovePlayer playerScript = target.GetComponent<MovePlayer>();
        playerScript.reproducirSonido("enemyDie");
        Destroy(Instantiate(_explosion, transform.position + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity), 2.0f);
        Destroy(gameObject);
    }

    private void destruirSiMuyAbajo() {
        target = GameObject.Find("Player");
        if(target.transform.position.y - transform.position.y >= 5f) Destroy(gameObject);
    }

    private void girar() {
        Vector3 aux1 = delante.transform.position;
        Vector3 aux2 = detras.transform.position;

        Vector3 escalaActual = transform.localScale;
        escalaActual.z *= -1;
        transform.localScale = escalaActual;

        delante.transform.position = aux2;
        detras.transform.position = aux1;
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
            if(live <= 0) { //muere
                live = 0;
                die();
            }
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Entorno" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Trampa" || collision.gameObject.tag == "Cofre") {
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
