using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;


public class Player : MonoBehaviour, IDestructible, ISaveable
{
    public static Player Instance { get; private set; }

    [HideInInspector] public SimpleAudio sAudio;
    public InputReader inputReader = new InputReader();
    public Rigidbody2D body;
    [HideInInspector] public Animator animator;
    SpriteRenderer spriteRenderer;
    public BoxCollider2D bCollider;
    [SerializeField] LayerMask groundLayer;
    public PhysicsMaterial2D pMat;
    public Collider2D normalCollider, crouchCollider;
    public IInteractable currInteraction;
    public Transform ladderCenter;
    float lastXInput;
    public float knockBackForce = 1f;
    public float knockBackDir;

    public Transform rayCaster;

    [Range(1, 3)]
    [SerializeField] int health = 3;
    public int GetHealth => health;
    [HideInInspector]public bool knockedBack = false;


    [SerializeField]PlatformEffector2D effector;

    [SerializeField] EventReference carrot, star, jumpEvent;

    readonly int yVelocity = Animator.StringToHash("yVelocity");
    readonly int jump = Animator.StringToHash("Jump");
    readonly int IsWalking = Animator.StringToHash("IsWalking");
    readonly int Hurt = Animator.StringToHash("Hurt");
    readonly int aGrounded = Animator.StringToHash("IsGrounded");
    readonly int Dead = Animator.StringToHash("Dead");
    readonly int Crouching = Animator.StringToHash("Crouch");
    readonly int Climbing = Animator.StringToHash("Climb");

    public bool IsGrounded { get; private set; }

    public float moveSpeed, jumpForce, groundCheckDist;
    public float climbSpeed = 3f;

    StateMachine stateMachine;
    PlayerLocomotionState playerLocomotionState;
    PlayerAirState playerAirState;
    PlayerKnockBackState playerKnockBackState;
    PlayerDeathState playerDeathState;
    PlayerCrouchState playerCrouchState;
    PlayerClimbState playerClimbState;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bCollider = GetComponent<BoxCollider2D>();
        sAudio = GetComponent<SimpleAudio>();
        Instance = this;
        GameManager.Instance.uIManager.OnPlayerHpChanged(health);

        stateMachine = new StateMachine();
        playerLocomotionState = new PlayerLocomotionState();
        playerAirState = new PlayerAirState();
        playerKnockBackState = new PlayerKnockBackState();
        playerDeathState = new PlayerDeathState();
        playerClimbState = new PlayerClimbState();
        playerCrouchState = new PlayerCrouchState();

        stateMachine.AddAnyTransition(playerDeathState, () => health <= 0);
        stateMachine.AddAnyTransition(playerKnockBackState, () => knockedBack);

        stateMachine.AddTransition(playerLocomotionState, playerAirState, () => !IsGrounded);
        stateMachine.AddTransition(playerAirState, playerLocomotionState, () => IsGrounded);
        stateMachine.AddTransition(playerKnockBackState, playerLocomotionState, () => !knockedBack);

        stateMachine.AddTransition(playerLocomotionState, playerCrouchState, () => inputReader.yMovement < 0);
        stateMachine.AddTransition(playerCrouchState, playerLocomotionState, () => inputReader.yMovement == 0);

        stateMachine.AddTransition(playerLocomotionState, playerClimbState, () => ladderCenter != null && inputReader.yMovement != 0);
        stateMachine.AddTransition(playerAirState, playerClimbState, () => ladderCenter != null);
        stateMachine.AddTransition(playerClimbState, playerLocomotionState, () => ladderCenter == null || IsGrounded);

