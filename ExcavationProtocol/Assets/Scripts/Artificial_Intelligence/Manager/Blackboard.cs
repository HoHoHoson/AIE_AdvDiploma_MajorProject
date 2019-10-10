using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public GameManager          m_gameManager;
    public Transform[]          m_spawnPoints;
    public List<EnemyTemplate>  m_enemyTypes            = new List<EnemyTemplate>();

    private bool                m_wave_ongoing          = false;
    private EnemyTemplate       m_hold_spawn            = null;

    private Dictionary<Agent.EnemyType, EnemyTemplate> m_eMap = new Dictionary<Agent.EnemyType, EnemyTemplate>();

    public bool IsWaveOngoing() { return m_wave_ongoing; }

    void Start()
    {
        foreach (EnemyTemplate e in m_enemyTypes)
            e.InstantiateEnemyPool(this, m_activeEnemiesLimit);

        m_eMap = m_enemyTypes.ToDictionary(e => e.GetEnemyType());
    }

    void Update()
    {
        if (m_wave_ongoing == true)
            ProgressWave();
    }

    public void BeginWave()
    {
        m_wave_ongoing = true;

        foreach (EnemyTemplate e in m_enemyTypes)
            e.WaveBeginning();
    }

    public void EndWave()
    {
        m_wave_ongoing = false;
        m_gameManager.AddCurrency();

        foreach (EnemyTemplate e in m_enemyTypes)
            e.WaveEnding();
    }

    private void ProgressWave()
    {
        // EndWave when all enemies are dead
        if (TotalActiveEnemyCount() <= 0)
        {
            EndWave();
            return;
        }

        foreach (EnemyTemplate e in m_enemyTypes)
        {
            // Sort all active enemies into a list

            foreach (Agent a in e.GetActiveList())
                a.UpdateAgent();

            // Checks and removes any dead enemies
            var iterator = e.GetActiveList().First;
            while (iterator != null)
            {
                var next_iterator = iterator.Next;

                if (iterator.Value.IsDead())
                {
                    iterator.Value.SetActive(false);
                }

                iterator = next_iterator;
            }

            e.ActivateEnemy(RandomSpawnPoint());
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

    private int TotalActiveEnemyCount()
    {
        int total = 0;

        foreach (EnemyTemplate e in m_enemyTypes)
            total += e.GetActiveList().Count;

        return total;
    }
}
