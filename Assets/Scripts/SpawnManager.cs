using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameManager gameManager;
    private int levelWave;
    
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefab;

    private float spawnRange = 9f;

    public int powerupCount;
    public int enemyCount;
    public int waveNumber = 1;

    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    public int bossRound;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        SpawnEnemyWave(waveNumber);

        gameManager.UpdateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        powerupCount = FindObjectsOfType<PowerUp>().Length;

        if (gameManager.isGameActive && enemyCount == 0)
        {  
            gameManager.UpdateLevel();

            waveNumber++;

            for (int i = 0; i < powerupCount; i++)
            {
                var powerupRemove = GameObject.FindGameObjectsWithTag("Powerup");

                Destroy(powerupRemove[i].gameObject);
            }            
            
            //Spawn a boss every x number of waves
            if (waveNumber % bossRound == 0)
            {
                SpawnBossWave(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
            }

            SpawnPowerupWave(waveNumber - 1);
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {   
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int enemyPrefabsIndex = Random.Range(0, enemyPrefab.Length);
            Instantiate(enemyPrefab[enemyPrefabsIndex], GenerateSpawnPosition(), enemyPrefab[enemyPrefabsIndex].transform.rotation);
        }
    }

    void SpawnBossWave(int currentRound)
    {
        int miniEnemysToSpawn;

        //We dont want to divide by 0!
        if (bossRound != 0)
        {
            miniEnemysToSpawn = currentRound / bossRound;
        }
        else
        {
            miniEnemysToSpawn = 1;
        }

        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);
            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpawnPosition(), miniEnemyPrefabs[randomMini].transform.rotation);
        }
    }

    void SpawnPowerupWave(int powerupToSpawn)
    {
        for (int i = 0; i < powerupToSpawn; i++)
        {
            int powerupPrefabsIndex = Random.Range(0, powerupPrefab.Length);
            Instantiate(powerupPrefab[powerupPrefabsIndex], GenerateSpawnPosition(), powerupPrefab[powerupPrefabsIndex].transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return randomPos;
    }
}
