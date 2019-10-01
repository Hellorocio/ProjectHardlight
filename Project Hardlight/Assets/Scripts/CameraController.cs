using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IPointerClickHandler
{
    public bool enabled;

    int screenWidth;
    int screenHeight;
    public bool isCamMoving;

    public float zoomMax;
    public float zoomMin;

    public float minPanSpeed;
    public float maxPanSpeed;

    float panSpeed;
    float slope;

    Vector2 lastPos;
    public float maxMouseSensitivity;
    public float minMouseSensitivity;
    private float mouseSensitivity;

    float mouseSlope;

    public int screenBufferSize;

    Camera myCam;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;

        screenWidth = Screen.width;
        screenHeight = Screen.height;
        slope = (maxPanSpeed - minPanSpeed) / (zoomMax - zoomMin);
        mouseSlope = (maxMouseSensitivity - minMouseSensitivity) / (zoomMax - zoomMin);
    }

    public void Initialize()
    {
        myCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            MoveCam();
            ZoomCam();
        }
    }


    /// <summary>
    /// Handles double-clicking on UI elements (maybe only in the gameobject/children UI components?)
    /// Right now only seems to respond to minimap double-clicks but this could be incorrect
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Debug.Log("double click");
        }
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
        mouseSensitivity = mouseSlope * (myCam.orthographicSize - zoomMin) + minMouseSensitivity;
        //Debug.Log(panSpeed);
        Vector3 camPos = myCam.gameObject.transform.position;

        if (Input.GetMouseButtonDown(2)) // Pan if the user hold down the scroll wheel
        {
            lastPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            //Debug.Log("Panning");
            Vector2 tmp = Input.mousePosition;
            tmp -= lastPos;

            if (Mathf.Abs(tmp.x) < .01)
            {
                tmp.x = 0;
            }
            if (Mathf.Abs(tmp.y) < .01)
            {
                tmp.y = 0;
            }
            // Debug.Log("Current: " + gameObject.transform.position.x + " | " + gameObject.transform.position.y);
            //Debug.Log("New: " + tmp.x + " | " + tmp.y);
            // Debug.Log("Cumulative: " + transform.position.x + (tmp.x * mouseSensitivity) + " " + transform.position.y + (tmp.y * mouseSensitivity));
            //Vector3 adjusted = new Vector3(transform.position.x + (tmp.x * mouseSensitivity), transform.position.y + (tmp.y * mouseSensitivity), transform.position.z);
            camPos.x -= tmp.x * mouseSensitivity;
            camPos.y -= tmp.y * mouseSensitivity;
            lastPos = Input.mousePosition;

        }
        else
        {
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
            
        }
        myCam.gameObject.transform.position = camPos;
    }
}
