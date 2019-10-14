using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public FighterMove selectedFighter;
    public GameObject moveLoc;

    private void Start()
    {
        GameManager.Instance.SetCameraControls(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Needs more moveloc references or static variable, otherwise ordering a second unity overwrites first's moveloc
            // Also find bug where heroes disappear
            //Set state to move or update target
            Vector3 pos = Input.mousePosition;
            pos = Camera.main.ScreenToWorldPoint(pos);
            GameObject newMoveLoc = Instantiate(moveLoc);
            newMoveLoc.transform.position = new Vector3(pos.x, pos.y, 2);
            selectedFighter.StartMovingCommandHandle(newMoveLoc.transform);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.EnterMap();
        }
    }
}
