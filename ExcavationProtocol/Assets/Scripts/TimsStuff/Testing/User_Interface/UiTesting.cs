using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiTesting : MonoBehaviour
{
    int player_hp = 100, player_skill = 20;

    [Range(0, 100)]
    public int current_player_hp;

    [Range(0,20)]
    public int skill_cooldown;

    public Slider hp_bar, energy_bar;

    public bool player_take_dmg = false, player_use_ability = false, player_restore_hp = false;


    // Start is called before the first frame update
    void Start()
    {
        current_player_hp = player_hp;
        skill_cooldown = player_skill;
    }

    // Update is called once per frame
    void Update()
    {
        hp_bar.value = current_player_hp;
        energy_bar.value = skill_cooldown;


        if (skill_cooldown < player_skill)
        {
            skill_cooldown++;
        }

        if (player_take_dmg == true)
        {
            current_player_hp -= 20;
            player_take_dmg = false;
        }
        if (player_use_ability == true && skill_cooldown > 19)
        {
            skill_cooldown -= 20;
            player_use_ability = false;
        }
        if (player_restore_hp == true)
        {
            current_player_hp += 20;
            player_restore_hp = false;
        }
    }
}
