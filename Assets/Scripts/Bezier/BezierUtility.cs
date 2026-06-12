using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierUtility : MonoBehaviour
{
    internal static Vector3 BezierIntepolate3(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
    var u = 1 - t;
        var tt = t * t;
        return u * u * p0 + 2 * u * t * p1 + tt * p2;


    }
}
