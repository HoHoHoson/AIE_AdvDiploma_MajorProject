using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyTemplate
{
    [SerializeField]
    private GameObject          m_enemyPrefab       = null;
    public int                  m_health            = 3;
    public int                  m_damage            = 5;
    public int                  m_speed             = 300;
    public int                  m_spawnRate         = 100;
    [SerializeField]
    private int                 m_groupSize         = 1;

    private GameObject          m_group_nodes       = null;
    private List<GameObject>    m_inactive_enemies  = new List<GameObject>();

    public Agent.EnemyType GetEnemyType() { return m_enemyPrefab.GetComponentInChildren<Agent>().GetEnemyType(); }
    public int GetSpawnRate() { return m_spawnRate; }

    public void InstantiateEnemyPool(int max_active)
    {
        Agent agent = m_enemyPrefab.GetComponentInChildren<Agent>();

        if (agent == null)
        {
            Debug.Log("ERROR: EnemyTemplate Agent is NULL.");
            return;
        }

        GameObject editor_tab = new GameObject(agent.GetEnemyType().ToString());

        for (int i = 0; i < max_active; ++i)
        {
            GameObject instanced_enemy;

            instanced_enemy = Object.Instantiate(m_enemyPrefab, editor_tab.transform);
            instanced_enemy.SetActive(false);

            m_inactive_enemies.Add(instanced_enemy);
        }

        float height = m_enemyPrefab.GetComponentInChildren<CapsuleCollider>().height;
        float width = m_enemyPrefab.GetComponentInChildren<CapsuleCollider>().radius * 2;

        m_group_nodes = new GameObject(agent.GetEnemyType().ToString() + " Spawn Grid");

        float grid_index = (Mathf.Ceil(Mathf.Sqrt(m_groupSize)) * 0.5f) - 0.5f;
        for (float x = -grid_index; x <= grid_index; ++x)
            for (float y = -grid_index; y <= grid_index; ++y)
            {
                GameObject go = new GameObject(x + ", " + y);
                go.transform.parent = m_group_nodes.transform;

                go.transform.localPosition = new Vector3(x * width, 0, y * height);
            }
    }

    public void ActivateEnemy(in LinkedList<GameObject> active_enemies)
    {
        // Spawn stuff in groups here
    }

    public void DeactivateEnemy(in GameObject enemy)
    {
        enemy.SetActive(false);
        m_inactive_enemies.Add(enemy);
    }
}
