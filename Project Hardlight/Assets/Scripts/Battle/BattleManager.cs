using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All inputs should go int there
public class BattleManager : Singleton<BattleManager>
{
    public enum BattleState { Unknown, Battling, Targeting}

    public GameObject selectedHero;
    public Ability selectedAbility;
    public BattleState battleState;

    public void Start()
    {
        selectedHero = null;
        battleState = BattleState.Battling;
    }

    public void Update()
    {

        // TODO Move unit selection here

        // Select target
        if (battleState == BattleState.Targeting)
        {
            switch (selectedAbility.targetingType)
            {
                case Targeting.Type.TargetPosition:
                    selectedAbility.selectedPosition = Camera.main.ScreenToWorldPoint(pos);
                    // TODO (mchi) left off here: Now write the ability and cast the ability with the position
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && selectedHero != null)
        {
            // Clear any existing selected ability
            selectedAbility = null;

            Ability ability = (Ability) selectedHero.GetComponent<HeroAbilities>().abilityList[0];
            if (ability != null)
            {
                selectedAbility = ability;
                Debug.Log(selectedAbility.abilityName);
                Debug.Log(selectedAbility.targetingType);
                StartTargeting();
            }
        }
    }

    public void StartTargeting()
    {
        battleState = battleState.Targeting;

        Debug.Log("Starting Targeting");
        switch(selectedAbility.targetingType)
        {
            case Targeting.Type.TargetPosition:
                Debug.Log("Targeting Position");
                // TODO change cursor
                break;
            case Targeting.Type.TargetUnit:
                Debug.Log("Targeting Unit");
                // TODO change cursor
                break;
            default:
                break;
        }
    }

    public void SetSelectedHero(GameObject hero)
    {
        selectedHero = hero;
    }
}
