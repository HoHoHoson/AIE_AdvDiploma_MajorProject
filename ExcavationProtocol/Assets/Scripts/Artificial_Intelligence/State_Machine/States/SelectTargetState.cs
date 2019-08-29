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
    }

    public bool HasSetTarget()
    {
        if (m_target_aquired == null)
            return false;
        else
            return true;
    }
}
