using UnityEngine;

public class TimerCondition : Condition
{
    private float   m_timer = 0;
    private float   m_threshold = 0;

    public TimerCondition(float seconds) { m_threshold = seconds; }

    public override void InitiateCondition()
    {
        m_timer = 0;
    }

    public override bool Check()
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_threshold)
            return true;
        else
            return false;
    }
}
