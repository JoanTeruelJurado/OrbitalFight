using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossEnemy : MonoBehaviour
{
    public GameController _gameController;

    private float speed;
    private int rutina;
    private float timer;
    private Animator ani;
    private bool followingPlayer;
    private bool attacking;
    private bool direccionDerecha;
    private int shield = 400;
    private int shieldMax = 400;
    private int live = 200;
    private int liveMax = 200;

    public FloatingHealthBar healthBar;
    public GameObject target;
    public GameObject gun;

    //Lanzallamas
    public GameObject fire;
    private float timeEntreFuego = 0f;
    private float timeEntreFuegoMin = 0.07f;
    private float timerDisparandoFuego = 0f;
    private float timerDisparandoFuegoMax = 2.5f;
    private bool disparandoFuego = false; 

    //Cortes
    public GameObject corte;
    private float timeEntreCorte = 0f;
    private float timeEntreCorteMin = 0.5f;
    private float timerDisparandoCorte = 0f;
    private float timerDisparandoCorteMax = 2.5f;
    private bool lanzandoCortes = true;
    private List<Vector3> cortesLocations = new List<Vector3>();
    public GameObject corteLoc1;
    public GameObject corteLoc2;
    public GameObject corteLoc3;
    public GameObject corteLoc4;

    //
    private float tiempoEntreAtaques = 0f;
    private bool vaAAtacar = false;
    private bool cronningNewAttack = true;

    void Start()
    {
        speed = 40f;
        rutina = 0;
        timer = 0f;

        GameObject[] objects = GameObject.FindGameObjectsWithTag("PoscCortes");
        foreach (GameObject obj in objects)
        {
            cortesLocations.Add(obj.transform.position);
        }
    }


    void Update()
    {
        if(cronningNewAttack) {
            tiempoEntreAtaques = Random.Range(1000, 4000) * 0.001f; // Multiplicar por 0.001 para convertir milisegundos a segundos
        }
        if(tiempoEntreAtaques > 0f) {
            tiempoEntreAtaques -= Time.deltaTime;
            print(tiempoEntreAtaques);
        }
        else if(tiempoEntreAtaques <= 0f) {
            vaAAtacar = true;
        }

        if(!attacking && !vaAAtacar) {
            Vector3 center = new Vector3(0f,transform.position.y,0f);
            float angle = speed * Time.deltaTime;
            if(direccionDerecha) angle *= -1f;
            transform.RotateAround(center, Vector3.up, angle);
        }

        if(Vector3.Distance(transform.position, target.transform.position) < 10f && !attacking && vaAAtacar) {
            disparandoFuego = true;
            attacking = true;
        }
        else if(Vector3.Distance(transform.position, target.transform.position) < 16f && !attacking && vaAAtacar) {
            lanzandoCortes = true;
            attacking = true;
        }
        if(disparandoFuego) {
            timerDisparandoFuego += Time.deltaTime;
            timeEntreFuego += Time.deltaTime;
            if(timeEntreFuego >= timeEntreFuegoMin) {
                timeEntreFuego = 0f;
                Lanzallamas();
            }
            if(timerDisparandoFuego > timerDisparandoFuegoMax) {
                timerDisparandoFuego = 0f;
                timeEntreFuego = 0f;
                disparandoFuego = false;
                attacking = false;
                vaAAtacar = false;
                cronningNewAttack = true;
            }
        }
        if(lanzandoCortes) {
            timerDisparandoCorte += Time.deltaTime;
            timeEntreCorte += Time.deltaTime;
            if(timeEntreCorte >= timeEntreCorteMin) {
                timeEntreCorte = 0f;
                Cortar();
            }
            if(timerDisparandoCorte > timerDisparandoCorteMax) {
                timerDisparandoCorte = 0f;
                timeEntreCorte = 0f;
                lanzandoCortes = false;
                attacking = false;
                vaAAtacar = false;
                cronningNewAttack = true;
            }
        }
    }

    private IEnumerator move() {
        return null;
    }

    private void die() {
        _gameController.Setbosskilled();

        Destroy(gameObject);
    }

    public void respawn() {
        healthBar.pintarBarraBoss();
        healthBar.updateHealthBar(shield, shieldMax);
    }

    void lessLive(int damage) {
        shield -= damage;
        if(shield > 0) {
            healthBar.updateHealthBar(shield, shieldMax);
        }
        else {
            live += shield;
            shield = 0;
            healthBar.ShieldDestroyed();
            healthBar.updateHealthBar(live, liveMax);
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

    void Lanzallamas() {
        GameObject nuevoFireObject = Instantiate(fire, gun.transform.position, Quaternion.identity);
        // 'miraDerecha' es un atributo del componente 'bulletScript'
        fireScript fueguito = nuevoFireObject.GetComponent<fireScript>();
        fueguito.direccionDerecha = direccionDerecha;
    }

    void Cortar() {
        int pos = Random.Range(0, 4) + 1;
        Vector3 position = new Vector3(0f,0f,0f);
        bool derecha = false;
        switch(pos) {
            case(1):
                position = corteLoc1.transform.position;
                derecha = false;
                break;
            case(2):
                position = corteLoc2.transform.position;
                derecha = false;
                break;
            case(3):
                position = corteLoc3.transform.position;
                derecha = true;
                break;
            case(4):
                position = corteLoc4.transform.position;
                derecha = true;
                break;
        }
        GameObject nuevoCorteObject = Instantiate(corte, position, Quaternion.identity);
        // 'miraDerecha' es un atributo del componente 'bulletScript'
        bulletScript corteLanzado = nuevoCorteObject.GetComponent<bulletScript>();
        corteLanzado.miraDerecha = derecha;
        corteLanzado.equipedGun = "Corte";
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
