using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    Player script_player;
    public float blast_radius;
    public float blast_power;
    public float time_before_det = 0.1f;
    float countdown;
    int blast_dmg;

    bool has_det = false;

    // Start is called before the first frame update
    void Start()
    {
        script_player = FindObjectOfType<Player>();
        countdown = time_before_det;
        blast_dmg = script_player.skill_damage;
        has_det = false;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (Input.GetMouseButton(1) && countdown <= 0)
        {
            has_det = true;
            Explode();
        }
    }

    void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, blast_radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = hit.GetComponentInParent<Rigidbody>();
            }
            if (rb != null)
            {
                rb.AddExplosionForce(blast_power * rb.mass, explosionPos, blast_radius, 3.0f);
                if (hit.GetComponentInParent<Agent>() != null)
                {
                    hit.GetComponentInParent<Agent>().TakeDamage(blast_dmg);
                }
            }
        }
        Destroy(gameObject);
    }

    void Stick()
    {
        
    }
}
