using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSpot : MonoBehaviour
{
    public CombatInfo.Team team = CombatInfo.Team.Hero;

    public Buff healOverTimeBuff;

    public Dictionary<Attackable, BuffInstance> buffed;

    public float duration = 0.0f;
    
    public void Start()
    {
        buffed = new Dictionary<Attackable, BuffInstance>();
        
        if (duration > 0.0f)
        {
            StartCoroutine("DestroyMe");
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        Attackable attackable = other.gameObject.GetComponent<Attackable>();
        if (attackable != null)
        {
            if (attackable.team == CombatInfo.Team.Hero)
            {
                // Give them the buff
                buffed.Add(attackable, attackable.AddBuff(healOverTimeBuff));
            }
        }
    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
        Attackable attackable = other.gameObject.GetComponent<Attackable>();
        if (attackable != null)
        {
            if (attackable.team == CombatInfo.Team.Hero)
            {
                // Remove their buff
                buffed[attackable].EndBuff();
                buffed.Remove(attackable);
            }
        }
    }

    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(duration);
        foreach (KeyValuePair<Attackable, BuffInstance> entry in buffed)
        {
            if (entry.Key != null && entry.Value != null)
            {
                entry.Value.EndBuff();
            }
        }
        Destroy(gameObject);
    }
}
