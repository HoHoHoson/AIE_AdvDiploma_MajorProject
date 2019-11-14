using UnityEngine;

public class IdleState : State
{
    public IdleState(in Agent agent) : base(agent) { m_index = "IDLE"; }

    public override void UpdateState()
    {
        base.UpdateState();

        Vector3 idle_velocity = Vector3.zero;

        idle_velocity.y = m_agent.GetRigidbody().velocity.y;
        m_agent.GetRigidbody().velocity = idle_velocity;
        m_agent.GetRigidbody().angularVelocity = idle_velocity;

        m_agent.GetRigidbody().AddForce(Physics.gravity, ForceMode.Acceleration);
    }
}
