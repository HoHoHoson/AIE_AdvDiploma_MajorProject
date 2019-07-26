using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    private AIState m_current_state;
    private AIState m_previous_state;

    public void UpdateAgent(in AIAgent agent) { }
    public void ChangeState(ref AIAgent agent, AIState new_state) { }

    public ref AIState GetCurrentState() { return ref m_current_state; }
    public ref AIState GetPreviousState() { return ref m_previous_state; }
}
