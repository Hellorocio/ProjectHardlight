using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeOnionMonster : MonsterAI
{
    public int numProjectilesToSpawn;
    public bool spawnAllAtOnce = false;
    /// <summary>
    /// Plays the attack. This includes sync'ing the animator and sounds with dealing damage.
    /// If the player has moved out of range before the damage is dealt then the coroutine is ended early
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BasicAttack()
    {
        int locIndex = 0;
        List<Vector3> locList = GenerateProjectileLocs();
        while (locIndex < locList.Count)
        {
            Vector3 nextLoc = locList[locIndex];
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

            //if (currentTarget == null || !InBasicRangeOfTarget(currentTarget.transform.position))
            //{
            //    StopBasicAttacking();
            //}

            yield return new WaitForSeconds(realBasicAttackHitTime);
            if (spawnAllAtOnce)
            {
                for(int i = 0; i < locList.Count; i++)
                {
                    spawnPoint.transform.localPosition = locList[i] * .4f;
                    gameObject.GetComponentInChildren<SpriteRenderer>().flipX = (spawnPoint.transform.position.x < transform.position.x);
                    GameObject basicAttackProjectile = Instantiate(basicAttackPrefab);
                    basicAttackProjectile.transform.position = spawnPoint.transform.position;
                    basicAttackProjectile.GetComponent<GenericRangedMonsterProjectile>().damage = basicAttackDamage;
                    basicAttackProjectile.GetComponent<ProjectileMovement>().speed = basicAttackProjectileSpeed;
                    basicAttackProjectile.GetComponent<ProjectileMovement>().SetTarget(locList[i] * 200f);
                }
                locIndex = locList.Count;
                yield return new WaitForSeconds(basicAttackClip.length / basicAttackClipSpeedMultiplier - realBasicAttackHitTime);
            } else
            {
                
                for (int i = 0; i < locList.Count; i++)
                {
                    gameObject.GetComponentInChildren<SpriteRenderer>().flipX = (locList[i].x < transform.position.x);
                    spawnPoint.transform.localPosition = locList[i] * .4f;
                    gameObject.GetComponentInChildren<SpriteRenderer>().flipX = (spawnPoint.transform.position.x < transform.position.x);
                    GameObject basicAttackProjectile = Instantiate(basicAttackPrefab);
                    basicAttackProjectile.transform.position = spawnPoint.transform.position;
                    basicAttackProjectile.GetComponent<GenericRangedMonsterProjectile>().damage = basicAttackDamage;
                    basicAttackProjectile.GetComponent<ProjectileMovement>().speed = basicAttackProjectileSpeed;
                    basicAttackProjectile.GetComponent<ProjectileMovement>().SetTarget(locList[i] * 200f);
                    yield return new WaitForSeconds(1f);
                }
                locIndex = locList.Count;
            }
            
            /*
            if (currentTarget != null && InBasicRangeOfTarget(currentTarget.transform.position) && moveState == MoveState.basicAttacking)
            {
                gameObject.GetComponentInChildren<SpriteRenderer>().flipX = (currentTarget.transform.position.x < transform.position.x);
                spawnPoint.transform.localPosition = (currentTarget.transform.position - transform.position).normalized * .2f;
                GameObject basicAttackProjectile = Instantiate(basicAttackPrefab);
                basicAttackProjectile.transform.position = spawnPoint.transform.position;
                basicAttackProjectile.GetComponent<GenericRangedMonsterProjectile>().damage = basicAttackDamage;
                basicAttackProjectile.GetComponent<ProjectileMovement>().speed = basicAttackProjectileSpeed;
                basicAttackProjectile.GetComponent<ProjectileMovement>().SetTarget(currentTarget.transform.position);
                //DoBasicAttack(currentTarget);
            }
            */
            
            locIndex++;
        }
        ShowTiredUI(true);
        yield return new WaitForSeconds(timeBetweenAttacks);
        ShowTiredUI(false);
        jabsDone = 0;
        moveState = MoveState.stopped;
        attackCoroutine = null;
    }

    public List<Vector3> GenerateProjectileLocs()
    {
        List<Vector3> locs = new List<Vector3>();
        float randy = Random.value * 2 * Mathf.PI;
        for(int i = 0; i < numProjectilesToSpawn; i++)
        {
            float theta = (2 * Mathf.PI / numProjectilesToSpawn) * i + randy;
            Vector3 tmp = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), transform.position.z);
            if (!spawnAllAtOnce)
            {
                int randPlacer = Random.Range(0, locs.Count);
                locs.Insert(randPlacer, tmp);
            } else
            {
                locs.Add(tmp);
            }
            
        }
        return locs;
    }
}
