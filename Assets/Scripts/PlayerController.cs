using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private Collider playerCollider;
    public ParticleSystem smashParticle;
    
    private float speed = 500f;
    private bool isJump = false;
    private float limitDown = -10.0f;

    [Header("Powerup Manager")]    
    public GameObject powerupIndicator;    
    private Coroutine powerupCountdown;
    public PowerUpType currentPowerUp = PowerUpType.None;
    public bool hasPowerup = false;

    [Header("Pushback Powerup")]
    private float powerupStrenght = 15f;

    [Header("Rocket Powerup")]
    public GameObject rocketPrefab;
    private GameObject tmpRocket;

    [Header("Smash Powerup")]
    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    float floorY;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        playerRb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        focalPoint = GameObject.Find("FocalPoint");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");

        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput * Time.deltaTime);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
        smashParticle.transform.position = transform.position + new Vector3(0, -0.6f, 0); ;

        if (currentPowerUp == PowerUpType.Rockets && Input.GetButtonDown("Fire1"))
        {
            LaunchRockets();
        }

        if (currentPowerUp == PowerUpType.Smash && Input.GetButtonDown("Jump") && !isJump)        
        {
            isJump = true;
            StartCoroutine(Smash());
        }

        if (transform.position.y < limitDown)
        {
            gameManager.isGameActive = false;
            playerRb.isKinematic = true;
            transform.position = new Vector3(0, 0, 0);
            gameManager.GameOver();
        }
    }    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup") && !hasPowerup)
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;            
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);            

            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }

            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }

        if (other.CompareTag("Ground"))
        {
            smashParticle.Play();
            playerCollider.isTrigger = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup && currentPowerUp == PowerUpType.Pushback)
        {
            Rigidbody enemyRigibody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.transform.position - transform.position;

            enemyRigibody.AddForce(awayFromPlayer * powerupStrenght, ForceMode.Impulse);            
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        currentPowerUp = PowerUpType.None;
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        //Store the y position before taking off
        floorY = transform.position.y;

        //Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;

        while (Time.time < jumpTime)
        {
            //move the player up while still keeping their x velocity.
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        //Now move the player down
        while (transform.position.y > floorY)
        {   
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            playerCollider.isTrigger = true;
                       
            yield return null;
        }

        //Cycle through all enemies.
        for (int i = 0; i < enemies.Length; i++)
        {
            //Apply an explosion force that originates from our position.
            if (enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }   
        }     
    }

    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }
}
