using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSweepState : State
{
    private BossAI m_boss_agent;
    private Animator m_animator;

    public BossSweepState(in Agent agent, in Animator animator) : base(agent)
    {
        m_index = "BOSSSWEEP";

        m_boss_agent = m_agent as BossAI;
        m_animator = animator;
    }

    public override void InitialiseState()
    {
        base.InitialiseState();

        m_animator.ResetTrigger("Attack");
        m_animator.SetTrigger("Attack");

        GameObject target = m_agent.GetTarget();

        Mines hit_mine = target.GetComponentInChildren<Mines>();
        Player hit_fps = target.GetComponentInChildren<Player>();

        if (hit_mine != null)
        {
            hit_mine.MinesTakeDamage(m_agent.GetDamage());
        }
        else if (hit_fps != null)
        {
            hit_fps.PlayerTakenDamage(m_agent.GetDamage());
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        m_agent.GetRigidbody().velocity = Vector3.zero;
    }

    public bool AnimationEnded()
    {
        AnimatorStateInfo state_info = m_animator.GetCurrentAnimatorStateInfo(0);
        if (state_info.IsName("SweepAttack") || m_animator.IsInTransition(0))
            return false;

        return true;
    }
}
