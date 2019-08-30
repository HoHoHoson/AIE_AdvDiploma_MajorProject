using UnityEngine;

public class SmallAI : Agent
{
    [SerializeField] private float m_playerDetectionRange   = 10;
    [SerializeField] private float m_leapRange              = 5;
    [SerializeField] private float m_leapCooldown           = 2;
    [SerializeField] private float m_leapAngle              = 45;
    [SerializeField] private float m_leapForce              = 10;

    void Start()
    {
        m_type      = EnemyType.BASIC;
        m_rigidbody = GetComponent<Rigidbody>();
        m_target    = m_blackboard.m_gameManager.player_gameobject;

        InitiateStateMachine();
    }

    private void InitiateStateMachine()
    {
        m_state_machine = new StateMachine();

        // Selects a target for the AI
        State state = new IdleState(this); 
        m_state_machine.AddState(state);

        // Chases after the AI's set target
        state = new SeekTargetState(this, m_blackboard, m_playerDetectionRange);
        // Leaps at the targets face when in range
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new DistanceCondition(this, m_leapRange, DistanceCondition.Comparator.LESS) }));
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
        LeapAtState leap_state = m_state_machine.GetCurrentState() as LeapAtState;

        if (leap_state != null)
            leap_state.OnHit(collision.gameObject);
    }
}
