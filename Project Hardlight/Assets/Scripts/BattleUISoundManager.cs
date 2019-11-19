using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUISoundManager : MonoBehaviour
{
    public AudioClip rpgClick;
    public AudioClip sunlightSFX;
    public AudioClip starlightSFX;
    public AudioClip moonlightSFX;


    public AudioSource myAudio;

    public void PlayClip(AudioClip theClip)
    {
        myAudio.clip = theClip;
        myAudio.Play();
    }
}
