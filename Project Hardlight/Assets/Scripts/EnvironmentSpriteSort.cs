using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpriteSort : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(SpriteRenderer s in gameObject.GetComponentsInChildren<SpriteRenderer>()){
             s.sortingOrder = Mathf.RoundToInt(s.gameObject.transform.position.y * 100) * -1;
        }
    }
}
