using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTargetState : State
{
    private GameManager m_game_manager          = null;
    private float       m_player_detect_range   = 0;
    private bool        m_ignore_player         = false;

    private GameObject  m_target_aquired        = null;

    public SelectTargetState(in GameManager gm, float detect_range, bool ignore_player = false)
    {
        m_index = "SELECTTARGET";
        m_game_manager = gm;
        m_player_detect_range = detect_range;
        m_ignore_player = ignore_player;
    }

    public override void OnInitialise(in Agent agent)
    {
        if ((agent.transform.position - m_game_manager.player_gameobject.transform.position).sqrMagnitude < Mathf.Pow(m_player_detect_range, 2))
        {
            m_target_aquired = m_game_manager.player_gameobject;
        }
        else
        {
            GameObject  closest_mine = null;
            float       closest_distance_sqr = 0;

            foreach (GameObject mine in m_game_manager.mines_list)
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

            m_target_aquired = closest_mine;
        }

        agent.SetTarget(m_target_aquired);
    }

    public bool HasSetTarget()
    {
        if (m_target_aquired == null)
            return false;
        else
            return true;
    }
}
