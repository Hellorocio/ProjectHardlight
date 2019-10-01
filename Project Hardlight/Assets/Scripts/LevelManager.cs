using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{
    public Text dialoguePanel;
    public string winText = "We did it!";
    public string loseText = "Aw man!";
    public AudioClip loseMusic;

    private BattleManager battleManager;

    /// <summary>
    /// Subscribe to OnLevelEnd event so this script is notified when the level ends
    /// </summary>
    private void OnEnable()
    {
        GameObject battleManagerObj = GameObject.Find("BattleManager");
        if (battleManagerObj != null)
        {
            battleManager = battleManagerObj.GetComponent<BattleManager>();
            battleManager.OnLevelEnd += LevelEndEvent;
        }
    }

    /// <summary>
    /// Unsubscribe from OnLevelEnd event to avoid memory leaks
    /// </summary>
    private void OnDisable()
    {
        if (battleManager != null)
        {
            battleManager.OnLevelEnd -= LevelEndEvent;
        }
    }

    /// <summary>
    /// Called when the level ends, displays ending dialogue popup
    /// </summary>
    /// <param name="herosWin"></param>
    public void LevelEndEvent (bool herosWin)
    {
        if (herosWin)
        {
            dialoguePanel.text = winText;
            GameManager.Instance.WinLevel();
        }
        else
        {
            dialoguePanel.text = loseText;

            //play lose music
            AudioSource camera = GameObject.Find("Main Camera").GetComponent<AudioSource>();
            if (camera != null && loseMusic != null)
            {
                camera.clip = loseMusic;
                camera.Play();
            }
        }
        dialoguePanel.transform.parent.gameObject.SetActive(true);
    }

    public void ReturnToMap()
    {
        GameManager.Instance.LoadScene(1);
    }
}
