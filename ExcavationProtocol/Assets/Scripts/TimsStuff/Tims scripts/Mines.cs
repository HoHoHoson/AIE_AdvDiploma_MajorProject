using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mines : MonoBehaviour
{
    [SerializeField] private GameObject m_excavatorParts;
    [SerializeField] private Material m_excavatorMaterial;

    private Animator m_animator;

    int mine_hp = 100;
    public bool is_active = false;

    public int mine_hp_recover = 50;
    public int mine_max_hp = 100;

    [Header("Explosion Settings")]
    private float m_force = 100f;
    private float m_radius = 10f;
    private float m_upwardsModifier = 10f;

    GameManager gamemanager;

    void Start()
    {
        gamemanager = FindObjectOfType<GameManager>();
        m_animator = GetComponent<Animator>();

        if(is_active == true)
        {
            mine_hp = mine_max_hp;
        }
    }

    void Update()
    {
        if (mine_hp <= 0 && is_active == true)
        {
            DeactivateMine(ref gamemanager.active_mines);
        }

        if(is_active == false)
        {
            mine_hp = 0;
        }
    }

    public void Activate(ref int active, GameObject[] list)
    {
        is_active = true;
        mine_hp = mine_hp_recover;
        if (active < list.Length)
        {
            active++;
        }
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

    public void DeactivateMine(ref int active)
    {
        is_active = false;
        if (active > 0)
        {
            active--;
        }

        GameObject broken_excavator = Instantiate(m_excavatorParts, transform.position, transform.rotation);
        for (int i = 0; i < broken_excavator.transform.childCount; ++i)
        {
            GameObject excavator_piece = broken_excavator.transform.GetChild(i).gameObject;

            excavator_piece.GetComponent<Renderer>().material = m_excavatorMaterial;
            excavator_piece.AddComponent<MeshCollider>().convex = true;

            Rigidbody rb = excavator_piece.AddComponent<Rigidbody>();
            rb.AddExplosionForce(m_force, broken_excavator.transform.position, m_radius, m_upwardsModifier, ForceMode.Impulse);
        }

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
