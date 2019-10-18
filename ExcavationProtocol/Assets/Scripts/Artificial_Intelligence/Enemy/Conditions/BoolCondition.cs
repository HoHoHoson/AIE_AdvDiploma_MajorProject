using UnityEngine;

public class BoolCondition : Condition
{
    private enum BoolMode
    {
        STATEMENT = 0,
        INVERTEDSTATEMENT,
        TARGET
    }

    public delegate bool BoolFunction();

    private BoolFunction    m_bool_function = null;
    private GameObject      m_target        = null;
    private BoolMode        m_bool_mode;

    public BoolCondition(in BoolFunction bool_func, bool is_inverted = false)
    {
        m_bool_function = bool_func;

        if (is_inverted)
            m_bool_mode = BoolMode.INVERTEDSTATEMENT;
        else
            m_bool_mode = BoolMode.STATEMENT;
    }

    public BoolCondition(in GameObject enemy)
    {
        m_target = enemy;
        m_bool_mode = BoolMode.TARGET;
    }

    public override bool Check()
    {
        switch (m_bool_mode)
        {
            case BoolMode.STATEMENT:
                return m_bool_function();

            case BoolMode.INVERTEDSTATEMENT:
                return !m_bool_function();

            case BoolMode.TARGET:
                if (BoolFunctionSet() && m_bool_function() == true)
                {
                    m_bool_function = null;
                    return true;
                }
                else
                    return false;

            default:
                break;
        }

        Debug.Log("ERROR: Skipped over BoolCondition switch statement.");
        return false;
    }

    private bool BoolFunctionSet()
    {
        if (m_bool_function != null)
            return true;

        Mines       target_mine;
        Player  target_fps;

        if ((target_mine = m_target.GetComponentInChildren<Mines>()) != null)
        {
            m_bool_function = target_mine.GetActive;
        }
        else if ((target_fps = m_target.GetComponentInChildren<Player>()) != null)
        {
            m_bool_function = target_fps.IsPlayerDead;
        }
        else
        {
            Debug.Log("ERROR: BoolCondition set GameObject is not a valid target.");
            return false;
        }

        return true;
    }
}
