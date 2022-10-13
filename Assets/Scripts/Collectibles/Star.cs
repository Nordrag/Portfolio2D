using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Collectible
{
    public override void Collect()
    {
        Player.Instance.PlayStarSound();
        GameManager.Instance.uIManager.ChangeStarCount(1);
        GameManager.Instance.uIManager.SpawnStar(transform.position);
        gameObject.SetActive(false);
    }  
}
