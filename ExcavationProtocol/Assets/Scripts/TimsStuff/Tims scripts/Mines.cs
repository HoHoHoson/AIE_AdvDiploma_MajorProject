using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mines : MonoBehaviour
{
    [SerializeField] private GameObject m_excavatorParts;
    [SerializeField] private Material m_excavatorMaterial;
    [SerializeField] private ParticleSystem m_drillDust;

    private Animator m_animator;

    int mine_hp = 100;
    public bool is_active = false;

    public int mine_hp_recover = 50;
    public int mine_max_hp = 100;

    [Header("Explosion Settings")]
    [SerializeField] private float m_force = 100f;
    [SerializeField] private float m_radius = 10f;
    [SerializeField] private float m_upwardsModifier = 10f;
    [SerializeField] private AudioSource m_explosionSound;

    GameManager gamemanager;

    void Start()
    {
        gamemanager = FindObjectOfType<GameManager>();
        m_animator = GetComponent<Animator>();
        m_drillDust = Instantiate(m_drillDust, transform);

        if(is_active == true)
        {
            mine_hp = mine_max_hp;
        }
    }

    void Update()
    {
        if (mine_hp <= 0 && is_active == true)
        {
            DeactivateMine();
        }

        if(is_active == false)
        {
            mine_hp = 0;
        }
    }

    public void Activate()
    {
        is_active = true;
    }

    public void MinesTakeDamage(int damage)
    {
        if (mine_hp > 0)
        {
            mine_hp -= damage;
        }
    }

    public void AddMineHP()
    {
        mine_hp += mine_hp_recover;
    }

    public void DeactivateMine()
    {
        is_active = false;
        m_drillDust.Stop();

        GameObject broken_excavator = Instantiate(m_excavatorParts, transform.position, transform.rotation);
        for (int i = 0; i < broken_excavator.transform.childCount; ++i)
        {
            GameObject excavator_piece = broken_excavator.transform.GetChild(i).gameObject;
            excavator_piece.layer = 9;

            excavator_piece.GetComponent<Renderer>().material = m_excavatorMaterial;
            excavator_piece.AddComponent<MeshCollider>().convex = true;

            Rigidbody rb = excavator_piece.AddComponent<Rigidbody>();
            rb.AddExplosionForce(m_force, broken_excavator.transform.position, m_radius, m_upwardsModifier, ForceMode.Impulse);
        }

        if (m_explosionSound != null)
        {
            AudioSource audio = Instantiate(m_explosionSound, transform.position, Quaternion.identity);
            Destroy(audio.gameObject, audio.clip.length);
        }
        else
            Debug.Log("ERROR: Drill does not have an explosion sound.");

        gameObject.SetActive(false);
    }

    public bool GetActive()
    {
        return is_active;
    }

    public int GetCurrentHp()
    {
        return mine_hp;
    }
}
