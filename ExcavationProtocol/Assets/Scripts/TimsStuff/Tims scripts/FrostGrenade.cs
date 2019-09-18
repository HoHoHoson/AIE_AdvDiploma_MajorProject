using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostGrenade : MonoBehaviour
{
    Player script_fps;
    public ParticleSystem frostsplosion;
    public float bomb_timer = 3f;
    bool has_exploded = false;
    public float skill_3_radius = 5.0f;
    float countdown;
    public float freeze_time;
    float frost_countdown;

    // Start is called before the first frame update
    void Start()
    {
        script_fps = FindObjectOfType<Player>();

        countdown = bomb_timer;
        frost_countdown = freeze_time + bomb_timer;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        frost_countdown -= Time.deltaTime;

        if (countdown <= 0f && !has_exploded)
        {
            FreezeExplosion();
            Instantiate(frostsplosion, transform);
            
            has_exploded = true;
        }

        if (frost_countdown <= 0)
        {
            Unfreeze();
        }
    }

    public void FreezeExplosion()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, skill_3_radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = hit.GetComponentInParent<Rigidbody>();
            }
            if (rb != null && hit.transform.tag != "Player")
            {
                rb.isKinematic = true;
            }
        }
    }

    public void Unfreeze()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, skill_3_radius * 2);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = hit.GetComponentInParent<Rigidbody>();
            }
            if (rb != null && rb.isKinematic == true)
            {
                rb.isKinematic = false;
            }
        }
        Destroy(gameObject);
    }
}
