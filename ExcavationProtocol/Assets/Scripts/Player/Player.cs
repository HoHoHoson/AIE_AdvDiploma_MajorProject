using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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

	// values of the energy and hp for UI and Player stats
    protected int player_hp_current, player_energy_current;

	// Visor variables
	public Transform visor_transform;
	private Transform visor_defaultPos, visor_zoomPos;
    public Renderer visor_renderer;
    public Material visor_material;

	// Hud Colour change
    public Color full_health_color = Color.blue;
    public Color half_health_color = Color.yellow;
    public Color no_health_color = Color.red;

	// Guns Fire Rate
    public float fire_rate = 0.25f;

	// Reload Timers
	public float reload_time;	// Time Set Duration
	private float reload_clock;	// clock (dTime)

	// Variables for cooldowns
    public float interaction_cooldown = 0.25f;
    private float interaction_timer;
    #endregion

    #region PlayerMovement
	// Getting the camera that is on the player
    public Transform playerCamera;

	// Players Movement speed
    public float playerSpeed = 300;
	
	// Sprinting variables
	private bool is_sprinting = false;
	public float SprintMult = 2;

	// Camera sensitivity when aiming normally
    public float cameraSensitivity = 100;

	// Percentage of normal aim when in the ADS state
	private float ADSdif = 0.3f;

	// Clamps the camera when looking up and down
    public float maxCameraPitch = 50;
    public float minCameraPitch = -50;

	// Player components
    private Rigidbody m_player_rb;
    private CapsuleCollider m_player_collider;
    private float m_player_offset = 0;

	// Camera pitch and yaw for camera rotation
    private float m_camera_yaw = 0;
    private float m_camera_pitch = 0;

	// Jumpping variables
    [SerializeField]
    private readonly float jump_cooldown = 0.01f;
    private float jump_timer;
    private bool has_jumped;
    #endregion

    #region Skills

	// Skill 1 damage
    [Header("Player Skill Values")]
    public int skill_damage = 1;

	// prefab grab and the point where you throw from
    public GameObject bomb, turret, g_throw_point, frost_G;

	// throw forces when throwing the grenades
    public float throw_force = 20, throw_force_2 = 10;

	// checks if skill 2 is active
    protected bool skill_2_active = false;

	// Skill Timers
    public float skill_1 = 20;
    public float skill_2 = 10;
    public float skill_3 = 20;

	// skill timers
    [HideInInspector]
    public float skill_timer_1, skill_timer_2, skill_timer_3;

	// if the ability is ready or active
    private bool active_1;
    private bool active_2;
    private bool active_3;
    private readonly bool is_used;

	// Turret Prefabs
	public GameObject TurretPrefab;
	public int turret_cost = 100;
	public GameObject turretDrop;
	public float turret_place_dist = 10;

    #endregion

    #region Animator

	// Grabs the animator
    public Animator animator;

    #endregion

    #region Particle

	// particles for the gun and bugs when hit
    public Transform m_batteryEjectPoint;
    public ParticleSystem m_batteryEject = null;
    public ParticleSystem m_bloodSFX = null;
	public ParticleSystem m_laserFlash = null;

	#endregion

	#region FPSgun
	// damage that the gun shot does
	public int gun_damage = 1;
    
	// gun range
    public float weapon_range = 50f;
	
	// force added to the hit target if they have a rigidbody
    public float hit_force = 100f;

	// fire point
    public Transform gun_end;

	// time that it takes for the gun to ADS
    public float gun_ads_time = 1;

	// timer counting the time to ADS
	private float ads_timer;

	// is ADS'ing
	private bool b_ADS = false;

	// Grabs the fps cam
	private Camera fps_cam;

	// how long you see the line renderer
    private readonly WaitForSeconds shot_duration = new WaitForSeconds(0.001f);

	// takes in the line renderer
    private LineRenderer laser_line;

	// time to next fire
    private float next_fire;

	// takes in the sound system
    private SoundSystem m_sound_system;

	// gets the camera transform
    private Transform camera_transform;

	// for the gun ADS calculations
	public Transform GunPivot, GunOffset;

	// on hit blast radius when the gun shoots an enemy
	public float BulletBlast = 3;

	// gets crosshair for the UI
	public GameObject dot, crosshair;
	
	// check for reloads
	public bool isrealoading = false;

	#endregion


	// Functions
	#region StartUpdate

	// Start is called before the first frame update
	void Start()
    {
		// initialising variables
        animator = GetComponent<Animator>();

        m_player_rb = GetComponent<Rigidbody>();
        m_player_collider = GetComponent<CapsuleCollider>();
        m_player_offset = m_player_collider.height * 0.5f + 0.075f;

        laser_line = GetComponent<LineRenderer>();
        fps_cam = GetComponentInChildren<Camera>();

        player_hp_current = player_hp;
        player_energy_current = player_energy;

        List<Material> m = new List<Material>();
        visor_renderer.GetMaterials(m);
        visor_material = m[0];
        visor_material.SetColor("_EmissionColor", full_health_color);
        
		camera_transform = fps_cam.transform;
        skill_timer_1 = skill_1;
        skill_timer_3 = skill_3;

		// if the mouse sensitivity is not 0 it changes to the stats provided by the start menu else uses defaults
		if (PlayerPrefs.GetFloat("MouseSen") != 0)
		{
			cameraSensitivity = PlayerPrefs.GetFloat("MouseSen");
			ADSdif = PlayerPrefs.GetFloat("ADSSen");
		}

        m_sound_system = GetComponent<SoundSystem>();
        m_batteryEject = Instantiate(m_batteryEject, m_batteryEjectPoint);
    }

	void Update()
    {
		// jumps if grounded and space is pressed
        if (Input.GetKeyDown(KeyCode.Space) && GroundPlayer() == true)
            Jump();
    }

    void LateUpdate()
    {
        PlayerInputCamera();
    }

    void FixedUpdate()
    {
		// animations and inputs when the player jumps
        if (has_jumped == true || GroundPlayer(true))
        {
            PlayerInputMovement();

            if (Time.time > jump_timer && GroundPlayer())
			{
                has_jumped = false;
				// for animator
				animator.SetBool("Jumping", false);
			}
        }
    }

	/// <summary>
	/// For ejecting the battery ( Animation )
	/// </summary>
    public void PlayBatteryEject()
    {
        m_batteryEject.Play();
    }

    #endregion

    #region PlayerFunc

	/// <summary>
	/// Gets Current Hp
	/// </summary>
	/// <returns> Players current Hp </returns>
    public int GetPlayerHp()
    {
        return player_hp_current;
    }

	/// <summary>
	/// Gets Current Energy
	/// </summary>
	/// <returns> Players Current Energy </returns>
    public int GetPlayerEnergy()
    {
        return player_energy_current;
    }

	/// <summary>
	/// Deals with all of the inputs that the player does
	/// </summary>
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

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            CompleteAction3();
        }

		if (Input.GetKeyDown(KeyCode.E))
		{
			DeployTurret();
		}

        // Aim down sights function
        GunADS();

		// Handling animations between reloading and shooting, also deals with shooting
		if (player_energy_current <= 0 && isrealoading != true)
		{
			is_reloading = true;
			animator.SetBool("Aiming", false);
			animator.SetBool("Shooting", false);
			animator.SetBool("Reload", true);
			m_laserFlash.Stop();

			player_energy_current = player_energy;
		}
		else if (animator.GetBool("Reload") == false && player_energy_current > 0)
		{
			if (Input.GetMouseButton(0) && animator.IsInTransition(0) == false)
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

		// Reloading with R
		if (Input.GetKey(KeyCode.R) && isrealoading != true)
		{
			is_reloading = true;
			animator.SetBool("Shooting", false);
			animator.SetBool("Reload", true);
			m_laserFlash.Stop();
		}

        interaction_timer -= Time.deltaTime;
        if (interaction_timer < 0)
        {
            interaction_timer = 0;
        }
    }

	/// <summary>
	/// Player taking Damage
	/// </summary>
	/// <param name="damage"> Damage Taken </param>
	/// <returns> Players Current Hp </returns>
    public int PlayerTakenDamage(float damage)
    {
        if (player_hp_current > 0)
        {
            player_hp_current -= (int)damage;

            // Lerp color
            Color lerped_color;
            float ratio = player_hp_current / (float)player_hp;
			// Changing the colour of the hud based on the Players HP
            if (player_hp_current < player_hp / 2)
            {
                lerped_color = Vector4.Lerp(no_health_color, half_health_color, ratio * 2.0f); // x2 because half or health total range
            }
            else
            {
                lerped_color = Vector4.Lerp(half_health_color, full_health_color, (ratio - 0.5f) * 2.0f ); // x2 because half or health total range
            }

            visor_material.SetColor("_EmissionColor", lerped_color);
        }
        else if (player_hp_current <= 0)
        {
            player_hp_current = 0;
        }
        return player_hp_current;
    }
    
	/// <summary>
	/// Checks If The Player Is Dead
	/// </summary>
	/// <returns> if player is dead </returns>
    public bool IsPlayerDead()
    {
        if (player_hp <= 0)
            return true;
        else
            return false;
    }

	/// <summary>
	/// Gun Scoping with right click
	/// </summary>
    private void GunADS()
    {
        if (Input.GetMouseButton(1) && !is_reloading)
        {
            ads_timer += Time.deltaTime;
            animator.SetBool("Aiming", true);
			crosshair.SetActive(false);
			dot.SetActive(true);
			b_ADS = true;
        }
        else
        {
            ads_timer -= Time.deltaTime;
            animator.SetBool("Aiming", false);
			dot.SetActive(false);
			crosshair.SetActive(true);
			b_ADS = false;
        }

        ads_timer = Mathf.Clamp(ads_timer, 0, gun_ads_time);

        float t = ads_timer / gun_ads_time;
        //aim down sights values, think of counter strike
        GunOffset.localPosition = new Vector3(
            Mathf.Lerp(GunPivot.localPosition.x, GunPivot.localPosition.x - 0.1055f, t), // X value
            Mathf.Lerp(0, 0.01f, t), // Y value
            Mathf.Lerp(0, -0.25f, t)); // Z value

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
			if (b_ADS == true)
			{
				m_camera_yaw += Input.GetAxis("Mouse X") * (cameraSensitivity * ADSdif);
				m_camera_pitch -= Input.GetAxis("Mouse Y") * (cameraSensitivity * ADSdif);
			}
			else
			{
				m_camera_yaw += Input.GetAxis("Mouse X") * cameraSensitivity;
				m_camera_pitch -= Input.GetAxis("Mouse Y") * cameraSensitivity;
			}
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
    /// <returns> If Grounded </returns>
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

	/// <summary>
	/// makes the player Jump
	/// </summary>
    private void Jump()
    {
        jump_timer = Time.deltaTime + jump_cooldown;
		has_jumped = true;
		animator.SetBool("Jumping", true);
		m_sound_system.GetClip(1).GetAudioSource().PlayOneShot(m_sound_system.GetClip(1).GetAudioSource().clip);
        m_player_rb.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
    }
    
    #endregion

    #region SkillFunc

	/// <summary>
	/// places the turret on the ground and takes the currency away
	/// </summary>
	void DeployTurret()
	{
		if(script_gm.GetCurrency() >= turret_cost)
		{
			Vector3 rayOr = turretDrop.transform.position;
			if (Physics.Raycast(rayOr, -turretDrop.transform.up, out RaycastHit hit, 5))
			{
				if (hit.transform.gameObject.layer == 9)
				{
					GameObject trt = Instantiate(TurretPrefab);
					trt.transform.position = new Vector3(turretDrop.transform.position.x, hit.point.y, turretDrop.transform.position.z);
					script_gm.SubtractCurrency(turret_cost);
				}
			}
		}
	}

    /// <summary>
    /// Performs skill 1 ( Throws Grenade / Damages targets within a Radius with some knockback )
    /// </summary>
    IEnumerator SkillActive1()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject expl = Instantiate(bomb);
        expl.transform.position = g_throw_point.transform.position;
        expl.GetComponent<Rigidbody>().AddForce(g_throw_point.transform.forward * throw_force, ForceMode.Impulse);
    }

    /// <summary>
    /// Performs skill 3 ( Throws frost Grenade / Freezes targets within a Radius )
    /// </summary>
    IEnumerator SkillActive3()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject expl = Instantiate(frost_G);
        expl.transform.position = g_throw_point.transform.position;
        expl.GetComponent<Rigidbody>().AddForce(g_throw_point.transform.forward * throw_force, ForceMode.Impulse);
    }

	/// <summary>
	/// Completes the skill animatons and other things that happens
	/// </summary>
    public void CompleteAction1()
    {
        if (skill_timer_1 < skill_1)
            return;

        active_1 = false;
        skill_timer_1 = 0;
        animator.ResetTrigger("Throw");
        StartCoroutine(SkillActive1());
        active_1 = true;
        animator.SetTrigger("Throw");
    }

	/// <summary>
	/// Completes the skill animatons and other things that happens
	/// </summary>
	public void CompleteAction3()
    {
        if (skill_timer_3 < skill_3)
            return;

        active_3 = false;
        skill_timer_3 = 0;
        animator.ResetTrigger("Throw");
        StartCoroutine(SkillActive3());
        active_3 = true;
        animator.SetTrigger("Throw");
    }

	/// <summary>
	/// Skill timers for cooldowns and stuff
	/// </summary>
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

			// creates ray origin at the center of the screen
            Vector3 ray_origin = fps_cam.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
			
			// line start position ( from gun to end point )
            laser_line.SetPosition(0, gun_end.position);

			// gets the raycast things from origin to the cameras forward with range of weapon_range(50)
            if (Physics.Raycast(ray_origin, fps_cam.transform.forward, out RaycastHit hit_target, weapon_range))
            {
				// sets end of line to be hit point ( max weapon range for finish )
                laser_line.SetPosition(1, hit_target.point);

				// knocks back object hit if it has a rigid body
                if (hit_target.rigidbody != null)
                {
                    hit_target.rigidbody.AddForce(-hit_target.normal * hit_force);
                }

				// if enemy then take damage
                Agent hit_agent = hit_target.transform.GetComponent<Agent>();
                if (hit_agent != null)
                {
                    ExplosiveAI explosive_baddy = hit_agent as ExplosiveAI;
					// explosive bullets
					Collider[] colliders = Physics.OverlapSphere(hit_target.point, BulletBlast);

					foreach(Collider hit in colliders)
					{
						if(hit.gameObject.layer == 10)
						{
							// if explosive enemies weak spot
							if (explosive_baddy != null)
								explosive_baddy.LocationalDamage(hit_target, gun_damage);
							else
								hit_agent.TakeDamage(gun_damage);
						}
					}

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

	/// <summary>
	/// enables and disables line renderer to act as a gun fire
	/// </summary>
	/// <returns> how long the shot lasts for </returns>
    private IEnumerator ShotEffect()
    {
        laser_line.enabled = true;
        yield return shot_duration;
        laser_line.enabled = false;
	}

	/// <summary>
	/// starts the reload sound
	/// </summary>
    void ReloadStart()
    {
        m_sound_system.GetClip(2).GetAudioSource().PlayOneShot(m_sound_system.GetClip(2).GetAudioSource().clip);
    }

	/// <summary>
	/// resets the reload on completion through events
	/// </summary>
    void ReloadComplete()
	{
		is_reloading = false;
		animator.SetBool("Reload", false);

        player_energy_current = player_energy;
    }
    #endregion
}
