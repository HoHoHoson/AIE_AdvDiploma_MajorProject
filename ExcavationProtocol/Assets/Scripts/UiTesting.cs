using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTesting : MonoBehaviour
{
    int playerHP = 100, PlayerSkill = 20;

    [Range(0, 100)]
    public int currentPlayerHP;

    [Range(0,20)]
    public int SkillCooldown;

    public Slider HPBar, EnergyBar;

    public bool PlayerTakeDMG = false, PlayerUsesAbility = false, playerRestoreHp = false;


    // Start is called before the first frame update
    void Start()
    {
        currentPlayerHP = playerHP;
        SkillCooldown = PlayerSkill;
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.value = currentPlayerHP;
        EnergyBar.value = SkillCooldown;


        if (SkillCooldown < PlayerSkill)
        {
            SkillCooldown++;
        }

        if (PlayerTakeDMG == true)
        {
            currentPlayerHP -= 20;
            PlayerTakeDMG = false;
        }
        if (PlayerUsesAbility == true && SkillCooldown > 19)
        {
            SkillCooldown -= 20;
            PlayerUsesAbility = false;
        }
        if (playerRestoreHp == true)
        {
            currentPlayerHP += 20;
            playerRestoreHp = false;
        }
    }
}
