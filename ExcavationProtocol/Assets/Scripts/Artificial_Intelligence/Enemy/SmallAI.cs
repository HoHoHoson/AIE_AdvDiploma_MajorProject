using UnityEngine;

public class SmallAI : Agent
{
    [SerializeField] private float m_leapAngle = 45;
    [SerializeField] private float m_leapForce = 2;

    void Start()
    {
        m_type      = EnemyType.BASIC;
        m_rigidbody = GetComponent<Rigidbody>();

        InitiateStateMachine();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_blackboard == null)
        {
            Debug.Log("ERROR: AI Blackboard has not been set.");
        }
        else if (m_blackboard.m_gameManager == null)
        {
            Debug.Log("ERROR: AI Blackboard doesn't have an instance of a GameManager.");
        }
        else if (m_state_machine.GetCurrentState().GetIndex() == "LEAPAT" && collision.collider.GetComponent<FPSControl>())
        {
            m_blackboard.m_gameManager.PlayerTakenDamage(m_current_damage);
        }
    }

    private void InitiateStateMachine()
    {
        m_state_machine = new StateMachine();

        // Selects a target for the AI
        State state = new SelectTargetState(m_blackboard.m_gameManager, 5);
        // Idles if it can't find a single target
        state.AddTransition(new Transition("IDLE",
            new Condition[] { new BoolCondition((state as SelectTargetState).HasSetTarget, true) }));
        // Seeks target when a target is found
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new BoolCondition((state as SelectTargetState).HasSetTarget) }));
        m_state_machine.AddState(state);

        // AI does absolutely nothing
        state = new IdleState();
        m_state_machine.AddState(state);

        // Chases after the AI's set target
        state = new SeekTargetState();
        // Leaps at the targets face when in range
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new DistanceCondition(transform, m_target.transform, 3, DistanceCondition.Comparator.LESS) }));
        m_state_machine.AddState(state);

        // LEAP 4 FACE
        state = new LeapAtState(m_leapAngle, m_leapForce);
        // Goes back to seeking its target when out of range or after a cooldown
        state.AddTransition(new Transition("CHASETARGET",
            new Condition[] { new DistanceCondition(transform, m_target.transform, 3, DistanceCondition.Comparator.GREATER)
            , new TimerCondition(2) }));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SELECTTARGET");
    }
}
