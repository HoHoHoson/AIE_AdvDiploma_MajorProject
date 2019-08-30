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
                    return SqrMag() < m_sqr_threshold;;
                                 
                case Comparator.GREATER:
                    return SqrMag() > m_sqr_threshold;;
                                 
                case Comparator.EQUAL:
                    return SqrMag() == m_sqr_threshold;

                default:
                    break;
            }

        Debug.Log("ERROR: DistanceCondition switch statement failed.");
        return false;
    }

    private float SqrMag()
    {
        return Vector3.SqrMagnitude(m_end.position - m_start.position);
    }
}
