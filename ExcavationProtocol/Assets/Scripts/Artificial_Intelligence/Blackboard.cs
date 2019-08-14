using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public GameManager              m_gameManager;

    // Enemy types
    [Header("Enemy Settings")]
    public GameObject               m_enemyNormal;
    public GameObject               m_enemyExplosive;
    public GameObject               m_enemyBoss;

    public float                    m_normalSpawnRate       = 0;
    public float                    m_explosiveSpawnrate    = 0;
    public float                    m_bossSpawnrate         = 0;

    // Enemy spawn points
    public Transform[]              m_spawnPoints;

    public int                      m_enemyCount            = 50;

    [SerializeField]
    private int                     m_activeEnemiesLimit    = 20;
    private LinkedList<GameObject>  m_active_enemies        = new LinkedList<GameObject>();
    private List<GameObject>        m_inactive_normal       = new List<GameObject>();
    private List<GameObject>        m_inactive_explosive    = new List<GameObject>();
    private List<GameObject>        m_inactive_boss         = new List<GameObject>();

    private bool                    m_wave_ongoing          = true;

    private void Start()
    {
        float total_spawnrate;
        total_spawnrate = m_normalSpawnRate + m_explosiveSpawnrate + m_bossSpawnrate;

        m_normalSpawnRate = m_normalSpawnRate / total_spawnrate;
        m_explosiveSpawnrate = m_explosiveSpawnrate / total_spawnrate;
        m_bossSpawnrate = m_bossSpawnrate / total_spawnrate;

        GameObject stored_enemy_instances = new GameObject();
        stored_enemy_instances.name = "Enemies";

        for (int i = 0; i < m_activeEnemiesLimit; ++i)
        {
            GameObject new_enemy = Instantiate(m_enemyNormal, stored_enemy_instances.transform);
            new_enemy.SetActive(false);
            m_inactive_normal.Add(new_enemy);

            new_enemy = Instantiate(m_enemyExplosive, stored_enemy_instances.transform);
            new_enemy.SetActive(false);
            m_inactive_explosive.Add(new_enemy);

            new_enemy = Instantiate(m_enemyBoss, stored_enemy_instances.transform);
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

    public void EndWave()
    {
        m_wave_ongoing = false;

        while (m_active_enemies.Count > 0)
        {
            DeactivateEnemy(m_active_enemies.Last.Value.GetComponent<Agent>());
        }
    }

    private void ProgressWave()
    {
        // EndWave when all enemies are dead
        if (m_enemyCount <= 0)
        {
            EndWave();
            return;
        }

        // Checks and removes any dead enemies
        var iterator = m_active_enemies.First;
        while (iterator != null)
        {
            var next_iterator = iterator.Next;

            if (iterator.Value.GetComponent<Agent>().IsDead())
            {
                DeactivateEnemy(iterator.Value.GetComponent<Agent>());
                --m_enemyCount;
            }

            iterator = next_iterator;
        }

        // Spawns a new enemy when the spawn limit hasn't been reached and the enemy cap is higher than the spawn count
        if (m_active_enemies.Count > m_activeEnemiesLimit)
            Debug.Log("ERROR: Active enemies count exceeded the limit.");
        else if (m_active_enemies.Count == m_activeEnemiesLimit || m_enemyCount == m_active_enemies.Count)
            return;

        float spawn_roll = Random.value;

        if (spawn_roll < m_normalSpawnRate)
        {
            ActivateEnemy(Agent.EnemyType.BASIC);
        }
        else if (spawn_roll < (m_normalSpawnRate + m_explosiveSpawnrate))
        {
            ActivateEnemy(Agent.EnemyType.EXPLOSIVE);
        }
        else
        {
            ActivateEnemy(Agent.EnemyType.BOSS);
        }
    }

    private Vector3 RandomSpawnPoint()
    {
        Vector3 spawn_pos = Vector3.zero;

        if (m_spawnPoints.Length == 0)
        {
            Debug.Log("ERROR: There are no set spawn points.");
            return spawn_pos;
        }

        spawn_pos = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)].position;

        return spawn_pos;
    }

    private void DeactivateEnemy(in Agent agent)
    {
        if (agent.gameObject.activeSelf == false)
        {
            Debug.Log("ERROR: Enemy is not active.");
            return;
        }

        m_active_enemies.Remove(agent.gameObject);
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

    private void ActivateEnemy(Agent.EnemyType enemy_type)
    {
        GameObject spawned_enemy = null;

        switch (enemy_type)
        {
            case Agent.EnemyType.BASIC:
                {
                    if (m_inactive_normal.Count <= 0)
                        break;

                    spawned_enemy = m_inactive_normal[m_inactive_normal.Count - 1];

                    m_active_enemies.AddLast(spawned_enemy);
                    m_inactive_normal.RemoveAt(m_inactive_normal.Count - 1);

                    break;
                }

            case Agent.EnemyType.EXPLOSIVE:
                {
                    if (m_inactive_explosive.Count <= 0)
                        break;

                    spawned_enemy = m_inactive_explosive[m_inactive_explosive.Count - 1];

                    m_active_enemies.AddLast(spawned_enemy);
                    m_inactive_explosive.RemoveAt(m_inactive_explosive.Count - 1);

                    break;
                }

            case Agent.EnemyType.BOSS:
                {
                    if (m_inactive_boss.Count <= 0)
                        break;

                    spawned_enemy = m_inactive_boss[m_inactive_boss.Count - 1];

                    m_active_enemies.AddLast(spawned_enemy);
                    m_inactive_boss.RemoveAt(m_inactive_boss.Count - 1);

                    break;
                }

            default:
                Debug.Log("ERROR: ActivateEnemy() switch defaulted.");
                break;
        }

        if (spawned_enemy != null)
        {
            spawned_enemy.transform.position = RandomSpawnPoint();
            spawned_enemy.SetActive(true);
        }
        else
            Debug.Log("ERROR: Enemy object pool is empty or negative.");
    }
}
