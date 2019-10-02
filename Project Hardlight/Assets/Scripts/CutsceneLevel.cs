using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CutsceneLevel : MonoBehaviour
{ 
    public TextAsset cutsceneText;
    
    public UnityEvent onCutsceneEnd;

    void Start()
    {
        StartCoroutine(StartCutsceneWithDelay());
    }

    IEnumerator StartCutsceneWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log("Cutscene Level | Starting Cutscene");
        DialogueManager.Instance.onDialogueEnd.AddListener(CutsceneEnded);
        DialogueManager.Instance.StartDialogue(cutsceneText);
    }

    private void CutsceneEnded() 
    {
        onCutsceneEnd.Invoke();
        DialogueManager.Instance.onDialogueEnd.RemoveListener(CutsceneEnded);
    }

    public void EnterMap()
    {
        GameManager.Instance.EnterMap();
    }

    public void EnterTutorialBattle()
    {
        GameManager.Instance.EnterTutorialBattle();
    }
}
