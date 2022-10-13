using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicPredictor : MonoBehaviour
{
    [SerializeField] float finalVel, startVel, time, acceleration;
    [SerializeField] Vector2 dir;

    //displacement = (finalvel + startvel) / 2 * time
    private void OnDrawGizmos()
    {
        var displacement = ((finalVel + startVel) / 2) * time;
        var endPoint = (Vector2)transform.position + dir.normalized * displacement;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, endPoint);
    }
}
