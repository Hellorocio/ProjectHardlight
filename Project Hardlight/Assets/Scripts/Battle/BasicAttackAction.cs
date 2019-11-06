using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAttackAction : MonoBehaviour
{
    public abstract void DoBasicAttack(Fighter sourceFighter, GameObject target);

    public string title;
    public int range;
    public int damage;
    public float frequency = 1;
    public string description;
    public AudioClip sfx;
}
