﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyTemplate
{
    [SerializeField] private GameObject m_enemyPrefab   = null;
    [SerializeField] private int        m_health        = 3;
    [SerializeField] private int        m_damage        = 5;
    [SerializeField] private int        m_speed         = 300;
    [SerializeField] private int        m_spawnRate     = 100;

    [SerializeField]
    private int         m_groupSize         = 1;
    private GameObject  m_group_nodes       = null;
    private Vector3     m_grid_extents       = Vector3.zero;
    private float       m_grid_bounds_x     = 0;
    private float       m_grid_bounds_z     = 0;

    private List<Agent> m_inactive_enemies  = new List<Agent>();

    public Agent.EnemyType GetEnemyType() { return m_enemyPrefab.GetComponentInChildren<Agent>().GetEnemyType(); }
    public int GetSpawnRate() { return m_spawnRate; }
    public int GetGroupSize() { return m_groupSize; }

    public void InstantiateEnemyPool(in Blackboard blackboard, int max_active)
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
            Agent agent_instance;

            agent_instance = Object.Instantiate(m_enemyPrefab, editor_tab.transform).GetComponentInChildren<Agent>();
            agent_instance.SetBlackboard(blackboard);
            agent_instance.gameObject.SetActive(false);

            m_inactive_enemies.Add(agent_instance);
        }

        float grid_index = Mathf.Ceil(Mathf.Sqrt(m_groupSize)) * 0.5f;

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

    public bool ActivateEnemy(Vector3 spawn_point, in LinkedList<Agent> active_enemies, int enemy_count)
    {
        if (Physics.CheckBox(spawn_point, m_grid_extents, m_group_nodes.transform.rotation, 1 << 10))
        {
            return false;
        }

        m_group_nodes.transform.position = spawn_point;

        int spawn_number = enemy_count < m_groupSize ? (m_groupSize - enemy_count) : m_groupSize;

        for (int i = 0; i < spawn_number; ++ i)
        {
            Agent agent_instance = m_inactive_enemies[m_inactive_enemies.Count - 1];
            m_inactive_enemies.RemoveAt(m_inactive_enemies.Count - 1);

            agent_instance.SetStats(m_health, m_damage, m_speed);
            agent_instance.transform.position = m_group_nodes.transform.GetChild(i).position;

            agent_instance.gameObject.SetActive(true);
            active_enemies.AddLast(agent_instance);
        }

        return true;
    }

    public void DeactivateEnemy(in Agent agent)
    {
        agent.gameObject.SetActive(false);
        m_inactive_enemies.Add(agent);
    }
}