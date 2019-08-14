using UnityEngine;

public class ChaseTargetState : State
{
    public ChaseTargetState() { m_index = "CHASETARGET"; }

    public override void OnInitialise(in Agent agent)
    {
    }

    public override void UpdateState(in Agent agent)
    {
        Vector3 target_direction = agent.GetTarget().transform.position - agent.transform.position;
        target_direction.y = 0;
        target_direction = target_direction.normalized;

        agent.transform.rotation = Quaternion.LookRotation(target_direction);
    }

    public override void OnExit(in Agent agent)
    {
    }
}
