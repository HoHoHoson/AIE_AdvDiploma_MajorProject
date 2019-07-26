using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
    public virtual void Initialise() { }
    public virtual void UpdateState() { }
    public virtual void Exit() { }
}
