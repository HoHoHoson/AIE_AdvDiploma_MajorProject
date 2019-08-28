﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    FPSControl script_fps;
    public float bomb_timer = 3f;
    bool has_exploded;
    public float skill_1_radius = 5.0f;
    public float skill_1_power = 10.0f;
    private int skill_damage;
    float countdown;

    // Start is called before the first frame update
    void Start()
    {
        script_fps = FindObjectOfType<FPSControl>();
        skill_damage = script_fps.skill_damage;
        countdown = bomb_timer;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0f && !has_exploded)
        {
            Explode();
        }
    }

    public void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, skill_1_radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = hit.GetComponentInChildren<Rigidbody>();
            }
            if (rb != null && hit.transform.tag != "Player")
            {
                rb.AddExplosionForce(skill_1_power, explosionPos, skill_1_radius, 3.0f);
                if (hit.GetComponent<Agent>() != null)
                {
                    hit.GetComponent<Agent>().TakeDamage(skill_damage);
                }
            }
        }
        Destroy(gameObject);
    }
}