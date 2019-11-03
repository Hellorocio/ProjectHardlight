using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaJumpAbility : Ability
{

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
                Vector3 newPos = selectedFighter.transform.position;
                newPos.x -= 0.9f;
                transform.position = newPos;

                //set ninja's target to the fighter it teleported to
                thisFighter.SetIssuedCurrentTarget(selectedFighter);

                //enemy takes tamage
                selectedFighter.TakeDamage(GetDamage());
                if (gameObject.GetComponent<Fighter>().anim.HasState(0, Animator.StringToHash("Ability1")))
                {
                    Debug.Log("Ability1 anim is played");
                    gameObject.GetComponent<Fighter>().anim.Play("Ability1");
                }
                return true;
            }


            GenericMonsterAI monster = selectedTarget.GetComponent<GenericMonsterAI>();

            if (monster != null)
            {
                Vector3 newPos = monster.transform.position;
                newPos.x -= 0.9f;
                transform.position = newPos;

                //set ninja's target to the fighter it teleported to
                thisFighter.SetIssuedCurrentTarget(monster);

                //enemy takes tamage
                monster.TakeDamage(GetDamage());
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
