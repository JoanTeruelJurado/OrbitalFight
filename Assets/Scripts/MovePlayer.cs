using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    //animator
    public Animator _animator;
    //
    public GameController _gameController;

    public float rotationSpeed, jumpSpeed, gravity, radius;
    const float radioInterior = 3.5f;
    const float radioExterior = 6.77f;
    public bool miraDerecha = true;
    private bool hayQueGirar = false;
    Vector3 startDirection;
    float speedY;
    private bool isJumping;
    private bool seEstaPulsandoW = false;
    private bool godMode = false;
    private bool seEstaPulsandoG = false;
    private bool playerIsBlocked = false;

    //dash
    private bool canDash;
    private bool isDashing;
    private float dashingPower;
    private float dashingTimeMax;
    private float dashingTimeTimer;
    private float dashingCooldown;
    private float dashingCooldownTimer;
    public bool immortal = false;

    //plataformas
    private float tiempoPulsandoJ = 0.0f;
    private float tiempoRequeridoJ = 1.2f;
    private bool subiendoDeNivel = false;

    //shooting
    public GameObject balaPistola;
    public GameObject balaFusil;
    private bool fusilDesbloqueado = true;
    private enum Armas { Ninguna, Fusil, Pistola };
    private Armas armaEquipada = Armas.Pistola;
    private float tiempoEntreDisparosMinFusil = 0.3f;
    private float tiempoEntreDisparosMinPistola = 0.7f;
    private float tiempoEntreDisparos = 0f;
    public GameObject puntoDisparo;

    //munición
    private int municionPistolaCargadorAct = 6;
    private int municionFusilCargadorAct = 20;
    private int municionPistolaCargadorMax = 6;
    private int municionFusilCargadorMax = 20;
    private int municionPistolaRestante = 60; //60
    private int municionPistolaMaxima; //60
    private int municionFusilRestante = 100; //100
    private int municionFusilMaxima; //100
    private float timerRecarga = -1f;
    private float timeRecargaPistola = 0.8f;
    private float timeRecargaFusil = 1.8f;
    private bool canDisparar = true;
    private bool seEstaPulsandoQ = false;
    private bool seEstaPulsandoM = false;


    //sounds
    private AudioSource audioSource;
    public AudioClip blasterShot;
    public AudioClip rifleShot;
    public AudioClip changeGun;
    public AudioClip emptyMagazineShoot;
    public AudioClip reloadRifle;
    public AudioClip reloadPistol;
    public AudioClip changeSameHighRing;
    public AudioClip changeTopHighRing;
    public AudioSource musicAudioSource;
    public AudioClip destroyBullet;
    public AudioClip enemyDie;
    public AudioClip sparks;
    public AudioClip dashBossSound;
    public AudioClip corteBossSound;
    public AudioClip brakeBossSound;
    public AudioClip gameOver;
    public AudioClip victory;
    public AudioClip caidaBoss;
    public AudioClip gritoBoss;

    //tema anillos
    private bool ringExterior = true;
    private float altura;
    public GameObject barraPressingJGO;
    private PressingJ barraPressingJ;

    //interaccion
    private int shield = 100;
    private int live = 100;
    private float immortalityTimeMax = 2f;
    private float immortalityTime = -1f;

    //control arena boss
    private float alturaPlataformaBoss;
    public bossEnemy boss;

    //temporizador
    private float timeLEFT = 120f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (isDashing)
        {
            return;
        }
        // Store starting direction of the player with respect to the axis of the level
        Vector3 center = new Vector3(0, transform.position.y, 0);
        startDirection = transform.position - center;
        startDirection.y = 0.0f;
        startDirection.Normalize();
        isJumping = false;
        speedY = 0;
        radius = radioExterior;
        altura = 0f;

        canDash = true;
        isDashing = false;
        //dashingPower = 100f;
        dashingTimeTimer = 0f;
        dashingTimeMax = 0.5f;
        dashingCooldownTimer = -1f;
        dashingCooldown = 0.5f;
        dashingPower = 150f;
        alturaPlataformaBoss = 3;

        municionPistolaMaxima = municionPistolaRestante;
        municionFusilMaxima = municionFusilRestante;

        _gameController.SetHealth(100);
        _gameController.SetShield(100);

        timeLEFT = 120f;

        barraPressingJ = barraPressingJGO.GetComponent<PressingJ>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!playerIsBlocked) {
            if (timeLEFT <= 0) live = 0; // TIME HAS RUN OUT!

            timeLEFT -= Time.deltaTime;
            _gameController.SetTimer(timeLEFT);

            if (Input.GetKey(KeyCode.I)) live = 0; //DEBUG FOR DEAD ANIMATION
            animatorFunction();
            if (live == -100) { _gameController.Setplayerkilled(); audioSource.PlayOneShot(gameOver); return; }  // When dead do not compute a thing
            if (live <= 0) return;
            
        // if (Alreadydead) audioSource.PlayOneShot(gameOver);

            CharacterController charControl = GetComponent<CharacterController>();
            Vector3 position;

            if (immortalityTime >= 0)
            {
                immortalityTime += Time.deltaTime;
                if (immortalityTime > immortalityTimeMax) immortalityTime = -1f;
            }

            if (dashingCooldownTimer >= 0.0f)
            {
                dashingCooldownTimer += Time.deltaTime;
                if (dashingCooldownTimer >= dashingCooldown)
                {
                    canDash = true;
                    dashingCooldownTimer = -1f;
                }
            }

            if (isDashing || subiendoDeNivel)
            {
                if (isDashing)
                {
                    Dash();
                }
                if (subiendoDeNivel)
                {
                    SubirDeNivel();
                }
                return;
            }


            if (Input.GetKey(KeyCode.E) && canDash)
            {
                isDashing = true;
                canDash = false;
                dashingTimeTimer = 0f;
            }

            //recarga timer
            if (timerRecarga >= 0f)
            {
                timerRecarga += Time.deltaTime;
                float tiempoRecargaMin = 99f;
                if (armaEquipada == Armas.Fusil) tiempoRecargaMin = timeRecargaFusil;
                else if (armaEquipada == Armas.Pistola) tiempoRecargaMin = timeRecargaPistola;

                if (timerRecarga >= tiempoRecargaMin)
                {
                    timerRecarga = -1f;
                    canDisparar = true;
                    _animator.ResetTrigger("isReloading");
                }
            }


            //recarga botón
            if (Input.GetKey(KeyCode.R) && canDisparar)
            {
                Recarga();
                _animator.SetTrigger("isReloading");
            }


            //cambio de arma
            if (Input.GetKey(KeyCode.Q) && canDisparar && !seEstaPulsandoQ)
            {
                if (fusilDesbloqueado)
                {
                    if (armaEquipada == Armas.Pistola)
                    {
                        armaEquipada = Armas.Fusil;
                        _gameController.SetGunSelected(1);
                    }
                    else if (armaEquipada == Armas.Fusil)
                    {
                        armaEquipada = Armas.Pistola;
                        _gameController.SetGunSelected(2);
                    }
                    audioSource.PlayOneShot(changeGun);
                    tiempoEntreDisparos = 0f;
                }
                seEstaPulsandoQ = true;
            }
            if (seEstaPulsandoQ && !Input.GetKey(KeyCode.Q)) seEstaPulsandoQ = false;

            // Left-right movement
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                float angle;
                Vector3 direction, target;

                position = transform.position;
                angle = rotationSpeed * Time.deltaTime;
                Vector3 center = new Vector3(0, transform.position.y, 0);
                direction = position - center;

                if (Input.GetKey(KeyCode.D))
                {
                    angle = -angle;
                    if (!miraDerecha) hayQueGirar = true;
                    miraDerecha = true;
                }
                else
                {
                    if (miraDerecha) hayQueGirar = true;
                    miraDerecha = false;
                }

                target = center + Quaternion.AngleAxis(angle, Vector3.up) * direction;

                if (charControl.Move(target - position) != CollisionFlags.None)
                {
                    transform.position = position;
                    Physics.SyncTransforms();
                }
            }

            //ajustar orientación
            transform.LookAt(new Vector3(0, transform.position.y, 0));
            if (hayQueGirar)
            {
                Vector3 escalaActual = transform.localScale;
                escalaActual.x *= -1;
                transform.localScale = escalaActual;
                hayQueGirar = false;

                barraPressingJ.hayQueGirar();
            }

            // Apply up-down movement
            position = transform.position;
            if (charControl.Move(speedY * Time.deltaTime * Vector3.up) != CollisionFlags.None)
            {
                transform.position = position;
                Physics.SyncTransforms();
            }
            isJumping = false;
            if (charControl.isGrounded)
            {
                isJumping = false;
                if (speedY < 0.0f) speedY = 0.0f;
                if (Input.GetKey(KeyCode.W) && !seEstaPulsandoW)
                { //jumping
                    isJumping = true;
                    speedY = jumpSpeed;
                    seEstaPulsandoW = true;
                }
            }
            else { speedY -= gravity * Time.deltaTime; }
            if (seEstaPulsandoW && !Input.GetKey(KeyCode.W)) seEstaPulsandoW = false;

            //Atajos
            if (Input.GetKey(KeyCode.G) && !seEstaPulsandoG)
            { //god mode
                godMode = !godMode;
                seEstaPulsandoG = true;
                _gameController.SetGodMode(godMode);
            }
            if (seEstaPulsandoG && !Input.GetKey(KeyCode.G)) seEstaPulsandoG = false;

            if (Input.GetKey(KeyCode.M) && !seEstaPulsandoM)
            { //max ammo
                municionPistolaCargadorAct = municionPistolaCargadorMax;
                municionFusilCargadorAct = municionFusilCargadorMax;
                municionPistolaRestante = municionPistolaMaxima;
                municionFusilRestante = municionFusilMaxima;
                seEstaPulsandoM = true;
                _gameController.SetReloaded();
            }
            if (seEstaPulsandoM && !Input.GetKey(KeyCode.M)) seEstaPulsandoM = false;

            //Disparar
            tiempoEntreDisparos += Time.deltaTime;
            if ((Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.K)) && canDisparar) // 0 representa el botón izquierdo del ratón
            {
                if (armaEquipada == Armas.Fusil && tiempoEntreDisparos > tiempoEntreDisparosMinFusil)
                {
                    Disparar();
                    tiempoEntreDisparos = 0f;

                    _animator.SetInteger("Firing", 2);
                }
                else if (armaEquipada == Armas.Pistola && tiempoEntreDisparos > tiempoEntreDisparosMinPistola)
                {
                    Disparar();
                    tiempoEntreDisparos = 0f;
                    _animator.SetInteger("Firing", 1);
                }

            }
            else _animator.SetInteger("Firing", 0);

            switch (armaEquipada)
            {
                case Armas.Fusil:
                    _gameController.SetAmmoMag(municionFusilCargadorAct, municionFusilRestante);
                    break;
                case Armas.Pistola:
                    _gameController.SetAmmoMag(municionPistolaCargadorAct, municionPistolaRestante);
                    break;
            }
        }
        
    }

    private void Dash()
    {
        immortal = true;
        CharacterController charControl = GetComponent<CharacterController>();
        float anglePerStep = dashingPower * Time.deltaTime;
        if (miraDerecha) anglePerStep = -anglePerStep;
        Vector3 center = new Vector3(0, transform.position.y, 0);

        if (dashingTimeTimer < dashingTimeMax)
        {
            Vector3 position = transform.position;
            Vector3 direction = position - center;
            Vector3 target = center + Quaternion.AngleAxis(anglePerStep, Vector3.up) * direction;
            if (charControl.Move(target - position) != CollisionFlags.None)
            {
                transform.position = position;
                Physics.SyncTransforms();
                dashingTimeTimer = dashingTimeMax;
            }
            else dashingTimeTimer += Time.deltaTime;
            transform.LookAt(new Vector3(0, transform.position.y, 0));

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
            //
        }
        else
        {
            isDashing = false;
            immortal = false;
            //se empieza a contar el cooldown
            dashingCooldownTimer = 0f;
        }
    }

    private void SubirDeNivel()
    {
        transform.Translate(Vector3.up * rotationSpeed * Time.deltaTime / 6f);
        if (transform.position.y >= altura * 7f)
        {
            subiendoDeNivel = false;
            if (TryGetComponent<Collider>(out Collider collider)) collider.enabled = true;
            if (altura >= alturaPlataformaBoss)
            {
                //boss.respawn();
            }
        }
    }

    public void lessLive(int damage)
    {
        if (immortalityTime == -1f && !godMode)
        { //no está en tiempo de immortalidad
            audioSource.PlayOneShot(sparks);
            shield -= damage;
            _gameController.SetShield(shield);
            if (shield < 0)
            {
                live += shield;
                _gameController.SetHealth(live);
                shield = 0;
                if (live <= 0) return;
            }
        }
    }

    private void Recarga()
    {
        if (armaEquipada == Armas.Pistola)
        {
            if (municionPistolaRestante > 0)
            {
                int auxLasQueNoHaUsado = municionPistolaCargadorAct;
                int cuantasBalasCoge = municionPistolaCargadorMax;
                if (municionPistolaRestante - municionPistolaCargadorMax + auxLasQueNoHaUsado < 0)
                {
                    cuantasBalasCoge = municionPistolaRestante;
                }
                municionPistolaCargadorAct = cuantasBalasCoge;
                municionPistolaRestante = municionPistolaRestante - cuantasBalasCoge + auxLasQueNoHaUsado;
                audioSource.PlayOneShot(reloadPistol);
            }
        }
        else if (armaEquipada == Armas.Fusil)
        {
            if (municionFusilRestante > 0)
            {
                int auxLasQueNoHaUsado = municionFusilCargadorAct;
                int cuantasBalasCoge = municionFusilCargadorMax;
                if (municionFusilRestante - municionFusilCargadorMax + auxLasQueNoHaUsado < 0)
                {
                    cuantasBalasCoge = municionFusilRestante;
                }
                municionFusilCargadorAct = cuantasBalasCoge;
                municionFusilRestante = municionFusilRestante - cuantasBalasCoge + auxLasQueNoHaUsado;
                audioSource.PlayOneShot(reloadRifle);
            }
        }
        canDisparar = false;
        timerRecarga = 0f;
    }

    private void Disparar()
    {
        bool tieneMunicion = true;

        //si no tiene munición no puede disparar
        if (armaEquipada != Armas.Ninguna)
        {
            if (armaEquipada == Armas.Fusil)
            {
                if (municionFusilCargadorAct == 0)
                {
                    tieneMunicion = false;
                    audioSource.PlayOneShot(emptyMagazineShoot);
                }
            }
            else if (armaEquipada == Armas.Pistola)
            {
                if (municionPistolaCargadorAct == 0)
                {
                    tieneMunicion = false;
                    audioSource.PlayOneShot(emptyMagazineShoot);
                }
            }
        }

        if (armaEquipada != Armas.Ninguna && tieneMunicion)
        {
            GameObject nuevaBalaObject;
            if (armaEquipada == Armas.Fusil)
            {
                AudioSource.PlayClipAtPoint(rifleShot, transform.position, 0.5f);
                nuevaBalaObject = Instantiate(balaFusil, puntoDisparo.transform.position, Quaternion.identity);
                --municionFusilCargadorAct;
                if (municionFusilCargadorAct == 0)
                {
                    Recarga();
                }
            }
            else
            {
                AudioSource.PlayClipAtPoint(blasterShot, transform.position, 0.5f);
                nuevaBalaObject = Instantiate(balaPistola, puntoDisparo.transform.position, Quaternion.identity);
                --municionPistolaCargadorAct;
                if (municionPistolaCargadorAct == 0) Recarga();
            }
            Physics.IgnoreCollision(nuevaBalaObject.GetComponent<Collider>(), GetComponent<Collider>());
            // 'miraDerecha' es un atributo del componente 'bulletScript'
            bulletScript balita = nuevaBalaObject.GetComponent<bulletScript>();
            balita.miraDerecha = miraDerecha;
            balita.altura = altura;
            if (armaEquipada == Armas.Fusil) balita.equipedGun = "Fusil";
            else balita.equipedGun = "Pistola";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BulletEnemy" || other.gameObject.tag == "Corte")
        {
            if (!immortal)
            {
                bulletScript scriptBullet = other.gameObject.GetComponent<bulletScript>();
                if (scriptBullet != null)
                {
                    int damage = scriptBullet.damageHit;
                    lessLive(damage);
                }
            }
        }
        else if (other.gameObject.tag == "Jumper" || other.gameObject.tag == "ChangerRing")
        {
            tiempoPulsandoJ = 0.0f;
            barraPressingJ.pintarBarraPressingJ("Press J to jump");
        }
        else if (other.gameObject.tag == "Cofre") {
            tiempoPulsandoJ = 0.0f;
            if (!other.GetComponent<Cofre>().GetEmpty()) barraPressingJ.pintarBarraPressingJ("Press J to open");
        }
        else if (other.gameObject.tag == "Fire")
        {
            if (!immortal)
            {
                lessLive(5);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Jumper")
        {
            if (Input.GetKey(KeyCode.J))
            {
                tiempoPulsandoJ += Time.deltaTime;
                barraPressingJ.updateHealthBar(tiempoPulsandoJ, tiempoRequeridoJ);
                if (tiempoPulsandoJ >= tiempoRequeridoJ)
                {
                    audioSource.PlayOneShot(changeTopHighRing);
                    subiendoDeNivel = true;
                    altura += 1f;
                    barraPressingJ.updateHealthBar(0f, tiempoRequeridoJ);
                    barraPressingJ.esconderBarraPressingJ();
                    if (TryGetComponent<Collider>(out Collider collider)) collider.enabled = false;
                }
            }
            else
            {
                barraPressingJ.updateHealthBar(0f, tiempoRequeridoJ);
                tiempoPulsandoJ = 0.0f;
            }
        }
        else if (other.gameObject.tag == "ChangerRing")
        {
            if (Input.GetKey(KeyCode.J))
            {
                tiempoPulsandoJ += Time.deltaTime;
                barraPressingJ.updateHealthBar(tiempoPulsandoJ, tiempoRequeridoJ);
                if (tiempoPulsandoJ >= tiempoRequeridoJ)
                {
                    audioSource.PlayOneShot(changeSameHighRing);
                    ringExterior = !ringExterior;
                    barraPressingJ.updateHealthBar(0f, tiempoRequeridoJ);
                    barraPressingJ.esconderBarraPressingJ();

                    Vector3 center = new Vector3(0f, transform.position.y, 0f);
                    float distanciaSalto = ringExterior ? 3.3f : -3.3f;
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
        }
        else if (other.gameObject.tag == "Cofre")
        {
            if (Input.GetKey(KeyCode.J))
            {
                tiempoPulsandoJ += Time.deltaTime;
                barraPressingJ.updateHealthBar(tiempoPulsandoJ, tiempoRequeridoJ);
                if (tiempoPulsandoJ >= tiempoRequeridoJ)
                {
                    tiempoPulsandoJ = 0.0f;
                    
                    int tipodecofre = other.GetComponent<Cofre>().GetTipus();
                    barraPressingJ.esconderBarraPressingJ();
                    switch (tipodecofre)
                    {
                        case 0: //cofre de vida
                            live = 100;
                            shield = 100;

                            break;
                        case 1: //cofre de munición
                            municionFusilRestante = municionFusilMaxima;
                            municionPistolaRestante = municionPistolaMaxima;
                            _gameController.SetReloaded();
                            break;
                        default: break;
                    }
                }
            }
        }
        else {
            barraPressingJ.updateHealthBar(0f, tiempoRequeridoJ);
            tiempoPulsandoJ = 0.0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Jumper" || other.gameObject.tag == "ChangerRing" || other.gameObject.tag == "Cofre")
        {
            barraPressingJ.esconderBarraPressingJ();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            print("Hit");
            lessLive(30);
        }
    }

    public void reproducirSonido(string sonido)
    {
        if (sonido == "destroyBullet")
        {
            audioSource.PlayOneShot(destroyBullet);
        }
        else if (sonido == "enemyDie")
        {
            audioSource.PlayOneShot(enemyDie);
        }
        else if (sonido == "dashBossSound")
        {
            audioSource.PlayOneShot(dashBossSound);
        }
        else if (sonido == "corteBossSound")
        {
            audioSource.PlayOneShot(corteBossSound);
        }
        else if (sonido == "brakeBossSound")
        {
            audioSource.PlayOneShot(brakeBossSound);
        }
        else if (sonido == "victory")
        {
            audioSource.PlayOneShot(victory);
        }
        else if(sonido == "caidaBoss") {
            audioSource.PlayOneShot(caidaBoss);
            Invoke("LlamarRugido", 1.7f);
        }
        else if(sonido == "gritoBoss") {
            audioSource.PlayOneShot(gritoBoss);
        }
    }

    private void LlamarRugido() {
        reproducirSonido("gritoBoss");
    }

    public void block() {
        boss.respawn();
        playerIsBlocked = true;
    }

    public void unblock() {
        playerIsBlocked = false;
    }

    private void animatorFunction()
    {
        bool isWalking = _animator.GetBool("isWalking");
        bool ADpressed = Input.GetKey(KeyCode.A);
        ADpressed |= Input.GetKey(KeyCode.D);
        bool isAlreadyDashing = _animator.GetBool("isDashing");
        bool isAlreadyJumping = _animator.GetBool("isJumping");

        if (!isWalking && ADpressed) { _animator.SetBool("isWalking", true); }
        if (isWalking && !ADpressed) { _animator.SetBool("isWalking", false); }

        if (live == -100) { _animator.SetBool("isDead", false); live = -200; }
        else if (live <= 0 && live > -100) { _animator.SetBool("isDead", true);  live = -100; }

        if (isAlreadyDashing && !isDashing) { _animator.SetBool("isDashing", false); }
        if (!isAlreadyDashing && isDashing) { _animator.SetBool("isDashing", true); }

        if (isAlreadyJumping && !isJumping) { _animator.SetBool("isJumping", false); }
        if (!isAlreadyJumping && isJumping) { _animator.SetBool("isJumping", true); }

        if (live <= 0) {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isJumping", false);
            _animator.SetBool("isWalking", false);
        }

    }
}



