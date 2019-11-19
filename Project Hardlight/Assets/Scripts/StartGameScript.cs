using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameScript : MonoBehaviour
{
    public GameManager gameManager;
    public AudioSource audio;
    // Start is called before the first frame update
    public void StartNewGame()
    {
        StartCoroutine(StartingGameRoutine());

    }

    IEnumerator StartingGameRoutine()
    {
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        gameManager.NewCampaign();
    }
}
