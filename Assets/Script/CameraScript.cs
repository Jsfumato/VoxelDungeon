using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour
{
    private enum STATE : int
    {
        NONE = 0,
        ONE,
        TWO,
        OVER_MAX
    }

    private enum ZOOM
    {
        ZOOM_IN = 0,
        ZOOM_OUT,
        NONE
    }

    delegate void functionPointer();
    Dictionary<STATE, functionPointer> touchDictionary = new Dictionary<STATE, functionPointer>();

    private STATE prevState = STATE.NONE;
    private float prevDistance;

    private Vector2 prevTouchPos0;
    private Vector2 prevTouchPos1;
    private bool isMoving = false;

    public Camera playerCamera = null;

    // Use this for initialization
    void Start ()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void DoCameraControl(Vector2 curTuchPos0, Vector2 curTuchPos1)
    {
        float curDistance = Vector2.Distance(curTuchPos0, curTuchPos1);

        //Debug.LogWarning(isMoving);
        //if (isMoving == false)
        //{
        if (curDistance > prevDistance + 50 * Time.deltaTime)
        {
            //Debug.Log("Zoom In");
            CameraZooming(ZOOM.ZOOM_IN);
            return;
        }
        else if (curDistance < prevDistance - 50 * Time.deltaTime)
        {
            //Debug.Log("Zoom Out");
            CameraZooming(ZOOM.ZOOM_OUT);
            return;
        }
        //}
        if ((curDistance < prevDistance + 50 * Time.deltaTime) && (curDistance > prevDistance - 50 * Time.deltaTime))
        {
            //Debug.Log("Camera Move");
            CameraMove(curTuchPos0, curTuchPos1);
        }
    }

    void CameraZooming(ZOOM zoomState)
    {
        if (zoomState == ZOOM.ZOOM_IN)
        {
            if (playerCamera.orthographicSize >= 5)
                playerCamera.orthographicSize -= 0.5f;
        }
        if (zoomState == ZOOM.ZOOM_OUT)
        {
            if (playerCamera.orthographicSize <= 15)
                playerCamera.orthographicSize += 0.5f;
        }
    }

    void CameraMove(Vector2 curTouchPos0, Vector2 curTouchPos1)
    {
        isMoving = true;

        float cameraHeight = 2f * playerCamera.orthographicSize;
        float cameraWidth = cameraHeight * playerCamera.aspect;

        if (Vector2.Distance(curTouchPos0, prevTouchPos0) > 5)
        {
            prevTouchPos0 = curTouchPos0;
        }
        Vector2 moveVector = curTouchPos0 - prevTouchPos0;

        if (moveVector == Vector2.zero)
        {
            isMoving = false;
            return;
        }

        Vector3 cameraPos = playerCamera.transform.position;
        if (moveVector.x > 0)
        {
            cameraPos.x -= moveVector.x / Screen.width * cameraWidth;// * Time.deltaTime;
            cameraPos.z -= moveVector.x / Screen.width * cameraWidth;// * Time.deltaTime;
        }
        if (moveVector.x < 0)
        {
            cameraPos.z -= moveVector.x / Screen.width * cameraWidth;// * Time.deltaTime;
            cameraPos.x -= moveVector.x / Screen.width * cameraWidth;// * Time.deltaTime;
        }

        if (moveVector.y > 0)
        {
            cameraPos.x += moveVector.y / Screen.height * cameraHeight;// * Time.deltaTime;
            cameraPos.z -= moveVector.y / Screen.height * cameraHeight;// * Time.deltaTime;
        }
        if (moveVector.y < 0)
        {
            cameraPos.x += moveVector.y / Screen.height * cameraHeight;// * Time.deltaTime;
            cameraPos.z -= moveVector.y / Screen.height * cameraHeight;// * Time.deltaTime;
        }

        if (cameraPos.x > 50)
            cameraPos.x = 50;
        if (cameraPos.z > 50)
            cameraPos.z = 50;

        playerCamera.transform.position = cameraPos;
    }
}
