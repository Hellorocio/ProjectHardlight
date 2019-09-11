using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CommandsUIHandler : MonoBehaviour
{
    public Image background;
    public Text heroNameText;
    public Button ability1Button;
    public Button ability2Button;
    public Button targetButton;
    public GameObject battleManager;
    private bool isUIShowing = false;
    private GameObject currentlySelectedHero;
    private bool selectingTarget = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selectingTarget)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Input.mousePosition;
                Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));
                if(hitCollider != null)
                {
                    Fighter tmp = hitCollider.GetComponent<Fighter>();

                    if(tmp != null && ((tmp.team == CombatInfo.Team.Enemy && !currentlySelectedHero.GetComponent<Fighter>().healer)
                        || (tmp.team == CombatInfo.Team.Hero && currentlySelectedHero.GetComponent<Fighter>().healer)))
                    {
                        currentlySelectedHero.GetComponent<Fighter>().currentTarget = tmp.gameObject;
                    }
                    StartCoroutine(endTargeting());

                } else
                {
                    StartCoroutine(endTargeting());
                }
            }
        }
    }

    IEnumerator endTargeting()
    {
        yield return new WaitForEndOfFrame();
        selectingTarget = false;
        battleManager.GetComponent<BattleManager>().commandIsSettingNewTarget = false;
    }

    public void deselectedHero()
    {
        currentlySelectedHero = null;
        setEnabled(false);
        
    }

    public void selectHero(GameObject f)
    {
        if (!selectingTarget)
        {

            setEnabled(true);
            currentlySelectedHero = f;
            heroNameText.text = f.GetComponent<Fighter>().characterName;
            HeroAbilities tmpAbilities = f.GetComponent<HeroAbilities>();
            ability1Button.GetComponentInChildren<Text>().text = ((Ability)tmpAbilities.abilityList[0]).abilityName;
            if (tmpAbilities.abilityList.Count > 1)
            {
                ability2Button.GetComponentInChildren<Text>().text = ((Ability)tmpAbilities.abilityList[1]).abilityName;
            }
        }

    }

    private void setEnabled(bool b)
    {
        isUIShowing = b;
        background.gameObject.SetActive(b);
        heroNameText.gameObject.SetActive(b);
        ability1Button.gameObject.SetActive(b);
        ability2Button.gameObject.SetActive(b);
        targetButton.gameObject.SetActive(b);
    }

    public void setTargetButton()
    {
        selectingTarget = true;
        battleManager.GetComponent<BattleManager>().commandIsSettingNewTarget = true;
        
    }

    public void useAbilityOne()
    {
        battleManager.GetComponent<BattleManager>().commandIsUsingAbility1 = true;
    }

    public void useAbilityTwo()
    {
        battleManager.GetComponent<BattleManager>().commandIsUsingAbility2 = true;
    }

    private void castAbilityButton(int i)
    {

    }
}
