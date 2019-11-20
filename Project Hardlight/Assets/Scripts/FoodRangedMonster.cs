using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodRangedMonster : MonsterAI
{

    public int numProjectilesToSpawn;
    public float timeBetweenProjectiles;
    /// <summary>
    /// Plays the attack. This includes sync'ing the animator and sounds with dealing damage.
    /// If the player has moved out of range before the damage is dealt then the coroutine is ended early
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BasicAttack()
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
            gameObject.GetComponentInChildren<SpriteRenderer>().flipX = (currentTarget.transform.position.x < transform.position.x);
            spawnPoint.transform.localPosition = (currentTarget.transform.position - transform.position).normalized * .2f;
            for(int i = 0; i < numProjectilesToSpawn; i++)
            {
                GameObject basicAttackProjectile = Instantiate(basicAttackPrefab);
                basicAttackProjectile.transform.position = spawnPoint.transform.position;
                basicAttackProjectile.GetComponent<GenericRangedMonsterProjectile>().damage = basicAttackDamage;
                basicAttackProjectile.GetComponent<ProjectileMovement>().speed = basicAttackProjectileSpeed;
                basicAttackProjectile.GetComponent<ProjectileMovement>().SetTarget(currentTarget.transform.position);
                yield return new WaitForSeconds(timeBetweenProjectiles);
            }
                
            //DoBasicAttack(currentTarget);
        }
        else
        {
            StopBasicAttacking();
        }
        
        
        ShowTiredUI(true);
        yield return new WaitForSeconds(timeBetweenAttacks);
        ShowTiredUI(false);
        jabsDone = 0;
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }


   

}
