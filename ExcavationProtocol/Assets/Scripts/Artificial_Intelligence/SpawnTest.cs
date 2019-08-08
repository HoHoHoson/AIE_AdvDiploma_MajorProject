using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    // Wave counts
    public int wave = 1;
    // spawn width(x) and height(z) for the area that the enemies will spawn
    [Tooltip("Enter Spawn Area. \n width = x, height = z")]
    public int spawnWidth, spawnHeight;

    [Header("Enemy Spawn Rates")]

    public float normalSpawnRate = 0;
    public float explosiveSpawnrate = 0;
    public float bossSpawnrate = 0;

    public bool nextWave = false;
    
    public LinkedList<GameObject> activeEnemies;

    private int enemies_to_spawn;

    // spawn points for enemy and spawn count
    private int spawn_pos_x, spawn_pos_z;
    private int spawn_count;
    private int spawn_at_object;
    private int spawn_object_type, boss_spawn_object_type;
    private GameObject spawn_location;
    private GameObject enemy_type;
    private int num_of_normal, num_of_explode, num_of_boss;

    private bool spawned_all = false;
    private int current_wave;

    public GameManager gameManager;

    // Enemy types
    [Tooltip("Add in default enemy of type")]
    public GameObject enemyNormal, enemyExplosive, enemyBoss;
    // Enemy spawn points
    public Vector3[] spawnPoints;

    [SerializeField]
    private int                 m_activeEnemiesLimit = 20;
    private List<GameObject>    m_inactive_normal = new List<GameObject>();
    private List<GameObject>    m_inactive_explosive = new List<GameObject>();
    private List<GameObject>    m_inactive_boss = new List<GameObject>();

    private bool                m_wave_ongoing = false;
    private float               m_spawn_timer = 0;

    private void Start()
    {
        float total_spawnrate;
        total_spawnrate = normalSpawnRate + explosiveSpawnrate + bossSpawnrate;

        normalSpawnRate = normalSpawnRate / total_spawnrate;
        explosiveSpawnrate = explosiveSpawnrate / total_spawnrate;
        bossSpawnrate = bossSpawnrate / total_spawnrate;

        for (int i = 0; i < m_activeEnemiesLimit; ++i)
        {
            GameObject new_enemy = Instantiate(enemyNormal);
            new_enemy.SetActive(false);
            m_inactive_normal.Add(new_enemy);

            new_enemy = Instantiate(enemyExplosive);
            new_enemy.SetActive(false);
            m_inactive_explosive.Add(new_enemy);

            new_enemy = Instantiate(enemyBoss);
            new_enemy.SetActive(false);
            m_inactive_boss.Add(new_enemy);
        }
    }

    private void Update()
    {
        if (m_wave_ongoing == true)
            ProgressWave();
    }

    public void BeginWave()
    {
        m_wave_ongoing = true;
    }

    private void ProgressWave()
    {
        if (activeEnemies.Count > m_activeEnemiesLimit)
            Debug.Log("ERROR: Enemy count exceeded the enemy limit.");
        else if (activeEnemies.Count == m_activeEnemiesLimit)
            return;

        GameObject spawned_enemy;
        float spawn_roll = Random.value;

        if (spawn_roll < normalSpawnRate)
        {
            int index = m_inactive_normal.Count - 1;

            spawned_enemy = m_inactive_normal[index];


            m_inactive_normal.RemoveAt(index);
        }
        else if (spawn_roll < (normalSpawnRate + explosiveSpawnrate))
        {
            // Spawn Explosive 
        }
        else
        {
            // Spawn Boss
        }
    }

    private Vector3 RandomSpawnPoint()
    {
        Vector3 spawn_pos = Vector3.zero;

        if (spawnPoints.Length == 0)
        {
            Debug.Log("ERROR: There are no set spawn points.");
            return spawn_pos;
        }

        spawn_pos = spawnPoints[Random.Range(0, spawnPoints.Length)];

        return spawn_pos;
    }

    private void ToggleEnemyActive(in Agent agent)
    {
        if (agent.gameObject.activeSelf == true)
        {
            activeEnemies.Remove(agent.gameObject);
            agent.gameObject.SetActive(false);

            switch (agent.GetEnemyType())
            {
                case Agent.EnemyType.BASIC:
                    m_inactive_normal.Add(agent.gameObject);
                    break;

                case Agent.EnemyType.EXPLOSIVE:
                    m_inactive_explosive.Add(agent.gameObject);
                    break;

                case Agent.EnemyType.BOSS:
                    m_inactive_boss.Add(agent.gameObject);
                    break;

                default:
                    Debug.Log("ERROR: ToggleEnemyType() switch defaulted.");
                    break;
            }
        }
        else
        {

        }
    }
}
