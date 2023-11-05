using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public InputAction moveAction;
    public InputAction dodgeAction;
    public InputAction attackAction;

    public float playerSpeed = 0.1f;

    public enum PLAYER_STATE
    {
        IDLE,
        MOVING,
        DODGING,
        ATTACKING,
        DEAD
    }

    public PLAYER_STATE currentState = PLAYER_STATE.IDLE;

    public Animator playerAnimator;

    public float dodgeStartingVelocity = 2f;
    private Vector3 dodgeDestination;
    private Vector3 currentDodgeVelocity;

    public float dodgeLength = 2f;

    private float dodgeAnimDuration;

    public float dodgeAnimSpeed;
    
    private float dodgeAnimTimer = 0f;

    private float attackAnimHitboxActiveStart;

    private float attackAnimHitboxActiveEnd;

    public BoxCollider attackHitBox;

    private float attackAnimDuration;

    public float attackAnimSpeed;

    private float attackAnimTimer = 0f;

    private Vector3 RIGHT_VECTOR = new Vector3(1, 0, -1);
    private Vector3 UP_VECTOR = new Vector3(1, 0, 1);


    void Awake()
    {
        dodgeAction.performed += ctx => { this.OnDodge(ctx); };
        attackAction.performed += ctx => { this.OnAttack(ctx); };

        AnimationClip[] clips = playerAnimator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {

            if (clip.name.Contains("Roll"))
            {
                this.dodgeAnimDuration = clip.length / this.dodgeAnimSpeed;
            }
            else if (clip.name.Contains("Attack"))
            {
                this.attackAnimDuration = clip.length / this.attackAnimSpeed;
                this.attackAnimHitboxActiveStart = this.attackAnimDuration * 5 / 8;
                this.attackAnimHitboxActiveEnd = this.attackAnimDuration * 63 / 80;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveAmount = this.moveAction.ReadValue<Vector2>();

        if (this.currentState == PLAYER_STATE.ATTACKING)
        {
            this.attackAnimTimer += Time.deltaTime;

            if (!this.attackHitBox.enabled && this.attackAnimTimer >= this.attackAnimHitboxActiveStart)
            {
                this.attackHitBox.enabled = true;
            }

            if (this.attackHitBox.enabled && this.attackAnimTimer >= this.attackAnimHitboxActiveEnd)
            {
                this.attackHitBox.enabled = false;
            }

            if (this.attackAnimTimer >= this.attackAnimDuration)
            {
                //animation is done, move back to idle
                this.currentState = PLAYER_STATE.IDLE;
            }
        }

        if (this.currentState == PLAYER_STATE.DODGING)
        {
            this.dodgeAnimTimer += Time.deltaTime;

            transform.position = Vector3.SmoothDamp(transform.position, this.dodgeDestination, ref this.currentDodgeVelocity, this.dodgeAnimDuration);
            if (this.dodgeAnimTimer >= this.dodgeAnimDuration)
            {
                //animation is done, move back to idle
                this.currentState = PLAYER_STATE.IDLE;
            }
        }

        if (moveAmount.magnitude > 0 && 
            (this.currentState == PLAYER_STATE.IDLE || this.currentState == PLAYER_STATE.MOVING))
        {
            this.currentState = PLAYER_STATE.MOVING;
            if (moveAmount.magnitude > 1)
            {
                moveAmount = moveAmount.normalized;
            }
            Vector3 moveVector = (moveAmount.x * RIGHT_VECTOR + moveAmount.y * UP_VECTOR).normalized;

            transform.Translate(moveVector * this.playerSpeed * Time.deltaTime, relativeTo: Space.World);
            transform.LookAt(transform.position + moveVector);
        }
        else if (moveAmount.magnitude == 0 && this.currentState == PLAYER_STATE.MOVING)
        {
            this.currentState = PLAYER_STATE.IDLE;
        }

        //update the player animator with the current state
        this.playerAnimator.SetInteger("currentState", (int)this.currentState);
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        this.currentState = PLAYER_STATE.DODGING;
        this.dodgeAnimTimer = 0f;

        this.dodgeDestination = transform.position + transform.forward * this.dodgeLength;
        this.currentDodgeVelocity = transform.forward.normalized * this.dodgeStartingVelocity;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        this.currentState = PLAYER_STATE.ATTACKING;
        this.attackAnimTimer = 0f;
    }

    public void OnEnable()
    {
        moveAction.Enable();
        dodgeAction.Enable();
        attackAction.Enable();
    }

    public void OnDisable()
    {
        moveAction.Disable();
        dodgeAction.Disable();
        attackAction.Disable();
    }
}
