using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiTesting : MonoBehaviour
{
    public GameManager script_gamemanager;

    public Slider hp_bar, energy_bar;
    public Text wave_count, wave_enemiesleft;
    public Text energy_value, max_energy_value;

    public Slider skill_1, skill_2, skill_3;

    // resources
    public Text res_cost_text;


    // Start is called before the first frame update
    void Start()
    {
        script_gamemanager = GetComponent<GameManager>();

        hp_bar.maxValue = script_gamemanager.player_hp;
        energy_bar.maxValue = script_gamemanager.player_energy;

        skill_1.maxValue = script_gamemanager.skill_1;
        skill_2.maxValue = script_gamemanager.skill_2;
        skill_3.maxValue = script_gamemanager.skill_3;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }
        
    public void UpdateUI()
    {
        hp_bar.value = script_gamemanager.player_hp_current;
        energy_bar.value = script_gamemanager.player_energy_current;

        wave_count.GetComponent<Text>().text = script_gamemanager.wave_no.ToString();
        energy_value.GetComponent<Text>().text = script_gamemanager.player_energy_current.ToString();

        max_energy_value.GetComponent<Text>().text = "/ " + script_gamemanager.player_energy.ToString();

        res_cost_text.GetComponent<Text>().text = script_gamemanager.currency.ToString();

        wave_enemiesleft.GetComponent<Text>().text = script_gamemanager.num_of_enemies.ToString();

        skill_1.value = script_gamemanager.skill_timer_1;
        skill_2.value = script_gamemanager.skill_timer_2;
        skill_3.value = script_gamemanager.skill_timer_3;
    }
}
