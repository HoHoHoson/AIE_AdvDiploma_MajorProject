using UnityEngine;

public class SeekTargetState : State
{
    private Blackboard m_blackboard;
    private float m_player_detect_range;

    public SeekTargetState(in Blackboard blackboard, float detect_range)
    {
        m_index = "SEEKTARGET";
        m_blackboard = blackboard;
        m_player_detect_range = detect_range;
    }

    public override void UpdateState(in Agent agent)
    {
        UpdateTarget(agent);

        Vector3 target_direction = agent.GetTarget().transform.position - agent.transform.position;
        target_direction.y = 0;
        target_direction = target_direction.normalized;

        agent.transform.rotation = Quaternion.LookRotation(target_direction);

        Vector3 new_velocity = target_direction * agent.GetSpeed() * Time.deltaTime;
        new_velocity.y = agent.GetRB().velocity.y;

        agent.GetRB().velocity = new_velocity;
    }

    private void UpdateTarget(in Agent agent)
    {
        if (PlayerInRange(agent))
            return;

        GameObject closest_mine = null;
        float closest_distance_sqr = 0;

        foreach (GameObject mine in m_blackboard.m_gameManager.mines_list)
        {
            if (mine.activeInHierarchy == false)
                continue;

            if (closest_mine == null)
            {
                closest_mine = mine;
                closest_distance_sqr = (agent.transform.position - mine.transform.position).sqrMagnitude;

                continue;
            }

            float distance_sqr = (agent.transform.position - mine.transform.position).sqrMagnitude;

            if (distance_sqr < closest_distance_sqr)
            {
                closest_mine = mine;
                closest_distance_sqr = distance_sqr;
            }
        }

        if (closest_mine != null)
            agent.SetTarget(closest_mine);
        else
            agent.SetTarget(m_blackboard.m_gameManager.player_gameobject);
    }

    private bool PlayerInRange(in Agent agent)
    {
        Vector3 distance = agent.transform.position - m_blackboard.m_gameManager.player_gameobject.transform.position;

        float distance_sqr = distance.sqrMagnitude;
        float detect_range_sqr = m_player_detect_range * m_player_detect_range;

        if (distance_sqr < detect_range_sqr)
        {
            agent.SetTarget(m_blackboard.m_gameManager.player_gameobject);
            return true;
        }

        return false;
    }
}
