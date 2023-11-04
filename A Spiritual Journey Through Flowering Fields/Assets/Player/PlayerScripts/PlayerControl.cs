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

    public float dodgeAnimDuration = 1f;

    public float attackAnimDuration = 0.4f;

    public float attackAnimHitboxActiveStart = 0.25f;

    public float attackAnimHitboxActiveEnd = 0.325f;

    public BoxCollider attackHitBox;

    private float attackAnimTimer = 0f;

    private float dodgeAnimTimer = 0f;

    void Awake()
    {
        dodgeAction.performed += ctx => { this.OnDodge(ctx); };
        attackAction.performed += ctx => { this.OnAttack(ctx); };
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

            transform.Translate(new Vector3(moveAmount.x, 0, moveAmount.y) * this.playerSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(moveAmount.x, transform.position.y, moveAmount.y));
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
