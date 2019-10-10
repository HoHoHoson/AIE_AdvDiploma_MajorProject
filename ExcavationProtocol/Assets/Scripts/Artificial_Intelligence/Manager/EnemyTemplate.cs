using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyTemplate
{
    [SerializeField] private GameObject m_enemyPrefab   = null;

    [SerializeField] private int m_spawnsPerWave     = 1;
    [SerializeField] private int m_activeEnemiesLimit = 10;
    [SerializeField] private int m_spawnGroupSize   = 1;

    private int         m_enemies_left  = 0;
    private List<Agent> m_enemy_pool        = new List<Agent>();
    private Blackboard  m_blackboard;

    private GameObject  m_group_nodes   = null;
    private Vector3     m_grid_extents  = Vector3.zero;

    public Agent.EnemyType GetEnemyType() { return m_enemyPrefab.GetComponentInChildren<Agent>().GetEnemyType(); }
    public int GetGroupSize() { return m_spawnGroupSize; }
    public int GetEnemyCount() { return m_enemies_left; }
    public List<Agent> GetEnemyPool() { return m_enemy_pool; }

    public void InstantiateEnemyPool(in Blackboard blackboard)
    {
        Agent agent = m_enemyPrefab.GetComponentInChildren<Agent>();

        if (agent == null)
        {
            Debug.Log("ERROR: EnemyTemplate Agent is NULL.");
            return;
        }

        m_blackboard = blackboard;
        agent.InitialiseAgent(m_blackboard);

        for (int i = 0; i < m_activeEnemiesLimit; ++i)
            IncreaseEnemyPool();

        float grid_index = Mathf.Ceil(Mathf.Sqrt(m_spawnGroupSize)) * 0.5f;

        CapsuleCollider capsule = 
            Object.Instantiate(m_enemyPrefab, new Vector3(0, 100, 0), Quaternion.identity).GetComponentInChildren<CapsuleCollider>();

        m_grid_extents = capsule.bounds.size;
        m_grid_extents *= grid_index;
        m_grid_extents.y = capsule.bounds.extents.y;

        grid_index -= 0.5f;

        m_group_nodes = new GameObject(agent.GetEnemyType().ToString() + " Spawn Grid");

        for (float x = -grid_index; x <= grid_index; ++x)
            for (float y = -grid_index; y <= grid_index; ++y)
            {
                GameObject go = new GameObject(x + ", " + y);
                go.transform.parent = m_group_nodes.transform;

                go.transform.localPosition = 
                    new Vector3(x * capsule.bounds.size.x, 0, y * capsule.bounds.size.z);
            }

        GameObject.Destroy(capsule.transform.root.gameObject);
    }

    public void WaveBeginning()
    {
        // Increase enemy spawn count here

        m_enemies_left = m_spawnsPerWave;
    }

    public void WaveEnding()
    {
        foreach (Agent a in m_enemy_pool)
            if (a.gameObject.activeSelf == true)
                DeactivateEnemy(a);
    }

    public bool ActivateEnemy(Vector3 spawn_point)
    {
        int active_count = GetActiveCount();
        if (active_count > m_activeEnemiesLimit || active_count > m_enemies_left)           // Checks if the spawn limit has been breached
        {
            Debug.Log("ERROR: Active enemies exceedes a limit.");                           // Errors if it occurs
            return false;                                                                   // then skips spawn function
        }
        else if (active_count == m_activeEnemiesLimit || active_count == m_enemies_left)    // Also checks if a cap has been reached
            return false;                                                                   // skips spawn function if so

        //// This variable will store an enemy type and attempt to spawn it at the end of this function
        //EnemyTemplate enemy_spawn = null;

        //if (m_hold_spawn != null)
        //{
        //    enemy_spawn = m_hold_spawn; // If a reserved spawn exists, set that as the enemy_spawn
        //    m_hold_spawn = null;        // clear the reserve afterwards
        //}
        //else
        //{
        //    // Randomly sets enemy_type to any of the developer set enemy types
        //    int spawn_rate_roll = Random.Range(0, m_max_spawn_rate);
        //    int spawn_rate_threshold = 0;

        //    foreach (EnemyTemplate e in m_enemyTypes)
        //    {
        //        spawn_rate_threshold += e.GetSpawnRate();

        //        if (spawn_rate_roll < spawn_rate_threshold)
        //        {
        //            enemy_spawn = e;
        //            break;
        //        }
        //    }
        //}

        // Checks if there is enough space for the enemy spawns
        int free_slots = m_enemies_left - active_count;
        int group_size = free_slots >= m_spawnGroupSize ? m_spawnGroupSize : free_slots;
        int active_slots = m_spawnsPerWave - active_count;

        if (active_slots < group_size || Physics.CheckBox(spawn_point, m_grid_extents, m_group_nodes.transform.rotation, 1 << 10))
            return false;

        m_group_nodes.transform.position = spawn_point;

        for (int i = 0; i < group_size; ++i)
        {
            Agent agent_instance = GetEnemyInstance();

            agent_instance.ResetStats();
            agent_instance.transform.position = m_group_nodes.transform.GetChild(i).position;
        }

        return true;
    }

    public void DeactivateEnemy(in Agent enemy)
    {
        enemy.gameObject.SetActive(false);
        --m_enemies_left;
    }

    public List<Agent> GetEnemiesActive()
    {
        List<Agent> active_list = new List<Agent>();

        foreach (Agent a in m_enemy_pool)
            if (a.gameObject.activeSelf == true)
                active_list.Add(a);

        return active_list;
    }

    private Agent IncreaseEnemyPool()
    {
        Agent agent_instance = null;

        agent_instance = Object.Instantiate(m_enemyPrefab).GetComponentInChildren<Agent>();
        agent_instance.InitialiseAgent(m_blackboard);
        agent_instance.gameObject.SetActive(false);

        m_enemy_pool.Add(agent_instance);

        return agent_instance;
    }

    private int GetActiveCount()
    {
        int active_count = 0;

        foreach (Agent a in m_enemy_pool)
            if (a.gameObject.activeSelf == true)
                ++active_count;

        return active_count;
    }

    private Agent GetEnemyInstance()
    {
        Agent free_enemy = null;

        foreach (Agent a in m_enemy_pool)
            if (a.gameObject.activeSelf == false)
            {
                free_enemy = a;
                break;
            }

        if (free_enemy == null)
            free_enemy = IncreaseEnemyPool();

        free_enemy.gameObject.SetActive(true);

        return free_enemy;
    }
}
