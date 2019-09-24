using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements a fighter's movement
/// </summary>
public class FighterMove : MonoBehaviour
{
    private Fighter fighter;
    private FighterAttack fighterAttack;
    public bool testing;
    public bool followingMoveOrder;

    private Transform target;
    private bool moveFighter;

    // Start is called before the first frame update
    void Start()
    {
        fighter = GetComponent<Fighter>();
        fighterAttack = GetComponent<FighterAttack>();
    }

    void FixedUpdate()
    {
        MoveFighter();
    }

    void MoveFighter ()
    {
        if (moveFighter)
        {
            if (!fighterAttack.InRangeOfTarget())
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
            moveFighter = true;
        }
    }

    public void StartMovingCommandHandle(Transform t)
    {

        followingMoveOrder = true;

        if (fighter == null)
        {
            Start();
        }

        target = t;
        moveFighter = true;

    }

    /// <summary>
    /// Tells FighterAttack that this fighter is close enough to attacks
    /// </summary>
    public void StopMoving()
    {
        moveFighter = false;
        fighterAttack.StartBasicAttacking();
    }

    public void StopMovingCommandHandle()
    {
        followingMoveOrder = false;
        moveFighter = false;
        fighterAttack.SetCurrentTarget();
    }
}
