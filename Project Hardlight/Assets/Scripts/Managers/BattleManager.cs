using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// All inputs should go int there
// BattleManager handles LIVE FIGHTING. Anything outside of that, including setup, or UI stuff, should go elsewhere.
public class BattleManager : Singleton<BattleManager>
{
    public enum InputState { NothingSelected, HeroSelected, DraggingSelect, UpdatingTarget, CastingAbility, FollowingMoveCommand, BattleOver }

    public BattleConfig battleConfig;
    public bool allowDeselect = false;

    public GameObject notEnoughManaUI;
    public GameObject battleTargetPrefab;
    public GameObject moveLoc;
    public GameObject multiSelectionBox;
    public GameObject countdownUI;
    
    // The color to change hit Attackables to briefly
    public Color hitColor = new Color(1f, .5235f, .6194f);

    public PortraitHotKeyManager portraitHotKeyManager;
    //public CommandsUIHandler commandsUI;
    private GameObject battleTarget;

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
    
    [Header("donut touch")]
    public bool battleStarted;
    public Fighter selectedHero;
    public List<Fighter> multiSelectedHeros; //keeping this separate for now, maybe refactor later?
    public Ability selectedAbility;
    public InputState inputState;
   
    public int numEnemies;
    public int numHeros;

    private float startX;
    private float startY;
    public float sizingFactor = 0.01f;

    // Tutorial Events
    [HideInInspector]
    public UnityEvent onHeroSelected;
    [HideInInspector]
    public UnityEvent onSetTarget;
    [HideInInspector]
    public UnityEvent onMonsterDeath;
    [HideInInspector]
    public UnityEvent onBattleEnd;
    [HideInInspector]
    public UnityEvent onUseAbility;
    [HideInInspector]
    public UnityEvent onAbilityCast;
    [HideInInspector]
    public UnityEvent onAllHerosSelected;

    public void Initialize()
    {
        inputState = InputState.NothingSelected;
    }

