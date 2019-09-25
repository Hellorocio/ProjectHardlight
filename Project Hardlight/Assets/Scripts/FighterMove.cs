using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements a fighter's movement
/// </summary>
public class FighterMove : MonoBehaviour
{
    public enum MoveState {stopped, moving, paused}
    MoveState moveState = MoveState.stopped;
    private Fighter fighter;
    private FighterAttack fighterAttack;
    public bool testing;
    public bool followingMoveOrder;

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        fighter = GetComponent<Fighter>();
        fighterAttack = GetComponent<FighterAttack>();
    }

    private void OnTriggerStay2D(Collider2D other)
    { 
        Fighter collidedFighter = other.GetComponent<Fighter>();
        if (!GlobalSettings.overlapFighters && collidedFighter != null && moveState == MoveState.moving && 
                collidedFighter.team == fighter.team && ShouldFighterWait(collidedFighter))
        {
            moveState = MoveState.paused;
            print("moving: paused");
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
        if (moveState == MoveState.moving)
        {
            if (!fighterAttack.InRangeOfTarget(target, !followingMoveOrder))
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, fighter.GetSpeed() * Time.deltaTime);
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
        }
    }

    /// <summary>
    /// Called by FighterAttack when it gets a new currentTarget, starts moveLoop
    /// </summary>
    public void StartMoving (Transform t)
    {
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
        }
    }

    public void StartMovingCommandHandle(Transform t)
    {

        followingMoveOrder = true;
        fighterAttack.StopBasicAttacking();
        if (fighter == null)
        {
            Start();
        }

        target = t;
        moveState = MoveState.moving;

    }

    /// <summary>
    /// Tells FighterAttack that this fighter is close enough to attacks
    /// </summary>
    public void StopMoving()
    {
        moveState = MoveState.stopped;
        fighterAttack.StartBasicAttacking();
    }

    public void StopMovingCommandHandle()
    {
        followingMoveOrder = false;
        moveState = MoveState.stopped;
        Debug.Log("Deleting " + target.gameObject.name);
        Destroy(target.gameObject);
        target = null;
        fighterAttack.SetCurrentTarget();
    }


    /// <summary>
    /// If a fighter collided with another fighter, returns true if this fighter is in front
    /// Right now this assumes that enemies always go left and heros always go right, which is always the case
    /// </summary>
    /// <returns></returns>
    bool ShouldFighterWait (Fighter otherFighter)
    {
        bool fighterWait = false;

        if (fighter.team == CombatInfo.Team.Hero && fighter.transform.position.x < otherFighter.transform.position.x && !followingMoveOrder)
        {
            //for heros,  wait if other fighter's x position is greater
            fighterWait = true;
        }
        else if (fighter.team == CombatInfo.Team.Enemy && fighter.transform.position.x > otherFighter.transform.position.x)
        {
            //for enemies, wait if this other fighter's x position is less
            fighterWait = true;
        }

        return fighterWait;
    }
}
