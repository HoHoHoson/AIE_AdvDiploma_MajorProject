using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finite State Machine that manages the updating and transitioning of an AI Agent's behavior states.
/// </summary>
public class StateMachine
{
    private Dictionary<string, State> m_states = new Dictionary<string, State>();
    private State m_current_state;
    private State m_previous_state;

    public ref State GetCurrentState() { return ref m_current_state; }
    public ref State GetPreviousState() { return ref m_previous_state; }

    /// <summary>
    /// Assigns the StateMachine a behavior state to start from, said state must have already been stored with AddState().
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    /// <param name="start_state_key">Key for the state to be initialised from m_states.</param>
    public void InitiateStateMachine(in Agent agent, string start_state_key)
    {
        State start_state;
        if (m_states.TryGetValue(start_state_key, out start_state))
            ChangeState(agent, start_state);
        else
            Debug.Log("ERROR: Can't initiate StateMachine, state not found.");
    }

    /// <summary>
    /// Runs the update logic and the transition checks of the currently loaded state.
    /// <para>Should only be called by it's Agent's UpdateAgent().</para>
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    public void UpdateState(in Agent agent)
    {
        if (m_current_state == null)
        {
            Debug.Log("ERROR: StateMachine not initialised.");
            return;
        }

        m_current_state.UpdateState(agent);
        CanTransition(agent);
    }

    /// <summary>
    /// Adds a behavior state to the StateMachine for use.
    /// </summary>
    /// <param name="new_state"></param>
    public void AddState(State new_state)
    {
        if (m_states.ContainsKey(new_state.GetIndex()) == false)
            m_states.Add(new_state.GetIndex(), new_state);
        else
            Debug.Log("ERROR: StateMachine already contains this state.");
    }

    /// <summary>
    /// Changes the current state to another after recording it in m_previous_state.
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    /// <param name="state">The state to be transitioned to, it must be an existing state in m_states.</param>
    private void ChangeState(in Agent agent, in State state)
    {
        if (m_current_state != null)
            m_current_state.OnExit(agent);

        m_previous_state = m_current_state;
        m_current_state = state;

        m_current_state.OnInitialise(agent);
    }

    /// <summary>
    /// Checks all the transition conditions of the currently loaded state. Transitions to that state if the condition is triggered.
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    /// <returns>True if a state change was made, False otherwise.</returns>
    private bool CanTransition(in Agent agent)
    {
        foreach (Transition t in m_current_state.GetTransitions())
            if (t.ConditionMet(agent))
            {
                State transition_state;

                if (m_states.TryGetValue(t.GetStateIndex(), out transition_state))
                {
                    ChangeState(agent, transition_state);
                    return true;
                }
                else
                {
                    Debug.Log("ERROR: Transition condition was met but the state wasn't found.");
                    return false;
                }
            }

        return false;
    }
}
