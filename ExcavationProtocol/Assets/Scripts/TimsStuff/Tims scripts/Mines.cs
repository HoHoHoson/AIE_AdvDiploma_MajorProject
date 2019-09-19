using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mines : MonoBehaviour
{
    int mine_hp = 100;
    bool is_active = false;

    public int mine_hp_recover = 50;
    public int mine_max_hp = 100;

    GameManager gamemanager;

    void Start()
    {
        gamemanager = FindObjectOfType<GameManager>();
        mine_hp = mine_max_hp;
    }

    void Update()
    {
        if (mine_hp <= 0 && is_active == true)
        {
            DeactivateMine(ref gamemanager.active_mines);
        }
    }

    public void Activate(ref int active, GameObject[] list)
    {
        is_active = true;
        mine_hp = mine_hp_recover;
        if (active < list.Length)
        {
            active++;
        }
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

    public void DeactivateMine(ref int active)
    {
        is_active = false;
        if (active > 0)
        {
            active--;
        }
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
