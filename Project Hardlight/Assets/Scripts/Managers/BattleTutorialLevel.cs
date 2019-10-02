using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTutorialLevel : MonoBehaviour
{
    public TextAsset loadoutTutorialDialogue;
    
    void Start()
    {
        UIManager.Instance.SetLoadoutUI(true);
        DialogueManager.Instance.StartDialogue(loadoutTutorialDialogue);
    }
}
