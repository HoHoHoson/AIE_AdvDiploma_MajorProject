using UnityEngine;

public class SeekTargetState : State
{
    private Blackboard  m_blackboard;
    private float       m_player_detect_range;

    public SeekTargetState(in Agent agent, in Blackboard blackboard, float detect_range) : base(agent)
    {
        m_index                 = "SEEKTARGET";

        m_blackboard            = blackboard;
        m_player_detect_range   = detect_range;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        UpdateTarget();
        Seek();
    }

    private void UpdateTarget()
    {
        if (PlayerInRange())
            return;

        GameObject closest_mine = null;
        float closest_distance_sqr = 0;

        foreach (GameObject mine in m_blackboard.m_gameManager.mines_list)
        {
            if (mine.GetComponentInChildren<Mines>().GetActive() == false)
                continue;

            if (closest_mine == null)
            {
                closest_mine = mine;
                closest_distance_sqr = (m_agent.transform.position - mine.transform.position).sqrMagnitude;

                continue;
            }

            float distance_sqr = (m_agent.transform.position - mine.transform.position).sqrMagnitude;

            if (distance_sqr < closest_distance_sqr)
            {
                closest_mine = mine;
                closest_distance_sqr = distance_sqr;
            }
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
        Vector3 target_direction = m_agent.GetTarget().transform.position - m_agent.transform.position;
        target_direction.y = 0;
        target_direction = target_direction.normalized;

        Vector3 normal = CalculateNormal();

        m_agent.transform.rotation =
            Quaternion.RotateTowards(m_agent.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(target_direction, normal)), Time.deltaTime * 180);

        m_agent.GetRigidbody().velocity = (m_agent.transform.forward * m_agent.GetSpeed() * Time.deltaTime) + -normal * 10;
        m_agent.GetRigidbody().angularVelocity = Vector3.zero;
    }

    private Vector3 CalculateNormal()
    {
        Vector3 normal = Vector3.up;

        float   length_to_spheres   = (m_agent.GetCollider().height * 0.5f) - m_agent.GetCollider().radius;
        Vector3 start_sphere_center = m_agent.transform.position + m_agent.transform.forward * length_to_spheres;
        Vector3 end_sphere_center   = m_agent.transform.position + -m_agent.transform.forward * length_to_spheres;

        // Offset Y axis due to the pivot not being centered
        start_sphere_center.y += m_agent.GetCollider().radius;
        end_sphere_center.y += m_agent.GetCollider().radius;

        RaycastHit hit;
        if (Physics.CapsuleCast(start_sphere_center, end_sphere_center, m_agent.GetCollider().radius - 0.075f, -m_agent.transform.up, out hit, 0.15f)
           && Vector3.Angle(hit.normal, Vector3.up) <= m_agent.GetSlopeAngle())
        {
            normal = hit.normal;
        }

        return normal;
    }
}
