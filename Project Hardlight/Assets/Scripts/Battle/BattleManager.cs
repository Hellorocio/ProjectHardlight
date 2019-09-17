using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All inputs should go int there
public class BattleManager :  MonoBehaviour
{
    public enum InputState { NothingSelected, HeroSelected, UpdatingTarget, CastingAbility, FollowingMoveCommand}

    public BattleConfig battleConfig;

    private Fighter selectedHero;
    public Ability selectedAbility;
    public InputState inputState;
    public GameObject notEnoughManaUI;

    public bool commandIsSettingNewTarget;

    private CommandsUIHandler commandsUI;

    public void Start()
    {
        GameObject commandsUIObj = GameObject.Find("CommandsUI");
        if (commandsUIObj != null)
        {
            commandsUI = commandsUIObj.GetComponent<CommandsUIHandler>();
        }
        inputState = InputState.NothingSelected;
    }

    public void Update()
    {
        if(selectedHero != null && !selectedHero.gameObject.activeSelf)
        {
            UnsubscribeMaxManaEvent();
            selectedHero = null;
        }
        /////////////////// Idle
        if (inputState == InputState.NothingSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UpdateClickedHero();
            }
            
        }
        else if (inputState == InputState.HeroSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UpdateClickedHero();
            }
            if (Input.GetMouseButtonDown(1))
            {
                //Set state to move or update target

            }

            if ((Input.GetKeyDown(KeyCode.Alpha1)))
            {
                UseAbility(0);
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha2)))
            {
                UseAbility(1);
            }
        }
        else if (inputState == InputState.CastingAbility)
        {
            Debug.Log("Input state is casting ability");
            if (Input.GetMouseButtonDown(0))
            {
                // Select target
                UpdateSelectedTarget();

                if (selectedAbility.DoAbility())
                {
                    // Lose mana
                    selectedHero.LoseMana(selectedHero.fighterStats.maxMana);
                    commandsUI.SwitchButtonColor(false);
                    StopTargeting();
                    DeselectHero();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                StopTargeting();
            }
        }
        else if (inputState == InputState.UpdatingTarget)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Input.mousePosition;
                Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));
                if (hitCollider != null)
                {
                    //Updates the current target
                    Fighter tmp = hitCollider.GetComponent<Fighter>();
                    selectedHero.SetIssuedCurrentTarget(tmp);
                }
            }
        }
    }

    /// <summary>
    /// Determines targeting type and updates the abiltie
    /// </summary>
    void UpdateSelectedTarget()
    {
        Debug.Log("In Update Selected Target");
        switch (selectedAbility.targetingType)
        {
            case Targeting.Type.TargetPosition:
                {
                    Vector3 mousePos = Input.mousePosition;
                    selectedAbility.selectedPosition = Camera.main.ScreenToWorldPoint(mousePos);
                    // TODO (mchi) Neutral sound on failure
                }
                break;
            case Targeting.Type.TargetUnit:// M1
                {
                    Vector3 pos = Input.mousePosition;
                    Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));
                    if (hitCollider != null)
                    {
                        Fighter clickedFighter = hitCollider.gameObject.GetComponent<Fighter>();
                        Debug.Log("Clicked fighter: " + clickedFighter);
                        if (clickedFighter != null && clickedFighter.team == CombatInfo.Team.Enemy)
                        {
                            selectedAbility.selectedTarget = hitCollider.gameObject;
                            Debug.Log("got enemy target unit");
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Sets selected hero to the hero that was justed clicked
    /// </summary>
    void UpdateClickedHero ()
    {
        Vector3 pos = Input.mousePosition;
        Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));
        if (hitCollider != null && hitCollider.gameObject.layer != 9 && !commandIsSettingNewTarget) // if click hit the commandUI element, don't do anything
        {
            Fighter clickedFighter = hitCollider.gameObject.GetComponent<Fighter>();
            if (clickedFighter != null && clickedFighter.team == CombatInfo.Team.Hero)
            {
                SetSelectedHero(clickedFighter);
            }
            else
            {
                DeselectHero();
            }
        }
    }
    
    public void UseAbility (int abilityNum)
    {
        if (selectedHero != null)
        {
            if (selectedAbility != null)
            {
                StopTargeting();
            }

            // Clear any existing selected ability
            selectedAbility = null;
            
            Ability ability = (Ability)selectedHero.gameObject.GetComponent<HeroAbilities>().abilityList[abilityNum];
            if (ability != null)
            {
                // Check has enough mana
                if (selectedHero.GetCurrentMana() >= selectedHero.fighterStats.maxMana)
                {
                    selectedAbility = ability;

                    // Start targeting
                    StartTargeting();
                }
                else
                {
                    Debug.Log("Not enough mana");
                    if (notEnoughManaUI != null)
                    {
                        notEnoughManaUI.SetActive(true);
                    }
                }
            }
        }
    }

    public void StartTargeting()
    {
        Debug.Log("TARGETING | Started");

        inputState = InputState.CastingAbility;
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
        selectedAbility.selectedTarget = null;

        inputState = InputState.HeroSelected;
        selectedAbility = null;
        SetCursor(battleConfig.defaultCursor);

    }

    public void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void SetSelectedHero(Fighter hero)
    {
        if (!commandIsSettingNewTarget)
        {
            if (selectedHero != null)
            {
                selectedHero.SetSelectedUI(false);

                if (notEnoughManaUI != null)
                {
                    notEnoughManaUI.SetActive(false);
                }

                UnsubscribeMaxManaEvent();
            }

            selectedHero = hero;
            selectedHero.SetSelectedUI(true);
            commandsUI.EnableUI(hero.gameObject);
            commandsUI.SwitchButtonColor(selectedHero.GetCurrentMana() == selectedHero.fighterStats.maxMana);

            SubscribeMaxManaEvent();
        }
    }

    public void DeselectHero()
    {
        if (!commandIsSettingNewTarget)
        {
            if (selectedHero != null)
            {
                selectedHero.SetSelectedUI(false);
                UnsubscribeMaxManaEvent();
                selectedHero = null;
                commandsUI.DisableUI();
            }
        }
    }

    public void SetStateToUpdateTarget()
    {
        inputState = InputState.UpdatingTarget;
    }

    void OnMaxManaEvent ()
    {
        commandsUI.SwitchButtonColor(true);
    }

    void SubscribeMaxManaEvent()
    {
        selectedHero.OnMaxMana += OnMaxManaEvent;
    }

    void UnsubscribeMaxManaEvent ()
    {
        selectedHero.OnMaxMana -= OnMaxManaEvent;
    }

    private void OnDisable()
    {
        if (selectedHero != null)
        {
            UnsubscribeMaxManaEvent();
        }
    }
}
