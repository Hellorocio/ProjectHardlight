using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VesselCompletion : MonoBehaviour
{

    public Sprite none;
    public Sprite silver;
    public Sprite gold;

    public Image status;
    public GameObject fade;

    public Difficulty bestWon = Difficulty.NONE;
    
    // Start is called before the first frame update
    public void DifficultyWin(Difficulty difficulty)
    {
        
        // Hardcore always replaces
        if (difficulty == Difficulty.HARDCORE)
        {
            bestWon = Difficulty.HARDCORE;
            status.sprite = gold;
            fade.SetActive(false);
        } else if (difficulty == Difficulty.NORMAL && bestWon != Difficulty.HARDCORE)
        {
            bestWon = Difficulty.NORMAL;
            status.sprite = silver;
            fade.SetActive(false);
        }
    }
}
