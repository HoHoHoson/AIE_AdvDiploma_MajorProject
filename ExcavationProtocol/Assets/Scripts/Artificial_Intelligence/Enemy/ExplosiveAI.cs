using UnityEngine;

public class ExplosiveAI : Agent
{
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mine")
            m_current_health = 0;
    }
}
