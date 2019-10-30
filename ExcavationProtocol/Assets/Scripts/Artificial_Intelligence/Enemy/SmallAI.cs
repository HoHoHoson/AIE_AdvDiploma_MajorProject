using UnityEngine;

public class SmallAI : Agent
{
    [SerializeField] private float m_playerSeekRadius   = 10;
    [SerializeField] private float m_leapRadius         = 5;

    private Animator m_animator;

    public override void InitialiseAgent(in Blackboard blackboard)
    {
        base.InitialiseAgent(blackboard);

        m_type = EnemyType.BASIC;
        m_animator = GetComponent<Animator>();
    }

    public override void UpdateAgent()
    {
        base.UpdateAgent();

        if (m_state_machine.GetCurrentState() is LeapAtState)
            m_animator.SetBool("Attack", true);
        else
            m_animator.SetBool("Attack", false);
    }

    protected override void InitialiseStateMachine()
    {
        State state = new IdleState(this); 
        m_state_machine.AddState(state);

        // Chases after the AI's set target
        state = new SeekTargetState(this, m_blackboard, m_playerSeekRadius);
        // Leaps at the targets face when in range
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new CompareCondition(this, m_leapRadius, CompareCondition.Comparator.LESS),
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
