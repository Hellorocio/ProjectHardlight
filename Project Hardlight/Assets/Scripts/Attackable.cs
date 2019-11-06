using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    public BuffBar buffBar;
    
    public void AddBuff(Buff buff)
    {
        BuffInstance buffInstance = gameObject.AddComponent(typeof(BuffInstance)) as BuffInstance;
        buffInstance.SetBuff(buff);
        buffInstance.StartBuff();
        
        // Add to buff bar
        Debug.Log(gameObject.name);
        buffBar.AddBuffInstance(buffInstance);
    }
    
    // Called by the buff to tell you it's done
    public void RemoveBuff(BuffInstance buffInstance)
    {
        buffBar.RemoveBuffInstance(buffInstance);
    }
}
