using UnityEngine;

public class ChaseTargetState : State
{
    public ChaseTargetState() { m_index = "CHASETARGET"; }

    public override void UpdateState(in Agent agent)
    {
        Vector3 target_direction = agent.GetTarget().transform.position - agent.transform.position;
        target_direction.y = 0;
        target_direction = target_direction.normalized;

        agent.transform.rotation = Quaternion.LookRotation(target_direction);

        Vector3 new_velocity = target_direction * agent.GetSpeed() * Time.deltaTime;
        new_velocity.y = agent.GetRB().velocity.y;

        //RaycastHit hit;
        //if (Physics.CapsuleCast(agent.transform.position, 0.1f, out hit))

        agent.GetRB().velocity = new_velocity;
    }
}
