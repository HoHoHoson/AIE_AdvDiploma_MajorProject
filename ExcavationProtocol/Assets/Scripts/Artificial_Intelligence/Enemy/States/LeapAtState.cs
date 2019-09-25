using UnityEngine;

public class LeapAtState : State
{
    private float m_angle;
    private float m_force;

    private float m_cooldown;
    private float m_timer;

    private Collider m_collider;

    public LeapAtState(in Agent agent, float angle, float force, float cooldown) : base(agent)
    {
        m_index     = "LEAPAT";

        m_angle     = angle;
        m_force     = force;
        m_cooldown  = cooldown;

        m_collider = m_agent.GetComponentInChildren<Collider>();
    }

    public override void InitialiseState()
    {
        base.InitialiseState();

        m_collider.material.bounciness = 1;
        m_timer = 0;
        Leap();
    }

    public override void UpdateState()
    {
        // SUBJECT TO CHANGE
        m_timer += Time.deltaTime;
        m_agent.GetRigidbody().AddForce(Physics.gravity, ForceMode.Acceleration);
        // SUBJECT TO CHANGE

        TransitionCheck();
    }

    public override void ExitState()
    {
        m_collider.material.bounciness = 0;
    }

    // Need to implement a better transition condition. Maybe add an isGrounded check
    public bool IsCooldownOver()
    {
        return m_timer >= m_cooldown;
    }

    public void OnHit(in GameObject hit)
    {
        Mines   hit_mine    = hit.GetComponentInChildren<Mines>();
        Player  hit_fps     = hit.GetComponentInChildren<Player>();

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

        m_agent.GetRigidbody().AddForce(leap_direction * m_force, ForceMode.Impulse);

        m_agent.GetRigidbody().AddRelativeTorque(-Vector3.right * m_force, ForceMode.Impulse);
    }
}
