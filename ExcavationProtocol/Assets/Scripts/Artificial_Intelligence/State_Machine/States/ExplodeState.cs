using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeState : State
{
    float m_explosive_radius = 3;

    public ExplodeState(in Agent agent) : base(agent) { }

    public override void InitialiseState()
    {
        base.InitialiseState();

        Collider[] hit_colliders = Physics.OverlapSphere(m_agent.transform.position, m_explosive_radius);
        foreach (Collider c in hit_colliders)
        {

        }
    }
}
