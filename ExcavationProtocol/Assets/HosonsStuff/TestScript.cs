using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private int integer = 0;
    
    public ref int GetInt() { return ref integer; }

    private void Start()
    {
        // Polymorphism
        Derived b = new Derived();
        TestFun(b);

        GetInt() = 5;
        Debug.Log(integer);
        // Polymorphism

        int x = 0;
        int y = x++;
        Debug.Log(x + ", " + y);
    }

    private void Update()
    {
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

