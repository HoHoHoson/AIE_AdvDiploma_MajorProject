using UnityEngine;

public class IdleState : State
{
    public IdleState() { m_index = "IDLE"; }

    public override void OnInitialise(in Agent agent)
    {
    }

    public override void OnExit(in Agent agent)
    {
    }
}
