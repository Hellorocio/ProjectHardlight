﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// All inputs should go int there
// BattleManager handles LIVE FIGHTING. Anything outside of that, including setup, or UI stuff, should go elsewhere.
public class BattleManager : Singleton<BattleManager>
{
    public enum InputState { NothingSelected, HeroSelected, UpdatingTarget, CastingAbility, FollowingMoveCommand, BattleOver }

    public BattleConfig battleConfig;

    private Fighter selectedHero;
    public Ability selectedAbility;
    public InputState inputState;
    public GameObject notEnoughManaUI;
    public GameObject battleTargetPrefab;
    public GameObject moveLoc;

    public CommandsUIHandler commandsUI;
    private GameObject battleTarget;

    public int numEnemies;
    private int numHeros = 3; //hardcoded for now, we'll have to change this if we change # of heros in battle

    //called when the level starts (when all heros have been placed)
    public delegate void LevelStart();
    public event LevelStart OnLevelStart;

    //called when all enemies or all heros are defeated
    public delegate void LevelEnd(bool herosWin);
    public event LevelEnd OnLevelEnd;

    public CameraController camController;
    private float doubleClickTimer;
    private bool doubleClickPrimer;
    private float doubleClickTimeLimit = 0.3f;

    public List<GameObject> selectedVessels;
    bool battleStarted;

    public void Initialize()
    {
        inputState = InputState.NothingSelected;
    }

