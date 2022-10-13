using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlightVisualizer : MonoBehaviour
{
    [SerializeField] Bee bee;
    [SerializeField] float time;
    [SerializeField] bool flip = true; 

    private void OnDrawGizmos()
    {
        float v0 = flip ? -bee.moveSpeed : bee.moveSpeed;
        float v = v0;

        float displacement = ((v + v0) / 2) * time;        
        Vector2 endPos = new Vector2(transform.position.x + displacement, transform.position.y);

        Gizmos.DrawLine(transform.position, endPos);
    }
}
