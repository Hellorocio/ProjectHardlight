using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlingingSlimeMonster : MonsterAI
{
    [Space(10)]

    [Header("Fling Settings")]
    public float flingSpeed;
    public float damageMultiplier;
    private bool isFlying;
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
        targetPos = transform.position + (targetPos - transform.position).normalized * (basicAttackRange+1);
        isFlying = true;
        Color tmp = renderer.color;
        renderer.color = new Color(1, 0, 0);
        startedAttackPos = transform.position;
        animator.Play("Flying");
        if (targetPos.x < transform.position.x)
        {
            renderer.transform.Rotate(new Vector3(0, 0, -12f));
        }
        else
        {
            renderer.transform.Rotate(new Vector3(0, 0, 12f));
        }
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
            if(targetPos.x < transform.position.x)
            {
                renderer.transform.Rotate(new Vector3(0, 0, .5f));
            } else
            {
                renderer.transform.Rotate(new Vector3(0, 0, -.5f));
            }
            
            
            yield return null;
        }
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
            Attackable target = other.GetComponent<Attackable>();

            if (target != null && target.team == CombatInfo.Team.Hero)
            {
                float distTraveled = Vector3.Distance(startedAttackPos, transform.position);
                basicAttackDamage = (int)(distTraveled * damageMultiplier) + 1;
                DoBasicAttack(target.gameObject);
                isFlying = false;
                Vector3 dir = targetPos - transform.position;
                dir.Normalize();
                Vector3 lastPos = transform.position - dir * 2;
                transform.position = new Vector3(lastPos.x, lastPos.y, transform.position.z);

            }
        }

    }
}
