using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour
{
    enum TOUCH_MODE
    {
        TAP = 0,
        DRAG,
        PINCH
    }

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

    TOUCH_MODE touchMode = TOUCH_MODE.TAP;

    delegate void functionPointer();
    Dictionary<STATE, functionPointer> touchDictionary = new Dictionary<STATE, functionPointer>();

    private STATE prevState = STATE.NONE;
    private float prevDistance;

    private Vector2 prevTouchPos0;
    private Vector2 prevTouchPos1;
    private bool isMoving = false;

    public Camera playerCamera = null;
    float deltaTime = 0.0f;
    bool isTouched = false;
    public float validDoubleTapTime = 0.3f;
    int colorIdx = 0;


    Vector2 beginPos = Vector2.zero;
    Vector2 endPos = Vector2.zero;
    public float moveSpeed = 1.0f;
    public float rotationSpeed = 0.5f;


    public float scaleSensibility = 0.01f;
    Vector2 touchPos1;
    Vector2 touchPos2;

    float baseDist = 0.0f;
    float baseFovs = 10.0f;
    Vector2 baseDir = Vector2.zero;
    bool isPinchMode = false;
    
    // Use this for initialization
    void Start ()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }
	
	// Update is called once per frame
	void Update ()
    {
        deltaTime += Time.deltaTime;

        touchMode = TOUCH_MODE.DRAG;

        if (Input.touchCount > 1)
            touchMode = TOUCH_MODE.PINCH;

        Debug.Log(touchMode.ToString());
        switch (touchMode)
        {
            case TOUCH_MODE.TAP:
                TapProcecss();
                break;
            case TOUCH_MODE.DRAG:
                DragProcess();
                break;
            case TOUCH_MODE.PINCH:
                PinchProcess();
                break;
        }
	}

    void TapProcecss()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            if(deltaTime < validDoubleTapTime && isTouched == true && Input.mousePosition.x > (Screen.width*0.5))
            {
             
            }
        }
    }

    void DragProcess()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            beginPos = Input.mousePosition;
        }
        if(Input.GetButton("Fire1"))
        {
            float cameraHeight = 2f * playerCamera.orthographicSize;
            float cameraWidth = cameraHeight * playerCamera.aspect;

            endPos = Input.mousePosition;

            float xGap = endPos.x - beginPos.x;
            float yGap = endPos.y - beginPos.y;

            beginPos = Input.mousePosition;

            playerCamera.transform.position += new Vector3(-xGap * Mathf.Sqrt(2), 0.0f, -xGap * Mathf.Sqrt(2)) * moveSpeed / Screen.width * cameraWidth;
            playerCamera.transform.position += new Vector3(yGap * Mathf.Sqrt(2), 0.0f, -yGap * Mathf.Sqrt(2)) * moveSpeed / Screen.height * cameraHeight;
        }
    }

    void PinchProcess()
    {
        float baseDistance = Screen.width * scaleSensibility;

        if(Input.touches[0].phase == TouchPhase.Began || 
            Input.touches[1].phase == TouchPhase.Began)
        {
            touchPos1 = Input.touches[0].position;
            touchPos2 = Input.touches[1].position;

            baseDist = (touchPos1 - touchPos2).magnitude / baseDistance;
            baseFovs = playerCamera.fieldOfView;

            baseDir = touchPos2 - touchPos1;
        }

        if (Input.touches[0].phase == TouchPhase.Moved ||
            Input.touches[1].phase == TouchPhase.Moved)
        {
            touchPos1 = Input.touches[0].position;
            touchPos2 = Input.touches[1].position;

            float curDist = (touchPos1 - touchPos2).magnitude / baseDistance;
            playerCamera.fieldOfView = baseFovs - (baseDist - curDist);

            Vector3 checkVector = touchPos2 - touchPos1;
            float angle = ContAngle(baseDir, checkVector);

            playerCamera.transform.Rotate(Vector3.forward, angle * rotationSpeed, Space.World);
            baseDir = checkVector;

            beginPos = touchPos1;
            endPos = touchPos2;
        }
    }


    public float ContAngle(Vector3 baseDir, Vector3 targetDir)
    {
        float angle = Vector3.Angle(baseDir, targetDir);

        if(AngleDir(baseDir, targetDir, new Vector3(0.0f, 0.0f, 1.0f)) == -1)
        {
            angle = 360.0f - angle;
            if (angle >= 360.0f)
                angle -= 360.0f;
        }

        return angle;
    }

    public int AngleDir(Vector3 baseDir, Vector3 targetDir, Vector3 up)
    {
        Vector3 prep = Vector3.Cross(baseDir, targetDir);
        float dir = Vector3.Dot(prep, up);

        if (dir > 0.0f)
            return 1;
        else if (dir < 0.0f)
            return -1;

        return 0;

    }


    void GetTouchInput()
    {
        int touchCount = Input.touchCount;
        for(int i = 0; i< touchCount;++i)
        {
            Touch touch = Input.touches[i];
            TouchPhase phase = touch.phase;
            Vector2 touchPos = touch.position;
            Vector2 deltaPos = touch.deltaPosition;

            int findgerId = touch.fingerId;
            float deltaTime = touch.deltaTime;
            int tapCount = touch.tapCount;
        }
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
            //CameraMove(curTuchPos0, curTuchPos1);
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
}
