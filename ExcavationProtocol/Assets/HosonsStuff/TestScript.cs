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
