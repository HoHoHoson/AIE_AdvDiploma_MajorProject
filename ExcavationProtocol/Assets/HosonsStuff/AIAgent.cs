using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIAgent : MonoBehaviour
{
    enum EnemyType
    {
        BASIC = 0,
        EXPLOSIVE,
        BOSS
    }

    private int         m_health;
    private int         m_damage;
    private float       m_speed;
    private EnemyType   m_type;
    private GameObject  m_target;

    public virtual void UpdateAgent() { }
    public virtual void AttackAgent() { }
}
