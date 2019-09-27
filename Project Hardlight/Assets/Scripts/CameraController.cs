using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    int screenWidth;
    int screenHeight;
    public bool isCamMoving;

    public float zoomMax;
    public float zoomMin;

    public float minPanSpeed;
    public float maxPanSpeed;

    float panSpeed;
    float slope;

    public int screenBufferSize;

    Camera myCam;
    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        myCam = gameObject.GetComponent<Camera>();
        slope = (maxPanSpeed - minPanSpeed) / (zoomMax - zoomMin);
    }

    // Update is called once per frame
    void Update()
    {
        MoveCam();
        ZoomCam();
    }

    void ZoomCam()
    {
        float currentScrollDelta = Input.mouseScrollDelta.y * -1;
        
        if (currentScrollDelta != 0)
        {
            float currentZoom = myCam.orthographicSize;
            if (currentScrollDelta + currentZoom > zoomMax)
            {
                currentZoom = zoomMax;
            } else if(currentScrollDelta + currentZoom < zoomMin)
            {
                currentZoom = zoomMin;
            } else
            {
                myCam.orthographicSize += currentScrollDelta;
            }
            
        }
    }

    

    void MoveCam()
    {
        //panSpeed = (myCam.orthographicSize) * ((minPanSpeed / (maxPanSpeed - minPanSpeed)));

        panSpeed = slope * (myCam.orthographicSize - zoomMin) + minPanSpeed;

        Debug.Log(panSpeed);
        Vector3 camPos = transform.position;
        if (Input.mousePosition.x > screenWidth - screenBufferSize)
        {
            isCamMoving = true;
            camPos.x += panSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.x < screenBufferSize)
        {
            isCamMoving = true;
            camPos.x -= panSpeed * Time.deltaTime;
        }

        else if (Input.mousePosition.y > screenHeight - screenBufferSize)
        {
            isCamMoving = true;
            camPos.y += panSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.y < screenBufferSize)
        {
            isCamMoving = true;
            camPos.y -= panSpeed * Time.deltaTime;
        }
        else
        {
            isCamMoving = false;
        }
        gameObject.transform.position = camPos;
    }
}
