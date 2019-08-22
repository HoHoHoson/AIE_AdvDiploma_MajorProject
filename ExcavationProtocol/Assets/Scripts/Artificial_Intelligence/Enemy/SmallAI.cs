﻿using UnityEngine;

public class SmallAI : Agent
{
    [SerializeField]
    private Transform   m_childedModel;
    [SerializeField]
    private float       m_leapAngle = 45, m_leapForce = 2;

    void Start()
    {
        m_health    = 3;
        m_damage    = 5;
        m_speed     = 300;
        m_type      = EnemyType.BASIC;
        m_rigidbody = GetComponent<Rigidbody>();

        m_state_machine = new StateMachine();

        // Got to make a proper target selecting function
        m_target = GameObject.Find("FPS_Controller");

        State state = new LeapAtState(m_leapAngle, m_leapForce);
        state.AddTransition(new Transition("CHASETARGET",
            new Condition[] { new DistanceCondition(transform, m_target.transform, 3, DistanceCondition.Comparator.GREATER) }));
        m_state_machine.AddState(state);

        state = new ChaseTargetState();
        state.AddTransition(new Transition("LEAPAT",
            new Condition[] { new DistanceCondition(transform, m_target.transform, 3, DistanceCondition.Comparator.LESS) }));
        m_state_machine.AddState(state);

        m_state_machine.InitiateStateMachine(this, "CHASETARGET");
    }

    private void Update()
    {
        UpdateAgent();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_blackboard == null)
        {
            Debug.Log("ERROR: AI Blackboard has not been set.");
        }
        else if (m_state_machine.GetCurrentState().GetIndex() == "LEAPAT" && collision.collider.GetComponent<FPSControl>())
        {
            m_blackboard.m_gameManager.PlayerTakenDamage(m_damage);
        }
    }
}
