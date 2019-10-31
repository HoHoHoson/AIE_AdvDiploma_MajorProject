using UnityEngine;

public class SeekTargetState : State
{
    private float       m_grounding_force   = -1000f;
    private float       m_cast_offset       = 0.075f;
    private float       m_degrees_per_sec   = 90f;

    private Blackboard  m_blackboard;
    private float       m_player_detect_range;

    private Vector3     m_surface_normal    = Vector3.up;

    public SeekTargetState(in Agent agent, in Blackboard blackboard, float detect_range) : base(agent)
    {
        m_index                 = "SEEKTARGET";

        m_blackboard            = blackboard;
        m_player_detect_range   = detect_range;
    }

    public override void UpdateState()
    {
        UpdateTarget();
        Seek();

        TransitionCheck();
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

        GameObject closest_mine = null;
        float closest_distance_sqr = 0;

        
        
        if (m_blackboard.m_gameManager.Drill.GetComponentInChildren<Mines>().GetActive() == false)
		{ }

        if (closest_mine == null)
        {
            closest_mine = m_blackboard.m_gameManager.Drill;
            closest_distance_sqr = (m_agent.transform.position - m_blackboard.m_gameManager.Drill.transform.position).sqrMagnitude;

        }

        float distance_sqr = (m_agent.transform.position - m_blackboard.m_gameManager.Drill.transform.position).sqrMagnitude;

        if (distance_sqr < closest_distance_sqr)
        {
            closest_mine = m_blackboard.m_gameManager.Drill;
            closest_distance_sqr = distance_sqr;
        }
        

        if (closest_mine != null)
            m_agent.SetTarget(closest_mine);
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
        CalculateNormal();

        Vector3 target_direction = m_agent.GetTarget().transform.position - m_agent.transform.position;
        target_direction.y = 0;
        target_direction = target_direction.normalized;

        m_agent.transform.rotation =
            Quaternion.RotateTowards(m_agent.transform.rotation, 
            Quaternion.LookRotation(Vector3.ProjectOnPlane(target_direction, m_surface_normal)), 
            m_degrees_per_sec * Time.deltaTime);

        m_agent.GetRigidbody().velocity = (m_agent.transform.forward * m_agent.GetSpeed() + m_surface_normal * m_grounding_force) * Time.deltaTime;
        m_agent.GetRigidbody().angularVelocity = Vector3.zero;
    }

    private void CalculateNormal()
    {
        m_surface_normal = Vector3.up;

        float   length_to_spheres   = (m_agent.GetCollider().height * 0.5f) - m_agent.GetCollider().radius;
        Vector3 start_sphere_center = m_agent.transform.position + m_agent.transform.forward * length_to_spheres;
        Vector3 end_sphere_center   = m_agent.transform.position + -m_agent.transform.forward * length_to_spheres;

        // Offset Y axis due to the pivot not being centered
        start_sphere_center.y += m_agent.GetCollider().radius;
        end_sphere_center.y += m_agent.GetCollider().radius;

        RaycastHit hit;
        if (Physics.CapsuleCast(start_sphere_center, end_sphere_center, 
            m_agent.GetCollider().radius - m_cast_offset, -m_agent.transform.up, out hit, m_cast_offset * 2)
           && Vector3.Angle(hit.normal, Vector3.up) <= m_agent.GetSlopeAngle())
        {
            m_surface_normal = hit.normal;
        }
    }
}
