using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{

    public float duration = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        if (duration > 0.0f)
        {
            StartCoroutine("DestroyMe");
        }
    }


    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
