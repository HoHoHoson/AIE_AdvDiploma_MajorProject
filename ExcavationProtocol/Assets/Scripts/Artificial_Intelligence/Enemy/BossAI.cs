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
        state.AddTransition(new Transition("BEASTMODE", 
            new Condition[] { new CompareCondition(this, m_attackRange, CompareCondition.Comparator.LESS) }));
        m_state_machine.AddState(state);

        state = new SpinnyWinnyState(this, m_knockbackForce, m_movementMultiplierDuringAttack);
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new CompareCondition(this, m_attackRange, CompareCondition.Comparator.GREATER) }));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SEEKTARGET");
    }

    private void OnCollisionEnter(Collision collision)
    {
        State current = m_state_machine.GetCurrentState();

        if (current != null && current.GetIndex() == "BEASTMODE")
            (current as SpinnyWinnyState).OnHit(collision.gameObject);
    }
}
