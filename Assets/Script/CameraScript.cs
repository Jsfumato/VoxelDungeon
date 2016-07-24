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

    TOUCH_MODE touchMode = TOUCH_MODE.TAP;

    public Camera playerCamera = null;
    float deltaTime = 0.0f;

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

    // Use this for initialization
    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += Time.deltaTime;

        touchMode = TOUCH_MODE.DRAG;

        if (Input.touchCount > 1)
            touchMode = TOUCH_MODE.PINCH;

        //Debug.Log(touchMode.ToString());
        switch (touchMode)
        {
            case TOUCH_MODE.DRAG:
                TapProcecss();
                DragProcess();
                break;
            case TOUCH_MODE.PINCH:
                PinchProcess();
                break;
        }
    }

    void TapProcecss()
    {

    }

    void DragProcess()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            beginPos = Input.mousePosition;
        }
        if (Input.GetButton("Fire1"))
        {
            float cameraHeight = 2f * playerCamera.orthographicSize;
            float cameraWidth = cameraHeight * playerCamera.aspect;

            endPos = Input.mousePosition;

            float xGap = endPos.x - beginPos.x;
            float yGap = endPos.y - beginPos.y;

            beginPos = Input.mousePosition;

            Vector3 cameraRotate = playerCamera.transform.rotation.eulerAngles;
            playerCamera.transform.position += new Vector3(-xGap * Mathf.Cos(cameraRotate.y), 0.0f, -xGap * Mathf.Sin(cameraRotate.y)) * moveSpeed / Screen.width * cameraWidth;
            playerCamera.transform.position += new Vector3(yGap * Mathf.Cos(cameraRotate.y), 0.0f, -yGap * Mathf.Sin(cameraRotate.y)) * moveSpeed / Screen.height * cameraHeight;
        }
    }

    void PinchProcess()
    {
        float baseDistance = Screen.width * scaleSensibility;

        if (Input.touches[0].phase == TouchPhase.Began ||
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

            playerCamera.transform.Rotate(Vector3.up, angle * rotationSpeed, Space.World);

            baseDir = checkVector;

            beginPos = touchPos1;
            endPos = touchPos2;

            if (baseDist > curDist)
            {
                if (playerCamera.orthographicSize >= 5)
                    playerCamera.orthographicSize -= 0.5f;
            }
            else
            {
                if (playerCamera.orthographicSize <= 15)
                    playerCamera.orthographicSize += 0.5f;
            }
        }
    }


    public float ContAngle(Vector3 baseDir, Vector3 targetDir)
    {
        float angle = Vector3.Angle(baseDir, targetDir);

        if (AngleDir(baseDir, targetDir, new Vector3(0.0f, 0.0f, 1.0f)) == -1)
            return -angle;

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
}
