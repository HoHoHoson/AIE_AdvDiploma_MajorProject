using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Ui : MonoBehaviour
{
    // Variables

    #region Scripts

    public GameManager script_gamemanager;
    public Player script_player;
	public Blackboard script_blackboard;

    #endregion

    #region PlayerStats

    public GameObject player, hp_bar_gameObject;
    public Slider hp_bar, energy_bar;
    public Text wave_count, wave_enemiesleft;
    public Text energy_value, max_energy_value;

    public Slider skill_1, skill_2, skill_3;

    #endregion

    #region HUDVar
    protected float half_hp, low_hp;
    public Text Notifications;

    // resources
    public Text res_cost_text;

	#endregion
	
	#region EndGameStats
	int endWaveNo, timeMin, timeSec, endResources, endEnemyKillCount;
	float timeMs;
	public Text endWaveNoText, timeMinText, timeSecText, endResourcesText, endEnemyKillCountText;
	#endregion

	#region Drill
	public GameObject drill;
    #endregion

    // Functions

    #region StartUpdate

    void Start()
    {
        script_gamemanager = GetComponent<GameManager>();
		script_blackboard = GetComponent<Blackboard>();

		hp_bar.maxValue = script_player.player_hp;
        energy_bar.maxValue = script_player.player_energy;

        skill_1.maxValue = script_player.skill_1;
        skill_2.maxValue = script_player.turret_cost;
        skill_3.maxValue = script_player.skill_3;

        half_hp = hp_bar.maxValue / 2;
        low_hp = hp_bar.maxValue / 4;

		if (PlayerPrefs.GetInt("HPValBool") == 1)
		{
			hp_bar_gameObject.SetActive(true);
		}
		else
		{
			hp_bar_gameObject.SetActive(false);
		}
        
        drill.GetComponentInChildren<Slider>().maxValue = drill.GetComponent<Mines>().mine_max_hp;
        
    }
        
    public void UpdateUI()
    {
        hp_bar.value = script_player.GetPlayerHp();
        energy_bar.value = script_player.GetPlayerEnergy();
        wave_count.GetComponent<Text>().text = script_gamemanager.wave_no.ToString();
        energy_value.GetComponent<Text>().text = script_player.GetPlayerEnergy().ToString();
        max_energy_value.GetComponent<Text>().text = "/ " + script_player.player_energy.ToString();
        res_cost_text.GetComponent<Text>().text = script_gamemanager.GetCurrency().ToString();
        wave_enemiesleft.GetComponent<Text>().text = script_gamemanager.num_of_enemies.ToString();

		endWaveNo = script_gamemanager.wave_no;
		endResources = script_gamemanager.GetCurrency();
		endEnemyKillCount = script_blackboard.GetEnemyDeathCount();

		endWaveNoText.GetComponent<Text>().text = endWaveNo.ToString();
		endResourcesText.GetComponent<Text>().text = endResources.ToString();
		endEnemyKillCountText.GetComponent<Text>().text = endEnemyKillCount.ToString();

		if (timeMin <= 9)
		{
			timeMinText.GetComponent<Text>().text = "0" + timeMin + ":";
		}
		else
		{
			timeMinText.GetComponent<Text>().text = "" + timeMin + ":";
		}

		if(timeSec <= 9)
		{
			timeSecText.GetComponent<Text>().text = "0" + timeSec;
		}
		else
		{
			timeSecText.GetComponent<Text>().text = "" + timeSec;
		}

        skill_1.value = script_player.skill_timer_1;
        skill_2.value = script_gamemanager.GetCurrency();
        skill_3.value = script_player.skill_timer_3;
        HPColourChange(script_player.GetPlayerHp());
		UpdateTimer(ref timeMin, ref timeSec, ref timeMs);
        UpdateMineUi();
        UpdateScrollText("hello", false);
    }

	#endregion

	#region Timer

	public void UpdateTimer(ref int min, ref int sec, ref float ms)
	{
		if (script_gamemanager.dead_player == false)
		{
			ms += Time.deltaTime * 10;

			if (ms >= 9)
			{
				ms = 0;
				sec++;
			}

			if (sec >= 59)
			{
				sec = 0;
				min++;
			}
		}
	}

	#endregion

	#region HUD

	public void HPColourChange(int hp)
    {
        if (hp <= low_hp)
        {
            hp_bar.fillRect.GetComponent<Image>().color = Color.red;
        }
        else if (hp <= half_hp)
        {
            hp_bar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.yellow, hp / half_hp);
        }
        else
        {
            hp_bar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.cyan, (hp - half_hp) / half_hp);
        }
    }

    #endregion

    #region MinesUI

    public void UpdateMineUi()
    {
		drill.transform.GetChild(1).LookAt(player.transform, transform.up);
		drill.GetComponentInChildren<Slider>().value = drill.GetComponent<Mines>().GetCurrentHp();
    }

    #endregion

    #region ScrollText

    /// <summary>
    /// for scrolling text
    /// </summary>
    /// <param name="text"> Enter Text </param>
    /// <param name="scroll"> scroll (True/False) </param>
    public void UpdateScrollText(string text, bool scroll)
    {
        Notifications.text = text;

        if (scroll)
        {

        }

        
    }
    #endregion
}