        stateMachine.SetState(playerLocomotionState);
    }

    private void Update()
    {
        inputReader.ReadInput();
        CheckGrounded();
        SetAnimatorValues();

        if (inputReader.xMovement != 0)
        {
            lastXInput = inputReader.xMovement;
        }

        stateMachine.Update();

        var res = Physics2D.Raycast(rayCaster.position, Vector2.down, .7f, groundLayer);

        if (res)
        {
            var coll = res.collider.GetComponent<PlatformEffector2D>();

            if (coll)
            {
                effector = coll;
            }
            else
            {
                if (effector != null)
                {
                    effector.rotationalOffset = 0;
                }
                effector = null;
            }
        }
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void Jump()
    {
        body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetTrigger(jump);
    }

    public void TryJump()
    {
        if (inputReader.JumpPressed)
        {          
            if (inputReader.yMovement < 0)
            {
                if (effector)
                {
                    effector.rotationalOffset = 180;
                }
                return;
            }

            if (IsGrounded || ladderCenter != null)
            {
                ladderCenter = null;
                Jump();
                sAudio.Play(jumpEvent);
            }
        }
    }


    public void KnockBack(Vector2 pos)
    {      
        knockedBack = true;
        animator.SetTrigger(Hurt);
        knockBackDir = pos.x < transform.position.x ? 1 : -1;
    }

    public void Move()
    {
        body.velocity = new Vector2(inputReader.xMovement * moveSpeed, body.velocity.y);         
    }

    void SetAnimatorValues()
    {
        animator.SetBool(IsWalking, inputReader.xMovement != 0);
        animator.SetFloat(yVelocity, body.velocity.y);
        animator.SetBool(aGrounded, IsGrounded);
        animator.SetBool(Crouching, inputReader.yMovement < 0);
        animator.SetBool(Climbing, inputReader.yMovement > 0);
        spriteRenderer.flipX = lastXInput >= 0 ? false : true;
    }

    void CheckGrounded()
    {
        var bounds = bCollider.enabled? bCollider.bounds : crouchCollider.bounds;
        var check1 = Physics2D.Raycast(bounds.min, Vector2.down, groundCheckDist, groundLayer);
        var check2 = Physics2D.Raycast(new Vector2(bounds.center.x, bounds.min.y), Vector2.down, groundCheckDist, groundLayer);
        var check3 = Physics2D.Raycast(new Vector2(bounds.max.x, bounds.min.y), Vector2.down, groundCheckDist, groundLayer);

        IsGrounded = check1 || check2 || check3;       
    }

    private void OnDrawGizmosSelected()
    {
        var bounds = bCollider.bounds;

        Gizmos.DrawWireSphere(bounds.min, .5f);

        Debug.DrawRay(bounds.min, Vector2.down * groundCheckDist);
        Debug.DrawRay(new Vector2(bounds.center.x, bounds.min.y), Vector2.down * groundCheckDist);
        Debug.DrawRay(new Vector2(bounds.max.x, bounds.min.y), Vector2.down * groundCheckDist);

        var airTime = (jumpForce * -1) / Physics2D.gravity.y * body.gravityScale;
        airTime *= 2;

        float edgeRadius = bCollider.edgeRadius;

        const int resolution = 30; //the more we repeat the curved the line looks
        var prevPosRight = new Vector3(bounds.center.x, bounds.min.y - edgeRadius);
        var prevPosLeft = new Vector3(bounds.center.x, bounds.min.y - edgeRadius);

        for (var i = 1; i <= resolution; i++)
        {
            //var radius = characterCollider.edgeRadius;
            //the problem is, we need a vector2, but the equations only solve for a float as return, so we need to solve 2 of them
            //once for height (airTime), once for distance (left & rightDisp)
            var time = i / (float)resolution * airTime; //the height at the current iteration

            //equation for distance
            var rightDisp = new Vector3(moveSpeed, jumpForce) * time + (Vector3)Physics2D.gravity * time * time / 2 * body.gravityScale;
            var leftDisp = new Vector3(-moveSpeed, jumpForce) * time + (Vector3)Physics2D.gravity * time * time / 2 * body.gravityScale;


            //our current position, using the center of the collider again
            var drawPontRight = new Vector3(bounds.center.x, bounds.min.y) + rightDisp; //- new Vector3(radius, 0, 0); // - radius
            var drawPontLeft = new Vector3(bounds.center.x, bounds.min.y) + leftDisp; //- new Vector3(radius, 0, 0); ; // - radius

            Debug.DrawLine(prevPosLeft, drawPontLeft, Color.magenta);
            Debug.DrawLine(prevPosRight, drawPontRight, Color.magenta);

            prevPosLeft = drawPontLeft;
            prevPosRight = drawPontRight;

        }
    }

    public void TakeDamage()
    {
        health--;
        GameManager.Instance.uIManager.OnPlayerHpChanged(health);

        if (health <= 0)
        {
            body.velocity = Vector2.zero;
            body.gravityScale = 0;
            bCollider.enabled = false;
            animator.SetBool(Dead, true);
        }
    }

    public void AddHealth()
    {
        if (health < 3)
        {
            health++;
            GameManager.Instance.uIManager.OnPlayerHpChanged(health);
        }
    }

    public void AE_AfterDeath()
    {
        Debug.Log("throw screen");
        gameObject.SetActive(false);
    }   
 
    public void PlayCarrotSound()
    {
        sAudio.Play(carrot);
    }

    public void PlayStarSound()
    {
        sAudio.Play(star);
    }

    public void Save()
    {
        
    }

    public void Load()
    {
        
    }
}

