using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class FPSControl : MonoBehaviour
{
    public Transform playerCamera;

    public float playerSpeed = 300;

    public float cameraSensitivity = 1;
    public float maxCameraPitch = 50;
    public float minCameraPitch = -50;

    private Rigidbody m_player_rb;
    private CapsuleCollider m_player_collider;
    private float m_player_offset = 0;

    private float m_camera_yaw = 0;
    private float m_camera_pitch = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        m_player_rb = GetComponent<Rigidbody>();
        m_player_collider = GetComponent<CapsuleCollider>();
        m_player_offset = m_player_collider.height * 0.5f + 0.075f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
    }

    void LateUpdate()
    {
        PlayerInputCamera();
    }

    void FixedUpdate()
    {
        if (GroundPlayer())
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

        m_player_rb.velocity = direction;
    }
}
