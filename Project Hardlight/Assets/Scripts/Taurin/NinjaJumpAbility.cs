using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaJumpAbility : Ability
{

    public Buff invulnBuff;
    
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
            Attackable selectedAttackable = selectedTarget.GetComponent<Attackable>();
            FighterAttack thisFighterAttack = GetComponent<FighterAttack>();
            if (selectedAttackable.team == CombatInfo.Team.Enemy)
            {
                
                //do something different based on allight values
                Soul soul = GetComponent<Soul>();
                if (soul != null)
                {
                    foreach (AllightAttribute allight in soul.allightAttributes)
                    {
                        switch (allight.allightType)
                        {
                            case AllightType.SUNLIGHT:
                                //remember to scale based on allight.baseValue
                                break;
                            case AllightType.MOONLIGHT:
                                break;
                            case AllightType.STARLIGHT:
                                break;
                            default:
                                break;
                        }
                    }
                }

                //teleport ninja (play poof of smoke anim)
                Vector3 newPos = selectedAttackable.transform.position;
                newPos.x -= 0.9f;
                transform.position = newPos;

                //set ninja's target to the fighter it teleported to
                thisFighterAttack.SetIssuedCurrentTarget(selectedAttackable);

                //enemy takes danage
                selectedAttackable.TakeDamage(GetDamage());
                
                // Add invuln buff
                GetComponent<Attackable>().AddBuff(invulnBuff);
                
                if (gameObject.GetComponent<Fighter>().anim.HasState(0, Animator.StringToHash("Ability1")))
                {
                    Debug.Log("Ability1 anim is played");
                    gameObject.GetComponent<Fighter>().anim.Play("Ability1");
                }
                return true;
            }

        }
        return false;
    }

    public override float GetRange()
    {
        return this.baseEffectRange;
    }
}
