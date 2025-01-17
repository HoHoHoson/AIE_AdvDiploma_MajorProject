﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
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
    [SerializeField] private int m_minSpeed  = 500;
    [SerializeField] private int m_maxSpeed = 500;

    [SerializeField] protected float m_rotationSpeed = 90f;

    [SerializeField, Range(0, 90)]
    private int m_maxSlopeAngle = 70;

    [Header("Leap Settings")]
    [SerializeField] protected float m_leapCooldown = 2;
    [SerializeField] protected float m_leapAngle = 45;
    [SerializeField] protected float m_leapForce = 10;

    protected int               m_current_health;
    protected int               m_current_damage;
    protected float             m_current_speed;

    protected EnemyType         m_type;
    protected Rigidbody         m_rigidbody;
    protected CapsuleCollider   m_collider;
    protected Animator          m_animator;
    protected StateMachine      m_state_machine;
    protected Blackboard        m_blackboard;
    protected GameObject        m_target;

    private bool                m_cliff_leap = false;

    public int GetSlopeAngle() { return m_maxSlopeAngle; }
    public int GetDamage() { return m_damage; }
    public float GetSpeed() { return m_current_speed; }
    public EnemyType GetEnemyType() { return m_type; }
    public ref Rigidbody GetRigidbody() { return ref m_rigidbody; }
    public ref CapsuleCollider GetCollider() { return ref m_collider; }
    public ref StateMachine GetStateMachine() { return ref m_state_machine; }
    public ref GameObject GetTarget() { return ref m_target; }
    protected bool GetCliffLeap() { return m_cliff_leap; }

    public void SetSpeed(float new_speed) { m_current_speed = new_speed; }
    public void SetTarget(in GameObject value) { m_target = value; }

    public virtual void InitialiseAgent(in Blackboard blackboard)
    {
        m_blackboard = blackboard;

        if (m_blackboard == null)
            Debug.Log("ERROR: BlackBoard is NULL.");
        else
            m_target = m_blackboard.m_gameManager.player_gameobject;

        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.useGravity = false;

        m_collider = GetComponent<CapsuleCollider>();
        m_animator = GetComponent<Animator>();

        m_state_machine = new StateMachine();
        InitialiseStateMachine();
    }

    /// <summary>
    /// Updates the Agent by running the currently loaded behavior state.
    /// <para>Should be called every frame.</para>
    /// </summary>
    public virtual void UpdateAgent()
    {
        if (m_rigidbody.isKinematic)
        {
            m_animator.SetFloat("SpeedMultiplier", 0);
        }
        else
        {
            m_animator.SetFloat("SpeedMultiplier", 1);
            m_state_machine.UpdateStates(this);
        }
    }

    public virtual void ResetStats()
    {
        m_current_health    = m_health;
        m_current_damage    = m_damage;
        m_current_speed     = Random.Range(m_minSpeed, m_maxSpeed);

        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.isKinematic = false;
    }

    protected virtual void InitialiseStateMachine() { Debug.Log("ERROR: No defined StateMachine behaviors for " + m_type + " AI."); }

    public virtual void TakeDamage(int dmg) { m_current_health -= dmg; }

    public virtual bool IsDead() { return m_current_health <= 0; }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_state_machine == null)
            return;

        State current = m_state_machine.GetCurrentState();

        if (current != null && (current.GetIndex() == "LEAPAT"))
        {
            (current as LeapAtState).OnHit(collision.gameObject);
            return;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_state_machine == null)
            return;

        State current = m_state_machine.GetCurrentState();

        if (current != null && (current.GetIndex() == "LEAPAT"))
        {
            (current as LeapAtState).OnStay(collision.gameObject);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CliffBounds")
            m_cliff_leap = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CliffBounds")
            m_cliff_leap = false;
    }
}
