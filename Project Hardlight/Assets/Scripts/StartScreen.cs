using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public void Start()
    {
        GameManager.Instance.Initialize();
    }
}
