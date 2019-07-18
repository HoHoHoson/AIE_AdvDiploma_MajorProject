using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSControl : MonoBehaviour
{
    public float playerSpeed = 200;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3();
        float vertical_input = Input.GetAxis("Vertical");
        float horizontal_input = Input.GetAxis("Horizontal");

        direction += Vector3.forward * vertical_input + Vector3.right * horizontal_input;

        if (vertical_input != 0 && horizontal_input != 0)
            Vector3.Normalize(direction);

        direction *= playerSpeed * Time.deltaTime;
        direction.y = rb.velocity.y;

        rb.velocity = direction;
    }
}
