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
        //m_target = null;

        m_state_machine.ChangeState(this, new ChaseTargetState());
    }

    private void Update()
    {
        UpdateAgent();
    }
}
