using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono : MonoBehaviour {

    public Light light;

    public int TestField = 1;
    private Color TestFieldPrivate = new Color(1,1,1,1);

    public int TestProp { get; set; }
    public int TestPropGet { get { return -1; } }

    public void TestFunc() { Debug.Log("Wow!"); }
    public void TestFuncPrivate() { Debug.Log("Wow private!"); }

    private void Start()
    {
        CancelInvoke();
        InvokeRepeating("AddOne", 1f, 1f);
        Debug.Log("Wow Start!");
    }

    private void Update()
    {
        light.color = TestFieldPrivate;
    }

    private void AddOne()
    {
        TestField++;
        TestProp++;
    }
}
