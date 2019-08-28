using System;
using UnityEngine;

public class BoolCondition : Condition
{
    public delegate bool BoolFunction();

    private GameObject      m_target        = null;
    private BoolFunction    m_bool_function = null;

    public BoolCondition(in GameObject target, in BoolFunction bool_func = null)
    {
        m_target = target;
        m_bool_function = bool_func;
    }

    public override bool Check()
    {
        if (m_bool_function == null)
        {
            Mines target_mine;
            FPSControl target_fps;

            if ((target_mine = m_target.GetComponentInChildren<Mines>()) != null)
            {
                m_bool_function = new BoolFunction(target_mine.GetActive);
            }
            else if ((target_fps = m_target.GetComponentInChildren<FPSControl>()) != null)
            {
                m_bool_function = new BoolFunction(target_fps.GetPlayerHP);
            }
            else
            {
                Debug.Log("ERROR: Can't target GameObject");
                return false;
            }
        }

        if (m_bool_function() == true)
        {
            m_bool_function = null;
            return true;
        }
        else
            return false;
    }
}
