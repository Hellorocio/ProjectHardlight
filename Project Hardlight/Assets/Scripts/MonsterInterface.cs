using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface MonsterInterface
{
    
    void setActionNames(string[] names);
    string[] getActionNames();
    void callAction1();
    void callAction2();
    void callAction3();
    void callAction4();


}

public abstract class MonsterAI : MonoBehaviour, MonsterInterface
{
    public bool[] states;

    public abstract void callAction1();

    public abstract void callAction2();

    public abstract void callAction3();


    public abstract void callAction4();

    public abstract string[] getActionNames();

    public abstract void setActionNames(string[] names);
}
