using System;

/// <summary>
/// Class to be used in behavior states on order to setup transitions to different states when a condition is met.
/// </summary>
public class Transition
{
    private string              m_state_index;
    private Func<Agent, bool>   m_condition;

    public string GetStateIndex() { return m_state_index; }

    public Transition(string index, Func<Agent, bool> condition)
    {
        m_state_index = index;
        m_condition = condition;
    }

    /// <summary>
    /// Checks to see if this transition's condition was met.
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    /// <returns>True/False</returns>
    public bool ConditionMet(in Agent agent)
    {
        return m_condition(agent);
    }
}
