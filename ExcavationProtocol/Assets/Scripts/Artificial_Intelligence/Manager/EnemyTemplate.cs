using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyTemplate
{
    [SerializeField] private GameObject m_enemyPrefab   = null;

    [SerializeField, Tooltip("How many of these enemies are spawned at the start of each wave.")]
    private float m_spawnsPerWave = 1f;
    [SerializeField, Tooltip("Increases the spawn count for each wave passed by this amount.")]
    private float m_spawnIncrement = 1f;
    [SerializeField, Tooltip("Max possible number of active enemies in the scene.")]
    private float m_activeEnemiesLimit = 10f;
    [SerializeField, Tooltip("Increases the active limit count of enemies for each wave passed by this amount.")]
    private float m_activeIncrement = 1f;
    [SerializeField, Tooltip("Number of enemies that are spawned in together when this type gets spawned.")]
    private int m_spawnGroupSize   = 1;
    [SerializeField, Tooltip("Only spawns on every nth wave. Leave it empty if you want them on all waves.")]
    private int[] m_onWaves; 

    private int         m_enemies_left  = 0;
    private int         m_spawn_default;
    private int         m_active_default;
    private List<Agent> m_enemy_pool        = new List<Agent>();
    private Blackboard  m_blackboard;

    private GameObject  m_group_nodes   = null;
    private Vector3     m_grid_extents  = Vector3.zero;

    public Agent.EnemyType GetEnemyType() { return m_enemyPrefab.GetComponentInChildren<Agent>().GetEnemyType(); }
    public int GetGroupSize() { return m_spawnGroupSize; }
    public int GetEnemyCount() { return m_enemies_left; }
    public List<Agent> GetEnemyPool() { return m_enemy_pool; }

    public void InitialiseEnemyTemplate(in Blackboard blackboard)
    {
        Agent agent = m_enemyPrefab.GetComponentInChildren<Agent>();

        if (agent == null)
        {
            Debug.Log("ERROR: EnemyTemplate Agent is NULL.");
            return;
        }

        m_blackboard = blackboard;
        m_spawn_default = (int)m_spawnsPerWave;
        m_active_default = (int)m_activeEnemiesLimit;
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

        m_enemies_left = (int)m_spawnsPerWave;
    }

    public void WaveEnding()
    {
        foreach (Agent a in m_enemy_pool)
            if (a.gameObject.activeSelf == true)
                DeactivateEnemy(a);

        int current_wave = m_blackboard.CurrentWave() - 1;
        m_spawnsPerWave = m_spawn_default + m_spawnIncrement * current_wave;
        m_activeEnemiesLimit = m_active_default + m_activeIncrement * current_wave;
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
        int num_to_spawn = m_enemies_left - active_count;
        int group_size = num_to_spawn >= m_spawnGroupSize ? m_spawnGroupSize : num_to_spawn;
        int active_slots = (int)m_activeEnemiesLimit - active_count;

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

    public bool IsActive()
    {
        if (m_onWaves.Length == 0)
            return true;

        foreach (int i in m_onWaves)
            if (m_blackboard.CurrentWave() % i == 0)
                return true;

        return false;
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
