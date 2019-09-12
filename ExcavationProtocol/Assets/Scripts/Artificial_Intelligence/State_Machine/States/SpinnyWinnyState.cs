using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnyWinnyState : State
{
    private float m_default_speed = 0;
    private float m_force = 0;
    private float m_movementPenalty = 0;

    public SpinnyWinnyState(in Agent agent, float force, float movement) : base(agent)
    {
        m_index = "BEASTMODE";

        m_force = force;
        m_movementPenalty = movement;
    }

    public override void InitialiseState()
    {
        base.InitialiseState();

        m_default_speed = m_agent.GetSpeed();
        m_agent.SetSpeed(m_default_speed * m_movementPenalty);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        Vector3 direction = m_agent.GetTarget().transform.position - m_agent.transform.position;
        direction.y = 0;
        direction = direction.normalized;
      
        m_agent.GetRB().velocity = direction * m_agent.GetSpeed() * Time.deltaTime;
        m_agent.transform.Rotate(Vector3.up * 720 * Time.deltaTime);
    }

    public override void ExitState()
    {
        m_agent.SetSpeed(m_default_speed);
    }

    public void OnHit(in GameObject hit)
    {
        Rigidbody rb = hit.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            Vector3 force = hit.transform.position - m_agent.transform.position;
            force = force.normalized * m_force;
            rb.AddForce(force, ForceMode.Impulse);

            Player player = hit.GetComponentInChildren<Player>();
            if (player != null)
                player.script_gm.PlayerTakenDamage(m_agent.GetDamage());
        }
    }
}
