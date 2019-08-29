using System.Collections.Generic;

/// <summary>
/// An abstract class that contains generic functions fo use in AI behavior states.
/// </summary>
public class State
{
    protected string m_index;
    protected StateMachine m_state_machine;
    protected List<Transition> m_state_transitions = new List<Transition>();

    public string GetIndex() { return m_index; }
    public List<Transition> GetTransitions() { return m_state_transitions; }

    protected State(in StateMachine state_machine) { m_index = "NULL"; }

    /// <summary>
    /// Logic runs when this state gets loaded.
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    public virtual void OnInitialise(in Agent agent)
    {
        foreach (Transition t in m_state_transitions)
            t.InitiateConditions();
    }

    /// <summary>
    /// Update logic of this script to be called preferably each frame.
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    public virtual void UpdateState(in Agent agent) { }

    /// <summary>
    /// Logic runs when this state gets unloaded.
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    public virtual void OnExit(in Agent agent) { }

    /// <summary>
    /// Add a new transition condition to be checked for.
    /// <para>All transitio conditions will be checked in the StateMachine's UpdateState().</para>
    /// </summary>
    /// <param name="new_transition">Dictionary index of the behavior state that will be transitioned to if triggered.</param>
    public void AddTransition(Transition new_transition)
    {
        m_state_transitions.Add(new_transition);
    }

    public void TransitionCheck(in Agent agent)
    {
        foreach (Transition t in m_state_transitions)
            if (t.CheckConditions() == true)
            {
                m_state_machine.StateTransition(agent, t);
                break;
            }
    }
}
