
/// <summary>
/// Pure abstract class for creating custom conditions for state transitions.
/// <para>Only use for deriving your own conditions.</para>
/// </summary>
public class Condition
{
    public virtual void InitiateCondition() { }
    
    public virtual bool Check() { return false; }
}
