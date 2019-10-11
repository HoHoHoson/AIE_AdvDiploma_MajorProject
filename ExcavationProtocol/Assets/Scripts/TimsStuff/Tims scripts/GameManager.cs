using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // Variables
    #region Scripts
    public Ui script_UI; // Ui
    public Player script_player; // fps controller script
    public Blackboard script_bb; // blackboard script
    #endregion
    
    #region Loop

    [Header("GameLoop Variables")]
    public int active_mines = 1;
    public int mine_rep_cost = 1;
    public int mine_cost = 1;

    public GameObject[] mines_list;

    public bool player_take_dmg = false, player_restore_hp = false;

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

    #region Player

    public GameObject player_gameobject;
    bool unlocked_mouse;
    #endregion
    
    #region Wave
    [Header("Wave Values")]

    public int wave_no;

    public float time_to_next_wave = 20;
    private float wave_timer;

    [Tooltip("Max amount of enemies that need to be spawned.")]
    public int num_of_enemies;
    #endregion
    
    #region Currency

    [Header("Currency Values")]
    public int currency = 20;
    public int wave_reward = 5;
    public int cost_per_hp, cost_per_ammo;

    #endregion

    // Functions

    #region StartUpdate
    void Start()
    {
        Time.timeScale = 1;
        animator = player_gameobject.GetComponent<Player>().animator;
        
        script_player = player_gameobject.GetComponentInChildren<Player>();

        dead_player = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (GameObject mine in mines_list)
        {
            if(mine.GetComponent<Mines>().GetActive() == true && active_mines < 4)
            {
                active_mines++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(script_player.GetPlayerHp() <= 0 && dead_player == false)
        {
            dead_player = true;
            EndGame();
        }

        
        if (dead_player == false)
        {
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

            script_player.Inputs();
            script_player.SkillTimers();
            GameLoop();
            script_UI.UpdateUI();

            // temp
            if (Input.GetKey(KeyCode.P))
            {
                ReloadScene();
            }
        }
    }

    #endregion

    #region GamePlayFunc

    public void GameLoop()
    {
        num_of_enemies = script_bb.TotalEnemyCount();
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

    #endregion

    #region Currency

    public void AddCurrency()
    {
        currency += wave_reward * (active_mines + 1);
    }

    #endregion

    #region MenuFunc

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

    #endregion
}
