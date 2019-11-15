using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach to a GO with an attackable to say something when low
public class NotifHealthLow : MonoBehaviour
{
    public string text;
    public float percentThreshold = 0.5f;
    public float notificationDuration = 3.0f;

    [Header("Donut touch")]
    public bool triggered = false;

    public Attackable attackable;

    void Start()
    {
        attackable = GetComponent<Attackable>();
        if (attackable == null)
        {
            Debug.Log("WARNING: NotifHealthLow trigger on something without an Attackable");
        }

        if (text == "")
        {
            Debug.Log("WARNING: NotifHealthLow without any text to say");
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!triggered && attackable.GetHealth() < (percentThreshold * attackable.GetMaxHealth()))
        {
            triggered = true;
            GameManager.Instance.SayTop(text, notificationDuration);
        }
    }
}
