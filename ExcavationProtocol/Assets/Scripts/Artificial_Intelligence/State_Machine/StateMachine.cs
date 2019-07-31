
public class StateMachine
{
    private State m_current_state;
    private State m_previous_state;

    public void UpdateState(in Agent agent)
    {
        m_current_state.UpdateState(agent);
    }

    public void ChangeState(in Agent agent, State new_state)
    {
        if (m_current_state != null)
            m_current_state.OnExit(agent);

        m_previous_state = m_current_state;
        m_current_state = new_state;

        m_current_state.OnInitialise(agent);
    }

    public ref State GetCurrentState() { return ref m_current_state; }
    public ref State GetPreviousState() { return ref m_previous_state; }
}
