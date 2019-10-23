using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffInstance : MonoBehaviour
{
    public Buff buff;

    // Used to be able to see how much time is left on the buff in inspector
    public float timeLeft = 0.0f;
    private bool timingBuff = false;

    void Update()
    {
        if (timingBuff = true)
        {
            timeLeft -= Time.deltaTime;
        }
    }
    public void SetBuff(Buff buff)
    {
        this.buff = buff;
    }
    
    // Call when creating buff and ready to start
    public void StartBuff()
    {
        buff.InitializeBuff(gameObject);
        if (buff.buffDuration != 0.0f)
        {
            timeLeft = buff.buffDuration;
            timingBuff = true;
            StartCoroutine(BuffLoop());
        }
    }

    // Call explicitly if not given a duration
    public void EndBuff()
    {
        timingBuff = false;
        buff.CleanupBuff(gameObject);
        
        // Tell Fighter you're done (to update BuffBar mostly)
        GetComponent<Fighter>().RemoveBuff(this);
    }

    IEnumerator BuffLoop()
    {
        yield return new WaitForSeconds(buff.buffDuration);
        EndBuff();
        Destroy(this);
    }
}
