using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { None, Pushback, Rockets, Smash }

public class PowerUp : MonoBehaviour
{
    private GameManager gameManager;

    public PowerUpType powerUpType;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();        
    }

    private void Update()
    {
        if (!gameManager.isGameActive)
        {
            Destroy(gameObject);
        }        
    }
}
