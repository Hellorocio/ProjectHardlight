using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SoulSelection : MonoBehaviour
{
    public HeroSelection heroSelection;
    private bool isSoulSelected = false;
    public Text equipSoulButtonText;
    private int selectedSoul = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectSoul(int a)
    {
        equipSoulButtonText.text = "Equip Soul!";
        selectedSoul = a;
        isSoulSelected = true;
    }

    public void EquipSoul()
    {
        if (isSoulSelected)
        {
            equipSoulButtonText.text = "No Soul Selected";
            int tmp = selectedSoul;
            selectedSoul = -1;
            isSoulSelected = false;
            heroSelection.ReturnFromSoulSelection(tmp);
        }
    }
}
