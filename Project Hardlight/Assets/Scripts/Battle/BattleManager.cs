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
    public GameObject battleTargetPrefab;

    public CommandsUIHandler commandsUI;
    private GameObject battleTarget;

    public void Start()
    {
        
        inputState = InputState.NothingSelected;
    }

    public void Update()
    {
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
                    inputState = InputState.HeroSelected;
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
            case Targeting.Type.TargetUnit:
                {
                    Vector3 pos = Input.mousePosition;
                    Fighter clickedFighter = null;

                    //checking all overlapping colliders in case fighter isn't the first one that comes up
                    Collider2D[] hitColliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pos));
                    foreach (Collider2D collider in hitColliders)
                    {
                        Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
                        if (hitFighter != null)
                        {
                            clickedFighter = hitFighter;
                            break;
                        }
                    }

                    if (clickedFighter != null)
                    {
                        Debug.Log("Clicked fighter: " + clickedFighter);

                        //moved check for team into ability so it works with healing target unit abilities
                        selectedAbility.selectedTarget = clickedFighter.gameObject;
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
        if (hitCollider != null && hitCollider.gameObject.layer != 9) // if click hit the commandUI element, don't do anything
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
        if (selectedHero != null)
        {
            selectedHero.SetSelectedUI(false);

            if (notEnoughManaUI != null)
            {
                notEnoughManaUI.SetActive(false);
            }

            UnsubscribeHeroEvents();
        }

        selectedHero = hero;
        selectedHero.SetSelectedUI(true);

        Debug.Log(hero.name);
        commandsUI.EnableUI(hero.gameObject);
        commandsUI.SwitchButtonColor(selectedHero.GetCurrentMana() == selectedHero.fighterStats.maxMana);
        OnSwitchTargetEvent();
        SubscribeHeroEvents();
    }

    /// <summary>
    /// Deactiates selected hero UI and other things
    /// Called by event when hero dies
    /// </summary>
    public void DeselectHero()
    {
        if (selectedHero != null)
        {
            if (inputState == InputState.CastingAbility)
            {
                StopTargeting();
            }

            selectedHero.SetSelectedUI(false);
            UnsubscribeHeroEvents();
            selectedHero = null;
            commandsUI.DisableUI();
            inputState = InputState.NothingSelected;
        }
    }

    public void SetStateToUpdateTarget()
    {
        inputState = InputState.UpdatingTarget;
    }

    /// <summary>
    /// Event that happens when currentHero reaches max mana
    /// </summary>
    void OnMaxManaEvent ()
    {
        commandsUI.SwitchButtonColor(true);
    }

    /// <summary>
    /// Event that happens when the currentHero switches its target
    /// Creates a new battletarget if there isn't one (they will get destroyed when enemy dies)
    /// </summary>
    void OnSwitchTargetEvent ()
    {
        if (battleTarget == null)
        {
            battleTarget = Instantiate(battleTargetPrefab);
        }

        if (selectedHero.currentTarget != null)
        {
            battleTarget.transform.parent = selectedHero.currentTarget.transform;
            battleTarget.transform.localPosition = Vector3.zero;
        }
    }

    void SubscribeHeroEvents()
    {
        selectedHero.OnMaxMana += OnMaxManaEvent;
        selectedHero.OnSwitchTarget += OnSwitchTargetEvent;
        selectedHero.OnDeath += DeselectHero;
    }

    void UnsubscribeHeroEvents()
    {
        selectedHero.OnMaxMana -= OnMaxManaEvent;
        selectedHero.OnSwitchTarget -= OnSwitchTargetEvent;
        selectedHero.OnDeath += DeselectHero;
    }

    private void OnDisable()
    {
        if (selectedHero != null)
        {
            UnsubscribeHeroEvents();
        }
    }
}
