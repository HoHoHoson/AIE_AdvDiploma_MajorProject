using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class FPSControl : MonoBehaviour
{
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

    private bool has_jumped;

    public float skill_1_radius = 5.0f;
    public float skill_1_power = 10.0f;

    #endregion

    #region FPSgun
    public int gun_damage = 1;
    public float fire_rate = 0.25f;
    public float weapon_range = 50f;
    public float hit_force = 100f;
    public Transform gun_end;

    private Camera fps_cam;
    private WaitForSeconds shot_duration = new WaitForSeconds(0.07f);
    private LineRenderer laser_line;
    private float next_fire;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_player_rb = GetComponent<Rigidbody>();
        m_player_collider = GetComponent<CapsuleCollider>();
        m_player_offset = m_player_collider.height * 0.5f + 0.075f;

        laser_line = GetComponent<LineRenderer>();
        fps_cam = GetComponentInChildren<Camera>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GroundPlayer() == true)
            StartCoroutine(Jumping());

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        PlayerInputCamera();
    }

    void FixedUpdate()
    {
        if (has_jumped == true || GroundPlayer())
            PlayerInputMovement();
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
    bool GroundPlayer()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, m_player_offset + 0.1f))
        {
            Vector3 new_pos = hit.point;
            new_pos.y += m_player_offset;
            transform.position = new_pos;

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

        if (has_jumped == true)
            direction.y = m_player_rb.velocity.y;

        m_player_rb.velocity = direction;
    }

    /// <summary>
    /// Performs skill 1 ( Knockback AOE from Player )
    /// </summary>
    public void SkillActive1()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, skill_1_radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null && hit.transform.tag != "Player")
            {
                rb.AddExplosionForce(skill_1_power, explosionPos, skill_1_radius, 3.0f);
            }
        }
    }

    /// <summary>
    /// Performs skill 2 ( Disengage to point behind Player )
    /// </summary>
    public void SkillActive2()
    { 
        m_player_rb.AddRelativeForce(0, jump_force_y,-jump_force_z, ForceMode.Acceleration);
    }

    /// <summary>
    /// Fires the gun with raycasts
    /// </summary>
    public void GunFire(ref int current_en)
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
        yield return shot_duration;
        laser_line.enabled = false;
    }
    private IEnumerator Jumping()
    {
        has_jumped = true;
        m_player_rb.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
        yield return new WaitForSecondsRealtime(.5f);
        has_jumped = false;
    }
}
