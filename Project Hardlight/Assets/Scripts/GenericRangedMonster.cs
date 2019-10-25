using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRangedMonster : MonsterAI
{
   

    /// <summary>
    /// Plays the attack. This includes sync'ing the animator and sounds with dealing damage.
    /// If the player has moved out of range before the damage is dealt then the coroutine is ended early
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BasicAttack()
    {
        while (jabsDone < numJabsInAttack)
        {
            jabsDone++;
            animator.Play("BasicAttack");
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null && basicAttackSfx != null)
            {
                audioSource.clip = basicAttackSfx;
                audioSource.Play();
            }
            else
            {
                Debug.Log("No valid audioSource or sfx for this enemy's attack!!");
            }

            if (currentTarget == null || !InBasicRangeOfTarget(currentTarget.transform.position))
            {
                StopBasicAttacking();
            }

            yield return new WaitForSeconds(realBasicAttackHitTime);

            if (currentTarget != null && InBasicRangeOfTarget(currentTarget.transform.position) && moveState == MoveState.basicAttacking)
            {
                GameObject basicAttackProjectile = Instantiate(basicAttackPrefab);
                basicAttackProjectile.transform.position = spawnPoint.transform.position;
                basicAttackProjectile.GetComponent<GenericRangedMonsterProjectile>().damage = basicAttackDamage;
                basicAttackProjectile.GetComponent<ProjectileMovement>().speed = basicAttackProjectileSpeed;
                basicAttackProjectile.GetComponent<ProjectileMovement>().SetTarget(currentTarget);
                //DoBasicAttack(currentTarget);
            }
            else
            {
                StopBasicAttacking();
            }
            yield return new WaitForSeconds(basicAttackClip.length / basicAttackClipSpeedMultiplier - realBasicAttackHitTime);
        }
        yield return new WaitForSeconds(timeBetweenAttacks);
        jabsDone = 0;
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }


   

}
