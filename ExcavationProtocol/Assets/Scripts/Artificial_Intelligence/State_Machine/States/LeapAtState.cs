using UnityEngine;

public class LeapAtState : State
{
    private float m_angle;
    private float m_force;

    public LeapAtState(float leap_angle, float force)
    {
        m_index = "LEAPAT";
        m_angle = leap_angle;
        m_force = force;
    }

    public override void OnInitialise(in Agent agent)
    {
        Vector3 target_direction;
        target_direction = agent.GetTarget().transform.position - agent.transform.position;
        target_direction = Quaternion.Euler(-m_angle, 0, 0) * target_direction;

        agent.GetRB().AddForce(target_direction * m_force, ForceMode.Impulse);
    }
}
