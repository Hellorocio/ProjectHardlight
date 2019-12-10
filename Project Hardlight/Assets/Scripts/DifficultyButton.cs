using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    public TextMeshProUGUI myText;
    public TextMeshProUGUI otherText;

    public void Clicked()
    {
        myText.fontStyle = FontStyles.Underline;
        otherText.fontStyle = FontStyles.Normal;
    }
}
