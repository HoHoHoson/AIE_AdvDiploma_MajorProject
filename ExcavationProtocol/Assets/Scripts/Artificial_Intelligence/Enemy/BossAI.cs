using UnityEngine;

public class BossAI : Agent
{
    [SerializeField] private float m_detectRange = 30;
    [SerializeField] private float m_movementMultiplierDuringAttack = 0.9f;
    [SerializeField] private float m_attackRange = 8;
    [SerializeField] private float m_knockbackForce = 40;

    public override void InitialiseAgent(in Blackboard blackboard)
    {
        base.InitialiseAgent(blackboard);

        m_type = EnemyType.BOSS;
    }

    protected override void InitialiseStateMachine()
    {
        State state = new SeekTargetState(this, m_blackboard, m_detectRange);
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SEEKTARGET");
    }
}
