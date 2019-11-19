using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : Singleton<DialogueManager>
{
    public BattleUISoundManager soundManager;
    public GameObject box;
    public TextMeshProUGUI text;
    public GameObject nameLabel;

    public TextAsset script;

    public float scrollSpeed;

    public Image image;
    
    public UnityEvent onDialogueEnd;

    public List<ProfileSpriteData> profileDatas;
    Dictionary<string, Sprite> profileDict;

    public string commandPrefix = ">>";

    bool rInput;
    bool oldRInput;

    private bool initialized = false;
    
    private IEnumerator dialogueLoop = null;
    public void InitializeDialogueManager()
    {
        // convert profileDatas -> profileDict
        profileDict = new Dictionary<string, Sprite>();
        foreach (var data in profileDatas)
        {
            profileDict[data.name] = data.sprite;
        }

        ShowBox(false);
        ShowImage(false);
        ShowName(false);

        initialized = true;
    }

    private void Update()
    {
        rInput = Input.GetAxisRaw("Submit") > 0;
        if (!rInput)
        {
            rInput = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && dialogueLoop != null)
        {
            EndDialogue();
        }
    }

    public void StartDialogue(TextAsset inputScript)
    {

        if (!initialized)
        {
            InitializeDialogueManager();
        }
        // Check for already started dialogues
        if (dialogueLoop != null)
        {
            Debug.Log("Dialogue already playing. Stopping it now. This is bad. Going to unsubscribe other events for you too.");
            EndDialogue();
        }
        
        if (inputScript != null)
        {
            script = inputScript;
            ShowBox(true);

            dialogueLoop = RunDialogue();
            StartCoroutine(dialogueLoop);
        }
        else
        {
            onDialogueEnd.Invoke();
        }
    }

    // Called by coroutine RunDialogue
    public void EndDialogue()
    {
        //string tmp = script.text;
        text.text = "";
        ShowBox(false);
        ShowImage(false);
        ShowName(false);
        if (dialogueLoop != null)
        {
            StopCoroutine(dialogueLoop);
        }
        dialogueLoop = null;
        onDialogueEnd.Invoke();
    }

    IEnumerator RunDialogue()
    {
        // get individual lines from text asset
        string[] lines = script.text.Split('\n');
        bool didInput = false;
        
        for (var l = 0; l < lines.Length; l++)
        {
            var line = lines[l];

            if (line.StartsWith(commandPrefix))
            {
                DoProfileCommand(line.Substring(commandPrefix.Length));
                continue;
            }
            else if (line == "" || line == "\n" || line == "\r")
            {
                break;
            }

            var sb = new StringBuilder();

            foreach (char c in line)
            {
                didInput = CheckRPressed();

                // skip the text scroll if the speed is 0 or if there is an input
                if (scrollSpeed == 0 || didInput)
                {
                    text.text = line;
                    break;
                }

                // display one character at a time
                sb.Append(c);
                text.text = sb.ToString();
                yield return new WaitForSeconds(scrollSpeed);
            }

            // wait for input to advance to next line
            didInput = false;
            while (!didInput)
            {
                didInput = CheckRPressed();
                
                yield return 0;
            }
            soundManager.PlayClip(soundManager.rpgClick);
        }

        EndDialogue();
    }

    bool CheckRPressed()
    {
        
        bool result = rInput && !oldRInput;
        oldRInput = rInput;
        return result;
    }

    void DoProfileCommand(string line)
    {
        // for some reason i have to delete an empty character at the end of the command string
        line = line.TrimEnd();

        if (profileDict.ContainsKey(line))
        {
            ShowImage(true);
            ShowName(true, line);
            image.sprite = profileDict[line];
        }
        else
        {
            ShowImage(false);
            ShowName(false);
            //Debug.Log("No sprite corresponding to command " + line);
            return;
        }
    }

    private void ShowBox(bool shouldShow)
    {
        if (!shouldShow && dialogueLoop != null)
        {
            StopCoroutine(dialogueLoop);
        }

        box.SetActive(shouldShow);
    }

    private void ShowImage(bool shouldShow)
    {
        image.gameObject.SetActive(shouldShow);
        image.enabled = shouldShow;
    }

    private void ShowName (bool shouldShow, string name = "")
    {
        if (nameLabel != null)
        {
            nameLabel.SetActive(shouldShow);

            if (shouldShow)
            {
                nameLabel.GetComponentInChildren<TextMeshProUGUI>().text = name;
            }
        }
    }
}


[System.Serializable]
public struct ProfileSpriteData
{
    public string name;
    public Sprite sprite;
}