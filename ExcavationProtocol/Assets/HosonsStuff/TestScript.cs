using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
        Derived b = new Derived();
        TestFun(b);
    }

    void TestFun(in Base b)
    {
        b.Over();
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
