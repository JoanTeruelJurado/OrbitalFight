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

    //dash
    private bool canDash;
    private bool isDashing;
    private float dashingPower;
    private float dashingTimeMax;
    private float dashingTimeTimer;
    private float dashingCooldown;
    private float dashingCooldownTimer = -1f;
    public bool immortal = false;

    //plataformas
    private float tiempoPulsandoJ = 0.0f;
    private float tiempoRequeridoJ = 1.2f;
    private bool subiendoDeNivel = false;

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
    public GameObject puntoDisparo;

    //tema anillos
    private bool ringExterior = true;
    private float altura;

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
        if(isDashing) {
            return;
        }
        // Store starting direction of the player with respect to the axis of the level
        Vector3 center = new Vector3(0,transform.position.y,0);
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
        dashingCooldown = 0.5f;
        dashingPower = 150f;
        alturaPlataformaBoss = boss.transform.position.y-1f;

        _gameController.SetHealth(100);
        _gameController.SetShield(100);

        timeLEFT = 120f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeLEFT <= 0) live = 0; // TIME HAS RUN OUT!

        timeLEFT -= Time.deltaTime;
        _gameController.SetTimer(timeLEFT);

        if (Input.GetKey(KeyCode.I)) live = 0; //DEBUG FOR DEAD ANIMATION // TO ERASE
        animatorFunction();
        if (live == -100) { _gameController.Setplayerkilled(); return; }  // When dead do not compute a thing
        //if (live <= 0) return; 
        CharacterController charControl = GetComponent<CharacterController>();
        Vector3 position;
        
        if(immortalityTime >= 0) {
            immortalityTime += Time.deltaTime;
            if(immortalityTime > immortalityTimeMax) immortalityTime = -1f;
        }

        if(isDashing || subiendoDeNivel) {
            if(isDashing) {
                Dash();
            }
            if(subiendoDeNivel) {
                SubirDeNivel();
            }
            return;
        }

        if(dashingCooldownTimer >= 0.0f) {
            dashingCooldownTimer += Time.deltaTime;
            if(dashingCooldownTimer >= dashingCooldown) {
                canDash = true;
                dashingTimeTimer = -1f;
            }
        }

        if (Input.GetKey(KeyCode.E) && canDash) {
            isDashing = true;
            canDash = false;
            dashingTimeTimer = 0f;
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
                if(!miraDerecha) hayQueGirar = true;
                miraDerecha = true;
            }
            else {
                if(miraDerecha) hayQueGirar = true;
                miraDerecha = false;
            }
            
            target = center + Quaternion.AngleAxis(angle, Vector3.up) * direction;
            
            if (charControl.Move(target - position) != CollisionFlags.None) {
                transform.position = position;
                Physics.SyncTransforms();
            }
        }

        //ajustar orientación
        transform.LookAt(new Vector3(0,transform.position.y,0));
        if(hayQueGirar) {
            Vector3 escalaActual = transform.localScale;
            escalaActual.x *= -1;
            transform.localScale = escalaActual;
            hayQueGirar = false;
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
            if (Input.GetKey(KeyCode.W)) { //jumping
                isJumping = true;
                speedY = jumpSpeed;
            }
        }
        else { speedY -= gravity * Time.deltaTime; }
        

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

        if(Input.GetKey(KeyCode.Z)) { //arma
            armaEquipada = Armas.Fusil;
            tiempoEntreDisparos = 0f;
            _gameController.SetGunSelected(1);
        }
        if(Input.GetKey(KeyCode.X)) { //laser
            armaEquipada = Armas.Pistola;
            tiempoEntreDisparos = 0f;
            _gameController.SetGunSelected(2);
        }
    }

    private void Dash() {
        immortal = true;
        CharacterController charControl = GetComponent<CharacterController>();
        float anglePerStep = dashingPower * Time.deltaTime;
        if(miraDerecha) anglePerStep = -anglePerStep;
        Vector3 center = new Vector3(0,transform.position.y,0);

        if (dashingTimeTimer < dashingTimeMax) {
            Vector3 position = transform.position;
            Vector3 direction = position - center;
            Vector3 target = center + Quaternion.AngleAxis(anglePerStep, Vector3.up) * direction;
            if (charControl.Move(target - position) != CollisionFlags.None) {
                transform.position = position;
                Physics.SyncTransforms();
                dashingTimeTimer = dashingTimeMax;
            }
            else dashingTimeTimer += Time.deltaTime;
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
            //
        }
        else {
            isDashing = false;
            immortal = false;
            //se empieza a contar el cooldown
            dashingCooldownTimer = 0f;
        }
        // canDash = false;
        // isDashing = true;

        // CharacterController charControl = GetComponent<CharacterController>();
        // Vector3 position;
        // float anglePerStep = dashingPower * Time.deltaTime;
        // if(miraDerecha) anglePerStep = -anglePerStep;
        // Vector3 center = new Vector3(0,transform.position.y,0);
        // Vector3 direction;

        // float elapsedTime = 0;
        // while (elapsedTime < dashingTimeTimer) {
        //     position = transform.position;
        //     direction = position - center;
        //     Vector3 target = center + Quaternion.AngleAxis(anglePerStep, Vector3.up) * direction;
        //     if (charControl.Move(target - position) != CollisionFlags.None) {
        //         transform.position = position;
        //         Physics.SyncTransforms();
        //         elapsedTime = dashingTimeTimer;
        //     }
        //     else elapsedTime += Time.deltaTime;
        //     transform.LookAt(new Vector3(0,transform.position.y,0));

        //     //codigo caida
        //     position = transform.position;
        //     if (charControl.Move(speedY * Time.deltaTime * Vector3.up) != CollisionFlags.None)
        //     {
        //         transform.position = position;
        //         Physics.SyncTransforms();
        //     }
        //     if (charControl.isGrounded)
        //     {
        //         if (speedY < 0.0f) speedY = 0.0f;
        //     }
        //     else speedY -= gravity * Time.deltaTime;
        //     /////
            
        //     yield return null; // espera hasta el próximo frame
        // }

        // isDashing = false;
        // yield return new WaitForSecondsRealtime(dashingCooldown);
        // canDash = true;
    }

    private void SubirDeNivel() {
        transform.Translate(Vector3.up * rotationSpeed * Time.deltaTime / 6f);
        if (transform.position.y >= altura * 7f) {
            subiendoDeNivel = false;
            if (TryGetComponent<Collider>(out Collider collider)) collider.enabled = true;
            if(alturaPlataformaBoss < transform.position.y) {
                boss.respawn();
            }
        }
    }

    private void Disparar()
    {
        // Instancia una nueva bala en el centro del jugador
        if(armaEquipada != Armas.Ninguna) {
            GameObject nuevaBalaObject;
            if(armaEquipada == Armas.Fusil) nuevaBalaObject = Instantiate(balaFusil, transform.position, Quaternion.identity);
            else nuevaBalaObject = Instantiate(balaPistola, transform.position, Quaternion.identity);
            Physics.IgnoreCollision(nuevaBalaObject.GetComponent<Collider>(), GetComponent<Collider>());
            // 'miraDerecha' es un atributo del componente 'bulletScript'
            bulletScript balita = nuevaBalaObject.GetComponent<bulletScript>();
            balita.miraDerecha = miraDerecha;
            balita.altura = altura;
            if(armaEquipada == Armas.Fusil) balita.equipedGun = "Fusil";
            else balita.equipedGun = "Pistola";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.tag == "BulletEnemy" || other.gameObject.tag == "Corte") {
            if(!immortal) {
                bulletScript scriptBullet = other.gameObject.GetComponent<bulletScript>();
                if (scriptBullet != null){
                    int damage = scriptBullet.damageHit;
                    lessLive(damage);
                }
            }
        }
        else if(other.gameObject.tag == "Jumper" || other.gameObject.tag == "ChangerRing") {
            tiempoPulsandoJ = 0.0f;
        }
        else if(other.gameObject.tag == "Fire") {
            if(!immortal) {
                lessLive(5);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Jumper") {
            if (Input.GetKey(KeyCode.J))
            {
                tiempoPulsandoJ += Time.deltaTime;
                if (tiempoPulsandoJ >= tiempoRequeridoJ) {
                    subiendoDeNivel = true;
                    altura += 1f;
                    if (TryGetComponent<Collider>(out Collider collider)) collider.enabled = false;
                }
                // {
                //     Vector3 position = transform.position;
                //     position.y += 6.1f;
                //     altura += 1f;
                //     transform.position = position;

                //     //control boss
                //     if(alturaPlataformaBoss < position.y) {
                //         boss.respawn();
                //     }
                // }
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

    void lessLive(int damage) {
        if(immortalityTime == -1f) { //no está en tiempo de immortalidad
            shield -= damage;
            _gameController.SetShield(shield);
            if (shield < 0) {
                live += shield;
                _gameController.SetHealth(live);
                shield = 0;
                if(live < 0) { //muere
                    live = 0;
                }
            }
            print(shield);
            print(live);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy") {
            print("Hit");
            lessLive(10);
        }
    }

    private void animatorFunction() {
        bool isWalking = _animator.GetBool("isWalking");
        bool ADpressed = Input.GetKey(KeyCode.A);
        ADpressed |= Input.GetKey(KeyCode.D);
        bool isAlreadyDashing = _animator.GetBool("isDashing");
        bool isAlreadyJumping = _animator.GetBool("isJumping");

        if (!isWalking && ADpressed) { _animator.SetBool("isWalking", true); }
        if (isWalking && !ADpressed) { _animator.SetBool("isWalking", false); }
        
        if (live == -100) { _animator.SetBool("isDead", false);}
        else if (live <= 0) { _animator.SetBool("isDead", true); live = -100; }
        
        if (isAlreadyDashing && !isDashing) { _animator.SetBool("isDashing", false); }
        if (!isAlreadyDashing && isDashing) { _animator.SetBool("isDashing", true); }

        if (isAlreadyJumping && !isJumping) { _animator.SetBool("isJumping", false); }
        if (!isAlreadyJumping && isJumping) { _animator.SetBool("isJumping", true); }
    }      
}



