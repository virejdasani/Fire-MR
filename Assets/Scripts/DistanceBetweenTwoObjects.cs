using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DistanceBetweenTwoObjects : MonoBehaviour
{
    public GameObject target1;
    public GameObject target2;
    public float distanceX;
    public float distanceY;
    public float distanceZ;
    public float distanceTotal;

    void Start()
    {

    }

    void Update()
    {
        Vector3 delta = target2.transform.position - target1.transform.position;
        distanceX = delta.x;
        distanceY = delta.y;
        distanceZ = delta.z;
        distanceTotal = delta.magnitude * 100;
    }
}
