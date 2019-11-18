using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements a fighter's movement
/// </summary>
public class FighterMove : MonoBehaviour
{
    public enum MoveState {stopped, moving, paused}
    public MoveState moveState = MoveState.stopped;
    private Fighter fighter;
    private FighterAttack fighterAttack;
    public bool followingMoveOrder;
    [HideInInspector]
    public SpriteRenderer sprite;
    

    private Transform target;
    LineRenderer targetLine;

    // Start is called before the first frame update
    void Start()
    {
        fighter = GetComponent<Fighter>();
        fighterAttack = GetComponent<FighterAttack>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D other)
    { 
        Fighter collidedFighter = other.GetComponent<Fighter>();
        if (!GlobalSettings.overlapFighters && collidedFighter != null && moveState == MoveState.moving && 
                collidedFighter.GetComponent<Attackable>().team == GetComponent<Attackable>().team && ShouldFighterWait(collidedFighter))
        {
            moveState = MoveState.paused;

            //print("moving: paused");
        }
    }

    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (moveState == MoveState.paused)
        {
            moveState = MoveState.moving;
        }
    }

    void FixedUpdate()
    {
        MoveFighter();

    }

    void MoveFighter ()
    {
        //Debug.Log("Current obj is " + gameObject.name + " | moveState is " + moveState);
        //Debug.Log("Game obj is " + gameObject.name + " | current target is null? = " + (fighterAttack.currentTarget == null));
        if (moveState == MoveState.moving)
        {

            if (target != null && !fighterAttack.InRangeOfTarget(target, !followingMoveOrder))
            {
                //update fighter position
                transform.position = Vector3.MoveTowards(transform.position, target.position, fighter.GetSpeed() * Time.deltaTime);
                
                //update line position
                if (targetLine != null)
                {
                    targetLine.SetPosition(1, transform.position);
                }
            }
            else
            {
                if (followingMoveOrder)
                {
                    StopMovingCommandHandle();
                } else
                {
                    StopMoving();
                }
            }
        } else
        {
            if (fighter.anim != null)
            {
                int nameHash = fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
                if (fighter.anim.HasState(0, Animator.StringToHash("Idle")) && (nameHash == Animator.StringToHash("Walk")))
                {
                    fighter.anim.Play("Idle");
                }
            }
        }
    }

    /// <summary>
    /// Called by FighterAttack when it gets a new currentTarget, starts moveLoop
    /// </summary>
    public void StartMoving (Transform t)
    {
        //Debug.Log(t.name + " is new target in SM");
        if (!followingMoveOrder)
        {
            //Debug.Log("Recieved Start Moving Transform: " + t.name);
            //make sure everything has been initialized
            if (fighter == null)
            {
                Start();
            }

            target = t;
            moveState = MoveState.moving;
            /*
             * && (fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Ability1")) ||
                fighter.anim.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Ability2")
             * */
            if (fighter.anim.HasState(0, Animator.StringToHash("Walk")))
            {
                fighter.anim.Play("Walk");
            }
            
            TurnToFace();
        }
    }

    public void StartMovingCommandHandle(Transform t)
    {
        //Debug.Log(t.name + " is new target in SMCH");
        //remove any old targets
        if (followingMoveOrder)
        {
            
            Destroy(target.gameObject);
        }
        //set new target
        followingMoveOrder = true;
        if (fighter == null)
        {
            Start();
        }
        //fighterAttack.StopBasicAttacking(); // prevent exploit with move a bunch to get lots of basic attacks

        target = t;
        moveState = MoveState.moving;
        if (fighter.anim.HasState(0, Animator.StringToHash("Walk")))
        {
            fighter.anim.Play("Walk");
        }

        targetLine = target.GetComponentInChildren<LineRenderer>();

       // if (fighter.anim.HasState(0, Animator.StringToHash("Walk")))
       // {
         //   fighter.anim.Play("Walk");
       // }
        
        TurnToFace();

    }

    /// <summary>
    /// Tells FighterAttack that this fighter is close enough to attacks
    /// </summary>
    public void StopMoving()
    {
        moveState = MoveState.stopped;
        if (fighter.anim.HasState(0, Animator.StringToHash("Idle")))
        {
            fighter.anim.Play("Idle");
        }

        fighterAttack.StartBasicAttacking();
    }

    /// <summary>
    /// Sorry the parameter here is hacky, for some reason this was causing the anim to idle when walking is interrupted
    /// Even though start moving is called after and plays the walking anim. I know, weird.
    /// </summary>
    /// <param name="playIdle"></param>
    public void StopMovingCommandHandle(bool playIdle = true)
    {
        followingMoveOrder = false;
        moveState = MoveState.stopped;
        if (fighter.anim.HasState(0, Animator.StringToHash("Idle")) && playIdle)
        {
            fighter.anim.Play("Idle");
        }
        //Debug.Log("Deleting " + target.gameObject.name);
        //Debug.Log(target.name + " is getting destroyed!");
        if(target != null && target.name == "MoveLoc(Clone)")
        {
            Destroy(target.gameObject);
        }
        
        target = null;

        if (BattleManager.Instance.battleStarted)
        {
            fighterAttack.StartBasicAttacking();
        }        
    }


    /// <summary>
    /// If a fighter collided with another fighter, returns true if this fighter is in front
    /// Right now this assumes that enemies always go left and heros always go right, which is always the case
    /// </summary>
    /// <returns></returns>
    bool ShouldFighterWait (Fighter otherFighter)
    {
        
        bool fighterWait = false;

        if ((!sprite.flipX && fighter.transform.position.x < otherFighter.transform.position.x) ||
               (sprite.flipX && fighter.transform.position.x > otherFighter.transform.position.x))
        {
            //if ((fighter.team == CombatInfo.Team.Hero && !followingMoveOrder) || fighter.team == CombatInfo.Team.Enemy)
            if (fighter.GetComponent<Attackable>().team == CombatInfo.Team.Enemy) //heros never wait
            {
                fighterWait = true;
            }
        }

        return fighterWait;
    }

    /// <summary>
    /// Flips the fighter's appearence based on direction its going
    /// </summary>
    public void TurnToFace ()
    {
        sprite.flipX = (target.position.x < transform.position.x);
    }

    /// <summary>
    /// Accessor used by fighterAttack
    /// </summary>
    /// <returns></returns>
    public MoveState GetMoveState ()
    {
        return moveState;
    }
}
