using UnityEngine;

public class ExplosiveAI : Agent
{
    [SerializeField] ParticleSystem m_exlodeSFX = null;
    [SerializeField] private float  m_explosiveRadius = 3;
    [SerializeField] private int    m_friendlyFireDamage = 5;

    private bool m_friendly_fire = false;

    public override void InitialiseAgent(in Blackboard blackboard)
    {
        m_type = EnemyType.EXPLOSIVE;

        base.InitialiseAgent(blackboard);
    }

    protected override void InitialiseStateMachine()
    {
        State state = new SeekTargetState(this, m_blackboard, 0);
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SEEKTARGET");
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);

        m_friendly_fire = true;
    }

    public override bool IsDead()
    {
        if (base.IsDead())
        {
            Explode();
            return true;
        }
        else
            return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == m_target)
        {
            m_friendly_fire = false;
            m_current_health = 0;
        }
    }

    private void Explode()
    {
        Collider[] hit_colliders = Physics.OverlapSphere(transform.position, m_explosiveRadius);

        foreach (Collider c in hit_colliders)
        {
            Agent agent = c.GetComponentInChildren<Agent>();
            if (agent != null)
            {
                if (m_friendly_fire)
                    agent.TakeDamage(m_friendlyFireDamage);

                continue;
            }

            Mines mine = c.GetComponentInChildren<Mines>();
            if (mine != null)
            {
                mine.MinesTakeDamage(m_current_damage);
                continue;
            }

            Player player = c.GetComponentInChildren<Player>();
            if (player != null)
                m_blackboard.m_gameManager.script_player.PlayerTakenDamage(m_current_damage);
        }

        GameObject sfx = Instantiate(m_exlodeSFX, transform.position, Quaternion.identity).gameObject;
        Destroy(sfx, m_exlodeSFX.main.duration);
    }
}
