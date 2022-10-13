using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField]Collider2D coll;
    float enterY;
    [SerializeField] Transform exitPos;
    [Range(0, 1)]
    [SerializeField] float exitHeightPercentage;
    [SerializeField]GameObject blocker;
    bool over;

    public float ExitHeight => coll.bounds.max.y * exitHeightPercentage; 

    public void Interact()
    {      
        Player.Instance.ladderCenter = transform;
        enterY = Player.Instance.transform.position.y;
        over = false;
        if(blocker != null) blocker.SetActive(false);       
    }
  
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            over = true;
            Player.Instance.ladderCenter = null;          
            if(blocker != null) blocker.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {          
            Player.Instance.currInteraction = this;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (enterY > transform.position.y) return;
            if (Player.Instance.transform.position.y >= ExitHeight && !over)
            {
                Player.Instance.transform.position = exitPos.position;
                Player.Instance.body.velocity = Vector2.zero;
                Player.Instance.ladderCenter = null;
                Player.Instance.currInteraction = null;
                over = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        var bottom = new Vector2(coll.bounds.center.x, coll.bounds.min.y);      
        Debug.DrawLine(bottom, new Vector3(bottom.x, ExitHeight), Color.magenta);
    }
}
