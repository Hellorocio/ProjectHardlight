using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HeroSelection : MonoBehaviour
{
    private int lastHeroSelected = -1;
    public GameObject soulSelectionMenu;
    public Text debugPartyLineup;
    public Text startButtonText;
    private List<Vector2> lineup = new List<Vector2>();
    private int partySize = 0;
    public GameObject partyPlacement;
    private GameObject enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GameObject.Find("Enemies");
        foreach (Fighter f in enemyParent.GetComponentsInChildren<Fighter>())
        {
            f.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToSoulSelection(int h)
    {
        if (partySize < 3)
        {
            lastHeroSelected = h;
            soulSelectionMenu.SetActive(true);
        }
    }

    public void ReturnFromSoulSelection(int s)
    {
        lineup.Add(new Vector2(lastHeroSelected, s));
        soulSelectionMenu.SetActive(false);
        GameObject.Find("HeroPortrait" + lastHeroSelected).GetComponent<Button>().interactable = false;
        string output = "";
        foreach(Vector2 hero in lineup)
        {
            output += "Hero# " + hero.x + "Soul#" + hero.y + " | ";
        }
        debugPartyLineup.text = output;
        partySize++;
        startButtonText.text = partySize + "/3 Champions Selected";
    }

    public void FinalizeParty()
    {
        if(partySize == 3)
        {
            partyPlacement.SetActive(true);
            partyPlacement.GetComponent<HeroPlacer>().StartHeroPlacement(lineup);
            gameObject.SetActive(false);
        }
    }

    public void RemoveLastMember()
    {
        if (partySize > 0)
        {
            Vector2 tmp = lineup[lineup.Count-1];
            lineup.RemoveAt(lineup.Count - 1);
            GameObject.Find("HeroPortrait" + tmp.x).GetComponent<Button>().interactable = true;
            partySize--;
            startButtonText.text = partySize + "/3 Champions Selected";
            string output = "";
            foreach (Vector2 hero in lineup)
            {
                output += "Hero# " + hero.x + "Soul#" + hero.y + " | ";
            }
            debugPartyLineup.text = output;
        }
    }


}
