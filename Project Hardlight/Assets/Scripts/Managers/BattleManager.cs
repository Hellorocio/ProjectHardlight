using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// All inputs should go int there
// BattleManager handles LIVE FIGHTING. Anything outside of that, including setup, or UI stuff, should go elsewhere.
public class BattleManager : Singleton<BattleManager>
{
    public enum InputState { NothingSelected, HeroSelected, DraggingSelect, UpdatingTarget, CastingAbility, FollowingMoveCommand, BattleOver }

    public BattleConfig battleConfig;

    private Fighter selectedHero;
    [HideInInspector]
    public List<Fighter> multiSelectedHeros; //keeping this separate for now, maybe refactor later?
    public Ability selectedAbility;
    public InputState inputState;
    public GameObject notEnoughManaUI;
    public GameObject battleTargetPrefab;
    public GameObject moveLoc;
    public GameObject multiSelectionBox;

    public PortraitHotKeyManager portraitHotKeyManager;
    //public CommandsUIHandler commandsUI;
    private GameObject battleTarget;

    private int numEnemies;
    private int numHeros;

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

    private float startX;
    private float startY;
    public float sizingFactor = 0.01f;

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (selectedVessels[0].activeSelf)
            {
                SetSelectedHero(selectedVessels[0].GetComponent<Fighter>());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (selectedVessels[1].activeSelf)
            {
                SetSelectedHero(selectedVessels[1].GetComponent<Fighter>());
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (selectedVessels[2].activeSelf)
            {
                SetSelectedHero(selectedVessels[2].GetComponent<Fighter>());
            }
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // escape battle
            BattleOver(false);
        }


        if (doubleClickTimer <= doubleClickTimeLimit)
        {
            doubleClickTimer += Time.deltaTime;
        } else
        {
            doubleClickPrimer = false;
        }

        if (inputState == InputState.HeroSelected || inputState == InputState.CastingAbility || inputState == InputState.UpdatingTarget)
        {
            if ((Input.GetKeyDown(KeyCode.Q)))
            {
                UseAbility(0);
            }
            else if ((Input.GetKeyDown(KeyCode.W)))
            {
                UseAbility(1);
            }
            else if ((Input.GetKeyDown(KeyCode.A)))
            {
                SetStateToUpdateTarget();
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
            //commandsUI.SwitchButtonColor(false);
            StopTargeting();
            //DeselectHero();
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
    bool UpdateClickedHero()
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
                        camController.GetCamera().orthographicSize = camController.zoomMin;
                    }
                }
            } else
            {
                SetSelectedHero(clickedHero);
            }
            doubleClickPrimer = true;
            doubleClickTimer = 0;
        }
        return clickedHero != null;
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
            else if (inputState == InputState.UpdatingTarget)
            {
                SetCursor(battleConfig.defaultCursor);
            }

            // Clear any existing selected ability
            selectedAbility = null;

            Ability ability = (Ability)selectedHero.gameObject.GetComponent<VesselData>().abilities[abilityNum];
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
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }
    public void SetSelectedHeroButtonHandler(int i)
    {
        SetSelectedHero(selectedVessels[i].GetComponent<Fighter>());
    }

    /// <summary>
    /// Sets the currently selected hero to a single hero (by clicking on the hero or using hotkeys 1-3)
    /// </summary>
    /// <param name="hero"></param>
    public void SetSelectedHero(Fighter hero)
    {
        if ((selectedHero != null && selectedHero != hero) || multiSelectedHeros.Count > 0)
        {
            DeselectHero();
        }

        selectedHero = hero;
        selectedHero.SetSelectedUI(true);
        inputState = InputState.HeroSelected;

        portraitHotKeyManager.LoadNewlySelectedHero(hero);
        OnSwitchTargetEvent();
        SubscribeHeroEvents();
    }
    

    /// <summary>
    /// Deactiates selected hero UI and other things
    /// Called by event when hero dies
    /// </summary>
    public void DeselectHero()
    {
        if (inputState == InputState.UpdatingTarget)
        {
            SetCursor(battleConfig.defaultCursor);
        }

        if (selectedHero != null)
        {
            if (inputState == InputState.CastingAbility)
            {
                StopTargeting();
            }

            portraitHotKeyManager.DeselectedHero();
            selectedHero.SetSelectedUI(false);
            UnsubscribeHeroEvents();
            selectedHero = null;
            //commandsUI.DisableUI();
            inputState = InputState.NothingSelected;
            if (battleTarget != null)
            {
                battleTarget.SetActive(false);
            }
        }
        else if (multiSelectedHeros.Count > 0)
        {
            portraitHotKeyManager.DeselectedHero();

            //deselect all multi selected heros
            foreach (Fighter f in multiSelectedHeros)
            {
                f.SetSelectedUI(false);
            }
            inputState = InputState.NothingSelected;
            multiSelectedHeros.Clear();
        }
    }

    /// <summary>
    /// Called by an invisible button on behind all the other UI
    /// Used to prevent race conditions between button clicking and Input.OnMouseDown
    /// </summary>
    public void SelectNonBattleButton (BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (GameManager.Instance.gameState == GameState.FIGHTING)
        {
            if (pointerData.button == PointerEventData.InputButton.Left && (inputState == InputState.NothingSelected || inputState == InputState.HeroSelected))
            {
                if (!UpdateClickedHero())
                {
                    DeselectHero();
                }
            }
            else if (inputState == InputState.HeroSelected && pointerData.button == PointerEventData.InputButton.Right)
            {
                //move fighter
                // Needs more moveloc references or static variable, otherwise ordering a second unity overwrites first's moveloc
                // Also find bug where heroes disappear
                //Set state to move or update target
                //Debug.Log("Ordered a move");

                Vector3 pos = Input.mousePosition;
                pos = Camera.main.ScreenToWorldPoint(pos);

                //start moving hero
                if (selectedHero != null)
                {
                    //init moveloc
                    GameObject newMoveLoc = Instantiate(moveLoc);
                    newMoveLoc.SetActive(true);
                    newMoveLoc.transform.position = new Vector3(pos.x, pos.y, 2);

                    //init line
                    LineRenderer line = newMoveLoc.GetComponentInChildren<LineRenderer>();
                    line.positionCount = 2;
                    line.SetPosition(0, newMoveLoc.transform.position);
                    line.SetPosition(1, selectedHero.transform.position);
                    selectedHero.GetComponent<FighterMove>().StartMovingCommandHandle(newMoveLoc.transform);
                }
                else if (multiSelectedHeros.Count > 0)
                {
                    //move multiple heroes
                    foreach (Fighter f in multiSelectedHeros)
                    {
                        //init moveloc
                        GameObject newMoveLoc = Instantiate(moveLoc);
                        newMoveLoc.SetActive(true);
                        newMoveLoc.transform.position = new Vector3(pos.x, pos.y, 2);

                        //init line
                        LineRenderer line = newMoveLoc.GetComponentInChildren<LineRenderer>();
                        line.positionCount = 2;
                        line.SetPosition(0, newMoveLoc.transform.position);
                        line.SetPosition(1, multiSelectedHeros[0].transform.position);
                        f.GetComponent<FighterMove>().StartMovingCommandHandle(newMoveLoc.transform);
                    }
                }

            }
            else if (inputState == InputState.UpdatingTarget)
            {
                if (pointerData.button == PointerEventData.InputButton.Left)
                {
                    //select new target
                    Vector3 pos = Input.mousePosition;
                    Collider2D[] hitCollider = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pos));
                    foreach (Collider2D hit in hitCollider)
                    {
                        Fighter tmp = hit.GetComponent<Fighter>();
                        if (tmp != null)
                        {
                            if (selectedHero != null)
                            {
                                selectedHero.GetComponent<FighterAttack>().SetIssuedCurrentTarget(tmp);
                            }
                            else if (multiSelectedHeros.Count > 0)
                            {
                                //set target for multiple heroes
                                foreach (Fighter f in multiSelectedHeros)
                                {
                                    f.GetComponent<FighterAttack>().SetIssuedCurrentTarget(tmp);
                                }
                            }
                        }
                    }
                }

                //called on left or right mouse button click (this is what disables selection on right click)
                inputState = InputState.HeroSelected;
                SetCursor(battleConfig.defaultCursor);
            }
            else if (inputState == InputState.CastingAbility)
            {
                if (pointerData.button == PointerEventData.InputButton.Left)
                {
                    TargetSelected();
                }
                else if (pointerData.button == PointerEventData.InputButton.Right)
                {
                    StopTargeting();
                }
            }
        }
    }

    public void StopDraggingSelection ()
    {
        if (inputState == InputState.DraggingSelect)
        {
            //TODO: end drag- select multiple if multiple selected, select 1 if 1 selected, otherwise deselect
            inputState = InputState.NothingSelected;
            if (multiSelectedHeros.Count == 0)
            {
                inputState = InputState.NothingSelected;
            }
            else if (multiSelectedHeros.Count == 1)
            {
                inputState = InputState.HeroSelected;
                SetSelectedHero(multiSelectedHeros[0]);
            }
            else
            {
                //selected multiple heros, show multi-selected hero menu
                inputState = InputState.HeroSelected;
                portraitHotKeyManager.LoadMultiSelectedHeros();

            }
            multiSelectionBox.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when invisible button in the background is clicked and dragged
    /// </summary>
    public void StartDraggingSelection (BaseEventData data)
    {
        //print("select multiple");
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData.button == PointerEventData.InputButton.Left && GameManager.Instance.gameState == GameState.FIGHTING && 
            (inputState == InputState.NothingSelected || inputState == InputState.HeroSelected || inputState == InputState.DraggingSelect))
        {
            DeselectHero();
            multiSelectedHeros.Clear();

            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20);
            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            startX = position.x;
            startY = position.y;
            position = Camera.main.ScreenToWorldPoint(position);
            multiSelectionBox.transform.position = position;
            multiSelectionBox.transform.localScale = Vector3.zero;
            multiSelectionBox.gameObject.SetActive(true);
            inputState = InputState.DraggingSelect;
        }
    }

    public void OnDraggingSelection ()
    {
        if (inputState == InputState.DraggingSelect)
        {
            Vector3 size = multiSelectionBox.transform.localScale;

            size.x = (Input.mousePosition.x - startX) * sizingFactor * 0.5f * Camera.main.orthographicSize;

            size.y = (Input.mousePosition.y - startY) * sizingFactor * 0.5f * Camera.main.orthographicSize;
            Vector2 difs = new Vector2(size.x - multiSelectionBox.transform.localScale.x, size.y - multiSelectionBox.transform.localScale.y);
            multiSelectionBox.transform.localScale = size;

            Vector3 tmpPos = multiSelectionBox.transform.position;
            tmpPos.x += difs.x * 2.5f;
            tmpPos.y += difs.y * 2.5f;
            multiSelectionBox.transform.position = tmpPos;
        }
        
    }

    public void SetStateToUpdateTarget()
    {
        if (inputState == InputState.CastingAbility)
        {
            StopTargeting();
        }

        inputState = InputState.UpdatingTarget;
        SetCursor(battleConfig.changeTargetCursor);
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
            v.GetComponent<FighterAttack>().LevelStart();
        }

        //call levelStart on enemies
        GameObject enemyParent = GameObject.Find("Enemies");
        numHeros = selectedVessels.Count;
        numEnemies = 0;
        foreach (FighterAttack f in enemyParent.GetComponentsInChildren<FighterAttack>())
        {
            numEnemies++;
            f.LevelStart();
        }

        //This event was causing tons of problems, so we're getting rid of it for now
        //OnLevelStart?.Invoke();
        portraitHotKeyManager.InitBattlerUI(selectedVessels);
        battleStarted = true;
    }

    /// <summary>
    /// Event that happens when currentHero reaches max mana
    /// </summary>
    void OnMaxManaEvent(Fighter f)
    {
        Debug.Log("MANAAA");
        //commandsUI.SwitchButtonColor(true);
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
            else if (multiSelectedHeros.Contains(fighter))
            {
                multiSelectedHeros.Remove(fighter);
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
        portraitHotKeyManager.AllUISwitch(false);
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
