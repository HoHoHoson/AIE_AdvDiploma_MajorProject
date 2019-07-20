using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSControl : MonoBehaviour
{
    public Transform playerModel;
    public Transform cameraOrbit;
    public Transform playerCamera;
    
    public float playerSpeed = 200;

    public float cameraSensitivity = 1;
    public float cameraMaxPitch = 50;
    public float cameraMinPitch = 50;

    public Vector3 cameraOffset;
    public float offsetMinDistance = 0.1f;

    private Rigidbody rb;

    private float camera_yaw = 0;
    private float camera_pitch = 0;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        /// Camera rotation ///

        camera_yaw += Input.GetAxis("Mouse X") * cameraSensitivity;
        camera_pitch += Input.GetAxis("Mouse Y") * cameraSensitivity;
        // Clamp pitch value to prevent neck snapping
        camera_pitch = Mathf.Clamp(camera_pitch, cameraMinPitch, cameraMaxPitch);

        cameraOrbit.rotation = Quaternion.Euler(-camera_pitch, camera_yaw, 0);

        /// Camera Collision ///

        RaycastHit hit;
        if (Physics.Raycast(cameraOrbit.position, -cameraOrbit.forward, out hit, -cameraOffset.z, ~(1 << 2)))
            playerCamera.localPosition = new Vector3(0, 0, -Mathf.Clamp(hit.distance, offsetMinDistance, -cameraOffset.z));
        else
            playerCamera.localPosition = cameraOffset;
    }

    private void FixedUpdate()
    {
        /// Player movement ///

        // Removes the pitch rotation from cameraOrbit.forward so that it can be used for linear movement
        // (doesn't change height if the camera is pitched)
        Vector3 direction = cameraOrbit.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        direction.y = 0;
        direction = direction.normalized;

        // Rotate player model to the direction of movement 
        if (direction != Vector3.zero)
            playerModel.rotation = Quaternion.LookRotation(direction);

        direction *= playerSpeed * Time.deltaTime;

        direction.y = rb.velocity.y;
        rb.velocity = direction;
    }
}
