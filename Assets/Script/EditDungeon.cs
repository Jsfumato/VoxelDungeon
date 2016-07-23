using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EditDungeon : MonoBehaviour
{
    const int MAX_WORLD_SIZE = 32;

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
    Dictionary<STATE, functionPointer> touchDictionary;

    private STATE prevState = STATE.NONE;
    private float prevDistance;

    private Vector2 prevTouchPos0;
    private Vector2 prevTouchPos1;
    private bool isMoving = false;

    private List<BlockInfo> sparseBlockInfo = new List<BlockInfo>();
    private Stack<BlockInfo> updateBlockInfo = new Stack<BlockInfo>();
    private Dictionary<BLOCK_TYPE, GameObject> blockPrefabDic = new Dictionary<BLOCK_TYPE, GameObject>();

    private Block[] worldBlockInfo = new Block[MAX_WORLD_SIZE * MAX_WORLD_SIZE * MAX_WORLD_SIZE];

    private Vector3 prevSelectedPos = new Vector3();

    public Camera playerCamera = null;
    public GameObject indicator = null;
    public GameObject emptyColli = null;

    public Button blockButton = null;
    public Button saveButton = null;
    public GameObject PopUpMenu = null;
    public GameObject Canvas = null;
    public GameObject BlockList = null;

    private Object[] prefabs;

    private GameObject selectdBlock = null;
    private GameObject emptyColliders = null;
    public GameObject testBlock = null;
    public GameObject ButtonPrefab = null;

    void Awake()
    {
        touchDictionary = new Dictionary<STATE, functionPointer>();
        if (playerCamera == null)
            playerCamera = Camera.main;

        indicator = Instantiate(indicator) as GameObject;
        PopUpMenu = Instantiate(PopUpMenu) as GameObject;
        emptyColliders = new GameObject();
        emptyColliders.name = "EmptyColliders";

        // need "resources" folder
        // Loads list of prefabs in "Resources/Prefab" path
        prefabs = Resources.LoadAll("Prefab", typeof(GameObject));

        SetBlockList(prefabs, BlockList);

        SetPopupGUI(prefabs, Canvas);

        SetBaseBlock(prefabs);

        saveButton.onClick.AddListener(SaveEditData);
        blockButton.onClick.AddListener(() => SelectMenu(blockButton));

        touchDictionary.Add(STATE.NONE, TouchByNone);
        touchDictionary.Add(STATE.ONE, TouchByOne);
        touchDictionary.Add(STATE.TWO, TouchByTwo);
    }

    void Update ()
    {
        DoTouchAction(Input.touchCount);

        // update block info
        UpdateRender();
    }

    void DoTouchAction(int touchCount)
    {
        STATE key = (STATE)touchCount;

        if (touchCount >= (int)STATE.OVER_MAX)
            key = STATE.TWO;

        foreach (Touch touch in Input.touches)
        {
            int pointerID = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(pointerID))
            {
                // at least on touch is over a canvas UI
                return;
            }
        }
        touchDictionary[key]();
    }

    void TouchByNone()
    {
        prevState = STATE.NONE;
        return;
    }

    void TouchByOne()
    {
        Vector3 selectedPos = SelectBlock(Input.GetTouch(0).position);

        prevState = STATE.ONE;
        return;
    }

    void TouchByTwo()
    {
        if (prevState != STATE.TWO)
        {
            prevDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            prevState = STATE.TWO;
            return;
        }

        if (prevState == STATE.TWO)
        {
            float curDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

            DoCameraControl(Input.GetTouch(0).position, Input.GetTouch(1).position);

            prevDistance = curDistance;
            prevTouchPos0 = Input.GetTouch(0).position;
            prevTouchPos1 = Input.GetTouch(1).position;
            prevState = STATE.TWO;
            return;
        }
    }
    ///////////////////////////////////////////////////////////////////////
    //                        UI Action Methods                          //
    ///////////////////////////////////////////////////////////////////////

    void SelectMenu(Button button)
    {
        Animator panelAnimator = button.GetComponentInParent<Animator>();

        if (panelAnimator.GetBool("isHidden") == true)
            panelAnimator.SetBool("isHidden", false);
        else
            panelAnimator.SetBool("isHidden", true);
    }

    void SaveEditData()
    {
        Debug.Log("Save Data!");

    }

    void SetPopupGUI(Object[] prefabList, GameObject panel)
    {
        PopUpMenu = (GameObject)Instantiate(PopUpMenu);
        PopUpMenu.GetComponent<RectTransform>().SetParent(panel.transform, false);
        PopUpMenu.SetActive(true);

        //Debug.Log(PopUpMenu.transform.childCount);
        foreach(Transform child in PopUpMenu.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() => SetBlock(prevSelectedPos, BLOCK_TYPE.SKULL));
        }
    }

    void SetBlockList(Object[] prefabList, GameObject panel)
    {
        int position = 0;
        // test code
        foreach(GameObject prefab in prefabList)
        {
            // Block Layer == 10
            if (prefab.layer != 10)
                continue;

            if (prefab.name == "Empty")
                continue;

            AppendBlockToList(prefab, panel, position++);
        }
    }

    void AppendBlockToList(GameObject blockPrefab, GameObject blockList, int position)
    {
        Debug.LogWarning("Append blockPrefabDic : " + blockPrefab.name);
        // Add to Dictionary
        BLOCK_TYPE bType = blockPrefab.GetComponent<Block>().bType;
        blockPrefabDic.Add(bType, blockPrefab);

        // GUI
        Debug.Log("Append Block Prefab : " + blockPrefab.name);

        GameObject btn = Instantiate(ButtonPrefab) as GameObject;
        btn.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.9f - (float)(position) / 10);
        btn.GetComponent<RectTransform>().anchorMax = new Vector2(1.0f, 1.0f - (float)(position) / 10);
        
        btn.transform.SetParent(blockList.transform, false);
        btn.GetComponentInChildren<Text>().text = blockPrefab.name;

        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Block : " + blockPrefab.name);
            selectdBlock = blockPrefab;
        });
    }

    ///////////////////////////////////////////////////////////////////////
    //                       Edit Action Methods                         //
    ///////////////////////////////////////////////////////////////////////

    void UpdateRender()
    {
        while (updateBlockInfo.Count != 0)
        {
            BlockInfo block = updateBlockInfo.Pop();
            Vector3 position = new Vector3(block.posX, block.posY, block.posZ);

            GameObject typePrefab = blockPrefabDic[block.type];
            GameObject instPrefab = Instantiate(typePrefab) as GameObject;

            instPrefab.AddComponent<BoxCollider>();
            instPrefab.transform.rotation = Quaternion.Euler(new Vector3(0.0f, block.rotation, 0.0f));
            instPrefab.transform.position = position;
        }
    }

    void SetBaseBlock(Object[] prefabs)
    {
        foreach (GameObject prefab in prefabs)
        {
            if (prefab.name != "block_base")
                continue;

            for (int i = 0; i < MAX_WORLD_SIZE; ++i)
            {
                for (int j = 0; j < MAX_WORLD_SIZE; ++j)
                {
                    sparseBlockInfo.Add(new BlockInfo(i, 0, j, BLOCK_TYPE.BASE));
                    updateBlockInfo.Push(new BlockInfo(i, 0, j, BLOCK_TYPE.BASE));
                }
            }
        }

        //for (int posX = 0; posX < MAX_WORLD_SIZE; ++posX)
        //{
        //    for(int posZ = 0; posZ < MAX_WORLD_SIZE; ++posZ)
        //    {
        //        int index = posX * MAX_WORLD_SIZE * MAX_WORLD_SIZE + posZ * MAX_WORLD_SIZE;
        //        Vector3 pos = new Vector3(posX, 0, posZ);
        //        GameObject empty = Instantiate(emptyColli) as GameObject;
        //        empty.name = "test";
        //        empty.transform.position = pos;
        //        empty.transform.parent = emptyColliders.transform;

        //        //Block testBlock = new Block(BLOCK_TYPE.ATTACHABLE);
        //        worldBlockInfo[index] = new Block(BLOCK_TYPE.ATTACHABLE);
        //    }
        //}
        return;
    }

    // http://answers.unity3d.com/questions/561909/how-to-order-raycastall-by-distance-on-ios.html
    Vector3 SelectBlock(Vector2 touchPosition)
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, 0));

        List<RaycastResult> hitList = new List<RaycastResult>();

        hitList.Clear();
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            hitList.Add(new RaycastResult(hit));
        }

        hitList.Sort();

        if (hitList.Capacity == 0)
        {
            indicator.transform.position = new Vector3(0, -1000, 0);
            return prevSelectedPos;
        }

        if (hits[0].collider.gameObject.layer != 10)
        {
            return prevSelectedPos;
        }

        Vector3 pos = hits[0].transform.position;
        //pos.y += 1.0f;

        Vector3 normal = hits[0].normal;
        normal = hits[0].transform.TransformDirection(normal);

        if(normal == hits[0].transform.up)
        {
            Debug.Log(normal == hits[0].transform.up);
            pos = new Vector3(pos.x, pos.y + 1, pos.z);
        }
        if (normal == -hits[0].transform.up)
        {
            Debug.Log(normal == -hits[0].transform.up);
            pos = new Vector3(pos.x, pos.y - 1, pos.z);
        }
        if (normal == hits[0].transform.forward)
        {
            Debug.Log(normal == hits[0].transform.forward);
            pos = new Vector3(pos.x, pos.y, pos.z+1);
        }
        if (normal == -hits[0].transform.forward)
        {
            Debug.Log(normal == -hits[0].transform.forward);
            pos = new Vector3(pos.x, pos.y, pos.z-1);
        }
        if (normal == hits[0].transform.right)
        {
            Debug.Log(normal == hits[0].transform.right);
            pos = new Vector3(pos.x+1, pos.y, pos.z);
        }
        if (normal == -hits[0].transform.right)
        {
            Debug.Log(normal == -hits[0].transform.right);
            pos = new Vector3(pos.x-1, pos.y, pos.z);
        }

        prevSelectedPos = pos;
        indicator.transform.position = pos;
        indicator.transform.rotation = Quaternion.identity;

        Debug.Log(pos);
        return pos;
    }

    void SetBlock(Vector3 selectedPos, BLOCK_TYPE bType)
    {
        GameObject block = Instantiate(selectdBlock) as GameObject;
        block.transform.position = selectedPos;

        int posX = (int)selectedPos.x;
        int posZ = (int)selectedPos.z;
        int index = posX * MAX_WORLD_SIZE * MAX_WORLD_SIZE + posZ * MAX_WORLD_SIZE + (int)selectedPos.y;

        // TODO:
        //Block blockInfo = new Block(bType);
        //worldBlockInfo[index] = blockInfo;

        // Check Attachable
        //CheckAttachable(0, 1, 0, selectedPos);
        //CheckAttachable(1, 0, 0, selectedPos);
        //CheckAttachable(-1,0, 0, selectedPos);
        //CheckAttachable(0, 0, 1, selectedPos);
        //CheckAttachable(0, 0,-1, selectedPos);
    }

    void CheckAttachable(int x, int y, int z, Vector3 offset)
    {
        int posX = (int)offset.x;
        int posZ = (int)offset.z;
        int posY = (int)offset.y;

        int index = MAX_WORLD_SIZE * MAX_WORLD_SIZE * posX + MAX_WORLD_SIZE * posZ + posY;
        index += MAX_WORLD_SIZE * MAX_WORLD_SIZE * x;
        index += MAX_WORLD_SIZE * z;
        index += y;

        if (index < 0)
            return;

        if ((worldBlockInfo[index] == null) || (worldBlockInfo[index].bType == BLOCK_TYPE.NONE))
        {
            //Debug.LogWarning("ATTACHABLE (" + offset.x + x +", " + offset.y + y +", " + offset.z + z+")");
            // TODO:
            //worldBlockInfo[index] = new Block(BLOCK_TYPE.ATTACHABLE);

            Vector3 pos = new Vector3(posX + x, posY + y, posZ + z);
            GameObject empty = Instantiate(emptyColli) as GameObject;
            empty.transform.position = pos;
            empty.transform.parent = emptyColliders.transform;
        }
    }
    
    ///////////////////////////////////////////////////////////////////////
    //                      Camera Action Methods                        //
    ///////////////////////////////////////////////////////////////////////

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





// RatcastResult redefine
public class RaycastResult : System.IComparable<RaycastResult>
{
    public float distance;
    public Collider collider;
    public Vector2 textureCoord;
    public RaycastResult(RaycastHit hit)
    {
        distance = hit.distance;
        collider = hit.collider;
        textureCoord = hit.textureCoord;
    }

    public int CompareTo(RaycastResult other)
    {
        return distance.CompareTo(other.distance);
    }
}