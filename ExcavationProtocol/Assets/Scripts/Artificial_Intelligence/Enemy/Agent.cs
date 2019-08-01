﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    protected enum EnemyType
    {
        BASIC = 0,
        EXPLOSIVE,
        BOSS
    }

    protected int           m_health;
    protected int           m_damage;
    protected float         m_speed;
    protected EnemyType     m_type;
    protected Rigidbody     m_rigidbody;
    protected StateMachine  m_state_machine;
    protected GameObject    m_target;

    public float GetSpeed() { return m_speed; }
    public ref Rigidbody GetRB() { return ref m_rigidbody; }
    public ref GameObject GetTarget() { return ref m_target; }

    public virtual void UpdateAgent()
    {
        m_state_machine.UpdateState(this);
    }

    public virtual void AttackAgent() { }
}
