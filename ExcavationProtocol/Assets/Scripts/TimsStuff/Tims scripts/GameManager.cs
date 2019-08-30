﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // scripts
    #region Scripts
    public UiTesting script_UI; // Ui
    public FPSControl script_fps; // fps controller script
    public Blackboard script_bb; // blackboard script
    #endregion

    // Main Game Variables
    #region Loop
    [Header("GameLoop Variables")]
    public int active_mines = 1;
    public int mine_rep_cost = 1;
    public int mine_cost = 1;

    public GameObject[] mines_list;

    public bool player_take_dmg = false, player_restore_hp = false;

    public Transform camera_transform;
    #endregion

    #region Animator

    private Animator animator;

    #endregion

    // Player Values
    #region Player
    [Header("Player Values")]

    public GameObject player_gameobject;
    // Health
    public int player_hp = 100;
    // Energy
    public int player_energy = 150;

    [HideInInspector]
    public int player_hp_current, player_energy_current;

    private int energy_gain_temp;
    #endregion

    // Wave Variables
    #region Wave
    [Header("Wave Values")]

    public int wave_no;

    public float time_to_next_wave = 20;
    private float wave_timer;

    [Tooltip("Max amount of enemies that need to be spawned.")]
    public int num_of_enemies;
    #endregion

    // Currency Values
    #region Currency
    [Header("Currency Values")]
    public int currency = 20;
    public int wave_reward = 5;
    public int cost_HP, cost_ammo;
    #endregion

    // Player Ability Values
    #region PlayerAbilities
    [Header("Player Skill Values")]
    public float skill_1 = 20;
    public float skill_2 = 10;
    public float skill_3 = 50;
    public float skill_3_duration = 20;

    [Tooltip("percent of damage reduced by skill 3 ( half = 2 )")]
    public float skill_3_dmg_reduction = 2;

    [HideInInspector]
    public float skill_timer_1, skill_timer_2, skill_timer_3;

    private bool active_1;
    private bool active_2;
    private bool active_3, is_used;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        animator = player_gameobject.GetComponent<Animator>();
        player_hp_current = player_hp;
        player_energy_current = player_energy;
        num_of_enemies = script_bb.m_enemyCount;
        player_hp_current = player_hp;

        skill_timer_1 = skill_1;
        skill_timer_2 = skill_2;
        skill_timer_3 = skill_3;
    }

    // Update is called once per frame
    void Update()
    {
        if (active_1)
        {
            skill_timer_1 += Time.deltaTime;
        }
        if (skill_timer_1 > skill_1)
        {
            skill_timer_1 = skill_1;
        }

        if (active_2)
        {
            skill_timer_2 += Time.deltaTime;
        }
        if (skill_timer_2 > skill_2)
        {
            skill_timer_2 = skill_2;
        }

        if (active_3)
        {
            skill_timer_3 += Time.deltaTime;
        }
        if (skill_timer_3 > skill_3)
        {
            skill_timer_3 = skill_3;
        }
        if (skill_timer_3 > skill_3_duration)
        {
            is_used = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CompleteAction1();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CompleteAction2();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            CompleteAction3();
        }

        if (Input.GetMouseButtonDown(0) && player_energy_current > 0)
        {
            script_fps.GunFire(ref player_energy_current);
        }
        else if (Input.GetMouseButtonDown(0) && player_energy_current <= 0)
        {
            StartCoroutine(OutOfAmmo());
        }

        if (Input.GetKey(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(camera_transform.position, camera_transform.forward, out hit, 5.0f))
            {
                Interaction(hit.transform.gameObject);
            }
        }
        GameLoop();
        script_UI.UpdateUI();
    }

    public void CompleteAction1()
    {
        if (skill_timer_1 < skill_1)
            return;

        active_1 = false;
        skill_timer_1 = 0;
        script_fps.SkillActive1();
        active_1 = true;
    }

    public void CompleteAction2()
    {
        if (skill_timer_2 < skill_2)
            return;

        active_2 = false;
        skill_timer_2 = 0;
        script_fps.SkillActive2();
        active_2 = true;
    }

    public void CompleteAction3()
    {
        if (skill_timer_3 < skill_3)
            return;

        active_3 = false;
        skill_timer_3 = 0;
        script_fps.SkillActive3();
        active_3 = true;
    }

    public void Interaction(GameObject interactable)
    {
        if (interactable.tag == "EnergyTerminal" && player_energy_current < player_energy && currency >= cost_ammo)
        {
            if (player_energy_current > player_energy - 10 && player_energy_current != player_energy)
            {
                energy_gain_temp = player_energy - player_energy_current;
                player_energy_current += energy_gain_temp;
                currency -= cost_ammo;
            }
            else
            {
                player_energy_current += 10;
                currency -= cost_ammo;
            }
        }
        else if (interactable.tag == "HealthTerminal" && player_hp_current < player_hp && currency >= cost_HP)
        {
            if (player_hp_current < player_hp)
            {
                player_hp_current += 20;
                currency -= cost_HP;
            }
        }
        else if (interactable.tag == "Mine")
        {
            if(interactable.GetComponent<Mines>().GetActive() == true && currency >= mine_cost)
            {
                interactable.GetComponent<Mines>().Activate(ref active_mines, mines_list);
                currency -= mine_cost;
            }
            else if (interactable.GetComponent<Mines>().GetCurrentHp() < interactable.GetComponent<Mines>().mine_max_hp && currency > mine_rep_cost)
            {
                interactable.GetComponent<Mines>().AddMineHP();
                currency -= mine_rep_cost;
            }
        }
    }

    public int PlayerTakenDamage(float damage)
    {
        player_hp_current -= (int)damage;
        return player_hp_current;   
    }

    private IEnumerator OutOfAmmo()
    {
        animator.SetBool("OutOfAmmo", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("OutOfAmmo", false);
    }

    public void GameLoop()
    {
        num_of_enemies = script_bb.m_enemyCount;
        if(script_bb.GetWaveOngoing() == false)
        {
            wave_timer += Time.deltaTime;
            if(wave_timer >= time_to_next_wave)
            {
                wave_no++;

                script_bb.BeginWave();
            }
        }
        if(script_bb.GetWaveOngoing() == true && wave_timer > 0)
        {
            wave_timer = 0;
        }
    }

    public void AddCurrency()
    {
        currency += wave_reward * (active_mines + 1);
    }
}
