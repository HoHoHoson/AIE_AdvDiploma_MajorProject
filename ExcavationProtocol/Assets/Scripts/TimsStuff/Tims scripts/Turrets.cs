using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turrets : MonoBehaviour
{
	public int m_damage = 1;
	public float turret_range = 10;
	public float fire_rate = 0.15f;
	public float max_ammo = 100;
	private float current_ammo;
	
	public GameObject turret_center;
	public ParticleSystem End1 = null, End2 = null;
    private SoundSystem m_sound_system;

	private GameObject Target = null;
	private float prevDist;

	float next_fire;
	public float turnspeed = 1;

	void Start()
    {
		current_ammo = max_ammo;

		prevDist = turret_range;
        m_sound_system = GetComponent<SoundSystem>();
    }
	
    void Update()
    {
		Shoot(ref current_ammo);

		if (current_ammo <= 0)
		{
			turret_center.transform.rotation = new Quaternion(0f,0f,0f,1f);

			Destroy(gameObject);
		}
    }

	GameObject FindTarget()
	{
		Collider[] colliders = Physics.OverlapSphere(turret_center.transform.position, turret_range);

		foreach (Collider enemies in colliders)
		{
			if(enemies.gameObject.layer == 10)
			{
				float distance = Vector3.Distance(turret_center.transform.position, enemies.transform.position);
				if(distance < prevDist)
				{
					Target = enemies.gameObject;
					prevDist = distance;
				}
			}
		}

		if (Target == null)
		{
			prevDist = turret_range;
		}
		return Target;
	}

	void Shoot(ref float currentAmmo)
	{
		GameObject target = FindTarget();

		if (target != null && currentAmmo > 0)
		{

			Vector3 direction = target.transform.position - transform.position;
			Quaternion Rotate = Quaternion.LookRotation(direction);
			Vector3 Rot = Quaternion.Lerp(turret_center.transform.rotation, Rotate, Time.deltaTime * turnspeed).eulerAngles;
			turret_center.transform.rotation = Quaternion.Euler(0, Rot.y, 0);
			

			if (Time.time > next_fire)
			{
				next_fire = Time.time + fire_rate;


				if(target.GetComponent<Agent>().IsDead() != true)
				{
					target.GetComponent<Agent>().TakeDamage(m_damage);
					End1.Play();
					End2.Play();
                    m_sound_system.PlayRandom();
					currentAmmo -= 1;
				}
				else
				{
					target = null;
					End1.Stop();
					End2.Stop();
					prevDist = turret_range;
				}
			}
		}
	}
}
