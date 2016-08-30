using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using SimpleJSON;
using UnityEngine.SceneManagement;

public class EditDungeon : MonoBehaviour
{
    // GameObject
    public GameObject blockManagerObj;
    private BlockManager blockManagerScript;

    public GameObject blockDataManagerObj;
    private BlockDataManager blockDataManagerScript;

    public GameObject playerDataManagerObj;
    private PlayerDataManager playerDataManagerScript;

    private CameraScript touchManager;

    public GameObject saveDataPopUp = null;
    private GameObject saveDataPopUpInst = null;
    // Variables

    //public Light mainLight = null;
    //public Toggle lightToggle = null;
    public Camera playerCamera = null;

    public Button blockButton = null;
    public Button saveButton = null;
    public Button returnButton = null;

    public GameObject PopUpMenu = null;
    public GameObject Canvas = null;
    public GameObject BlockList = null;

    private GameObject selectedBlock = null;
    private Quaternion selectedBlockRotate = Quaternion.identity;

    public GameObject testBlock = null;
    public GameObject ButtonPrefab = null;

    private bool isSetBottom = false;
    private Vector3 prevSelectedPos;

    void Start()
    {
        // Load Manager Scripts
        playerDataManagerObj = GameObject.Find("PlayerDataManager");
        playerDataManagerScript = playerDataManagerObj.GetComponent<PlayerDataManager>();
        blockManagerScript = blockManagerObj.GetComponent<BlockManager>();
        blockDataManagerScript = blockDataManagerObj.GetComponent<BlockDataManager>();
        touchManager = GetComponent<CameraScript>();

        // Load GameObjects
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Do Init
        prevSelectedPos = Vector3.zero;

        SetPopupGUI(Canvas);
        SetDataSaveGUI(Canvas);

        blockButton.onClick.AddListener(() => SelectMenu(blockButton));
        returnButton.onClick.AddListener(() => {
            playerDataManagerScript.SetPlayerState(SCENE_STATE.MY_SCENE);
            SceneManager.LoadScene("MyScene");
        });
    }

    void Update()
    {
        if (isSetBottom == false)
        {
            blockDataManagerScript.SetBaseBlock(new BasePos(0, 0, 0));
            blockDataManagerScript.SetAddBaseObj();
            isSetBottom = true;
        }

        if (Input.touchCount == 1 && (Input.touches[0].phase == TouchPhase.Began))
        {
            if (IsPointerOverUIObject(Input.touches[0]) != true)
            {
                Vector2 pos = Input.touches[0].position;
                prevSelectedPos = touchManager.SelectBlock(pos);
            }
        }

        // update block info
        blockDataManagerScript.UpdateRender();
    }

    // http://answers.unity3d.com/questions/867982/ui-buttons-touch-input-problem.html
    private bool IsPointerOverUIObject(Touch t)
    {
        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(t.position.x, t.position.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
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

    void SetDataSaveGUI(GameObject panel)
    {
        saveDataPopUp = Instantiate(saveDataPopUp) as GameObject;
        saveDataPopUp.transform.SetParent(panel.transform, false);

        saveDataPopUp.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(()=>
        {
            string dName = saveDataPopUp.transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text;
            string dInfo = saveDataPopUp.transform.GetChild(1).gameObject.GetComponentInChildren<Text>().text;
            blockDataManagerScript.SaveEditData(dName, dInfo);
            saveDataPopUp.transform.GetChild(2).gameObject.GetComponent<Button>().enabled = false;
        });
        saveDataPopUp.SetActive(false);

        saveButton.onClick.AddListener(() => {

            if (saveDataPopUp.activeSelf == false)
            {
                saveDataPopUp.SetActive(true);
                PopUpMenu.SetActive(false);
            }
            else
            {
                saveDataPopUp.SetActive(false);
                PopUpMenu.SetActive(true);
                ClearSetDataSaveGUI();
            }
        });
    }

    void ClearSetDataSaveGUI()
    {
        saveDataPopUp.transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = "";
        saveDataPopUp.transform.GetChild(1).gameObject.GetComponentInChildren<Text>().text = "";

        saveDataPopUp.transform.GetChild(2).gameObject.GetComponent<Button>().enabled = true;
    }

    void SetPopupGUI(GameObject panel)
    {
        PopUpMenu = Instantiate(PopUpMenu) as GameObject;
        PopUpMenu.GetComponent<RectTransform>().SetParent(panel.transform, false);
        PopUpMenu.SetActive(true);

        //Debug.Log(PopUpMenu.transform.childCount);
        var buildBtn = PopUpMenu.transform.GetChild(0).GetComponent<Button>();
        var rotateBtn = PopUpMenu.transform.GetChild(1).GetComponent<Button>();

        buildBtn.onClick.AddListener(() => 
        {
            blockDataManagerScript.SetBlock(prevSelectedPos, blockManagerScript.selectedBlock.GetComponent<BlockScript>().bType, selectedBlockRotate);
            selectedBlockRotate = Quaternion.identity;
        });

        rotateBtn.onClick.AddListener(() =>
        {
            selectedBlockRotate *= Quaternion.Euler(Vector3.up * 90.0f);
        });
    }
}
