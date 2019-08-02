using UnityEngine;

public class IdleState : State
{
    public IdleState() { m_index = "IDLE"; }

    public override void OnInitialise(in Agent agent)
    {
        Debug.Log("Entering Idle State.");
    }

    public override void OnExit(in Agent agent)
    {
        Debug.Log("Exiting Idle State.");
    }
}
