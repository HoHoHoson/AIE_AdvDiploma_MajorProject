using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    // Variables 
    #region Scripts
    public GameManager script_gm;
    #endregion

    #region PlayerStats

    [Header("Player Values")]

    // Health
    public int player_hp = 100;
    // Energy
    public int player_energy = 150;


    protected int player_hp_current, player_energy_current;

    private int energy_gain_temp;

    // makes gun automatic
    public bool auto_gun;
    public float fire_rate = 0.25f;

	public float reload_time;	// Time Set Duration
	private float reload_clock;	// clock (dTime)

    public float interaction_cooldown = 0.25f;
    private float interaction_timer;

    #endregion

    #region PlayerMovement
    public Transform playerCamera;

    public float playerSpeed = 300;
	private bool is_sprinting = false;
	public float SprintMult = 2;

    public float cameraSensitivity = 1;
    public float maxCameraPitch = 50;
    public float minCameraPitch = -50;

    public float jump_force_z = 100, jump_force_y=10;
    private Vector3 jump_back;


    private Rigidbody m_player_rb;
    private CapsuleCollider m_player_collider;
    private float m_player_offset = 0;

    private float m_camera_yaw = 0;
    private float m_camera_pitch = 0;

    [SerializeField]
    private readonly float jump_cooldown = 0.01f;
    private float jump_timer;
    private bool has_jumped;
    #endregion

    #region Skills

    [Header("Player Skill Values")]
    public int skill_damage = 1;

    public GameObject bomb, land_mine, g_throw_point, frost_G;

    public float throw_force = 20, throw_force_2 = 10;

    protected bool skill_2_active = false;

    public float skill_1 = 20;
    public float skill_2 = 10;
    public float skill_3 = 20;

    [HideInInspector]
    public float skill_timer_1, skill_timer_2, skill_timer_3;

    private bool active_1;
    private bool active_2;
    private bool active_3;
    private readonly bool is_used;

    #endregion

    #region Animator

    public Animator animator;

    #endregion

    #region Particle

    public ParticleSystem m_bloodSFX = null;
	public ParticleSystem m_laserFlash = null;

	#endregion

	#region FPSgun

	public int gun_damage = 1;
    
    public float weapon_range = 50f;
    public float hit_force = 100f;
    public Transform gun_end;
    public float gun_ads_time = 1;

	private float ads_timer;


	private Camera fps_cam;
    private readonly WaitForSeconds shot_duration = new WaitForSeconds(0.001f);
    private LineRenderer laser_line;
    private float next_fire;
    private SoundSystem m_sound_system;

    private Transform camera_transform;

	public Transform GunPivot, GunOffset;
    #endregion

    // Functions

    #region StartUpdate

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        m_player_rb = GetComponent<Rigidbody>();
        m_player_collider = GetComponent<CapsuleCollider>();
        m_player_offset = m_player_collider.height * 0.5f + 0.075f;

        laser_line = GetComponent<LineRenderer>();
        fps_cam = GetComponentInChildren<Camera>();

        player_hp_current = player_hp;
        player_energy_current = player_energy;
        
		camera_transform = fps_cam.transform;
        skill_timer_1 = skill_1;
        skill_timer_3 = skill_3;

        m_sound_system = GetComponent<SoundSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GroundPlayer() == true)
            Jump();
    }

    void LateUpdate()
    {
        PlayerInputCamera();
    }

    void FixedUpdate()
    {
        if (has_jumped == true || GroundPlayer(true))
        {
            PlayerInputMovement();

            if (Time.time > jump_timer && GroundPlayer())
			{
                has_jumped = false;
				animator.SetBool("Jumping", false);
			}
        }
    }

    #endregion

    #region PlayerFunc

    public int GetPlayerHp()
    {
        return player_hp_current;
    }

    public int GetPlayerEnergy()
    {
        return player_energy_current;
    }

    public void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            CompleteAction1();
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Alpha2))
        {
			is_sprinting = true;
        }
		else
		{
			is_sprinting = false;
		}

        if (Input.GetKeyDown(KeyCode.F)|| Input.GetKeyDown(KeyCode.Alpha3))
        {
            CompleteAction3();
        }

        // Aim down sights function
        GunADS();

		if (player_energy_current <= 0)
		{
			animator.SetBool("Aiming", false);
			animator.SetBool("Shooting", false);
			animator.SetBool("Reload", true);
			m_laserFlash.Stop();

			player_energy_current = player_energy;
		}
		else if (animator.GetBool("Reload") == false && player_energy_current > 0)
		{
			if (Input.GetMouseButton(0))
			{
				animator.SetBool("Shooting", true);
				m_laserFlash.Play();
				GunFire(ref player_energy_current, fire_rate);
			}
			else
			{
				m_laserFlash.Stop();
				animator.SetBool("Shooting", false);
			}
		}

		if (Input.GetKey(KeyCode.R))
		{
			animator.SetBool("Shooting", false);
			animator.SetBool("Reload", true);
			m_laserFlash.Stop();

			player_energy_current = player_energy;
		}

        interaction_timer -= Time.deltaTime;
        if (interaction_timer < 0)
        {
            interaction_timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Set Jumping variable in the animator to true
			if (has_jumped == false)
			{
			}
        }
        else
        {
            // If not it remains false
			if (has_jumped == true)
			{
			}
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
    
    public bool IsPlayerDead()
    {
        if (player_hp <= 0)
            return true;
        else
            return false;
    }

    private void GunADS()
    {
        if (Input.GetMouseButton(1))
        {
            ads_timer += Time.deltaTime;
            animator.SetBool("Aiming", true);
        }
        else
        {
            ads_timer -= Time.deltaTime;
            animator.SetBool("Aiming", false);
        }

        ads_timer = Mathf.Clamp(ads_timer, 0, gun_ads_time);

        float t = ads_timer / gun_ads_time;
        //aim down sights values, think of counter strike
        GunOffset.localPosition = new Vector3(
            Mathf.Lerp(GunPivot.localPosition.x, GunPivot.localPosition.x - 0.1055f, t), // X value
            Mathf.Lerp(0, 0f, t), // Y value
            Mathf.Lerp(0, 0f, t)); // Z value

        fps_cam.fieldOfView = Mathf.Lerp(60, 30, t);
    }

    #endregion

    #region CameraFunc

    /// <summary>
    /// Handles the camera movement based on mouse movement input
    /// </summary>
    void PlayerInputCamera()
    {
        if (script_gm.is_paused == false && script_gm.dead_player == false)
        {
            m_camera_yaw += Input.GetAxis("Mouse X") * cameraSensitivity;
            m_camera_pitch -= Input.GetAxis("Mouse Y") * cameraSensitivity;
            m_camera_pitch = Mathf.Clamp(m_camera_pitch, minCameraPitch, maxCameraPitch);

            playerCamera.localRotation = Quaternion.Euler(m_camera_pitch, 0, 0);
            transform.rotation = Quaternion.Euler(0, m_camera_yaw, 0);
        }
    }

    #endregion

    #region PlayerMovementFunc

    /// <summary>
    /// Keeps the player grounded.
    /// <para>Returns True/False depending on if the player is grounded or not.</para>
    /// </summary>
    /// <returns></returns>
    bool GroundPlayer(bool ground = false)
    {

        Vector3 spherecast_origin;
        spherecast_origin = transform.position + (m_player_collider.height * 0.5f - m_player_collider.radius) * -transform.up;

        if (Physics.SphereCast(spherecast_origin, m_player_collider.radius - 0.075f, -transform.up, out RaycastHit hit, 0.2f, 1 << 9))
        {
            if (ground == true)
                transform.position -= (hit.distance - 0.15f) * transform.up;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Moves the player in the direction of the player keyboard input, relative to the first person camera.
    /// </summary>
    void PlayerInputMovement()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        direction = transform.TransformDirection(direction);
        direction = direction.normalized;

        float average_speed = (Math.Abs(Input.GetAxisRaw("Vertical")) + Math.Abs(Input.GetAxisRaw("Horizontal"))) / 2;
        animator.SetFloat("Speed", average_speed);
		if (is_sprinting == false)
		{
			direction *= playerSpeed * Time.deltaTime;
		}
		else
		{
			direction *= (playerSpeed * SprintMult) * Time.deltaTime;
		}

        if (has_jumped == true)
            direction.y = m_player_rb.velocity.y;

        m_player_rb.velocity = direction;
    }

    private void Jump()
    {
        jump_timer = Time.deltaTime + jump_cooldown;
		has_jumped = true;
		animator.SetBool("Jumping", true);
		m_sound_system.GetClip(1).GetAudioSource().PlayOneShot(m_sound_system.GetClip(1).GetAudioSource().clip);
        m_player_rb.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
    }

  //  private void OnCollisionEnter(Collision collision)
  //  {
		//if (has_jumped == true /*&& Time.deltaTime > jump_timer*/ && collision.gameObject.layer == LayerMask.GetMask("Ground"))
		//{
		//	has_jumped = false;
		//	animator.SetBool("Jumping", false);
		//}
  //  }
    
    #endregion

    #region SkillFunc

    /// <summary>
    /// Performs skill 1 ( Throws Grenade / Damages targets within a Radius with some knockback )
    /// </summary>
    public void SkillActive1()
    {
        GameObject expl = Instantiate(bomb);
        expl.transform.position = g_throw_point.transform.position;
        expl.GetComponent<Rigidbody>().AddForce(g_throw_point.transform.forward * throw_force, ForceMode.Impulse);
    }

    /// <summary>
    /// Performs skill 2 ( Places a Mine / allows the player to place and deternate a mine
    /// dealing damage to targets and knocking everything with a RB away from it )
    /// </summary>
    //public void SkillActive2()
    //{
	//	
    //}

    /// <summary>
    /// Performs skill 3 ( Throws frost Grenade / Freezes targets within a Radius )
    /// </summary>
    public void SkillActive3()
    {
        GameObject expl = Instantiate(frost_G);
        expl.transform.position = g_throw_point.transform.position;
        expl.GetComponent<Rigidbody>().AddForce(g_throw_point.transform.forward * throw_force, ForceMode.Impulse);
    }

    public void CompleteAction1()
    {
        if (skill_timer_1 < skill_1)
            return;

        active_1 = false;
        skill_timer_1 = 0;
        SkillActive1();
        active_1 = true;

        animator.ResetTrigger("Throw");
        animator.SetTrigger("Throw");
    }

	//public void CompleteAction2()
	//{
	//	//if (skill_timer_2 < skill_2)
	//	//	return;
	//	//
	//	//active_2 = false;
	//	//skill_timer_2 = 0;
	//	SkillActive2();
	//	//active_2 = true;
	//}

	public void CompleteAction3()
    {
        if (skill_timer_3 < skill_3)
            return;

        active_3 = false;
        skill_timer_3 = 0;
        SkillActive3();
        active_3 = true;

        animator.ResetTrigger("Throw");
        animator.SetTrigger("Throw");
    }


    public void SkillTimers()
    {
        if (active_1)
        {
            skill_timer_1 += Time.deltaTime;
        }
        if (skill_timer_1 > skill_1)
        {
            skill_timer_1 = skill_1;
        }

        //if (active_2)
        //{
        //    skill_timer_2 += Time.deltaTime;
        //}
        //if (skill_timer_2 > skill_2)
        //{
        //    skill_timer_2 = skill_2;
        //}

        if (active_3)
        {
            skill_timer_3 += Time.deltaTime;
        }
        if (skill_timer_3 > skill_3)
        {
            skill_timer_3 = skill_3;
        }
    }
    #endregion

    #region GunFireFunc

    /// <summary>
    /// Fires the gun with raycasts
    /// </summary>
    public void GunFire(ref int current_en, float fire_rate)
    {
        if (Time.time > next_fire)
        {
            next_fire = Time.time + fire_rate;
            current_en--;
            StartCoroutine(ShotEffect());

            m_sound_system.GetClip(0).PlayAudio();

            Vector3 ray_origin = fps_cam.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            laser_line.SetPosition(0, gun_end.position);

            if (Physics.Raycast(ray_origin, fps_cam.transform.forward, out RaycastHit hit_target, weapon_range))
            {
                laser_line.SetPosition(1, hit_target.point);

                if (hit_target.rigidbody != null)
                {
                    hit_target.rigidbody.AddForce(-hit_target.normal * hit_force);
                }

                if (hit_target.transform.GetComponent<Agent>() != null)
                {
                    hit_target.transform.GetComponent<Agent>().TakeDamage(gun_damage);

                    GameObject sfx = Instantiate(m_bloodSFX.gameObject, hit_target.point, Quaternion.identity);
                    Destroy(sfx, m_bloodSFX.main.duration);
                }
            }
            else
            {
                laser_line.SetPosition(1,ray_origin + (fps_cam.transform.forward * weapon_range));
            }
        }
    }

    private IEnumerator ShotEffect()
    {
        laser_line.enabled = true;
        yield return shot_duration;
        laser_line.enabled = false;
	}

	void ReloadComplete()
	{
		animator.SetBool("Reload", false);
	}
    #endregion
}
