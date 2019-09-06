using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public GameManager          m_gameManager;
    public Transform[]          m_spawnPoints;
    public List<EnemyTemplate>  m_enemyTypes            = new List<EnemyTemplate>();
    public int                  m_enemyCount            = 50;
    private int                 m_enemyCountReset;

    [SerializeField]
    private int                 m_activeEnemiesLimit    = 20;
    private LinkedList<Agent>   m_active_enemies        = new LinkedList<Agent>();

    private bool                m_wave_ongoing          = false;
    private int                 m_max_spawn_rate        = 0;
    private EnemyTemplate       m_hold_spawn            = null;

    private Dictionary<Agent.EnemyType, EnemyTemplate> m_eMap = new Dictionary<Agent.EnemyType, EnemyTemplate>();

    public int GetEnemyCount() { return m_enemyCount; }
    public bool IsWaveOngoing() { return m_wave_ongoing; }

    void Start()
    {
        foreach (EnemyTemplate e in m_enemyTypes)
        {
            m_max_spawn_rate += e.GetSpawnRate();
            e.InstantiateEnemyPool(this, m_activeEnemiesLimit);
        }

        m_eMap = m_enemyTypes.ToDictionary(e => e.GetEnemyType());
        m_enemyCountReset = m_enemyCount;
    }

    void Update()
    {
        if (m_wave_ongoing == true)
            ProgressWave();
    }

    public void BeginWave()
    {
        m_wave_ongoing = true;
        m_enemyCount = m_enemyCountReset;
    }

    public void EndWave()
    {
        m_wave_ongoing = false;
        m_gameManager.AddCurrency();
        while (m_active_enemies.Count > 0)
        {
            RemoveEnemy(m_active_enemies.Last.Value);
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

        foreach (Agent a in m_active_enemies)
            a.UpdateAgent();

        // Checks and removes any dead enemies
        var iterator = m_active_enemies.First;
        while (iterator != null)
        {
            var next_iterator = iterator.Next;

            if (iterator.Value.IsDead())
            {
                RemoveEnemy(iterator.Value);
                --m_enemyCount;
            }

            iterator = next_iterator;
        }

        SpawnEnemy();
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

    private void SpawnEnemy()
    {
        if (m_active_enemies.Count > m_activeEnemiesLimit || m_active_enemies.Count > m_enemyCount)         // Checks if the spawn limit has been breached
        {                                                                                                   
            Debug.Log("ERROR: Active enemies exceedes a limit.");                                           // Errors if it occurs
            return;                                                                                         // then skips spawn function
        }
        else if (m_active_enemies.Count == m_activeEnemiesLimit || m_active_enemies.Count == m_enemyCount)  // Also checks if a cap has been reached
            return;                                                                                         // skips spawn function if so

        // This variable will store an enemy type and attempt to spawn it at the end of this function
        EnemyTemplate enemy_spawn = null;

        if (m_hold_spawn != null) 
        {
            enemy_spawn = m_hold_spawn; // If a reserved spawn exists, set that as the enemy_spawn
            m_hold_spawn = null;        // clear the reserve afterwards
        }
        else 
        {
            // Randomly sets enemy_type to any of the developer set enemy types
            int spawn_rate_roll = Random.Range(0, m_max_spawn_rate);
            int spawn_rate_threshold = 0;

            foreach (EnemyTemplate e in m_enemyTypes)
            {
                spawn_rate_threshold += e.GetSpawnRate();

                if (spawn_rate_roll < spawn_rate_threshold)
                {
                    enemy_spawn = e;
                    break;
                }
            }
        }

        // Checks if there is enough space for the enemy spawns
        int free_slots = m_enemyCount - m_active_enemies.Count;
        int group_size = free_slots >= enemy_spawn.GetGroupSize() ? enemy_spawn.GetGroupSize() : free_slots;

        int active_slots = m_activeEnemiesLimit - m_active_enemies.Count;
        if (active_slots < group_size)
        {
            // If there isn't enough space, store the spawn as a reserve
            m_hold_spawn = enemy_spawn;
        }
        else
        {
            // Otherwise, the enemies will get spawned
            enemy_spawn.ActivateEnemy(m_active_enemies, RandomSpawnPoint(), group_size);
        }
    }

    private void RemoveEnemy(in Agent agent)
    {
        if (agent.gameObject.activeSelf == false)
        {
            Debug.Log("ERROR: Enemy is not active.");
            return;
        }

        m_active_enemies.Remove(agent);
        agent.gameObject.SetActive(false);

        m_eMap[agent.GetEnemyType()].DeactivateEnemy(agent);
    }
}
