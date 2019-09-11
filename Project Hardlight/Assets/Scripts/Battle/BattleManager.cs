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

    public GameObject commandsUI;

    public bool commandIsSettingNewTarget = false;
    public bool commandIsUsingAbility1 = false;
    public bool commandIsUsingAbility2 = false;

    public void Start()
    {
        selectedHero = null;
        inputState = InputState.Idle;
    }

    public void Update()
    {
        if(selectedHero != null && !selectedHero.activeSelf)
        {
            selectedHero = null;
        }
        /////////////////// Idle
        if (inputState == InputState.Idle)
        {
            // M1
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Input.mousePosition;
                Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));
                if ((hitCollider != null && hitCollider.gameObject.layer == 9) || commandIsSettingNewTarget) // if click hit the commandUI element, don't do anything
                {

                }
                else
                {
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
            }
            // Key 1 to select ability
            // TODO (Change to actually selecting an ability, keyboard for now)
            else if ((Input.GetKeyDown(KeyCode.Alpha1) || commandIsUsingAbility1)&& selectedHero != null)
            {
                commandIsUsingAbility1 = false;
                if (selectedAbility != null)
                {
                    StopTargeting();
                }

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
            else if ((Input.GetKeyDown(KeyCode.Alpha2) || commandIsUsingAbility2) && selectedHero != null)
            {
                commandIsUsingAbility2 = false;
                if (selectedAbility != null)
                {
                    StopTargeting();
                }

                // Clear any existing selected ability
                selectedAbility = null;
                Ability ability = (Ability)selectedHero.GetComponent<HeroAbilities>().abilityList[1];
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
                        {
                            Vector3 mousePos = Input.mousePosition;
                            selectedAbility.selectedPosition = Camera.main.ScreenToWorldPoint(mousePos);
                            if (selectedAbility.DoAbility())
                            {
                                StopTargeting();
                                DeselectHero();
                            }
                            // TODO (mchi) Neutral sound on failure
                        }
                        break;
                    case Targeting.Type.TargetUnit:// M1
                        {
                            Vector3 pos = Input.mousePosition;
                            Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));
                            bool clickedEnemyFighter = false;

                            if (hitCollider != null)
                            {
                                Fighter clickedFighter = hitCollider.gameObject.GetComponent<Fighter>();
                                if (clickedFighter != null)
                                {
                                    if (clickedFighter.team == CombatInfo.Team.Enemy)
                                    {
                                        clickedEnemyFighter = true;
                                    }
                                }
                            }

                            if (clickedEnemyFighter)
                            {
                                selectedAbility.selectedTarget = hitCollider.gameObject;
                            }
                        }
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
                SetCursor(battleConfig.targetUnitCursor);
                // TODO change cursor
                break;
            case Targeting.Type.Instant:
                // TODO check validity before casting ability
                break;
            default:
                break;
        }

        // TODO(mchi) Not sure where to put this exactly yet
        selectedAbility.StartTargeting();
    }

    public void StopTargeting()
    {
        Debug.Log("TARGETING | Stopped");

        selectedAbility.StopTargeting();

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
        if (!commandIsSettingNewTarget)
        {
            if (selectedHero != null)
            {
                selectedHero.GetComponent<Fighter>().SetSelectedUI(false);
            }

            selectedHero = hero;
            selectedHero.GetComponent<Fighter>().SetSelectedUI(true);
            commandsUI.GetComponent<CommandsUIHandler>().selectHero(hero);
        }
    }

    public void DeselectHero()
    {
        if (!commandIsSettingNewTarget)
        {
            if (selectedHero != null)
            {
                selectedHero.GetComponent<Fighter>().SetSelectedUI(false);
                selectedHero = null;
                commandsUI.GetComponent<CommandsUIHandler>().deselectedHero();
            }
        }
    }
}
