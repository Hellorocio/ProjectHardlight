using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerrhaBasicAttack : BasicAttackAction
{

    public GameObject gooPrefab;
    public float animationDelay;
    private Coroutine basicAttack;

    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        if (basicAttack == null)
        {
            basicAttack = StartCoroutine(BasicAttackWithAnimationDelay(sourceFighter, target));
        }

    }

    IEnumerator BasicAttackWithAnimationDelay(Fighter sourceFighter, GameObject target)
    {
        
        yield return new WaitForSeconds(animationDelay);
        GameObject gooAttack = Instantiate(gooPrefab);
        gooAttack.transform.parent = target.transform.Find("Appearance");
        gooAttack.transform.localPosition = Vector3.zero;

        float damage = sourceFighter.GetBasicAttackDamage();
        Attackable attackable = target.GetComponent<Attackable>();

        attackable.TakeDamage(damage);
        sourceFighter.GainMana(10);
        basicAttack = null;
    }
}
