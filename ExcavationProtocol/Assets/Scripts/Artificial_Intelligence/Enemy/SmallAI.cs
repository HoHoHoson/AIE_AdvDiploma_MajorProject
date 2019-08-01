using UnityEngine;

public class SmallAI : Agent
{
    void Start()
    {
        m_health = 0;
        m_damage = 0;
        m_speed = 300;
        m_rigidbody = GetComponent<Rigidbody>();
        m_type = EnemyType.BASIC;

        m_state_machine = new StateMachine();
        m_target = GameObject.Find("FPS_Controller");

        State state = new IdleState();
        state.AddTransition(new Transition("CHASETARGET", 
            a => Vector3.Distance(a.transform.position, m_target.transform.position) > 3));
        m_state_machine.AddState(state);

        state = new ChaseTargetState();
        state.AddTransition(new Transition("IDLE",
            a => Vector3.Distance(a.transform.position, m_target.transform.position) < 3));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "IDLE");
    }

    private void Update()
    {
        UpdateAgent();
    }
}
