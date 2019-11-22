using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlingingSlimeMonster : MonsterAI
{
    [Space(10)] [Header("Fling Settings")] public float flingSpeed;
    public float damageMultiplier;
    public float travelDist;
    public float rotationOffset;
    public float rotateAmt;
    
    [Header("donut touch")]
    public bool isFlying;
    private Vector3 startedAttackPos;
    private Vector3 targetPos;

    /// <summary>
    /// Plays the attack. This includes sync'ing the animator and sounds with dealing damage.
    /// If the player has moved out of range before the damage is dealt then the coroutine is ended early
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BasicAttack()
    {

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        animator.Play("SlimePrelaunch");
        yield return new WaitForSeconds(.75f);
        targetPos = currentTarget.transform.position;
        targetPos.z = transform.position.z;
        targetPos = transform.position + (targetPos - transform.position).normalized * (travelDist);
        isFlying = true;
        Color tmp = renderer.color;
        renderer.color = new Color(1, 0, 0);
        startedAttackPos = transform.position;
        animator.Play("Flying");
        if (targetPos.x < transform.position.x)
        {
            renderer.transform.Rotate(new Vector3(0, 0, -rotationOffset));
        }
        else
        {
            renderer.transform.Rotate(new Vector3(0, 0, rotationOffset));
        }
        GetComponent<BoxCollider2D>().isTrigger = true;
        while (!InBodyRangeOfTarget(targetPos))
        {
            if (!isFlying)
            {
                break;
            }

            Vector3 movementDirection = targetPos - transform.position;
            movementDirection.Normalize();
            Vector3 newPos = transform.position + movementDirection * flingSpeed * Time.deltaTime;
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            //Debug.Log(renderer.transform.rotation.z);
            //float newZ = renderer.transform.rotation.z + flingSpeed * Time.deltaTime;
            //renderer.transform.rotation = new Quaternion(renderer.transform.rotation.x, renderer.transform.rotation.y, renderer.transform.rotation.z + flingSpeed * Time.deltaTime, renderer.transform.rotation.w);
            if (targetPos.x < transform.position.x)
            {
                renderer.transform.Rotate(new Vector3(0, 0, rotateAmt));
            }
            else
            {
                renderer.transform.Rotate(new Vector3(0, 0, -rotateAmt));
            }


            yield return null;
        }
        GetComponent<BoxCollider2D>().isTrigger = false;
        animator.Play("Postlaunch");
        isFlying = false;
        renderer.transform.rotation = new Quaternion(0, 0, 0, renderer.transform.rotation.w);
        renderer.color = tmp;

        ShowTiredUI(true);
        yield return new WaitForSeconds(timeBetweenAttacks);
        ShowTiredUI(false);
        jabsDone = 0;
        renderer.flipX = (targetPos.x < transform.position.x);
        yield return new WaitForSeconds(1f);
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //GameObject target = GetComponent<ProjectileMovement>().targetObject;
        if (isFlying)
        {
            Attackable target = other.gameObject.GetComponent<Attackable>();

            if (target != null && target.team == CombatInfo.Team.Hero)
            {
                float distTraveled = Vector3.Distance(startedAttackPos, transform.position);
                basicAttackDamage = (int)(distTraveled * damageMultiplier) + 1;
                DoBasicAttack(target.gameObject);


            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                isFlying = false;

            }
        }

    }
}
