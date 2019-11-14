using UnityEngine;

public class BossAI : Agent
{
    [Header("Boss Settings")]
    [SerializeField] private float m_detectRange = 30f;
    [SerializeField] private float m_attackRange = 4f;

    public override void InitialiseAgent(in Blackboard blackboard)
    {
        base.InitialiseAgent(blackboard);

        m_type = EnemyType.BOSS;
    }

    protected override void InitialiseStateMachine()
    {
        State state = new SeekTargetState(this, m_blackboard, m_detectRange, m_rotationSpeed);
        state.AddTransition(new Transition("BOSSSWEEP", 
            new Condition[] { new CompareCondition(this, m_attackRange, CompareCondition.Comparator.LESS) }));
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new BoolCondition(GetCliffLeap) }));
        m_state_machine.AddState(state);

        state = new BossSweepState(this, GetComponent<Animator>());
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new BoolCondition((state as BossSweepState).AnimationEnded) }));
        m_state_machine.AddState(state);

        state = new LeapAtState(this, m_leapAngle, m_leapForce, m_leapCooldown);
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new BoolCondition((state as LeapAtState).LeapComplete) }));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SEEKTARGET");
    }
}
