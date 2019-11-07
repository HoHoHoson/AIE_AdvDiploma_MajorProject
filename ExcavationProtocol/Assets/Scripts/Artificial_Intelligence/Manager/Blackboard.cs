using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public GameManager m_gameManager;

    [SerializeField] private float                  m_intermissionTime = 5f;
    [SerializeField] private Transform[]            m_spawnPoints;
    [SerializeField] private List<EnemyTemplate>    m_enemyTypes = new List<EnemyTemplate>();

    private float               m_intermission_timer    = 0f;
    private int                 m_waves_passed          = 0; 
    private bool                m_wave_ongoing          = false;
    private EnemyTemplate       m_hold_spawn            = null;
	[HideInInspector]
	public GameObject			mine;

	public int CurrentWave() { return m_waves_passed + 1; }
    public bool IsWaveOngoing() { return m_wave_ongoing; }

    void Start()
    {
        foreach (EnemyTemplate e in m_enemyTypes)
            e.InitialiseEnemyTemplate(this);

		mine = GameObject.FindGameObjectWithTag("Mine");
    }

    public void UpdateBlackboard()
    {
        if (m_wave_ongoing == true)
            ProgressWave();
        else
        {
            m_intermission_timer += Time.deltaTime;

            if (m_intermission_timer >= m_intermissionTime)
                BeginWave();
        }
    }

    public int TotalEnemyCount()
    {
        int total = 0;

        foreach (EnemyTemplate e in ActiveEnemyTypes())
            total += e.GetEnemyCount();

        return total;
    }

    private void BeginWave()
    {
        m_wave_ongoing = true;
        m_intermission_timer = 0;

        foreach (EnemyTemplate e in ActiveEnemyTypes())
            e.WaveBeginning();
    }

    private void EndWave()
    {
        m_wave_ongoing = false;
        ++m_waves_passed;

        m_gameManager.AddCurrency();

        foreach (EnemyTemplate e in ActiveEnemyTypes())
            e.WaveEnding();
    }

    private void ProgressWave()
    {
        // EndWave when all enemies are dead
        if (TotalEnemyCount() <= 0)
        {
            EndWave();
            return;
        }

        foreach (EnemyTemplate e in ActiveEnemyTypes())
        {
            List<Agent> active_enemies = e.GetEnemiesActive();

            foreach (Agent a in active_enemies)
            {
				// Checks and removes any dead enemies
				if (a.IsDead())
				{
					if (a.GetEnemyType() == Agent.EnemyType.BOSS)
					{ 
						m_gameManager.AddCurrency(10);
					}
					e.DeactivateEnemy(a);
				}
                if (a.transform.position.y < -10)
                    a.gameObject.SetActive(false);

                // Then updates the agent
                a.UpdateAgent();
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

    private List<EnemyTemplate> ActiveEnemyTypes()
    {
        List<EnemyTemplate> active_types = new List<EnemyTemplate>();

        foreach (EnemyTemplate e in m_enemyTypes)
            if (e.IsActive())
                active_types.Add(e);

        return active_types;
    }
}
