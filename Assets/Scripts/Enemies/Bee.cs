using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Enemy
{
     
    Vector2 startPos;
    Vector2 targetPos;
    [SerializeField] Vector2 endPos;
    public Rigidbody2D body;

    protected override void Awake()
    {
        base.Awake();
        startPos = transform.position;
        targetPos = endPos;
    }

    private void Update()
    {        
        spriteRenderer.flipX = Player.Instance.transform.position.x < transform.position.x ? false : true;
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        if (isAlive)
        {
            var dir = targetPos - (Vector2)transform.position;
            dir.Normalize();

            body.velocity = moveSpeed * dir;          

            if (Vector2.Distance(targetPos, transform.position) <= .01f)
            {
                targetPos = Vector2.Distance(transform.position, startPos) > Vector2.Distance(transform.position, endPos) ? startPos : endPos;
            }
        }       
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(endPos, 1);
    }

    [ContextMenu("SetPos")]
    public void SetEndPos()
    {
        endPos = transform.position;
    }
}
