using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All inputs should go int there
public class BattleManager : Singleton<BattleManager>
{
    public enum InputState { Unknown, Idle, Targeting}

    public BattleConfig battleConfig;

    public GameObject selectedHero;
    public Ability selectedAbility;
    public InputState inputState;

    public void Start()
    {
        selectedHero = null;
        inputState = InputState.Idle;
    }

    public void Update()
    {

        /////////////////// Idle
        if (inputState == InputState.Idle)
        {
            // M1
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Input.mousePosition;
                Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));

                bool clickedHeroFighter = false;

                if (hitCollider != null)
                {
                    Fighter clickedFighter = hitCollider.gameObject.GetComponent<Fighter>();
                    if (clickedFighter != null)
                    {
                        if (clickedFighter.team == CombatInfo.Team.Hero)
                        {
                            clickedHeroFighter = true;
                        }
                    }
                }

                if (clickedHeroFighter)
                {
                    SetSelectedHero(hitCollider.gameObject);
                }
                else
                {
                    DeselectHero();
                }
            }
            // Key 1 to select ability
            // TODO (Change to actually selecting an ability, keyboard for now)
            else if (Input.GetKeyDown(KeyCode.Alpha1) && selectedHero != null)
            {
                // Clear any existing selected ability
                selectedAbility = null;
                Ability ability = (Ability)selectedHero.GetComponent<HeroAbilities>().abilityList[0];
                if (ability != null)
                {
                    selectedAbility = ability;

                    // Start targeting
                    StartTargeting();
                }
            }
        }
        //////////////// Targeting
        else if (inputState == InputState.Targeting)
        {
            // M1
            if (Input.GetMouseButtonDown(0))
            {
                // Select target
                switch (selectedAbility.targetingType)
                {
                    case Targeting.Type.TargetPosition:
                        Vector3 mousePos = Input.mousePosition;
                        selectedAbility.selectedPosition = Camera.main.ScreenToWorldPoint(mousePos);
                        // TODO (mchi) left off here: Now write the ability and cast the ability with the position
                        break;
                    default:
                        break;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                StopTargeting();
            }
        }
    }

    public void StartTargeting()
    {

        Debug.Log("TARGETING | Started");

        inputState = InputState.Targeting;
        switch (selectedAbility.targetingType)
        {
            case Targeting.Type.TargetPosition:
                Debug.Log("TARGETING | Target Position");
                SetCursor(battleConfig.targetPositionCursor);
                break;
            case Targeting.Type.TargetUnit:
                Debug.Log("TARGETING | Target Unit");
                // TODO change cursor
                break;
            case Targeting.Type.Instant:
                // TODO check validity before casting ability
                break;
            default:
                break;
        }
    }

    public void StopTargeting()
    {
        Debug.Log("TARGETING | Stopped");

        inputState = InputState.Idle;
        selectedAbility = null;
        SetCursor(battleConfig.defaultCursor);
    }

    public void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void SetSelectedHero(GameObject hero)
    {
        if (selectedHero != null)
        {
            selectedHero.GetComponent<Fighter>().SetSelectedUI(false);
        }

        selectedHero = hero;
        selectedHero.GetComponent<Fighter>().SetSelectedUI(true);
    }

    public void DeselectHero()
    {
        if (selectedHero != null)
        {
            selectedHero.GetComponent<Fighter>().SetSelectedUI(false);
            selectedHero = null;
        }
    }
}
