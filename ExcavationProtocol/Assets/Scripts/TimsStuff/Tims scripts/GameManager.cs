using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // scripts
    public UiTesting script_UI; // Ui
    public FPSControl script_fps; // fps controller script

    // Main Game Variables
    [Header("GameLoop Variables")]
    public int active_mines = 1;
    
    public bool player_take_dmg = false, player_restore_hp = false;


    // Player Values
    [Header("Player Values")]
    // Health
    public int player_hp = 100;
    // Energy
    public int player_energy = 150;

    [HideInInspector]
    public int player_hp_current, player_energy_current;

    private int energy_gain_temp;

    
    public float fire_rate = 0.2f;
    private float fire_rate_timer;
    private bool is_shooting;

    // Wave Variables
    [Header("Wave Values")]

    public int wave_no;
    public int current_wave;

    [Tooltip("Max amount of enemies that need to be spawned.")]
    public int num_of_enemies;


    // Currency Values
    [Header("Currency Values")]
    public int currency = 20;
    public int wave_reward = 5;
    public int cost_HP, cost_ammo;
    [HideInInspector]
    public int cost_mine;

    // Player Ability Values
    [Header("Player Skill Values")]
    public float skill_1 = 20;
    public float skill_2 = 10;
    public float skill_3 = 50;

    [Tooltip("percent of damage reduced by skill 3 ( half = 2 )")]
    public float skill_3_dmg_reduction = 2;

    [HideInInspector]
    public float skill_timer_1, skill_timer_2, skill_timer_3;

    private bool active_1;
    private bool active_2;
    private bool active_3, is_used;

    // Start is called before the first frame update
    void Start()
    {
        player_hp_current = player_hp;
        player_energy_current = player_energy;

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

        fire_rate_timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CompleteAction1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CompleteAction2();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CompleteAction3();
        }

        if (player_take_dmg == true && player_hp_current > 0)
        {
            player_hp_current -= 20;
            player_take_dmg = false;
        }

        if (player_restore_hp == true && player_hp_current < player_hp && currency >= cost_HP)
        {
            player_hp_current += 20;
            currency -= cost_HP;
            player_restore_hp = false;
        }

        if (Input.GetMouseButton(0) && player_energy_current > 0)
        {
            if (fire_rate_timer > fire_rate)
            {
                Fire();
                fire_rate_timer = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && player_energy_current < player_energy && currency >= cost_HP)
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
        is_used = true;
        active_3 = true;
    }

    public void Fire()
    {
        player_energy_current--;
        script_fps.GunFire();
    }

    public void Interaction(GameObject interactable)
    {

    }

    public int PlayerTakenDamage(float damage)
    {
        if(is_used)
        {
            player_hp_current -= Mathf.RoundToInt(damage / skill_3_dmg_reduction);
            return player_hp_current;
        }
        else
        {
            player_hp_current -= (int)damage;
            return player_hp_current;
        }
    }
}
