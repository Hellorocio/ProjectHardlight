using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All inputs should go int there
public class BattleManager :  MonoBehaviour
{
    public enum InputState { NothingSelected, HeroSelected, UpdatingTarget, CastingAbility, FollowingMoveCommand}

    public BattleConfig battleConfig;

    public GameObject selectedHero;
    public Ability selectedAbility;
    public InputState inputState;

    public bool commandIsSettingNewTarget;

    private GameObject commandsUI;

    public void Start()
    {
        //commandsUI = GameObject.Find("CommandsUI");
        selectedHero = null;
        inputState = InputState.NothingSelected;
    }

    public void Update()
    {
        if(selectedHero != null && !selectedHero.activeSelf)
        {
            selectedHero = null;
        }
        /////////////////// Idle
        if (inputState == InputState.NothingSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UpdateClickedHero();
                // if setting new target

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
                UpdateSelectedTarget(); // BUG: ability target isn't getting properly updated

                // Check has enough mana
                Fighter selectedFighter = selectedHero.GetComponent<Fighter>();
                if (selectedFighter.GetCurrentMana() >= selectedFighter.manaCosts)
                {
                    if (selectedAbility.DoAbility())
                    {
                        // Lose mana
                        selectedFighter.LoseMana(selectedFighter.manaCosts);
                        StopTargeting();
                        DeselectHero();
                    }
                }
                else
                {
                    Debug.Log("Not enough mana");
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                StopTargeting();
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
                    Collider2D[] hitCollider2 = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pos));
                    if(hitCollider2 == null)
                    {
                        Debug.Log("HitCollider2 is null");
                    } else
                    {
                        Debug.Log("First element in hitcollider2 is " + hitCollider2[0]);
                        Debug.Log("Hitcollider 2 size is " + hitCollider2.Length);
                    }
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
                SetSelectedHero(hitCollider.gameObject);
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
            Ability ability = (Ability)selectedHero.GetComponent<HeroAbilities>().abilityList[abilityNum];
            if (ability != null)
            {
                selectedAbility = ability;

                // Start targeting
                StartTargeting();
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
            GameObject.Find("CommandsUI").GetComponent<CommandsUIHandler>().selectHero(hero);
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
                GameObject.Find("CommandsUI").GetComponent<CommandsUIHandler>().deselectedHero();
            }
        }
    }
}
