using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnCooldown : MonoBehaviour
{
    public float timer = 5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CooldownTimer());
        
    }

    IEnumerator CooldownTimer ()
    {
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }
}
