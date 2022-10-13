using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingStarAnim : MonoBehaviour
{
    Vector2 end;
    [SerializeField]float speed;

    private void Update()
    {    
        if (Vector2.Distance(transform.position, end) <= .1f)
        {
            Destroy(gameObject);
        }
    }
    public void OnCreate(Vector2 endPos)
    {
        end = endPos;
        transform.DOMove(end, speed).SetEase(Ease.Linear).OnComplete(() => GameManager.Instance.uIManager.TweenStar());
    }
}
