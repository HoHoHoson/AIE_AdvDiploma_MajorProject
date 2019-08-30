using UnityEngine;

public class LeapAtState : State
{
    private float m_angle;
    private float m_force;

    private float m_cooldown;
    private float m_timer;   

    public LeapAtState(in Agent agent, float angle, float force, float cooldown) : base(agent)
    {
        m_index     = "LEAPAT";

        m_angle     = angle;
        m_force     = force;
        m_cooldown  = cooldown;
    }

    public override void InitialiseState()
    {
        base.InitialiseState();

        m_timer = 0;
        Leap();
    }

    public override void UpdateState()
    {
        if (m_timer < m_cooldown)
            m_timer += Time.deltaTime;

        base.UpdateState();
    }

    public bool IsCooldownOver()
    {
        return m_timer >= m_cooldown;
    }

    public void OnHit(in GameObject hit)
    {
        Mines       hit_mine    = hit.GetComponentInChildren<Mines>();
        FPSControl  hit_fps     = hit.GetComponentInChildren<FPSControl>();

        if (hit_mine != null)
        {
            hit_mine.MinesTakeDamage(m_agent.GetDamage());
        }
        else if (hit_fps != null)
        {
            hit_fps.script_gm.PlayerTakenDamage(m_agent.GetDamage());
        }
    }

    private void Leap()
    {
        Vector3 leap_direction;

        leap_direction = m_agent.GetTarget().transform.position - m_agent.transform.position;
        leap_direction.y = 0;
        leap_direction = leap_direction.normalized;
        leap_direction = Quaternion.AngleAxis(m_angle, Vector3.Cross(leap_direction, Vector3.up)) * leap_direction;

        m_agent.GetRB().AddForce(leap_direction * m_force, ForceMode.Impulse);
    }
}
