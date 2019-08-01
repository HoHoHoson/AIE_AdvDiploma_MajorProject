using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private Dictionary<string, State> m_states = new Dictionary<string, State>();
    private State m_current_state;
    private State m_previous_state;

    public ref State GetCurrentState() { return ref m_current_state; }
    public ref State GetPreviousState() { return ref m_previous_state; }

    public void InitiateStateMachine(in Agent agent, string start_state_key)
    {
        State start_state;
        if (m_states.TryGetValue(start_state_key, out start_state))
            ChangeState(agent, start_state);
        else
            Debug.Log("ERROR: Can't initiate StateMachine, state not found.");
    }

    public void UpdateState(in Agent agent)
    {
        m_current_state.UpdateState(agent);

        CanTransition(agent);
    }

    public void AddState(State new_state)
    {
        if (m_states.ContainsKey(new_state.GetIndex()) == false)
            m_states.Add(new_state.GetIndex(), new_state);
        else
            Debug.Log("ERROR: StateMachine already contains this state.");
    }

    private void ChangeState(in Agent agent, in State state)
    {
        if (m_current_state != null)
            m_current_state.OnExit(agent);

        m_previous_state = m_current_state;
        m_current_state = state;

        m_current_state.OnInitialise(agent);
    }

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
