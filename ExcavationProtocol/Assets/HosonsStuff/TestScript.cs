using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private CapsuleCollider cap;
    private Vector3 normal = Vector3.up;
    private int integer = 0;
    
    public ref int GetInt() { return ref integer; }

    private void Start()
    {
        cap = GetComponent<CapsuleCollider>();

        Derived b = new Derived();
        TestFun(b);

        GetInt() = 5;
        Debug.Log(integer);
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.CapsuleCast(
            transform.position + transform.forward * (cap.height * 0.5f - cap.radius)
            , transform.position + -transform.forward * (cap.height * 0.5f - cap.radius)
            , cap.radius - 0.075f, -transform.up, out hit, 0.075f * 2))
        {
            Debug.DrawRay(hit.point, hit.normal * 3, Color.magenta);

            if (Vector3.Angle(hit.normal, Vector3.up) > 80)
                normal = Vector3.up;
            else
                normal = hit.normal;
        }
        else
            normal = Vector3.up;

        Vector3 target_direction = Vector3.forward;
        target_direction = Vector3.ProjectOnPlane(target_direction, normal);

        Quaternion target_rotation = 
            Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target_direction, normal), Time.deltaTime * 7);

        transform.rotation = target_rotation;

        GetComponent<Rigidbody>().velocity = transform.forward * 3 + -normal * 4;
    }

    void TestFun(in Base b)
    {
        b.Over();
    }

    private void OnDrawGizmos()
    {
        if (cap == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, cap.bounds.extents * 2);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + transform.forward * (cap.height * 0.5f - cap.radius), cap.radius);
        Gizmos.DrawSphere(transform.position + -transform.forward * (cap.height * 0.5f - cap.radius), cap.radius);
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

