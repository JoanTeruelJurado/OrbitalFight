using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossEnemy : MonoBehaviour
{
    public GameController _gameController;

    private Vector3 center;
    private float speed;
    private int rutina;
    private float timer;
    private bool armorActive = true;
    
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
    private MovePlayer movePlayer;

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
    private bool lanzandoCortes = false;
    private List<Vector3> cortesLocations = new List<Vector3>();
    public GameObject corteLoc1;
    public GameObject corteLoc2;
    public GameObject corteLoc3;
    public GameObject corteLoc4;

    //dashing
    private bool dashing = false;
    private float timerDashing = 0f;
    private float timerDashingMax = 0.9f;
    private float dashingSpeed;
    private float dashCooldown = 2f;
    private float dashTimerCooldown = -1f;
    private float dashSpeed = 130f;

    //
    private float tiempoEntreAtaques = 0f;
    private bool vaAAtacar = false;
    private bool cronningNewAttack = true;

    //sounds
    public AudioSource audioSource;
    public AudioClip movement;
    public AudioClip flamethrower;
    public AudioSource _MovementaudioSource;
    public AudioClip dashSound;
    public AudioClip corteSound;

    //animations
    public Animator _animator;
    private bool isWalking = false;

    //calcular distancia player
    public GameObject delante;
    public GameObject detras;

    //animación entrada
    private bool isAnimacionEntrada = true;
    private Rigidbody miRigidbody;
    private bool isDead = false;
    void Start()
    {
        isDead = false;
        speed = 40f;
        rutina = 0;
        timer = 0f;
        movePlayer = target.GetComponent<MovePlayer>();
        center = new Vector3(0f, transform.position.y, 0f);
        miRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDead) { return; }
        if(mismaAltura()) {
            if(isAnimacionEntrada) {
                //block player
                MovePlayer player = target.GetComponent<MovePlayer>();
                player.block();
                miRigidbody.useGravity = true;
            }
            else {
                if (!_MovementaudioSource.isPlaying) _MovementaudioSource.Play();
                isWalking = false;
                float distanciaAlPlayer = Vector3.Distance(transform.position, target.transform.position);
                if (dashing){
                    isWalking = true;
                    timerDashing += Time.deltaTime;
                    if (timerDashing >= timerDashingMax)
                    {
                        dashing = false;
                        dashTimerCooldown = 0f;
                        return;
                    }
                    if (distanciaAlPlayer > 5f)
                    {
                        Vector3 center = new Vector3(0f, transform.position.y, 0f);
                        float angle = dashSpeed * Time.deltaTime;
                        if (direccionDerecha) angle *= -1f;
                        transform.RotateAround(center, Vector3.up, angle);
                    }
                    else
                    {
                        dashing = false;
                        dashTimerCooldown = 0f;
                    }
                    return;
                }
                else {
                    if(dashTimerCooldown >= 0f) {
                        dashTimerCooldown += Time.deltaTime;
                        if(dashTimerCooldown >= dashCooldown) {
                            dashTimerCooldown = -1f;
                        }
                    }
                    if (cronningNewAttack)
                    {
                        tiempoEntreAtaques = Random.Range(1, 4);
                        cronningNewAttack = false;
                    }
                    if (tiempoEntreAtaques > 0f)
                    {
                        tiempoEntreAtaques -= Time.deltaTime;
                    }
                    else if (tiempoEntreAtaques <= 0f)
                    {
                        vaAAtacar = true;
                    }

                    if (!attacking && !vaAAtacar)
                    {
                        if (distanciaAlPlayer > 4f)
                        {
                            if(!_MovementaudioSource.isPlaying) _MovementaudioSource.Play();
                            isWalking = true;
                            Vector3 center = new Vector3(0f, transform.position.y, 0f);
                            float angle = speed * Time.deltaTime;
                            if (direccionDerecha) angle *= -1f;
                            transform.RotateAround(center, Vector3.up, angle);
                        }
                    }

                    if (distanciaAlPlayer < 6f && !attacking && vaAAtacar)
                    {
                        disparandoFuego = true;
                        attacking = true;
                        bool aux = direccionDerecha;
                        if(Vector3.Distance(detras.transform.position, target.transform.position) < Vector3.Distance(delante.transform.position, target.transform.position)) {
                            direccionDerecha = !direccionDerecha;
                        }
                        if(aux != direccionDerecha) { //se gira si lo detecta en el otro sentido
                            girar();
                        }
                    }
                    else if (distanciaAlPlayer >= 6f && distanciaAlPlayer <= 10f && !attacking && vaAAtacar)
                    {
                        lanzandoCortes = true;
                        attacking = true;
                    }
                    else if (distanciaAlPlayer > 10f && !attacking && dashTimerCooldown == -1f) // && !vaAAtacar
                    {
                        MovePlayer playerScript = target.GetComponent<MovePlayer>();
                        playerScript.reproducirSonido("dashBossSound");
                        //audioSource.PlayOneShot(dashSound);
                        dashing = true;
                        timerDashing = 0f;
                    }


                    if (disparandoFuego)
                    {
                        timerDisparandoFuego += Time.deltaTime;
                        timeEntreFuego += Time.deltaTime;
                        if (timeEntreFuego >= timeEntreFuegoMin)
                        {
                            timeEntreFuego = 0f;
                            Lanzallamas();
                        }
                        if (timerDisparandoFuego > timerDisparandoFuegoMax)
                        {
                            timerDisparandoFuego = 0f;
                            timeEntreFuego = 0f;
                            disparandoFuego = false;
                            attacking = false;
                            vaAAtacar = false;
                            cronningNewAttack = true;
                        }

                    }
                    else audioSource.Pause();

                    if (lanzandoCortes)
                    {
                        timerDisparandoCorte += Time.deltaTime;
                        timeEntreCorte += Time.deltaTime;
                        if (timeEntreCorte >= timeEntreCorteMin)
                        {
                            timeEntreCorte = 0f;
                            Cortar();
                        }
                        if (timerDisparandoCorte > timerDisparandoCorteMax)
                        {
                            timerDisparandoCorte = 0f;
                            timeEntreCorte = 0f;
                            lanzandoCortes = false;
                            attacking = false;
                            vaAAtacar = false;
                            cronningNewAttack = true;
                        }
                    }
                }
                
                animatorFunction();
            }
        }
    }

    private void girar() {
        Vector3 aux1 = delante.transform.position;
        Vector3 aux2 = detras.transform.position;

        Vector3 escalaActual = transform.localScale;
        escalaActual.x *= -1;
        transform.localScale = escalaActual;

        delante.transform.position = aux2;
        detras.transform.position = aux1;
    }

    private bool mismaAltura() {
        return Mathf.Abs(transform.position.y - target.transform.position.y) < 15f;
    }

    private void animatorFunction()
{
   // if (isWalking && !_MovementaudioSource.isPlaying) _MovementaudioSource.Play();
    //else _MovementaudioSource.Pause();
    
    bool isAlreadyWalking = _animator.GetBool("isWalking");
    bool isAlreadyDashing = _animator.GetBool("isDashing");


    if (!isWalking && isAlreadyWalking) { _animator.SetBool("isWalking", false); }
    if (isWalking && !isAlreadyWalking) { _animator.SetBool("isWalking", true); }

    if (isAlreadyDashing && !dashing) { _animator.SetBool("isDashing", false); }
    if (!isAlreadyDashing && dashing) { _animator.SetBool("isDashing", true); }

}

    private IEnumerator move()
    {
        return null;
    }

    public bool GetisDead() { return isDead;  }

    private void die()
    {
        isDead = true;
        _gameController.Setbosskilled();

        MovePlayer playerScript = target.GetComponent<MovePlayer>();
        playerScript.reproducirSonido("victory");
        _animator.SetTrigger("isDead");
        disparandoFuego = false;
        lanzandoCortes = false;
        _MovementaudioSource.Pause();
    }

    public void respawn()
    {
        healthBar.pintarBarraBoss();
        healthBar.updateHealthBar(shield, shieldMax);
    }

    void lessLive(int damage)
    {
        shield -= damage;
        if (shield > 0)
        {
            healthBar.updateHealthBar(shield, shieldMax);
        }
        if(shield <= 0 && armorActive) {
            MovePlayer playerScript = target.GetComponent<MovePlayer>();
            playerScript.reproducirSonido("dashBossSound");
        }
        if(shield <= 0)
        {
            armorActive = false;
            live += shield;
            shield = 0;
            healthBar.ShieldDestroyed();
            healthBar.updateHealthBar(live, liveMax);
            if (live < 0)
            { //muere
                live = 0;
                if (!isDead) die();
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Entorno" && isAnimacionEntrada) 
        {
            MovePlayer playerScript = target.GetComponent<MovePlayer>();
            playerScript.reproducirSonido("caidaBoss");
            Invoke("desactivarAnimacionEntrada", 5f);
        }
    }

    private void desactivarAnimacionEntrada() {
        isAnimacionEntrada = false;
        //unblock player
        MovePlayer player = target.GetComponent<MovePlayer>();
        player.unblock();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "BulletPlayer")
        {
            bulletScript scriptBullet = collider.gameObject.GetComponent<bulletScript>();
            if (scriptBullet != null)
            {
                int damage = scriptBullet.damageHit;
                lessLive(damage);
            }
        }
    }

    void Lanzallamas()
    {
        if(_MovementaudioSource.isPlaying) _MovementaudioSource.Pause();
        if (!audioSource.isPlaying) audioSource.PlayOneShot(flamethrower);
       
        
        GameObject nuevoFireObject = Instantiate(fire, gun.transform.position, Quaternion.identity);
        // 'miraDerecha' es un atributo del componente 'bulletScript'
        fireScript fueguito = nuevoFireObject.GetComponent<fireScript>();
        fueguito.direccionDerecha = direccionDerecha;
    }

    void Cortar()
    {
        if(_MovementaudioSource.isPlaying) _MovementaudioSource.Pause();
        int pos = Random.Range(0, 4) + 1;
        Vector3 position = new Vector3(0f, 0f, 0f);
        bool derecha = false;
        switch (pos)
        {
            case (1):
                position = corteLoc1.transform.position;
                derecha = false;
                break;
            case (2):
                position = corteLoc2.transform.position;
                derecha = false;
                break;
            case (3):
                position = corteLoc3.transform.position;
                derecha = true;
                break;
            case (4):
                position = corteLoc4.transform.position;
                derecha = true;
                break;
        }
        MovePlayer playerScript = target.GetComponent<MovePlayer>();
        playerScript.reproducirSonido("corteBossSound");
        //audioSource.PlayOneShot(corteSound);
        GameObject nuevoCorteObject = Instantiate(corte, position, Quaternion.identity);
        // 'miraDerecha' es un atributo del componente 'bulletScript'
        bulletScript corteLanzado = nuevoCorteObject.GetComponent<bulletScript>();
        corteLanzado.miraDerecha = derecha;
        corteLanzado.equipedGun = "Corte";
    }

}