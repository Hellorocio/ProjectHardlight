using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class just holds the different lines between map nodes (dotted and solid)
/// And has fuctions for revealing the different ones
/// </summary>
public class MapLine : MonoBehaviour
{
    public GameObject[] mapLines; //[0] = dotted, [1] = solid

    public void ShowDottedLine ()
    {
        mapLines[0].SetActive(true);
    }

    public void ShowSolidLine ()
    {
        mapLines[1].SetActive(true);
    }
}
