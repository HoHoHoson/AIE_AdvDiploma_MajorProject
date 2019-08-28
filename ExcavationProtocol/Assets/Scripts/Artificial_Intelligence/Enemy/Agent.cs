using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// Base class for all AI. 
/// <para>Should be derived from to create custom AI.</para>
/// </summary>
public class Agent : MonoBehaviour
{
    public enum EnemyType
    {
        BASIC = 0,
        EXPLOSIVE,
        BOSS
    }

    [SerializeField] private int m_health = 3;
    [SerializeField] private int m_damage = 5;
    [SerializeField] private int m_speed  = 500;

    protected int           m_current_health;
    protected int           m_current_damage;
    protected int           m_current_speed;

    protected EnemyType     m_type;
    protected Rigidbody     m_rigidbody;
    protected StateMachine  m_state_machine;
    protected Blackboard    m_blackboard;
    protected GameObject    m_target;

    public void SetBlackboard(in Blackboard blackboard) { m_blackboard = blackboard; }

    public float GetSpeed() { return m_current_speed; }
    public EnemyType GetEnemyType() { return m_type; }
    public ref Rigidbody GetRB() { return ref m_rigidbody; }
    public ref GameObject GetTarget() { return ref m_target; }

    public void SetTarget(in GameObject value) { m_target = value; }

    /// <summary>
    /// Updates the Agent by running the currently loaded behavior state.
    /// <para>Should be called every frame.</para>
    /// </summary>
    public virtual void UpdateAgent()
    {
        m_state_machine.UpdateState(this);
    }

    public virtual void InstantiateStats()
    {
        m_current_health    = m_health;
        m_current_damage    = m_damage;
        m_current_speed     = m_speed;
    }

    public void TakeDamage(int dmg) { m_current_health -= dmg; }

    public bool IsDead() { return m_current_health <= 0; }
}
