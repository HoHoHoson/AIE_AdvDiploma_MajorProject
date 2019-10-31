using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mines : MonoBehaviour
{
    int mine_hp = 1000;
    public bool is_active = false;

    public int mine_hp_recover = 50;
    public int mine_max_hp = 100;

    GameManager gamemanager;

    void Start()
    {
        gamemanager = FindObjectOfType<GameManager>();
        if(is_active == true)
        {
            mine_hp = mine_max_hp;
        }
    }

    void Update()
    {
        if (mine_hp <= 0 && is_active == true)
        {
            DeactivateMine();
        }

        if(is_active == false)
        {
            mine_hp = 0;
        }
    }

    public void Activate()
    {
        is_active = true;
    }

    public void MinesTakeDamage(int damage)
    {
        if (mine_hp > 0)
        {
            mine_hp -= damage;
        }
    }

    public void AddMineHP()
    {
        mine_hp += mine_hp_recover;
    }

    public void DeactivateMine()
    {
        is_active = false;
    }

    public bool GetActive()
    {
        return is_active;
    }

    public int GetCurrentHp()
    {
        return mine_hp;
    }
}