public class InputReader
{
    public float xMovement { get; private set; }
    public float yMovement { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    public void ReadInput()
    {
        xMovement = Input.GetAxisRaw("Horizontal");
        yMovement = Input.GetAxisRaw("Vertical");
        JumpPressed = Input.GetButtonDown("Jump");
        InteractPressed = Input.GetButtonDown("Interact");
    }
}

public class PlayerLocomotionState : IState
{
    Player player;

    public PlayerLocomotionState()
    {
        player = Player.Instance;
    }

    public void FixedStateUpdate()
    {
        player.Move();
    }

    public void OnEnter()
    {
        player.body.sharedMaterial = null;
    }

    public void OnExit()
    {

    }

    public void StateUpdate()
    {
        player.TryJump();

        if (player.inputReader.InteractPressed)
        {
            player.currInteraction.Interact();
        }
    }
}

public class PlayerAirState : IState
{
    Player player;

    public PlayerAirState()
    {
        player = Player.Instance;
    }

    public void FixedStateUpdate()
    {
        player.Move();
    }

    public void OnEnter()
    {
        player.body.sharedMaterial = player.pMat;
    }

    public void OnExit()
    {

    }

    public void StateUpdate()
    {
        if (player.inputReader.InteractPressed)
        {
            player.currInteraction.Interact();
        }
    }
}

public class PlayerKnockBackState : IState
{
    Player player;
    float knockBackDir;
    float fallOffSpeed = 1f;
    float maxStateTime = .5f;
    float cMaxTime;

    public PlayerKnockBackState()
    {
        player = Player.Instance;
    }

    public void FixedStateUpdate()
    {
        player.body.velocity = new Vector2(knockBackDir, player.body.velocity.y - fallOffSpeed * Time.fixedDeltaTime);
    }

    public void OnEnter()
    {
        knockBackDir = player.knockBackDir * player.knockBackForce;      
        cMaxTime = maxStateTime;

        player.body.velocity = new Vector2(knockBackDir, player.knockBackForce);
    }

    public void OnExit()
    {

    }

    public void StateUpdate()
    {
        cMaxTime -= Time.deltaTime;

        if (cMaxTime <= 0)
        {
            player.knockedBack = false;
        }
        Debug.Log(knockBackDir);
    }
}

public class PlayerDeathState : IState
{
    Player player;

    public PlayerDeathState()
    {
        player = Player.Instance;
    }

    public void FixedStateUpdate()
    {

    }

    public void OnEnter()
    {
        Debug.Log("throw screen + death sound");        
    }

    public void OnExit()
    {

    }

    public void StateUpdate()
    {

    }
}

public class PlayerCrouchState : IState
{
    Player player;

    public PlayerCrouchState()
    {
        player = Player.Instance;
    }

    public void FixedStateUpdate()
    {

    }

    public void OnEnter()
    {
        player.body.velocity = new Vector2(0, player.body.velocity.y);
        player.normalCollider.enabled = false;
        player.crouchCollider.enabled = true;
    }

    public void OnExit()
    {
        player.normalCollider.enabled = true;
        player.crouchCollider.enabled = false;
    }

    public void StateUpdate()
    {
        player.TryJump();
    }
}

public class PlayerClimbState : IState
{
    Player player;

    float gravScale;
    public PlayerClimbState()
    {
        player = Player.Instance;
    }

    public void FixedStateUpdate()
    {
        player.body.velocity = new Vector2(0, player.inputReader.yMovement * player.climbSpeed);
    }

    public void OnEnter()
    {
        float displacement = player.transform.position.y > player.ladderCenter.position.y ? -.5f : .2f;
        gravScale = player.body.gravityScale;
        player.body.gravityScale = 0;
        player.body.velocity = Vector2.zero;
        player.transform.position = new Vector3(player.ladderCenter.position.x, player.transform.position.y + displacement, 0);
        
    }

    public void OnExit()
    {
        player.body.gravityScale = gravScale;
        player.ladderCenter = null;
    }  

    public void StateUpdate()
    {
        player.TryJump();
    }
}