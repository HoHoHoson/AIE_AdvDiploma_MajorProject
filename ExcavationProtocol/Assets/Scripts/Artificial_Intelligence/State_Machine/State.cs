using System.Collections.Generic;

public class State
{
    protected string m_index;
    protected List<Transition> m_state_transitions = new List<Transition>();

    public string GetIndex() { return m_index; }
    public List<Transition> GetTransitions() { return m_state_transitions; }

    protected State() { m_index = "NULL"; }

    public virtual void OnInitialise(in Agent agent) { }

    public virtual void UpdateState(in Agent agent) { }

    public virtual void OnExit(in Agent agent) { }

    public void AddTransition(Transition new_transition)
    {
        m_state_transitions.Add(new_transition);
    }
}
