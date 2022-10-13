using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : Collectible
{
    public override void Collect()
    {
        if (Player.Instance.GetHealth < 3 && Player.Instance.GetHealth > 0)
        {
            Debug.Log("add carrot sound");
            Player.Instance.AddHealth();
            gameObject.SetActive(false);
        }       
    }
}
