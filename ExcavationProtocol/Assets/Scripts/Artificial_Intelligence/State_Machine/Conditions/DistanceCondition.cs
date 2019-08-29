using UnityEngine;

public class DistanceCondition : Condition
{
    public enum Comparator
    {
        LESS = 0,
        GREATER,
        EQUAL
    }

    private GameObject m_start;
    private GameObject m_end;
    private float m_sqr_threshold;
    private Comparator m_comparator;

    public DistanceCondition(in GameObject start, in GameObject end, float threshold, Comparator comparator)
    {
        m_start = start;
        m_end = end;
        m_sqr_threshold = threshold * threshold;
        m_comparator = comparator;
    }

    public override bool Check()
    {
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
        return Vector3.SqrMagnitude(m_end.transform.position - m_start.transform.position);
    }
}