    public void Update()
    {
        // Don't do BattleManager updates if not fighting
        if (GameManager.Instance.gameState != GameState.FIGHTING || inputState == InputState.BattleOver)
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
            if (selectedVessels.Count >= 2 && selectedVessels[1].activeSelf)
            {
                SetSelectedHero(selectedVessels[1].GetComponent<Fighter>());
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (selectedVessels.Count >= 3 && selectedVessels[2].activeSelf)
            {
                SetSelectedHero(selectedVessels[2].GetComponent<Fighter>());
            }
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // escape battle
            BattleOver(false);

            //pause
            //GameManager.Instance.gameState = GameState.PAUSED;
            //Time.timeScale = 0;
        }


        if (doubleClickTimer <= doubleClickTimeLimit)
        {
            doubleClickTimer += Time.deltaTime;
        } else
        {
            doubleClickPrimer = false;
        }

        if (inputState != InputState.BattleOver)
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
                UseAbility(2);
            }
            else if ((Input.GetKeyDown(KeyCode.S)))
            {
                UseAbility(3);
            }
            else if ((Input.GetKeyDown(KeyCode.Z)))
            {
                UseAbility(4);
            }
            else if ((Input.GetKeyDown(KeyCode.X)))
            {
                UseAbility(5);
            }
            /*
            else if ((Input.GetKeyDown(KeyCode.A)))
            {
                SetStateToUpdateTarget();
            }
            */
            
        }
    }

    /// <summary>
    /// Happens when a player clicks on a target during cast ability or they start targeting for targetInstant abilities
    /// </summary>
    void TargetSelected()
    {
        // Select target
        UpdateSelectedTarget();

        if(selectedHero != null && inputState != InputState.BattleOver
            && (selectedAbility.targetingType == Targeting.Type.TargetPosition || selectedAbility.selectedTarget != null))
        {
            if (selectedAbility.DoAbility())
            {
                // Play sound
                if (selectedAbility.sfx != null)
                {
                    selectedHero.GetComponent<AudioSource>().PlayOneShot(selectedAbility.sfx);
                }
                else
                {
                    Debug.Log(selectedAbility.abilityName + " is missing sfx btw");
                }
                // Lose mana
                selectedHero.LoseMana(selectedHero.GetMaxMana());
                //commandsUI.SwitchButtonColor(false);
                StopTargeting();
                //DeselectHero();

            }
            else
            {
                if(selectedAbility.targetingType == Targeting.Type.TargetPosition)
                {
                    Vector3 newClickPoint = new Vector3(selectedAbility.selectedPosition.x, selectedAbility.selectedPosition.y, selectedHero.transform.position.z);
                    Vector3 rangePoint = Vector3.MoveTowards(selectedHero.transform.position, newClickPoint, selectedAbility.GetRange());

                    float moveDist = Vector3.Distance(rangePoint, newClickPoint);
                    Vector3 InRangePoint = Vector3.MoveTowards(selectedHero.transform.position, newClickPoint, moveDist + 2);
                    GameObject newMoveLoc = Instantiate(moveLoc);
                    newMoveLoc.transform.position = new Vector3(InRangePoint.x, InRangePoint.y, 2);
                    newMoveLoc.SetActive(true);
                    //init line
                    LineRenderer line = newMoveLoc.GetComponentInChildren<LineRenderer>();
                    line.positionCount = 2;
                    line.SetPosition(0, newMoveLoc.transform.position);
                    line.SetPosition(1, selectedHero.transform.position);
                    selectedHero.GetComponent<FighterMove>().StartMovingCommandHandle(newMoveLoc.transform);

                } else
                {
                    Vector3 newClickPoint = new Vector3(selectedAbility.selectedTarget.transform.position.x, selectedAbility.selectedTarget.transform.position.y, selectedHero.transform.position.z);
                    Vector3 rangePoint = Vector3.MoveTowards(selectedHero.transform.position, newClickPoint, selectedAbility.GetRange());

                    float moveDist = Vector3.Distance(rangePoint, newClickPoint);
                    Vector3 InRangePoint = Vector3.MoveTowards(selectedHero.transform.position, newClickPoint, moveDist + 2);
                    GameObject newMoveLoc = Instantiate(moveLoc);
                    newMoveLoc.transform.position = new Vector3(InRangePoint.x, InRangePoint.y, 2);
                    newMoveLoc.SetActive(true);
                    //init line
                    LineRenderer line = newMoveLoc.GetComponentInChildren<LineRenderer>();
                    line.positionCount = 2;
                    line.SetPosition(0, newMoveLoc.transform.position);
                    line.SetPosition(1, selectedHero.transform.position);
                    selectedHero.GetComponent<FighterMove>().StartMovingCommandHandle(newMoveLoc.transform);
                }
                
                
    
            }
        }
        //if (selectedHero != null && selectedAbility.DoAbility() && inputState != InputState.BattleOver)
        //{
            
        //}
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
                    GameObject clickedFighter = null;

                    //checking all overlapping colliders in case fighter isn't the first one that comes up
                    Collider2D[] hitColliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pos));
                    foreach (Collider2D collider in hitColliders)
                    {
                        Fighter hitFighter = collider.gameObject.GetComponent<Fighter>();
                        if (hitFighter != null)
                        {
                            clickedFighter = hitFighter.gameObject;
                            break;
                        }
                        GenericMonsterAI monster = collider.gameObject.GetComponent<GenericMonsterAI>();

                        if (monster != null)
                        {
                            clickedFighter = monster.gameObject;
                            break;
                        }
                        
                        
                    }

                    if (clickedFighter != null)
                    {
                        //Debug.Log("Clicked fighter: " + clickedFighter);

                        //moved check for team into ability so it works with healing target unit abilities
                        selectedAbility.selectedTarget = clickedFighter;
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
            if (clickedFighter != null)
            {
                Debug.Log("got a clicked figter");
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
                                Debug.Log("b2");
                DeselectHero();
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

        onUseAbility.Invoke();
        onUseAbility.RemoveAllListeners();
        
        //if (selectedHero != null)
        {
            Fighter castingHero = selectedVessels[0].GetComponent<Fighter>();
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

            Ability ability = (Ability)selectedVessels[0].GetComponent<VesselData>().abilities[0];
            switch (abilityNum)
            {
                case 0:
                    break;
                case 1:
                    ability = (Ability)selectedVessels[0].GetComponent<VesselData>().abilities[1];
                    break;
                case 2:
                    ability = (Ability)selectedVessels[1].GetComponent<VesselData>().abilities[0];
                    castingHero = selectedVessels[1].GetComponent<Fighter>();
                    break;
                case 3:
                    ability = (Ability)selectedVessels[1].GetComponent<VesselData>().abilities[1];
                    castingHero = selectedVessels[1].GetComponent<Fighter>();
                    break;
                case 4:
                    ability = (Ability)selectedVessels[2].GetComponent<VesselData>().abilities[0];
                    castingHero = selectedVessels[2].GetComponent<Fighter>();
                    break;
                case 5:
                    ability = (Ability)selectedVessels[2].GetComponent<VesselData>().abilities[1];
                    castingHero = selectedVessels[2].GetComponent<Fighter>();
                    break;

            }
            if (ability != null && castingHero.GetComponent<Attackable>().GetHealth() > 0)
            {
                SetSelectedHero(castingHero);
                
                if (castingHero.GetCurrentMana() >= castingHero.GetMaxMana())
                {
                    selectedAbility = ability;

                    // Start targeting
                    StartTargeting();
                }
                else
                {
                    // Not enough mana, this will be activated in CheckEnoughMana also if the player clicks the ability button
                    // (but not with hotkeys, so I'm leaving it for now)
                    if (notEnoughManaUI != null && !notEnoughManaUI.activeSelf)
                    {
                        notEnoughManaUI.SetActive(true);
                    }
                }
            }
        }
    }

    /// <summary>
    // This is a dumb workaround because buttons can't be clicked when they're greyed out (effectively disabled)
    // But we want the not enough mana popup to still work, so this is called by the OnClick EventTrigger (sometimes concurrently with UseAbility)
    // And it just shows the popup if there's not enough mana
    /// </summary>
    public void CheckEnoughMana (int heroIndex)
    {
        Fighter castingHero = selectedVessels[heroIndex].GetComponent<Fighter>();

        if (castingHero.GetCurrentMana() < castingHero.GetMaxMana())
        {
            //Debug.Log("Not enough mana");
            if (notEnoughManaUI != null && !notEnoughManaUI.activeSelf)
            {
                notEnoughManaUI.SetActive(true);
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

        onAbilityCast.Invoke();
        onAbilityCast.RemoveAllListeners();

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
        DeselectHero();
        multiSelectedHeros.Clear();
        SetSelectedHero(selectedVessels[i].GetComponent<Fighter>());
    }

    /// <summary>
    /// Sets the currently selected hero to a single hero (by clicking on the hero or using hotkeys 1-3)
    /// </summary>
    /// <param name="hero"></param>
    public void SetSelectedHero(Fighter hero)
    {
        // Select this hero
        DeselectHero();
        multiSelectedHeros.Clear();
        selectedHero = hero;
        selectedHero.SetSelectedUI(true);
        inputState = InputState.HeroSelected;

        portraitHotKeyManager.LoadNewlySelectedHero(hero);
        OnSwitchTargetEvent();
        SubscribeHeroEvents();

        onHeroSelected.Invoke();
        onHeroSelected.RemoveAllListeners();
        
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

        if (multiSelectedHeros.Count > 0)
        {
            portraitHotKeyManager.DeselectedHero();

            //deselect all multi selected heros
            foreach (Fighter f in multiSelectedHeros)
            {
                Debug.Log("turning off selected ui");
                f.SetSelectedUI(false);
            }
            inputState = InputState.NothingSelected;
            multiSelectedHeros.Clear();

            if (selectedHero != null)
            {
                UnsubscribeHeroEvents();
                selectedHero.SetSelectedUI(false);
                selectedHero = null;
            }
        }
        else if (selectedHero != null)
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
        
    }

    /// <summary>
    /// Called by an invisible button on behind all the other UI
    /// Used to prevent race conditions between button clicking and Input.OnMouseDown
    /// This controls hero selection/deselection, target selection, movement, and ability casting
    /// </summary>
    public void SelectNonBattleButton (BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (GameManager.Instance.gameState == GameState.FIGHTING)
        {
            if (pointerData.button == PointerEventData.InputButton.Left && (inputState == InputState.NothingSelected || inputState == InputState.HeroSelected))
            {
                if (!UpdateClickedHero() && allowDeselect)
                {
                    DeselectHero();
                }
            }
            else if (inputState == InputState.HeroSelected && pointerData.button == PointerEventData.InputButton.Right)
            {
                //select new target
                bool foundEnemy = false;
                Vector3 pos = Input.mousePosition;
                Collider2D[] hitCollider = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pos));
                foreach (Collider2D hit in hitCollider)
                {
                    Attackable attackable = hit.GetComponent<Attackable>();
                    if (attackable != null)
                    {
                        if (selectedHero != null)
                        {
                            selectedHero.GetComponent<FighterAttack>().SetIssuedCurrentTarget(attackable);
                            foundEnemy = true;
                            break;
                        }
                        else if (multiSelectedHeros.Count > 0)
                        {
                            //set target for multiple heroes
                            foreach (Fighter f in multiSelectedHeros)
                            {
                                f.GetComponent<FighterAttack>().SetIssuedCurrentTarget(attackable);
                                foundEnemy = true;
                            }
                            break;
                        }
                    }
                }

                if (foundEnemy)
                {
                    onSetTarget.Invoke();
                    onSetTarget.RemoveAllListeners();
                }
                if (!foundEnemy)
                {
                    //move fighter
                    // Needs more moveloc references or static variable, otherwise ordering a second unity overwrites first's moveloc
                    // Also find bug where heroes disappear
                    //Set state to move or update target
                    //Debug.Log("Ordered a move");

                    pos = Input.mousePosition;
                    pos = Camera.main.ScreenToWorldPoint(pos);
                    //start moving hero
                    if (multiSelectedHeros.Count > 0)
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
                    } else if (selectedHero != null)
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
                }                              

            }
            else if (inputState == InputState.UpdatingTarget)
            {
                if (pointerData.button == PointerEventData.InputButton.Left)
                {
                    // Moved to right click when hero is selected
                    
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
                DeselectHero();
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

                if (multiSelectedHeros.Count == selectedVessels.Count)
                {
                    onAllHerosSelected.Invoke();
                    onAllHerosSelected.RemoveAllListeners();
                }
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
        StartCoroutine(DoCountdown());
    }

    /// <summary>
    /// Moved start battle logic into this IEnumerator so that we can countdown before starting everything
    /// </summary>
    /// <returns></returns>
    IEnumerator DoCountdown()
    {
        countdownUI.SetActive(true);
        portraitHotKeyManager.InitBattlerUI(selectedVessels);
        foreach (GameObject v in selectedVessels)
        {
            v.GetComponent<FighterAttack>().enabled = false;
            v.GetComponent<FighterMove>().enabled = false;
            v.GetComponent<Fighter>().enabled = false;
        }
        Text textComponent = countdownUI.GetComponent<Text>();
        textComponent.text = "3";
        yield return new WaitForSeconds(.75f);
        textComponent.text = "2";
        yield return new WaitForSeconds(.75f);
        textComponent.text = "1";
        yield return new WaitForSeconds(.75f);
        countdownUI.SetActive(false);

        //call levelStart on vessels
        foreach (GameObject v in selectedVessels)
        {
            v.GetComponent<Fighter>().enabled = true;
            v.GetComponent<FighterMove>().enabled = true;
            v.GetComponent<FighterAttack>().enabled = true;
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

        foreach (MonsterAI f in enemyParent.GetComponentsInChildren<MonsterAI>())
        {
            if (f.transform.parent.GetComponent<SwarmMasterAI>() == null)
            {
                numEnemies++;
            }
            
            f.LevelStart();
        }

        foreach (SwarmMasterAI f in enemyParent.GetComponentsInChildren<SwarmMasterAI>())
        {
            numEnemies++;
            f.LevelStart();
        }

        GameManager.Instance.SetCameraControls(true);


        //This event was causing tons of problems, so we're getting rid of it for now
        //OnLevelStart?.Invoke();
        //portraitHotKeyManager.InitBattlerUI(selectedVessels);
        battleStarted = true;
    }

    /// <summary>
    /// Event that happens when currentHero reaches max mana
    /// </summary>
    void OnMaxManaEvent(Fighter f)
    {
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
        if(selectedHero != null)
        {
            GameObject ct = selectedHero.GetComponent<FighterAttack>().currentTarget;
            if (ct != null)
            {
                battleTarget.SetActive(true);
                battleTarget.transform.parent = ct.transform;
                battleTarget.transform.localPosition = Vector3.zero;
            }
        }
        
    }

    public void OnDeath()
    {
        numEnemies--;
        
        onMonsterDeath.Invoke();

        if (numEnemies <= 0)
        {
            BattleOver(true);
        }
    }
    /// <summary>
    /// Called by all fighters when they die
    /// Determines if this death ends the level, and invokes OnLevelEvent if it does
    /// </summary>
    /// <param name="team"></param>
    public void OnDeath (Attackable attackable)
    {
        if (attackable.team == CombatInfo.Team.Hero)
        {
            
            numHeros--;
            if (numHeros <= 0)
            {
                BattleOver(false);
            }
            else if (attackable.GetComponent<Fighter>() == selectedHero)
            {
                //currently selected hero died, so deselect them
                DeselectHero();
            }
            else if (multiSelectedHeros.Contains(attackable.GetComponent<Fighter>()))
            {
                multiSelectedHeros.Remove(attackable.GetComponent<Fighter>());
                DeselectHero();
            }
        }
        else
        {
            numEnemies--;
            
            onMonsterDeath.Invoke();

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
        onBattleEnd.Invoke();
        onBattleEnd.RemoveAllListeners();

        //cleanup for this script
        DeselectHero();
        portraitHotKeyManager.AllUISwitch(false);
        battleStarted = false;
        inputState = InputState.BattleOver;
        selectedVessels = new List<GameObject>();

        //GameManager.Instance.EndFighting(herosWin); //Now called in PostBattleUI
        if (!TutorialManager.Instance.firstTutorialBattle)
        {
            UIManager.Instance.postBattleUI.StartPostBattle(herosWin);
        }
        
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
