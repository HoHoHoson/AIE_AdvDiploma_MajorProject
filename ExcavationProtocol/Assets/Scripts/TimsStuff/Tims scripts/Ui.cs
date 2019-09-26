using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Ui : MonoBehaviour
{
    public GameManager script_gamemanager;

    public Slider hp_bar, energy_bar;
    public Text wave_count, wave_enemiesleft;
    public Text energy_value, max_energy_value;

    public Slider skill_1, skill_2, skill_3;

    public Image hud;

    protected float half_hp, low_hp;

    // resources
    public Text res_cost_text;

    public Text Notifications;
    //float x_axis;

    public GameObject[] drills;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        script_gamemanager = GetComponent<GameManager>();

        hp_bar.maxValue = script_gamemanager.player_hp;
        energy_bar.maxValue = script_gamemanager.player_energy;

        skill_1.maxValue = script_gamemanager.skill_1;
        skill_2.maxValue = script_gamemanager.skill_2;
        skill_3.maxValue = script_gamemanager.skill_3;

        half_hp = hp_bar.maxValue / 2;
        low_hp = hp_bar.maxValue / 4;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }
        
    public void UpdateUI()
    {
        hp_bar.value = script_gamemanager.GetPlayerHp();
        energy_bar.value = script_gamemanager.GetPlayerEnergy();

        wave_count.GetComponent<Text>().text = script_gamemanager.wave_no.ToString();
        energy_value.GetComponent<Text>().text = script_gamemanager.GetPlayerEnergy().ToString();

        max_energy_value.GetComponent<Text>().text = "/ " + script_gamemanager.player_energy.ToString();

        res_cost_text.GetComponent<Text>().text = script_gamemanager.currency.ToString();

        wave_enemiesleft.GetComponent<Text>().text = script_gamemanager.num_of_enemies.ToString();

        skill_1.value = script_gamemanager.skill_timer_1;
        skill_2.value = script_gamemanager.skill_timer_2;
        skill_3.value = script_gamemanager.skill_timer_3;
        HPColourChange(script_gamemanager.GetPlayerHp());
        UpdateMineUi();
        UpdateScrollText("This it a pop up", false);
    }

    public void HPColourChange(int hp)
    {
        if (hp <= low_hp)
        {
            hp_bar.fillRect.GetComponent<Image>().color = Color.red;
            hud.color = Color.red;
        }
        else if (hp <= half_hp)
        {
            hp_bar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.yellow, hp / half_hp);
            hud.color = Color.Lerp(Color.red, Color.yellow, hp / half_hp);
        }
        else
        {
            hp_bar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.cyan, (hp - half_hp) / half_hp);
            hud.color = Color.Lerp(Color.yellow, Color.cyan, (hp - half_hp) / half_hp);
        }
    }

    public void UpdateMineUi()
    {
        foreach (var mine in drills)
        {
            mine.transform.GetChild(0).LookAt(player.transform, transform.up);
            mine.GetComponentInChildren<Slider>().value = mine.GetComponent<Mines>().GetCurrentHp();
        }
    }

    /// <summary>
    /// for scrolling text
    /// </summary>
    /// <param name="text"> Enter Text </param>
    /// <param name="fade"> Fade (True/False) </param>
    /// <param name="scroll"> scroll (True/False) </param>
    public void UpdateScrollText(string text, bool scroll)
    {
        Notifications.text = text;

        if (scroll)
        {

        }

        
    }
}
