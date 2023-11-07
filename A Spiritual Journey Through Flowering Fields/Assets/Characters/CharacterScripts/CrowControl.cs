using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowControl : MonoBehaviour
{
    public Animator crowAnimator;

    public float moveSpeed;
    public float attackAnimSpeed;

    public float waitBeforeAttackingOnSpawn = 3;
    private float waitBeforeAttackingTimer;

    public float attackRange = 0.5f;

    private PlayerControl playerControl;
    private GameObject player;

    private bool attackMode = false;
    
    private float attackTimer;
    private float attackAnimDuration;
    private float attackAnimHitboxActiveStart;
    private float attackAnimHitboxActiveEnd;

    public BoxCollider attackAnimHitbox;

    public enum CROW_STATE {
        IDLE,
        ATTACKING,
        DEAD
    }

    public CROW_STATE currentState = CROW_STATE.IDLE;

    void Awake() {
        AnimationClip[] clips = crowAnimator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains("Attack"))
            {
                this.attackAnimDuration = clip.length / this.attackAnimSpeed;
                this.attackAnimHitboxActiveStart = this.attackAnimDuration * 5 / 8;
                this.attackAnimHitboxActiveEnd = this.attackAnimDuration * 63 / 80;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.FindGameObjectsWithTag("Player")[0];
        this.playerControl = this.player.GetComponent<PlayerControl>();
        this.waitBeforeAttackingTimer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        if (this.waitBeforeAttackingTimer > this.waitBeforeAttackingOnSpawn && !this.playerControl.beingAttacked) {
            this.attackMode = true;
            this.playerControl.beingAttacked = true;
        }
        else {
            this.waitBeforeAttackingTimer += Time.deltaTime;
        }

        if (this.attackMode && this.currentState != CROW_STATE.ATTACKING) {
            if ((this.player.transform.position - this.transform.position).magnitude <= this.attackRange) {
                //trigger an attack
                this.currentState = CROW_STATE.ATTACKING;
                this.attackTimer = 0f;
            }
            else {
                //too far, start moving toward player.
                Vector3 moveVector = (this.player.transform.position - transform.position).normalized;
                transform.Translate(moveVector * this.moveSpeed * Time.deltaTime, relativeTo: Space.World);
                transform.LookAt(transform.position + -moveVector);
            }
        }
        else if (this.currentState == CROW_STATE.ATTACKING) {

            this.attackTimer += Time.deltaTime;
            if (this.attackTimer >= this.attackAnimDuration) {
                this.currentState = CROW_STATE.IDLE;
            }
            else if (this.attackTimer >= this.attackAnimHitboxActiveEnd) {
                this.attackAnimHitbox.enabled = false;
            }
            else if (this.attackTimer >= this.attackAnimHitboxActiveStart){
                this.attackAnimHitbox.enabled = true;
            }
        }

        //update the animator
        this.crowAnimator.SetInteger("currentState", (int)this.currentState);
    }

    public void OnHit() {
        if (this.attackMode) {
            this.playerControl.beingAttacked = false;
        }
        GameObject.FindGameObjectsWithTag("WorldControl")[0].GetComponent<WorldControl>().CrowKilled();
        Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            this.playerControl.OnHit();
        }
    }

}
