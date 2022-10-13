using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : Enemy
{
    [SerializeField] Transform raycaster;
    [SerializeField] float wallCheck, groundCheck;
    Rigidbody2D body;
    [SerializeField] LayerMask obstacle;

    float xDir = -1;
    
    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckforGroundAndWall();
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        body.velocity = new Vector2(xDir * moveSpeed, body.velocity.y);
    }


    void CheckforGroundAndWall()
    {
        var ground = Physics2D.Raycast(raycaster.position, Vector2.down, groundCheck, obstacle);
        var wall = Physics2D.Raycast(raycaster.position, raycaster.right, wallCheck, obstacle);

        if (wall || !ground)
        {
            xDir *= -1;

            transform.rotation = transform.rotation.y == 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);          
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(raycaster.position, groundCheck * Vector3.down);
        Gizmos.DrawRay(raycaster.position, raycaster.right * -1 * wallCheck);      
    }
}
