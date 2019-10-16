using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CutsceneLevel : MonoBehaviour
{
    public string cutsceneName;
    public Sprite bgImage;
    public TextAsset cutsceneText;
    public UnityEvent onCutsceneEnd;
}
