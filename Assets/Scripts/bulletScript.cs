using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public float rotationSpeed;
    public bool miraDerecha;
    public float altura;
    public string equipedGun;
    private float tiempoVidaAct;
    public int damageHit;
    private Vector3 center;
    private GameObject player;

    void Start() {
        float tiempoVidaMax = 7f;
        if(equipedGun == "Fusil") {
            tiempoVidaMax = 2f;
            damageHit = 20;
        }
        else if(equipedGun == "Pistola") {
            tiempoVidaMax = 0.3f;
            damageHit = 35;
        }
        else if(equipedGun == "Corte") {
            tiempoVidaMax = 1.7f;
            damageHit = 40;
            rotationSpeed = 120f;
        }
        else {
            tiempoVidaMax = 1.5f;
            damageHit = 40;
        }

        //rotationSpeed = 100f;
        Destroy(gameObject, tiempoVidaMax);
        center = new Vector3(0f,transform.position.y,0f);
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        float angle = rotationSpeed * Time.deltaTime;
        if(miraDerecha) angle *= -1f;
        transform.RotateAround(center, Vector3.up, angle);

        //rotando la bala como toca
        transform.LookAt(new Vector3(0,transform.position.y,0));
        Quaternion rotacionActual = transform.rotation;
        Quaternion nuevaRotacion = Quaternion.Euler(rotacionActual.eulerAngles + new Vector3(0, 0, 90f));
        transform.rotation = nuevaRotacion;

        if(equipedGun == "Corte") {
            rotacionActual = transform.rotation;
            nuevaRotacion = Quaternion.Euler(rotacionActual.eulerAngles + new Vector3(0f, 0, 90f));
            transform.rotation = nuevaRotacion;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        // Destruye la bala cuando colisiona con otro objeto
        if(gameObject.tag == "BulletPlayer" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Cofre" && collision.gameObject.tag != "Trampa" && collision.gameObject.tag != "ChangerRing" && collision.gameObject.tag != "Jumper") {
            player = GameObject.Find("Player");
            MovePlayer playerScript = player.GetComponent<MovePlayer>();
            playerScript.reproducirSonido("destroyBullet");
            Destroy(gameObject);
        }
        if((gameObject.tag == "BulletEnemy" || gameObject.tag == "Corte") && (collision.gameObject.tag != "EnemyV1" && collision.gameObject.tag != "EnemyV2" && collision.gameObject.tag != "Boss" && collision.gameObject.tag != "ChangerRing" && collision.gameObject.tag != "Jumper")) {
            if(collision.gameObject.tag == "Player") {
                MovePlayer p = collision.GetComponent<MovePlayer>();
                if(!p.immortal) {
                    Destroy(gameObject);
                }
            }
            else {
                MovePlayer playerScript = player.GetComponent<MovePlayer>();
                playerScript.reproducirSonido("destroyBullet");
                Destroy(gameObject);
            }
        }
    }

    // Calcula la nueva posición de la bala
    //     float anglePerStep = rotationSpeed * Time.deltaTime;
    //     if(miraDerecha) anglePerStep = -anglePerStep;
    //     Vector3 center = new Vector3(0, 3 * altura, 0);
    //     Vector3 direction = transform.position - center;
    //     Vector3 target = center + Quaternion.AngleAxis(anglePerStep, Vector3.up) * direction;

    //     // Mueve la bala hacia la nueva posición
    //     transform.position = Vector3.MoveTowards(transform.position, target, rotationSpeed * Time.deltaTime);

    //     // Rotación
    //     transform.LookAt(new Vector3(0, transform.position.y, 0));

    //     // Verificar tiempo de vida
    //     tiempoVidaAct += Time.deltaTime;
    //     if (tiempoVidaAct >= tiempoVidaMax)
    //     {
    //         Destroy(gameObject);
    //     }
}
