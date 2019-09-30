using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindyLevelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       GameManager.Instance.StartCampaign();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
