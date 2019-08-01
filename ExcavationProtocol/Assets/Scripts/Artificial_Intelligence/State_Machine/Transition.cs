using System;

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

    public bool ConditionMet(in Agent ai)
    {
        return m_condition(ai);
    }
}
