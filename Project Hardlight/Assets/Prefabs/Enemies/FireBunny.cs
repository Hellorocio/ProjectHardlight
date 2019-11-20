using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBunny : MonsterAI
{
    // TODO make into buff
    public float enragedAttackSpeedMultiplier = 2.0f;
    public float enragedAttackDamageMultiplier = 2.0f;
    public float enragedMovementSpeedMultiplier = 2.0f;
    public float enragedRangeMultiplier = 2.0f;
    public Color enragedColor;
    
    // TODO can show this explodying around
    public GameObject explosion;

    [Header("Donut touch")]
    public bool enraged = false;
    public override void BehaviorUpdate()
    {
        UpdateTarget();

        if (anyValidTargets)
        {
            
            // Enrage if attacking Vessel: Gain attack speed and movement speed
            if (!enraged && currentTarget.GetComponent<Fighter>() != null)
            {
                Enrage();
            } else if (enraged && currentTarget.GetComponent<Fighter>() == null)
            {
                Unenrage();
            }
            
            // ATTACK!
            DecideAttack();
        }
        
    }

    public override IEnumerator BasicAttack()
    {
        while (jabsDone < numJabsInAttack)
        {
            jabsDone++;
            int nameHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            while (nameHash == Animator.StringToHash("Attack"))
            {
                yield return new WaitForEndOfFrame();
            }
            animator.Play("BasicAttack");

            yield return new WaitForSeconds(realBasicAttackHitTime);
            if (audioSource != null && basicAttackSfx != null)
            {
                audioSource.clip = basicAttackSfx;
                audioSource.Play();
            }

            if (InBasicRangeOfTarget(currentTarget.transform.position) && moveState == MoveState.basicAttacking)
            {
                DoBasicAttack(currentTarget);
            }
            else
            {
                StopBasicAttacking();
            }
            
            yield return new WaitForSeconds(basicAttackClip.length/basicAttackClipSpeedMultiplier - realBasicAttackHitTime);
        }
        ShowTiredUI(true);
        yield return new WaitForSeconds(timeBetweenAttacks);
        ShowTiredUI(false);
        jabsDone = 0;
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }

    public void Enrage()
    {
        Debug.Log("Enraged");
        timeBetweenAttacks /= enragedAttackSpeedMultiplier;
        basicAttackDamage = (int) (basicAttackDamage * enragedAttackDamageMultiplier);
        moveSpeed *= enragedMovementSpeedMultiplier;
        basicAttackRange *= enragedRangeMultiplier;
        enraged = true;

        explosion.SetActive(true);
    }

    public void Unenrage()
    {
        // Unenrage if attacking non-Vessel
        Debug.Log("Unenrage");
        timeBetweenAttacks *= enragedAttackSpeedMultiplier;
        basicAttackDamage = (int) (basicAttackDamage / enragedAttackDamageMultiplier);
        moveSpeed /= enragedMovementSpeedMultiplier;
        basicAttackRange /= enragedRangeMultiplier;
        enraged = false;

        explosion.SetActive(false);
    }
}