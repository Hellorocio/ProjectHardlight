using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PostBattleUI : MonoBehaviour
{
    public AudioClip winSound;
    public AudioClip loseSound;
    public string winTitle = "Victory";
    public string loseTitle = "RIP";
    public TextMeshProUGUI title;
    public GameObject fragmentsGained;
    public TextMeshProUGUI[] fragmentNums; //[0] = sun, [1] = moon, [2] = star
    public GameObject continueButton;
    
    private bool storeWin;

    /// <summary>
    /// Shows post battle UI, tells GameManager to generate fragments and displays those fragments gained
    /// </summary>
    /// <param name="win"></param>
    public void StartPostBattle (bool win)
    {
        storeWin = win;
        title.gameObject.SetActive(true);
        fragmentsGained.SetActive(false);
        continueButton.SetActive(true);
        GetComponent<Image>().enabled = true;

        if (win)
        {
            title.text = winTitle;
            if (winSound != null && GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().clip = winSound;
                GetComponent<AudioSource>().Play();
            }

            /*
            if (!TutorialManager.Instance.tutorialEnabled)
            {
                int[] newFragments = GameManager.Instance.AddFragments();

                // set fragments gained list
                for (int i = 0; i < fragmentNums.Length; i++)
                {
                    if (newFragments.Length > i && newFragments[i] != 0)
                    {
                        fragmentNums[i].text = "X " + newFragments[i];
                        fragmentNums[i].transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        fragmentNums[i].transform.parent.gameObject.SetActive(false);
                    }
                }

                fragmentsGained.SetActive(true);
            }
            */
        }
        else
        {
            title.text = loseTitle;
            if (loseSound != null && GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().clip = loseSound;
                GetComponent<AudioSource>().Play();
            }
        }
    }

    /// <summary>
    /// Continues game flow by calling EndFighting in gameManager
    /// </summary>
    public void PressPostBattleContinueButton ()
    {
        GameManager.Instance.EndFighting(storeWin);
        DisablePostBattleUI();
    }

    /// <summary>
    /// Disables the UI, called on continue button pressed or skip tutorial button pressed;
    /// </summary>
    public void DisablePostBattleUI ()
    {
        title.gameObject.SetActive(false);
        fragmentsGained.SetActive(false);
        continueButton.SetActive(false);
        GetComponent<Image>().enabled = false;
    }
}