    public void Update()
    {
        // Don't do BattleManager updates if not fighting
        if (GameManager.Instance.gameState != GameState.FIGHTING)
        {
            return;
        }
        
        if(doubleClickTimer <= doubleClickTimeLimit)
        {
            doubleClickTimer += Time.deltaTime;
        } else
        {
            doubleClickPrimer = false;
        }

        if (inputState == InputState.NothingSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UpdateClickedHero();
            }

        }
        else if (inputState == InputState.HeroSelected)
        {
            //Debug.Log("Current state is Hero Selected");
            if (Input.GetMouseButtonDown(0))
            {
                UpdateClickedHero();
            }
            if (Input.GetMouseButtonDown(1))
            {
                // Needs more moveloc references or static variable, otherwise ordering a second unity overwrites first's moveloc
                // Also find bug where heroes disappear
                //Set state to move or update target
                //Debug.Log("Ordered a move");
                Vector3 pos = Input.mousePosition;
                pos = Camera.main.ScreenToWorldPoint(pos);
                GameObject newMoveLoc = Instantiate(moveLoc);
                newMoveLoc.transform.position = new Vector3(pos.x, pos.y, 2);
                selectedHero.GetComponent<FighterMove>().StartMovingCommandHandle(newMoveLoc.transform);
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
            //Debug.Log("Input state is casting ability");
            if (Input.GetMouseButtonDown(0))
            {
                TargetSelected();
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
                Collider2D[] hitCollider = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pos));
                foreach(Collider2D hit in hitCollider)
                {
                    Fighter tmp = hit.GetComponent<Fighter>();
                    if (tmp != null)
                    {
                        
                        selectedHero.GetComponent<FighterAttack>().SetIssuedCurrentTarget(tmp);
                        inputState = InputState.HeroSelected;
                    }
                }
                /*
                if (hitCollider != null && hitCollider)
                {
                    //Updates the current target
                    Fighter tmp = hitCollider.GetComponent<Fighter>();
                    selectedHero.GetComponent<FighterAttack>().SetIssuedCurrentTarget(tmp);
                    inputState = InputState.HeroSelected;
                }
                */
            }
        }
    }

    /// <summary>
    /// Happens when a player clicks on a target during cast ability or they start targeting for targetInstant abilities
    /// </summary>
    void TargetSelected()
    {
        // Select target
        UpdateSelectedTarget();

        if (selectedHero != null && selectedAbility.DoAbility() && inputState != InputState.BattleOver)
        {
            // Lose mana
            selectedHero.LoseMana(selectedHero.GetMaxMana());
            commandsUI.SwitchButtonColor(false);
            StopTargeting();
            DeselectHero();
        }
    }

    /// <summary>
    /// Determines targeting type and updates the abiltie
    /// </summary>
    void UpdateSelectedTarget()
    {
        //Debug.Log("In Update Selected Target");
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
                        //Debug.Log("Clicked fighter: " + clickedFighter);

                        //moved check for team into ability so it works with healing target unit abilities
                        selectedAbility.selectedTarget = clickedFighter.gameObject;
                    }
                }
                break;
            case Targeting.Type.Instant:
                {
                    selectedAbility.selectedPosition = selectedHero.transform.position;
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Sets selected hero to the hero that was justed clicked
    /// </summary>
    void UpdateClickedHero()
    {
        Vector3 pos = Input.mousePosition;
        Collider2D[] colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pos));
        Fighter clickedHero = null;
        foreach (Collider2D collider in colliders)
        {
            Fighter clickedFighter = collider.gameObject.GetComponent<Fighter>();
            if (clickedFighter != null && clickedFighter.team == CombatInfo.Team.Hero)
            {
                clickedHero = clickedFighter;
                break;
            }
        }

        if (clickedHero != null)
        {
            if (clickedHero.Equals(selectedHero))
            {
                //If the player double-clicks on a hero then we should focus on that hero
                if (doubleClickPrimer && doubleClickTimer <= doubleClickTimeLimit)
                {
                    //Debug.Log("Double-clicked Hero");
                    if(camController != null) //Safety check to avoid breaking other scenes while testing
                    {
                        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        worldPoint.z = camController.transform.position.z;
                        camController.transform.position = worldPoint;
                        camController.gameObject.GetComponent<Camera>().orthographicSize = camController.zoomMin;
                    }
                }
            } else
            {
                SetSelectedHero(clickedHero);
            }
            doubleClickPrimer = true;
            doubleClickTimer = 0;
        }
    }

    /// <summary>
    /// Called by the ability buttons to start casting an ability
    /// </summary>
    /// <param name="abilityNum"></param>
    public void UseAbility(int abilityNum)
    {
        if (!TutorialManager.Instance.usedAbility)
        {
            TutorialManager.Instance.usedAbility = true;
        }
        
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
                if (selectedHero.GetCurrentMana() >= selectedHero.GetMaxMana())
                {
                    selectedAbility = ability;

                    // Start targeting
                    StartTargeting();
                }
                else
                {
                    //Debug.Log("Not enough mana");
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
        //Debug.Log("TARGETING | Started");

        inputState = InputState.CastingAbility;
        switch (selectedAbility.targetingType)
        {
            case Targeting.Type.TargetPosition:
                //Debug.Log("TARGETING | Target Position");
                SetCursor(battleConfig.targetPositionCursor);
                break;
            case Targeting.Type.TargetUnit:
                //Debug.Log("TARGETING | Target Unit");
                SetCursor(battleConfig.targetUnitCursor);
                // TODO change cursor
                break;
            case Targeting.Type.Instant:
                // TODO check validity before casting ability
                //Debug.Log("TARGETING | Target Instant");
                TargetSelected();
                return;
            default:
                break;
        }

        // TODO(mchi) Not sure where to put this exactly yet
        selectedAbility.StartTargeting();
    }

    public void StopTargeting()
    {
        //Debug.Log("TARGETING | Stopped");

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
        inputState = InputState.HeroSelected;

        //Debug.Log(hero.name);
        commandsUI.gameObject.SetActive(true);
        commandsUI.EnableUI(hero.gameObject);
        commandsUI.SwitchButtonColor(selectedHero.GetCurrentMana() == selectedHero.GetMaxMana());
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
            if (battleTarget != null)
            {
                battleTarget.SetActive(false);
            }
        }
    }

    public void SetStateToUpdateTarget()
    {
        inputState = InputState.UpdatingTarget;
    }

    /// <summary>
    /// (USED TO) Invokes the OnLevelStart event (StartBattle is called by heroPlacer when all heros have been placed)
    /// (NOW) Just calls levelstart manually on all vessels and enemies, because we were getting tons of bugs with the events
    /// </summary>
    public void StartBattle ()
    {
        //call levelStart on vessels
        foreach (GameObject v in selectedVessels)
        {
            numEnemies++;
            v.GetComponent<FighterAttack>().LevelStart();
        }

        //call levelStart on enemies
        GameObject enemyParent = GameObject.Find("Enemies");
        numEnemies = 0;
        if (SceneManager.GetActiveScene().buildIndex == 8)
        {
            numHeros = 1;
        }
        else
        {
            numHeros = 3;
        }
        
        foreach (FighterAttack f in enemyParent.GetComponentsInChildren<FighterAttack>())
        {
            numEnemies++;
            f.LevelStart();
        }
        
        //This event was causing tons of problems, so we're getting rid of it for now
        //OnLevelStart?.Invoke();
        battleStarted = true;
    }

    /// <summary>
    /// Event that happens when currentHero reaches max mana
    /// </summary>
    void OnMaxManaEvent(Fighter f)
    {
        Debug.Log("MANAAA");
        commandsUI.SwitchButtonColor(true);
    }

    /// <summary>
    /// Event that happens when the currentHero switches its target
    /// Creates a new battletarget if there isn't one (they will get destroyed when enemy dies)
    /// </summary>
    void OnSwitchTargetEvent()
    {
        if (!battleStarted)
        {
            return;
        }

        if (battleTarget == null)
        {
            battleTarget = Instantiate(battleTargetPrefab);
        }
        
        GameObject ct = selectedHero.GetComponent<FighterAttack>().currentTarget;
        if (ct != null)
        {
            battleTarget.SetActive(true);
            battleTarget.transform.parent = ct.transform;
            battleTarget.transform.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Called by all fighters when they die
    /// Determines if this death ends the level, and invokes OnLevelEvent if it does
    /// </summary>
    /// <param name="team"></param>
    public void OnDeath (Fighter fighter)
    {
        if (fighter.team == CombatInfo.Team.Hero)
        {
            numHeros--;
            if (numHeros <= 0)
            {
                BattleOver(false);
            }
            else if (fighter == selectedHero)
            {
                //currently selected hero died, so deselect them
                DeselectHero();
            }
        }
        else
        {
            numEnemies--;
            if (numEnemies <= 0)
            {
                BattleOver(true);
            }
        }
    }

    /// <summary>
    /// Invokes levelEnd event, sets state to BattleOver, and disables UI 
    /// </summary>
    /// <param name="herosWin"></param>
    public void BattleOver (bool herosWin)
    {
        //Invoke levelEnd event so levelManager knows to show dialogue
        OnLevelEnd?.Invoke(herosWin);

        //cleanup for this script
        DeselectHero();
        battleStarted = false;
        inputState = InputState.BattleOver;
        selectedVessels = new List<GameObject>();

        GameManager.Instance.EndFighting(herosWin);
    }

    void SubscribeHeroEvents()
    {
        selectedHero.OnMaxMana += OnMaxManaEvent;
        selectedHero.GetComponent<FighterAttack>().OnSwitchTarget += OnSwitchTargetEvent;
    }

    void UnsubscribeHeroEvents()
    {
        selectedHero.OnMaxMana -= OnMaxManaEvent;
        selectedHero.GetComponent<FighterAttack>().OnSwitchTarget -= OnSwitchTargetEvent;
    }

    private void OnDisable()
    {
        if (selectedHero != null)
        {
            UnsubscribeHeroEvents();
        }
    }
}
