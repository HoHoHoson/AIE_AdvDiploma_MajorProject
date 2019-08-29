using UnityEngine;

public class SmallAI : Agent
{
    [SerializeField] private float m_playerDetectionRange = 7;
    [SerializeField] private float m_leapRange = 5;
    [SerializeField] private float m_leapCooldown = 2;
    [SerializeField] private float m_leapAngle = 45;
    [SerializeField] private float m_leapForce = 2;

    void Start()
    {
        m_type      = EnemyType.BASIC;
        m_rigidbody = GetComponent<Rigidbody>();
        m_target    = m_blackboard.m_gameManager.player_gameobject;

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
        State state = new IdleState(); 
        m_state_machine.AddState(state);

        // Chases after the AI's set target
        state = new SeekTargetState(m_blackboard, m_playerDetectionRange);
        // Leaps at the targets face when in range
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new DistanceCondition(gameObject, m_target, m_leapRange, DistanceCondition.Comparator.LESS) }));
        m_state_machine.AddState(state);

        // LEAP 4 FACE
        state = new LeapAtState(m_leapAngle, m_leapForce);
        // Goes back to seeking its target when out of range or after a cooldown
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new DistanceCondition(gameObject, m_target, m_leapRange, DistanceCondition.Comparator.GREATER)
            , new TimerCondition(m_leapCooldown) }));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SEEKTARGET");
    }
}
