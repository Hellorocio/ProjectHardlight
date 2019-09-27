using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaJumpAbility : Ability
{
    public float baseEffectRange;

    public GameObject attackTargetUnit;

    private bool targeting;


    public override bool StartTargeting()
    {
        targeting = true;
        return true;
    }

    public override void StopTargeting()
    {
        targeting = false;
    }

    public override bool DoAbility()
    {
        if (selectedTarget != null)
        {
            Debug.Log("Ninja jump ability");
            Fighter selectedFighter = selectedTarget.GetComponent<Fighter>();
            FighterAttack thisFighter = GetComponent<FighterAttack>();
            if (selectedFighter != null && selectedFighter.team == CombatInfo.Team.Enemy)
            {
                //teleport ninja (play poof of smoke anim)
                Vector3 newPos = selectedFighter.transform.position;
                newPos.x -= 0.9f;
                transform.position = newPos;

                //set ninja's target to the fighter it teleported to
                thisFighter.SetIssuedCurrentTarget(selectedFighter);

                //enemy takes tamage
                selectedFighter.TakeDamage(GetDamage());

                return true;
            }
        }
        return false;
    }

    public float GetRange()
    {
        return baseEffectRange;
    }
}
