using System;
using System.Collections.Generic;

/// <summary>
/// Class to be used in behavior states on order to setup transitions to different states when a condition is met.
/// </summary>
public class Transition
{
    private string          m_state_index;
    private List<Condition> m_conditions;

    public string GetStateIndex() { return m_state_index; }

    public Transition(string index, Condition[] conditions)
    {
        m_state_index = index;
        m_conditions = new List<Condition>(conditions);
    }

    /// <summary>
    /// Checks to see if this transition's condition was met.
    /// </summary>
    /// <param name="agent">Reference the Agent that owns the StateMachine in order to make changes to it.</param>
    /// <returns>True/False</returns>
    public bool CheckConditions()
    {
        foreach (Condition c in m_conditions)
            if (c.Check())
                return true;

        return false;
    }
}
