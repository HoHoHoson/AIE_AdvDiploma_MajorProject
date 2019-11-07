using UnityEngine;

public class BossAI : Agent
{
    [SerializeField] private float m_detectRange = 30f;
    [SerializeField] private float m_attackRange = 0f;

    private bool m_damage_frames = false;

    public float GetAttackRange() { return m_attackRange; }
    public bool IsDamaging() { return m_damage_frames; }
    public void ToggleDamageFrames(bool ian) { m_damage_frames = !m_damage_frames; }

    public override void InitialiseAgent(in Blackboard blackboard)
    {
        base.InitialiseAgent(blackboard);

        m_type = EnemyType.BOSS;
    }

    protected override void InitialiseStateMachine()
    {
        State state = new SeekTargetState(this, m_blackboard, m_detectRange);
        state.AddTransition(new Transition("BOSSSWEEP", 
            new Condition[] { new CompareCondition(this, m_attackRange, CompareCondition.Comparator.LESS) }));
        m_state_machine.AddState(state);

        state = new BossSweepState(this, GetComponent<Animator>());
        state.AddTransition(new Transition("SEEKTARGET",
            new Condition[] { new BoolCondition((state as BossSweepState).AnimationEnded) }));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "SEEKTARGET");
    }
}
