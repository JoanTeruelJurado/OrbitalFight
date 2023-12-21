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

    void Start()
    {
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
                direccionDerecha = Random.Range(0,2) == 0 ? false : true;
                //StartCoroutine(move());
                rutina = 1;
            }
        }
        Vector3 center = new Vector3(0f,transform.position.y,0f);
        float angle = speed * Time.deltaTime;
        if(direccionDerecha) angle *= -1f;
        transform.RotateAround(center, Vector3.up, angle);
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
