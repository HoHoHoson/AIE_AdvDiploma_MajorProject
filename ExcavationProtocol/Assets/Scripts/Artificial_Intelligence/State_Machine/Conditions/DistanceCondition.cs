using UnityEngine;

public class DistanceCondition : Condition
{
    public enum Comparator
    {
        LESS = 0,
        GREATER,
        EQUAL
    }

    private Agent       m_agent;
    private Transform   m_start;
    private Transform   m_end;
    private Comparator  m_comparator;
    private float       m_sqr_threshold;

    public DistanceCondition(in Transform start, in Transform end, float threshold, Comparator comparator)
    {
        m_start = start;
        m_end   = end;

        m_comparator    = comparator;
        m_sqr_threshold = threshold * threshold;
    }

    public DistanceCondition(in Agent agent, float threshold, Comparator comparator)
    {
        m_agent = agent;

        m_comparator    = comparator;
        m_sqr_threshold = threshold * threshold;
    }

    public override bool Check()
    {
        if (m_agent != null)
        {
            m_start = m_agent.transform;
            m_end   = m_agent.GetTarget().transform;
        }

        if (m_start != null && m_end != null)
            switch(m_comparator)
            {
                case Comparator.LESS:
                    return SqrDistance() < m_sqr_threshold;;
                                 
                case Comparator.GREATER:
                    return SqrDistance() > m_sqr_threshold;;
                                 
                case Comparator.EQUAL:
                    return SqrDistance() == m_sqr_threshold;

                default:
                    break;
            }

        Debug.Log("ERROR: DistanceCondition Check failed.");
        return false;
    }

    private float SqrDistance()
    {
        Collider start_collider = m_start.GetComponentInChildren<Collider>();
        Collider end_collider = m_end.GetComponentInChildren<Collider>();

        // Square distance between two objects, not factoring collider radii
        float length = Vector3.SqrMagnitude(ColliderPivot(end_collider) - ColliderPivot(start_collider));

        if (start_collider != null)
        {
            Vector3 extents = start_collider.bounds.extents;
            length -= (extents.x * extents.x) + (extents.z * extents.z);
        }

        if (end_collider != null)
        {
            Vector3 extents = end_collider.bounds.extents;
            length -= (extents.x * extents.x) + (extents.z * extents.z);
        }

        return length;
    }

    private Vector3 ColliderPivot(Collider collider)
    {
        Transform go_transform = collider.transform;

        Vector3 projected_pivot             = Vector3.ProjectOnPlane(go_transform.position, go_transform.up);
        Vector3 projected_collider_center   = Vector3.ProjectOnPlane(collider.bounds.center, go_transform.up);

        Vector3 distance_to_plane = go_transform.position - projected_pivot;

        return projected_collider_center + distance_to_plane;
    }
}
