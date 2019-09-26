using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // scripts
    #region Scripts
    public Ui script_UI; // Ui
    private Player script_fps; // fps controller script
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

    private Transform camera_transform;

    // Ui gameobjects to toggle them in pause and end state
    public GameObject pause_menu;
    public GameObject game_over;
    public GameObject game_play;
    
    public bool is_paused = false;
    public bool dead_player = false;
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

    
    protected int player_hp_current, player_energy_current;

    private int energy_gain_temp;

    // makes gun automatic
    public bool auto_gun;
    public float fire_rate = 0.25f;

    public float interaction_cooldown = 0.25f;
    private float interaction_timer;
    
    bool unlocked_mouse;
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
    public int cost_per_hp, cost_per_ammo;
    #endregion

    // Player Ability Values
    #region PlayerAbilities
    [Header("Player Skill Values")]
    public float skill_1 = 20;
    public float skill_2 = 10;
    public float skill_3 = 20;

    [HideInInspector]
    public float skill_timer_1, skill_timer_2, skill_timer_3;

    private bool active_1;
    private bool active_2;
    private bool active_3, is_used;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        animator = player_gameobject.GetComponentInChildren<Animator>();
        camera_transform = player_gameobject.GetComponentInChildren<Camera>().transform;
        script_fps = player_gameobject.GetComponentInChildren<Player>();
        player_hp_current = player_hp;
        player_energy_current = player_energy;
        num_of_enemies = script_bb.m_enemyCount;
        player_hp_current = player_hp;

        dead_player = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        skill_timer_1 = skill_1;
        skill_timer_2 = skill_2;
        skill_timer_3 = skill_3;
    }

    // Update is called once per frame
    void Update()
    {
        if(player_hp_current <= 0 && dead_player == false)
        {
            dead_player = true;
            EndGame();
        }
        if (dead_player == false)
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

            if (auto_gun)
            {
                if (Input.GetMouseButton(0) && player_energy_current > 0)
                {
                    script_fps.animator.SetBool("Shooting", true);
                    script_fps.GunFire(ref player_energy_current, fire_rate);
                }
                else if (Input.GetMouseButton(0) && player_energy_current <= 0)
                {
                    StartCoroutine(OutOfAmmo());
                }
                else
                    animator.SetBool("Shooting", false);
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && player_energy_current > 0)
                {
                    script_fps.GunFire(ref player_energy_current, fire_rate);
                }
                else if (Input.GetMouseButtonDown(0) && player_energy_current <= 0)
                {
                    StartCoroutine(OutOfAmmo());
                }
            }

            if (Input.GetKey(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(camera_transform.position, camera_transform.forward, out hit, 5.0f))
                {
                    Interaction(hit.transform.gameObject);
                }
            }


            interaction_timer -= Time.deltaTime;
            if (interaction_timer < 0)
            {
                interaction_timer = 0;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (unlocked_mouse == false)
                {
                    Pause();
                }
                else if (unlocked_mouse == true)
                {
                    Pause();
                }
            }

            GameLoop();
            script_UI.UpdateUI();

            // temp
            if (Input.GetKey(KeyCode.P))
            {
                ReloadScene();
            }
        }
    }


    // vvvvvvv Move to player script vvvvvv
    public int GetPlayerHp()
    {
        return player_hp_current;
    }

    public int GetPlayerEnergy()
    {
        return player_energy_current;
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
        if (interactable.tag == "EnergyTerminal" && player_energy_current < player_energy && currency >= 1 && interaction_timer == 0)
        {
            if (player_energy_current > player_energy - cost_per_ammo && player_energy_current != player_energy)
            {
                energy_gain_temp = player_energy - player_energy_current;
                player_energy_current += energy_gain_temp;
                currency -= 1;
            }
            else
            {
                player_energy_current += cost_per_ammo;
                currency -= 1;
            }
            interaction_timer = interaction_cooldown;
        }
        else if (interactable.tag == "HealthTerminal" && player_hp_current < player_hp && currency >= 1 && interaction_timer == 0)
        {
            if (player_hp_current < player_hp)
            {
                player_hp_current += cost_per_hp;
                currency -= 1;
            }
            interaction_timer = interaction_cooldown;
        }
        else if (interactable.tag == "Mine" && interaction_timer == 0)
        {
            if(interactable.GetComponent<Mines>().GetActive() == false && currency >= 1)
            {
                interactable.GetComponent<Mines>().Activate(ref active_mines, mines_list);
                currency -= 1;
            }
            else if (interactable.GetComponent<Mines>().GetCurrentHp() < interactable.GetComponent<Mines>().mine_max_hp && currency >= mine_rep_cost)
            {
                interactable.GetComponent<Mines>().AddMineHP();
                currency -= 1;
            }
            interaction_timer = interaction_cooldown;
        }
    }

    public int PlayerTakenDamage(float damage)
    {
        if (player_hp_current > 0)
        {
            player_hp_current -= (int)damage;
        }
        else if (player_hp_current <= 0)
        {
            player_hp_current = 0;
        }
        return player_hp_current;
    }

    private IEnumerator OutOfAmmo()
    {
        animator.SetBool("OutOfAmmo", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("OutOfAmmo", false);
    }
    // ^^^^^^                       ^^^^^^


    // vvv keep vvv

    public void GameLoop()
    {
        num_of_enemies = script_bb.m_enemyCount;
        if(script_bb.IsWaveOngoing() == false)
        {
            wave_timer += Time.deltaTime;
            if(wave_timer >= time_to_next_wave)
            {
                wave_no++;

                script_bb.BeginWave();
            }
        }
        if(script_bb.IsWaveOngoing() == true && wave_timer > 0)
        {
            wave_timer = 0;
        }
    }

    public void AddCurrency()
    {
        currency += wave_reward * (active_mines + 1);
    }

    public void Pause()
    {
        is_paused = !is_paused;
        if (is_paused == true)
        {
            pause_menu.SetActive(true);
            game_play.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            unlocked_mouse = true;
            Time.timeScale = 0;
        }
        else
        {
            pause_menu.SetActive(false);
            game_play.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            unlocked_mouse = false;
            Time.timeScale = 1;
        }
    }

    public void EndGame()
    {
        if (dead_player == true)
        {
            game_over.SetActive(true);
            game_play.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            unlocked_mouse = true;
            Time.timeScale = 0;
        }
    }

    public void LoadAnotherScene(int index)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        unlocked_mouse = true;
        Time.timeScale = 1;
        dead_player = false;
        SceneManager.LoadScene(index);
    }

    public void ReloadScene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        unlocked_mouse = false;
        Time.timeScale = 1;
        dead_player = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
