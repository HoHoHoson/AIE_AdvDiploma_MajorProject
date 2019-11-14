using UnityEngine;

public class ExplosiveAI : Agent
{
    [Header("Kapooya Man Settings")]
    [SerializeField] ParticleSystem m_explodeSFX = null;
    [SerializeField] AudioSource m_explodeSound = null;

    [SerializeField] private float  m_explosiveRadius = 3;
    [SerializeField] private int    m_friendlyFireDamage = 5;
    [SerializeField] private float  m_detectRange = 0;

    private bool m_friendly_fire = false;

    public bool IsLit() { return m_animator.GetBool("FuseLit"); }

    public override void InitialiseAgent(in Blackboard blackboard)
    {
        base.InitialiseAgent(blackboard);

        m_type = EnemyType.EXPLOSIVE;
    }

    public override void ResetStats()
    {
        base.ResetStats();

        m_animator.SetBool("FuseLit", false);
    }

    protected override void InitialiseStateMachine()
    {
        State state = new LeapAtState(this, m_leapAngle, m_leapForce, m_leapCooldown);
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new BoolCondition((state as LeapAtState).LeapComplete) }));
        m_state_machine.AddState(state);

        state = new SeekTargetState(this, m_blackboard, m_detectRange, m_rotationSpeed);
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new BoolCondition(GetCliffLeap) }));
        state.AddTransition(new Transition("IDLE",
            new Condition[] { new BoolCondition(IsLit) }));
        m_state_machine.AddState(state);

        state = new IdleState(this);
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new BoolCondition(IsLit, true) }));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SEEKTARGET");
    }

    public void LocationalDamage(in RaycastHit hit, int damage)
    {
        if (hit.collider.isTrigger == true)
        {
            m_friendly_fire = true;
            Explode();
        }
        else
            TakeDamage(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == m_target)
        {
            m_friendly_fire = false;

            m_animator.SetBool("FuseLit", true);
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

        GameObject sfx = Instantiate(m_explodeSFX, transform.position, Quaternion.identity).gameObject;
        AudioSource audio = Instantiate(m_explodeSound, sfx.transform);

        Destroy(sfx, audio.clip.length);

        m_current_health = 0;
    }
}
