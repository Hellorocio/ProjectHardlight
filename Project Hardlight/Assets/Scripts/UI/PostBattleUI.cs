using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PostBattleUI : MonoBehaviour
{
    [Header("General UI")]
    public AudioClip winSound;
    public AudioClip loseSound;

    public string winTitle = "Victory";
    public string loseTitle = "RIP";

    public Color winTextColor;
    public Color loseTextColor;

    public TextMeshProUGUI title;
    public GameObject continueButton;
    public GameObject enableOnVictory;

    [Header("Fragments UI")]
    public GameObject fragmentsGained;
    public TextMeshProUGUI[] fragmentNums; //[0] = sun, [1] = moon, [2] = star

    [Header("Souls UI")]
    public GameObject soulsGained;
    public TextMeshProUGUI soulsGainedText;
    public SoulIcon[] soulIcons;
    public SoulIcon singleSoul; // this one is bigger, so it looks better when you just gain 1 soul
    
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
            title.color = winTextColor;
            if (winSound != null && GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().clip = winSound;
                GetComponent<AudioSource>().Play();
            }

            enableOnVictory.SetActive(true);

            // display starter souls as gained souls at the end of the tutorial, woo
            if (TutorialManager.Instance.tutorialEnabled && TutorialManager.Instance.currentTutorialLevel == 1)
            {
                ShowGainedSouls(true);
            }

            // possibly gain souls at the end of levels
            if (!TutorialManager.Instance.tutorialEnabled)
            {
                ShowGainedSouls();
            }
        }
        else
        {
            title.text = loseTitle;
            title.color = loseTextColor;
            enableOnVictory.SetActive(false);
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
        enableOnVictory.SetActive(false);
        soulsGained.SetActive(false);
        GetComponent<Image>().enabled = false;

        if (GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().Stop();
        }
    }

    /// <summary>
    /// This was used generate and display fragments gained at the end of a battle and display it
    /// We aren't using it right now because fragments aren't used right now
    /// </summary>
    public void ShowGainedFragments()
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

    /// <summary>
    /// Used to generate and display the souls gained at the end of a battle
    /// </summary>
    /// <param name="showStarterSouls">This is used in the turorial battle, it displays starter souls (and generates them if needed)</param>
    public void ShowGainedSouls(bool showStarterSouls = false)
    {
        if (showStarterSouls)
        {
            // display starter souls (this is only used at the end of the second tutorial battle)
            if (GameManager.Instance.souls.Count < 3)
            {
                GameManager.Instance.GetStarterSouls();
            }

            List<Soul> souls = GameManager.Instance.souls;
            for (int i = 0; i < soulIcons.Length; i++)
            {
                if (i < souls.Count)
                {
                    soulIcons[i].SetSoul(souls[i]);
                    soulIcons[i].ShowIcon(true);
                }
                else
                {
                    soulIcons[i].ShowIcon(false);
                }
            }

            soulsGainedText.text = "You gained " + souls.Count + " souls!";
            soulsGained.SetActive(true);
            title.gameObject.SetActive(false);
        }
        else
        {
            // generate new souls and display them
            Soul[] newSouls = GameManager.Instance.AddSoulsAfterBattle();
            if (newSouls.Length > 0)
            {
                soulsGained.SetActive(true);
                title.gameObject.SetActive(false);

                for (int i = 0; i < soulIcons.Length; i++)
                {
                    soulIcons[i].ShowIcon(false);
                }

                if (newSouls.Length == 1)
                {
                    // just show 1 big soul
                    singleSoul.SetSoul(newSouls[0]);
                    singleSoul.ShowIcon(true);
                    soulsGainedText.text = "You got a new soul!";
                }
                else
                {
                    // gained multiple souls
                    for (int i = 0; i < soulIcons.Length; i++)
                    {
                        if (i < newSouls.Length)
                        {
                            soulIcons[i].SetSoul(newSouls[i]);
                            soulIcons[i].ShowIcon(true);
                        }
                    }
                    soulsGainedText.text = "You gained " + newSouls.Length + " souls!";
                }
            }
        }
    }
}
