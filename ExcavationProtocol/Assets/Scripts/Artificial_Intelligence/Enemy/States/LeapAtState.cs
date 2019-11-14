using UnityEngine;

public class LeapAtState : State
{
    private Collider    m_collider;
    private float       m_angle;
    private float       m_force;
    private float       m_cooldown;

    private bool    m_can_transition;
    private bool    m_attacked;
    private float   m_cooldown_timer;
    private float   m_reorientation_time = 0.3f;

    public LeapAtState(in Agent agent, float angle, float force, float cooldown) : base(agent)
    {
        m_index     = "LEAPAT";

        m_angle     = angle;
        m_force     = force;
        m_cooldown  = cooldown;

        m_can_transition    = false;
        m_attacked          = false;

        m_collider  = m_agent.GetComponentInChildren<Collider>();
    }

    public override void InitialiseState()
    {
        base.InitialiseState();

        m_collider.material.bounciness = 0.9f;
        m_cooldown_timer = m_cooldown;

        Leap();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        m_agent.GetRigidbody().AddForce(Physics.gravity, ForceMode.Acceleration);

        if (!m_can_transition)
            m_cooldown_timer -= Time.deltaTime;
        else
        {
            Vector3 target_euler = m_agent.transform.eulerAngles;
            target_euler.z = 0;

            m_agent.transform.rotation = 
                Quaternion.Lerp(m_agent.transform.rotation, Quaternion.Euler(target_euler), m_cooldown_timer / m_reorientation_time);

            m_cooldown_timer += Time.deltaTime;
        }
    }

    public override void ExitState()
    {
        m_collider.material.bounciness = 0;

        m_can_transition    = false;
        m_attacked          = false;
    }

    public void OnHit(in GameObject hit)
    {
        OnStay(hit);

        if (m_attacked == true)
            return;

        m_attacked = true;

        Mines   hit_mine    = hit.GetComponentInChildren<Mines>();
        Player  hit_fps     = hit.GetComponentInChildren<Player>();

        if (hit_mine != null)
        {
            hit_mine.MinesTakeDamage(m_agent.GetDamage());
        }
        else if (hit_fps != null)
        {
            hit_fps.PlayerTakenDamage(m_agent.GetDamage());
        }
        else
            m_attacked = false;
    }

    public void OnStay(in GameObject hit)
    {
        if (hit.layer == 9)
            if (m_cooldown_timer < 0)
            {
                m_agent.GetRigidbody().velocity = Vector3.zero;
                m_agent.GetRigidbody().angularVelocity = Vector3.zero;

                m_cooldown_timer = 0;
                m_can_transition = true;
            }
    }

    private void Leap()
    {
        Vector3 leap_direction;

        leap_direction = m_agent.GetTarget().transform.position - m_agent.transform.position;
        leap_direction = Quaternion.AngleAxis(m_angle, Vector3.Cross(leap_direction, Vector3.up)) * leap_direction;

        Vector3 orthogonal_velocity = m_agent.GetRigidbody().velocity;
        orthogonal_velocity.y = 0;
        Vector3 orthogonal_leap = new Vector3(leap_direction.x, 0, leap_direction.z);

        float velocity_angle_offset = Vector3.Angle(orthogonal_velocity, orthogonal_leap);
        velocity_angle_offset *= Vector3.Dot(orthogonal_leap, new Vector3(orthogonal_velocity.z, 0, -orthogonal_velocity.x)) < 0 ? -1 : 1;

        // Redirecting current velocity towards intended target
        m_agent.GetRigidbody().velocity = Quaternion.Euler(0, velocity_angle_offset, 0) * m_agent.GetRigidbody().velocity;
        // Applying leap force
        m_agent.GetRigidbody().AddForce(leap_direction.normalized * m_force, ForceMode.Impulse);
        // Adds spin to the leap
        m_agent.GetRigidbody().AddRelativeTorque(-Vector3.right * m_force, ForceMode.Impulse);
    }

    public bool LeapComplete()
    {
        return m_can_transition && m_cooldown_timer >= m_reorientation_time;
    }
}
