using UnityEngine;

public class IdleState : State
{
    public IdleState(in Agent agent) : base(agent) { m_index = "IDLE"; }
}
