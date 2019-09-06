using UnityEngine;

public class CompareCondition : Condition
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

    public CompareCondition(in Transform start, in Transform end, float threshold, Comparator comparator)
    {
        m_start = start;
        m_end   = end;

        m_comparator    = comparator;
        m_sqr_threshold = threshold * threshold;
    }

    public CompareCondition(in Agent agent, float threshold, Comparator comparator)
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
        // Square distance between two objects, not factoring collider radii
        float length = Vector3.SqrMagnitude(m_end.position - m_start.position);

        Collider start_collider = m_start.GetComponentInChildren<Collider>();
        Collider end_collider = m_end.GetComponentInChildren<Collider>();

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
}
