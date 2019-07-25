using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public int NumToSpawn;
    public int spawnWidth, spawnHeight, posX, posZ;
    private int SpawnCount;

    public GameObject normalEnemy, explosionBoi, bossBoi, spawnPoint1, spawnPoint2, spawnPoint3;

    public int Wave = 1;
    public int nextWave;


    int spawnAtObject;
    GameObject spawnLocation;

    int spawnObjectType, bossSpawnObjectType;
    GameObject enemyType;
    int NumOfNormal, NumOfExplode, numOfBoss;

    public int ExplRate = 5;
    public int BossRate = 20;
    public bool NextWaveNow = false;
    private bool hasSpawnedAll = false;
    int currentWave;

    public GameObject[] EnemyList;

    private void Start()
    {
        Waves();
    }

    void Update()
    {
        if (NextWaveNow == true)
        {
            Wave++;
            StopAllCoroutines();
            foreach(var enemy in EnemyList)
            {
                if (enemy != null && hasSpawnedAll == true)
                {
                    Destroy(enemy);
                }
            }

            SpawnCount = 0;
            nextWave = Wave + 1;
            NextWaveNow = false;
        }
        if (Wave > currentWave)
        {
            Waves();
        }
    }

    public void Waves()
    {
        currentWave = Wave;
        if (Wave < 2)
        {
            hasSpawnedAll = false;
            StartCoroutine(SpawnNormalEnemies(NumToSpawn));
            
        }
        else if (Wave < 4)
        {
            hasSpawnedAll = false;
            StartCoroutine(SpawnEnemies(NumToSpawn));
            
        }
        else
        {
            hasSpawnedAll = false;
            StartCoroutine(SpawnEnemiesWithBoss(NumToSpawn));
           
        }
    }

    IEnumerator SpawnNormalEnemies(int NumOfEnemies)
    {
        EnemyList = new GameObject[NumOfEnemies];
        while (SpawnCount < NumOfEnemies)
        {
            spawnAtObject = Random.Range(0, 3);

            switch (spawnAtObject)
            {
                case 0:
                    spawnLocation = spawnPoint1;
                    break;
                case 1:
                    spawnLocation = spawnPoint2;
                    break;
                case 2:
                    spawnLocation = spawnPoint3;
                    break;
                case 3:
                    spawnLocation = spawnPoint1;
                    break;
                default:
                    break;
            }

            posX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
            posZ = Random.Range(-spawnHeight / 2, spawnHeight / 2);

            EnemyList[SpawnCount] = Instantiate(normalEnemy, new Vector3(spawnLocation.transform.position.x + posX, 5,spawnLocation.transform.position.z + posZ), Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
            NumOfNormal++;
            SpawnCount++;
        }
        hasSpawnedAll = true;
    }

    IEnumerator SpawnEnemies(int NumOfEnemies)
    {
        EnemyList = new GameObject[NumOfEnemies];
        while (SpawnCount < NumOfEnemies)
        {
            spawnAtObject = Random.Range(0, 3);

            switch (spawnAtObject)
            {
                case 0:
                    spawnLocation = spawnPoint1;
                    break;
                case 1:
                    spawnLocation = spawnPoint2;
                    break;
                case 2:
                    spawnLocation = spawnPoint3;
                    break;
                case 3:
                    spawnLocation = spawnPoint1;
                    break;
                default:
                    break;
            }

            spawnObjectType = Random.Range(0, ExplRate);

            if (spawnObjectType == ExplRate - 1)
            {
                enemyType = explosionBoi;
                NumOfExplode++;
            }
            else
            {
                enemyType = normalEnemy;
                NumOfNormal++;
            }

            posX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
            posZ = Random.Range(-spawnHeight / 2, spawnHeight / 2);

            EnemyList[SpawnCount] = Instantiate(enemyType, new Vector3(spawnLocation.transform.position.x + posX,
                5, spawnLocation.transform.position.z + posZ), Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
            SpawnCount++;
        }
        hasSpawnedAll = true;
    }

    IEnumerator SpawnEnemiesWithBoss(int NumOfEnemies)
    {
        EnemyList = new GameObject[NumOfEnemies];
        while (SpawnCount < NumOfEnemies)
        {
            spawnAtObject = Random.Range(0, 3);

            switch (spawnAtObject)
            {
                case 0:
                    spawnLocation = spawnPoint1;
                    break;
                case 1:
                    spawnLocation = spawnPoint2;
                    break;
                case 2:
                    spawnLocation = spawnPoint3;
                    break;
                case 3:
                    spawnLocation = spawnPoint1;
                    break;
                default:
                    break;
            }

            spawnObjectType = Random.Range(0, ExplRate);
            bossSpawnObjectType = Random.Range(0, BossRate);

            if (spawnObjectType == ExplRate - 1)
            {
                enemyType = explosionBoi;
                NumOfExplode++;
            }
            else if(bossSpawnObjectType == BossRate - 1 && SpawnCount < NumOfEnemies - 10)
            {
                enemyType = bossBoi;
                numOfBoss++;
                SpawnCount = SpawnCount + 9;
            }
            else
            {
                enemyType = normalEnemy;
                NumOfNormal++;
            }

            posX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
            posZ = Random.Range(-spawnHeight / 2, spawnHeight / 2);

            EnemyList[SpawnCount] = Instantiate(enemyType, new Vector3(spawnLocation.transform.position.x + posX,
                5, spawnLocation.transform.position.z + posZ), Quaternion.identity);
            
            yield return new WaitForSeconds(0.1f);
            SpawnCount++;
        }
        hasSpawnedAll = true;
    }
}
