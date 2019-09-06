using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class FPSControl : MonoBehaviour
{
    #region Scripts
    public GameManager script_gm;
    #endregion


    #region PlayerMovement
    public Transform playerCamera;

    public float playerSpeed = 300;

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
    private float jump_cooldown = 0.01f;
    private float jump_timer;
    private bool has_jumped;

    public int skill_damage = 1;

    public GameObject bomb, g_throw_point, frost_G;

    public float throw_force = 100;

    public float skill_2_radius = 5.0f;
    public float skill_2_power = 10.0f;

    private int player_hp;


    #endregion

    #region Animator

    private Animator animator;

    #endregion

    #region Particle
    

    #endregion

    #region FPSgun
    public int gun_damage = 1;
    
    public float weapon_range = 50f;
    public float hit_force = 100f;
    public Transform gun_end;

    private Camera fps_cam;
    private WaitForSeconds shot_duration = new WaitForSeconds(0.001f);
    private LineRenderer laser_line;
    private float next_fire;


    bool unlocked_mouse;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponent<Animator>();

        m_player_rb = GetComponent<Rigidbody>();
        m_player_collider = GetComponent<CapsuleCollider>();
        m_player_offset = m_player_collider.height * 0.5f + 0.075f;

        laser_line = GetComponent<LineRenderer>();
        fps_cam = GetComponentInChildren<Camera>();

        player_hp = script_gm.GetPlayerHp();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GroundPlayer() == true)
            Jump();

        if (Input.GetKeyDown(KeyCode.Escape) && unlocked_mouse == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            unlocked_mouse = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && unlocked_mouse == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            unlocked_mouse = false;
        }
        if(Input.GetKey(KeyCode.P))
        {
            ReloadScene();
        }
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
                has_jumped = false;
        }
    }

    /// <summary>
    /// Handles the camera movement based on mouse movement input
    /// </summary>
    void PlayerInputCamera()
    {
        m_camera_yaw += Input.GetAxis("Mouse X") * cameraSensitivity;
        m_camera_pitch -= Input.GetAxis("Mouse Y") * cameraSensitivity;
        m_camera_pitch = Mathf.Clamp(m_camera_pitch, minCameraPitch, maxCameraPitch);

        playerCamera.localRotation = Quaternion.Euler(m_camera_pitch, 0, 0);
        transform.rotation = Quaternion.Euler(0, m_camera_yaw, 0);
    }

    /// <summary>
    /// Keeps the player grounded.
    /// <para>Returns True/False depending on if the player is grounded or not.</para>
    /// </summary>
    /// <returns></returns>
    bool GroundPlayer(bool ground = false)
    {
        RaycastHit hit;

        Vector3 spherecast_origin;
        spherecast_origin = transform.position + (m_player_collider.height * 0.5f - m_player_collider.radius) * -transform.up;

        if (Physics.SphereCast(spherecast_origin, m_player_collider.radius - 0.075f, -transform.up, out hit, 0.2f, 1 << 9))
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
        direction *= playerSpeed * Time.deltaTime;

        animator.SetFloat("Speed", Input.GetAxisRaw("Vertical"));

        if (has_jumped == true)
            direction.y = m_player_rb.velocity.y;

        m_player_rb.velocity = direction;
    }


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
    public void SkillActive2()
    {
        Vector3 explosionPos = new Vector3(transform.position.x, transform.position.y,transform.position.z);
        Collider[] colliders = Physics.OverlapSphere(explosionPos, skill_2_radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = hit.GetComponentInParent<Rigidbody>();
            }
            if (rb != null && hit.transform.tag != "Player")
            {
                rb.AddExplosionForce(skill_2_power, explosionPos, skill_2_radius, 3.0f);
                if (hit.GetComponent<Agent>() != null)
                {
                    hit.GetComponent<Agent>().TakeDamage(skill_damage);
                }
            }
        }
        m_player_rb.AddRelativeForce(0, jump_force_y, -jump_force_z, ForceMode.Acceleration);
    }

    /// <summary>
    /// Performs skill 3 ( Throws frost Grenade / Freezes targets within a Radius )
    /// </summary>
    public void SkillActive3()
    {
        GameObject expl = Instantiate(frost_G);
        expl.transform.position = g_throw_point.transform.position;
        expl.GetComponent<Rigidbody>().AddForce(g_throw_point.transform.forward * throw_force, ForceMode.Impulse);
    }

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

            Vector3 ray_origin = fps_cam.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            RaycastHit hit_target;

            laser_line.SetPosition(0, gun_end.position);

            if (Physics.Raycast(ray_origin, fps_cam.transform.forward, out hit_target, weapon_range))
            {
                laser_line.SetPosition(1, hit_target.point);

                if (hit_target.rigidbody != null)
                {
                    hit_target.rigidbody.AddForce(-hit_target.normal * hit_force);
                }

                if (hit_target.transform.GetComponent<Agent>() != null)
                {
                    hit_target.transform.GetComponent<Agent>().TakeDamage(gun_damage);
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
        animator.SetBool("Shooting", true);
        yield return shot_duration;
        laser_line.enabled = false;
        animator.SetBool("Shooting", false);
    }
    private void Jump()
    {
        jump_timer = Time.time + jump_cooldown;
        has_jumped = true;
        m_player_rb.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (has_jumped == true && Time.time > jump_timer)
            has_jumped = false;
    }

    void LoadAnotherScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    public bool GetPlayerHP()
    {
        if (player_hp <= 0)
            return true;
        else
            return false;
    }
}
