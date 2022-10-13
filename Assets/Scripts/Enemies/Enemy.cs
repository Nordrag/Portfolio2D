using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDestructible, ISaveable, ISerializationCallbackReceiver
{
    public float moveSpeed;    
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected SimpleAudio sAudio;
    [SerializeField]protected EventReference deathSound;

    string id;

    protected bool isAlive { get; private set; } = true;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();     
        sAudio = GetComponent<SimpleAudio>();       
    }

    protected abstract void Move();

    public void TakeDamage()
    {
        isAlive = false;      
        animator.Play("enemy-death");
        sAudio.Play(deathSound);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") &&isAlive)
        {
            if (Player.Instance.bCollider.bounds.min.y > transform.position.y)
            {
                Player.Instance.Jump();
                TakeDamage();
            }
            else
            {
                Player.Instance.KnockBack(transform.position);
                Player.Instance.TakeDamage();
            }
        }
    }

    public void AE_DeathAnimFinished()
    {
        gameObject.SetActive(false);
    }

    public void Save()
    {
       //next time
    }

    public void Load()
    {
        var thisCharacter = GameManager.Instance.saveFile.Characters.FirstOrDefault(x => x.Guid == id);
    }

    public void OnBeforeSerialize()
    {
        if (id == string.Empty)
        {
            id = Guid.NewGuid().ToString();
        }
    }

    public void OnAfterDeserialize()
    {
        
    }
}
