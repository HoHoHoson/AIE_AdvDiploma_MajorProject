using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiTesting : MonoBehaviour
{
    public SpawnTest script_spawntest;
    private int player_hp = 100;

    private int player_energy = 150;

    private int wave_current;

    [Range(0, 100)]
    public int current_player_hp;

    [Range(0, 150)]
    public int current_player_energy;


    public Slider hp_bar, energy_bar;

    public Text wave_count, wave_enemiesleft;
    public Text EnergyValue;

    int tempgain;

    public bool player_take_dmg = false, player_restore_hp = false;

    // skills
    public float player_skill_1 = 20;
    public float player_skill_2 = 10;
    public float player_skill_3 = 50;

    private float skill_cooldown_timer_1;
    private float skill_cooldown_timer_2;
    private float skill_cooldown_timer_3;

    public Slider skill_1, skill_2, skill_3;

    private bool active_1;
    private bool active_2;
    private bool active_3;


    // Start is called before the first frame update
    void Start()
    {
        current_player_hp = player_hp;
        current_player_energy = player_energy;

        skill_cooldown_timer_1 = player_skill_1;
        skill_cooldown_timer_2 = player_skill_2;
        skill_cooldown_timer_3 = player_skill_3;

        wave_current = script_spawntest.GetCurrentWave();
    }

    // Update is called once per frame
    void Update()
    {
        if(active_1)
        {
            skill_cooldown_timer_1 += Time.deltaTime;
        }
        if(skill_cooldown_timer_1 > player_skill_1)
        {
            skill_cooldown_timer_1 = player_skill_1;
        }

        if (active_2)
        {
            skill_cooldown_timer_2 += Time.deltaTime;
        }
        if (skill_cooldown_timer_2 > player_skill_2)
        {
            skill_cooldown_timer_2 = player_skill_2;
        }

        if (active_3)
        {
            skill_cooldown_timer_3 += Time.deltaTime;
        }
        if (skill_cooldown_timer_3 > player_skill_3)
        {
            skill_cooldown_timer_3 = player_skill_3;
        }

        hp_bar.value = current_player_hp;
        energy_bar.value = current_player_energy;

        wave_current = script_spawntest.GetCurrentWave();


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

        if (player_take_dmg == true && current_player_hp > 0)
        {
            current_player_hp -= 20;
            player_take_dmg = false;
        }

        if (player_restore_hp == true && current_player_hp < player_hp)
        {
            current_player_hp += 20;
            player_restore_hp = false;
        }

        if (Input.GetMouseButton(0) && current_player_energy > 0)
        {
            current_player_energy--;
        }

        if (Input.GetKeyDown(KeyCode.E) && current_player_energy < player_energy)
        {
            if (current_player_energy > player_energy - 10 && current_player_energy != player_energy)
            {
                tempgain = player_energy - current_player_energy;
                current_player_energy += tempgain;
            }
            else
            {
                current_player_energy += 10;
            }           
        }

        UpdateUI();
    }
        
    void UpdateUI()
    {
        wave_count.GetComponent<Text>().text = wave_current.ToString();
        EnergyValue.GetComponent<Text>().text = current_player_energy.ToString();

        skill_1.value = skill_cooldown_timer_1;
        skill_2.value = skill_cooldown_timer_2;
        skill_3.value = skill_cooldown_timer_3;
    }

    public void CompleteAction1()
    {
        if (skill_cooldown_timer_1 < player_skill_1)
            return;

        active_1 = false;
        skill_cooldown_timer_1 = 0;
        active_1 = true;
    }

    public void CompleteAction2()
    {
        if (skill_cooldown_timer_2 < player_skill_2)
            return;

        active_2 = false;
        skill_cooldown_timer_2 = 0;
        active_2 = true;
    }

    public void CompleteAction3()
    {
        if (skill_cooldown_timer_3 < player_skill_3)
            return;

        active_3 = false;
        skill_cooldown_timer_3 = 0;
        active_3 = true;
    }
}
