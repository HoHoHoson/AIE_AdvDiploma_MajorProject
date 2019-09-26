using UnityEngine;

public class SmallAI : Agent
{
    [SerializeField] private float m_playerDetectionRange   = 10;
    [SerializeField] private float m_leapRange              = 5;
    [SerializeField] private float m_leapCooldown           = 2;
    [SerializeField] private float m_leapAngle              = 45;
    [SerializeField] private float m_leapForce              = 10;

    public override void InitialiseAgent(in Blackboard blackboard)
    {
        m_type = EnemyType.BASIC;

        base.InitialiseAgent(blackboard);
    }

    protected override void InitialiseStateMachine()
    {
        State state = new IdleState(this); 
        m_state_machine.AddState(state);

        // Chases after the AI's set target
        state = new SeekTargetState(this, m_blackboard, m_playerDetectionRange);
        // Leaps at the targets face when in range
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new CompareCondition(this, m_leapRange, CompareCondition.Comparator.LESS) }));
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
