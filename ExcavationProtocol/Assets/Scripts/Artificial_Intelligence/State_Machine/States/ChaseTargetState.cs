using UnityEngine;

public class ChaseTargetState : State
{
    public ChaseTargetState() { m_index = "CHASETARGET"; }

    public override void OnInitialise(in Agent agent)
    {
        Debug.Log("Entering Chase State.");
    }

    public override void UpdateState(in Agent agent)
    {
        Vector3 chase_dir = agent.GetTarget().transform.position - agent.transform.position;
        chase_dir.y = 0;
        chase_dir = chase_dir.normalized * agent.GetSpeed() * Time.deltaTime;
        chase_dir.y = agent.GetRB().velocity.y;

        agent.GetRB().velocity = chase_dir;
    }

    public override void OnExit(in Agent agent)
    {
        Debug.Log("Exiting Chase State.");
    }
}
