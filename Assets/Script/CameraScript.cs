using UnityEngine;
using UnityEngine.UI;
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

    public GameObject blockDataManagerObj = null;
    private BlockDataManager blockDataManagerScript = null;

    public Camera playerCamera = null;
    //float deltaTime = 0.0f;

    Vector2 beginPos = Vector2.zero;
    Vector2 endPos = Vector2.zero;
    public float moveSpeed = 1.0f;
    public float rotationSpeed = 0.5f;

    public float scaleSensibility = 0.01f;
    Vector2 touchPos1;
    Vector2 touchPos2;

    float baseDist = 0.0f;
    //float baseFovs = 10.0f;
    float baseOrtho = 10;

    //Vector2 baseDir = Vector2.zero;



    public GameObject indicator = null;
    private Vector3 prevSelectedPos = new Vector3();



    // Use this for initialization
    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        indicator = Instantiate(indicator) as GameObject;
        blockDataManagerScript = blockDataManagerObj.GetComponent<BlockDataManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //deltaTime += Time.deltaTime;

        touchMode = TOUCH_MODE.DRAG;

        if (Input.touchCount > 1)
            touchMode = TOUCH_MODE.PINCH;

        switch (touchMode)
        {
            case TOUCH_MODE.DRAG:
                DragProcess();
                break;

            case TOUCH_MODE.PINCH:
                PinchProcess();
                break;
        }
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
            baseOrtho = playerCamera.orthographicSize;
        }

        if (Input.touches[0].phase == TouchPhase.Moved ||
            Input.touches[1].phase == TouchPhase.Moved)
        {
            touchPos1 = Input.touches[0].position;
            touchPos2 = Input.touches[1].position;

            float curDist = (touchPos1 - touchPos2).magnitude / baseDistance;
            playerCamera.orthographicSize = baseOrtho + (baseDist - curDist);

            if (playerCamera.orthographicSize > 15.0f)
                playerCamera.orthographicSize = 15.0f;
            if (playerCamera.orthographicSize < 5.0f)
                playerCamera.orthographicSize = 5.0f;

            beginPos = touchPos1;
            endPos = touchPos2;
        }
    }













    // http://answers.unity3d.com/questions/561909/how-to-order-raycastall-by-distance-on-ios.html
    public Vector3 SelectBlock(Vector2 touchPosition)
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, 0));

        List<NewRaycastResult> hitList = new List<NewRaycastResult>();

        hitList.Clear();
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            hitList.Add(new NewRaycastResult(hit));
        }

        hitList.Sort();

        if (hitList.Capacity == 0)
        {
            indicator.transform.position = new Vector3(0, -1000, 0);
            return prevSelectedPos;
        }

        Debug.Log(hitList[0].collider.transform.gameObject.layer);

        if (hits[0].collider.gameObject.layer == 20)
        {
            Vector3 bPos = hits[0].collider.gameObject.transform.position;
            int x = (int)(bPos.x - 7.5f);
            int y = (int)(bPos.y - 0.5f);
            int z = (int)(bPos.z - 7.5f);

            blockDataManagerScript.SetBaseBlock(new BasePos(x, y, z));
            blockDataManagerScript.SetAddBaseObj();
        }

        if (hits[0].collider.gameObject.layer != 10)
        {
            return prevSelectedPos;
        }

        Vector3 pos = hits[0].transform.position;
        //pos.y += 1.0f;

        Vector3 normal = hits[0].normal;
        normal = hits[0].transform.TransformDirection(normal);

        if (normal == hits[0].transform.up)
            pos = new Vector3(pos.x, pos.y + 1, pos.z);

        if (normal == -hits[0].transform.up)
            pos = new Vector3(pos.x, pos.y - 1, pos.z);

        if (normal == hits[0].transform.forward)
            pos = new Vector3(pos.x, pos.y, pos.z + 1);

        if (normal == -hits[0].transform.forward)
            pos = new Vector3(pos.x, pos.y, pos.z - 1);

        if (normal == hits[0].transform.right)
            pos = new Vector3(pos.x + 1, pos.y, pos.z);

        if (normal == -hits[0].transform.right)
            pos = new Vector3(pos.x - 1, pos.y, pos.z);

        prevSelectedPos = pos;
        indicator.transform.position = pos;
        indicator.transform.rotation = Quaternion.identity;


        Debug.Log(pos);
        return pos;
    }
}

// RatcastResult redefine
public class NewRaycastResult : System.IComparable<NewRaycastResult>
{
    public float distance;
    public Collider collider;
    public Vector2 textureCoord;
    public NewRaycastResult(RaycastHit hit)
    {
        distance = hit.distance;
        collider = hit.collider;
        textureCoord = hit.textureCoord;
    }

    public int CompareTo(NewRaycastResult other)
    {
        return distance.CompareTo(other.distance);
    }
}