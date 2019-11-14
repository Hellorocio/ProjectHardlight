using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stops movement after duration
public class KnockingBackBuff : Buff
{
    public override void CleanupBuff(GameObject affectedObject)
    {
        affectedObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
