using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiTesting : MonoBehaviour
{
    public GameManager script_gamemanager;

    public Slider hp_bar, energy_bar;
    public Text wave_count, wave_enemiesleft;
    public Text EnergyValue;

    public Slider skill_1, skill_2, skill_3;

    // resources
    public Text res_cost_text;


    // Start is called before the first frame update
    void Start()
    {
        script_gamemanager = GetComponent<GameManager>();

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

        wave_count.GetComponent<Text>().text = script_gamemanager.current_wave.ToString();
        EnergyValue.GetComponent<Text>().text = script_gamemanager.player_energy_current.ToString();

        res_cost_text.GetComponent<Text>().text = script_gamemanager.currency.ToString();

        skill_1.value = script_gamemanager.skill_timer_1;
        skill_2.value = script_gamemanager.skill_timer_2;
        skill_3.value = script_gamemanager.skill_timer_3;
    }
}
