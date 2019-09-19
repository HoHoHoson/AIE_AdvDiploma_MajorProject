using UnityEngine;

public class SeekTargetState : State
{
    private Blackboard  m_blackboard;
    private float       m_player_detect_range;

    private Vector3     m_normal = Vector3.up;

    public void SetNormal(Vector3 normal) { m_normal = normal; }

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

        Vector3 target_direction = m_agent.GetTarget().transform.position - m_agent.transform.position;
        target_direction.y = 0;
        target_direction = target_direction.normalized;

        m_agent.transform.rotation = Quaternion.LookRotation(target_direction);

        Vector3 new_velocity = target_direction * m_agent.GetSpeed() * Time.deltaTime;
        new_velocity.y = -10;

        m_agent.GetRB().velocity = new_velocity;
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
}
