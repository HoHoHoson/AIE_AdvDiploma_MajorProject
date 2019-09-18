using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private CapsuleCollider cap;
    
    private void Start()
    {
        cap = GetComponent<CapsuleCollider>();

        Derived b = new Derived();
        TestFun(b);
    }

    private void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.forward;
    }

    void TestFun(in Base b)
    {
        b.Over();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (cap != null)
            Gizmos.DrawCube(transform.position, cap.bounds.extents * 2);
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 new_up = Vector3.zero;
        List<Vector3> contacts = new List<Vector3>();

        foreach (ContactPoint point in collision.contacts)
            if (contacts.Contains(point.normal) == false)
                contacts.Add(point.normal);

        foreach (Vector3 p in contacts)
            new_up += p;

        new_up = new_up.normalized;

        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, new_up), new_up);
    }
}

public class Base
{
    public virtual void Over()
    {
        Debug.Log("Base class");
    }
}

public class Derived : Base
{
    public override void Over()
    {
        Debug.Log("Childed class");
    }
}

