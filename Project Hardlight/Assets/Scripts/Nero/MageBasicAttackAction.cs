using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBasicAttackAction : BasicAttackAction
{

    public GameObject mageBasicAttackPrefab;
    public GameObject spawnPoint;
    // Used for offsetting to match animation
    public float animationDelay;

    public void Start()
    {
    }

    public override void DoBasicAttack(Fighter thisFighter, GameObject target)
    {
        StartCoroutine(BasicAttackWithAnimationDelay(target));
    }
    
    IEnumerator BasicAttackWithAnimationDelay(GameObject target)
    {
        // TODO(mchi) scale animation delay to attack speed changes
        yield return new WaitForSeconds(animationDelay);
        
        GameObject mageBasicAttack = Instantiate(mageBasicAttackPrefab);
        Debug.Log(mageBasicAttack.name);
        mageBasicAttack.transform.position = spawnPoint.transform.position;
        mageBasicAttack.GetComponent<MageBasicAttackProjectile>().SetSource(GetComponent<Fighter>());
        ProjectileMovement projectile = mageBasicAttack.GetComponent<ProjectileMovement>();
        projectile.SetTarget(target);
        projectile.StartMovement();

        if (target != null)
        {
            //mageBasicAttack.GetComponent<ProjectileMovement>().SetTarget(gameObject, target);
        }
        yield break;
    }
}
