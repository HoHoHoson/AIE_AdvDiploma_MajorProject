using UnityEngine;

public class SeekTargetState : State
{
    private Blackboard  m_blackboard;
    private float       m_player_detect_range;

    private float   m_grounding_force   = -1000f;
    private float   m_gravity;
    private float   m_spherecast_offset = 0.075f;
    private float   m_rotate_per_sec    = 90f;
    private Vector3 m_surface_normal    = Vector3.up;

    private float   m_distance_threshold    = 0.01f;
    private float   m_reset_time            = 5f;
    private float   m_reset_timer           = 0;
    private Vector3 m_previous_position     = Vector3.zero;

    public SeekTargetState(in Agent agent, in Blackboard blackboard, float detect_range, float rotate_rate) : base(agent)
    {
        m_index                 = "SEEKTARGET";

        m_blackboard            = blackboard;
        m_player_detect_range   = detect_range;
        m_rotate_per_sec        = rotate_rate;
    }

    public override void InitialiseState()
    {
        base.InitialiseState();

        m_gravity = m_agent.GetRigidbody().velocity.y;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        UpdateTarget();
        Seek();

        if (IsStuck())
            m_agent.gameObject.SetActive(false);
    }

    public override void ExitState()
    {
        // Revert grounding force when exiting seek state
        Vector3 velocity = m_agent.GetRigidbody().velocity;
        velocity -= m_surface_normal * m_grounding_force * Time.deltaTime;

        m_agent.GetRigidbody().velocity = velocity;
    }

    private void UpdateTarget()
    {
        if (PlayerInRange())
            return;
		
        if (m_blackboard.mine.GetComponent<Mines>().is_active == true)
            m_agent.SetTarget(m_blackboard.mine);
        else
            m_agent.SetTarget(m_blackboard.m_gameManager.player_gameobject);
    }

    private bool PlayerInRange()
    {
        Vector3 distance = m_agent.transform.position - m_blackboard.m_gameManager.player_gameobject.transform.position;

        float distance_sqr = distance.sqrMagnitude;
        float detect_range_sqr = m_player_detect_range * m_player_detect_range;

        if (distance_sqr < detect_range_sqr)
        {
            m_agent.SetTarget(m_blackboard.m_gameManager.player_gameobject);
            return true;
        }

        return false;
    }

    private void Seek()
    {
        Vector3 grounding_velocity;

        if (CalculateNormal())
        {
            grounding_velocity = m_surface_normal * m_grounding_force;
            m_gravity = 0;
        }
        else
        {
            grounding_velocity = m_surface_normal * m_gravity;
            m_gravity += Physics.gravity.y;
        }

        Vector3 target_direction = m_agent.GetTarget().transform.position - m_agent.transform.position;
        target_direction.y = 0;
        target_direction = Vector3.ProjectOnPlane(target_direction, m_surface_normal).normalized;

        m_agent.transform.rotation =
            Quaternion.RotateTowards(m_agent.transform.rotation, Quaternion.LookRotation(target_direction), m_rotate_per_sec * Time.deltaTime);

        m_agent.GetRigidbody().velocity = (target_direction * m_agent.GetSpeed() + grounding_velocity) * Time.deltaTime;
        m_agent.GetRigidbody().angularVelocity = Vector3.zero;
    }

    private bool CalculateNormal()
    {
        float   length_to_spheres   = (m_agent.GetCollider().height * 0.5f) - m_agent.GetCollider().radius;
        Vector3 start_sphere_center = m_agent.GetCollider().bounds.center + m_agent.transform.forward * length_to_spheres;
        Vector3 end_sphere_center   = m_agent.GetCollider().bounds.center + -m_agent.transform.forward * length_to_spheres;

        RaycastHit hit;
        if (Physics.CapsuleCast(start_sphere_center, end_sphere_center, 
            m_agent.GetCollider().radius - m_spherecast_offset, -m_agent.transform.up, out hit, m_spherecast_offset * 2)
            && Vector3.Angle(Vector3.up, m_surface_normal) < m_agent.GetSlopeAngle())
        {
            m_surface_normal = hit.normal;;
            Debug.Log(Vector3.Angle(Vector3.up, m_surface_normal));
            return true;
        }
        else
            m_surface_normal = Vector3.up;

        return false;
    }

    private bool IsStuck()
    {
        bool is_stuck = false;
        Vector3 distance_delta = m_agent.transform.position - m_previous_position;

        if (distance_delta.sqrMagnitude <= m_distance_threshold)
        {
            m_reset_timer += Time.deltaTime;

            if (m_reset_timer >= m_reset_time)
            {
                m_reset_timer = 0;
                is_stuck = true;
            }
        }
        else
            m_reset_timer = 0;

        m_previous_position = m_agent.transform.position;

        return is_stuck;
    }
}
