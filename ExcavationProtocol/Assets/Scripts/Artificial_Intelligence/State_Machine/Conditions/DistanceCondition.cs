using UnityEngine;

public class DistanceCondition : Condition
{
    public enum Comparator
    {
        LESS = 0,
        GREATER,
        EQUAL
    }

    private Transform m_start;
    private Transform m_end;
    private float m_sqr_threshold;
    private Comparator m_comparator;

    public DistanceCondition(in Transform start, in Transform end, float threshold, Comparator comparator)
    {
        m_start = start;
        m_end = end;
        m_sqr_threshold = threshold * threshold;
        m_comparator = comparator;
    }

    public override bool Check()
    {
        switch(m_comparator)
        {
            case Comparator.LESS:
                return SqrMag() < m_sqr_threshold;;
                                 
            case Comparator.GREATER:
                return SqrMag() > m_sqr_threshold;;
                                 
            case Comparator.EQUAL:
                return SqrMag() == m_sqr_threshold;

            default:
                Debug.Log("ERROR: DistanceCondition switch default.");
                break;
        }

        return false;
    }

    private float SqrMag()
    {
        return Vector3.SqrMagnitude(m_end.position - m_start.position);
    }
}
