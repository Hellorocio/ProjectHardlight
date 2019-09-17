using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnCooldown : MonoBehaviour
{
    public float timer = 5f;
    public bool destroyAfterCooldown = true;
    
    void OnEnable()
    {
        StartCoroutine(CooldownTimer());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Destroys or disables gameobject after timer seconds have passed
    /// </summary>
    /// <returns></returns>
    IEnumerator CooldownTimer ()
    {
        yield return new WaitForSeconds(timer);
        if (destroyAfterCooldown)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
