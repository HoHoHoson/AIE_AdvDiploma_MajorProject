using UnityEngine;

public class SmallAI : Agent
{
    [SerializeField] private float  m_playerSeekRadius  = 10;
    [SerializeField] private float  m_leapRadius        = 5;
    [SerializeField] private float  m_randomLeapChance  = 0.01f;
    [SerializeField] private int    m_tickRate          = 10;

    private Animator m_animator;

    private float m_tick_time;
    private float m_random_leap_timer = 0;

    public override void InitialiseAgent(in Blackboard blackboard)
    {
        base.InitialiseAgent(blackboard);

        m_type = EnemyType.BASIC;
        m_animator = GetComponent<Animator>();
        m_tick_time = 1f / m_tickRate;
    }

    public override void UpdateAgent()
    {
        base.UpdateAgent();

        State current_state = m_state_machine.GetCurrentState();
        bool state_found = false;

        if (!state_found || current_state is SeekTargetState)
        {
            m_random_leap_timer += Time.deltaTime;
            state_found = true;
        }

        if (!state_found || current_state is LeapAtState)
        {
            m_animator.SetBool("Attack", true);
            state_found = true;
        }
        else
            m_animator.SetBool("Attack", false);
    }

    public bool RandomLeapCheck()
    {
        if (m_random_leap_timer > m_tick_time)
        {
            m_random_leap_timer -= m_tick_time;

            if (Random.value <= m_randomLeapChance)
                return true;
        }

        return false;
    }

    protected override void InitialiseStateMachine()
    {
        State state = new IdleState(this); 
        m_state_machine.AddState(state);

        // Chases after the AI's set target
        state = new SeekTargetState(this, m_blackboard, m_playerSeekRadius);
        // Leaps at the targets face when in range
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new BoolCondition(RandomLeapCheck),
                              new CompareCondition(this, m_leapRadius, CompareCondition.Comparator.LESS),
                              new BoolCondition(GetCliffLeap) }));
        m_state_machine.AddState(state);

        // LEAP 4 FACE
        state = new LeapAtState(this, m_leapAngle, m_leapForce, m_leapCooldown);
        // Goes back to seeking its target when out of range or after a cooldown
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new BoolCondition((state as LeapAtState).IsCooldownOver) }));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SEEKTARGET");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_state_machine == null)
            return;

        State current = m_state_machine.GetCurrentState();

        if (current != null && (current.GetIndex() == "LEAPAT"))
        {
            (current as LeapAtState).OnHit(collision.gameObject);
            return;
        }
    }
}
