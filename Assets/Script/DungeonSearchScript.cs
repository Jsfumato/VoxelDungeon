using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public class DungeonSearchScript : MonoBehaviour
{
    public GameObject NetworkManagerObj = null;
    private ClientAPI NetworkManager = null;

    public GameObject pDataManagerObj = null;
    private PlayerDataManager pDataManager = null;

    [Range(0, 6)]
    public int MAX_MAPSIZE = 4;
    public Camera mainCam = null;
    public GameObject godRay = null;

    // UI
    public Button enterDungeonBtn = null;
    public Button closeBtn = null;
    public Image bgPopupImage = null;
    public Text dNameText = null;
    public Text dInfoText = null;

    public GameObject dPrefab = null;
    private GameObject selectedTile = null;

    private List<int> dungeonIndex = null;

    bool isInited = false;

	void Start ()
    {
        // init
        NetworkManagerObj = GameObject.Find("NetworkManager");
        NetworkManager = NetworkManagerObj.GetComponent<ClientAPI>();

        pDataManagerObj = GameObject.Find("PlayerDataManager");
        pDataManager = pDataManagerObj.GetComponent<PlayerDataManager>();

        dungeonIndex = new List<int>();

        // do something to Set dTile
        SetDungeonIndexByRand();
        GetDungeonList();

        // UI
        enterDungeonBtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
        bgPopupImage.gameObject.SetActive(false);

        enterDungeonBtn.onClick.AddListener(()=>SceneManager.LoadScene("GameScene"));
        closeBtn.onClick.AddListener(DisSelectMapTile);
    }
	
    void GetDungeonList()
    {
        var rPkt = PacketInfo.MakeReqGetDungeonListPacket((short)MAX_MAPSIZE);
        NetworkManager.SendData(rPkt);
    }

	void Update ()
    {
        if (pDataManager.GetPlayerState() == SCENE_STATE.SEARCH_SCENE_LOADED 
            && isInited == false)
        {
            InitMapBlock();
            isInited = true;
        }

        SelectMapTile();

        if(selectedTile != null)
        {
            enterDungeonBtn.gameObject.SetActive(true);
            closeBtn.gameObject.SetActive(true);
            bgPopupImage.gameObject.SetActive(true);
            dNameText.gameObject.SetActive(true);
            dInfoText.gameObject.SetActive(true);
        }
        if (selectedTile == null)
        {
            enterDungeonBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(false);
            bgPopupImage.gameObject.SetActive(false);
            dNameText.gameObject.SetActive(false);
            dInfoText.gameObject.SetActive(false);
        }
    }


    void SelectMapTile()
    {
        if (Input.touchCount == 0)
            return;

        if (Input.touches[0].phase != TouchPhase.Began)
            return;

        if (selectedTile != null)
            return;

        GameObject selected = SelectObject(Input.touches[0].position);
        if (selected == null)
            return;

        if (selected.GetComponent<MapTileNetworkScript>().isDungeon == false)
            return;

        Debug.Log(selected);
        selectedTile = Instantiate(MapDataManager.MapDic[selected.GetComponent<TileInfo>().tileID]) as GameObject;

        //UI
        dNameText.text = selected.GetComponent<MapTileNetworkScript>().dData.dName;
        dInfoText.text = selected.GetComponent<MapTileNetworkScript>().dData.dInfo;

        Vector3 pos = mainCam.transform.position;
        pos.x -= (3 * (pos.x / pos.y) +1);
        pos.z -= (3 * (pos.z / pos.y)+1);
        pos.y -= 3;

        selectedTile.transform.position = pos;
    }

    void DisSelectMapTile()
    {         
        Destroy(selectedTile);
        selectedTile = null;
        return;
    }

    void SetDungeonIndexByRand()
    {
        while (dungeonIndex.Count < pDataManager.GetTotalDTileNum())
        {
            bool isExist = false;

            int randNum = Random.Range(1, MAX_MAPSIZE * MAX_MAPSIZE);
            foreach(var num in dungeonIndex)
            {
                if (num == randNum)
                    isExist = true;
            }

            if (isExist != true)
            {
                dungeonIndex.Add(randNum);
                Debug.Log("Rand : " + randNum);
            }
        }
    }

    void InitMapBlock()
    {
        Vector3 pos = mainCam.transform.position;
        pos.x += ((float)MAX_MAPSIZE / 2 - 0.5f);
        pos.z += ((float)MAX_MAPSIZE / 2 - 0.5f);

        mainCam.transform.position = pos;

        List<MapTileData> tData = pDataManager.GetMapTileDataList();

        // defence code
        if (dungeonIndex.Count < pDataManager.GetTotalDTileNum())
            SetDungeonIndexByRand();

        int i = 0;
        for (int x = 0; x< MAX_MAPSIZE;++x)
        {
            for (int z = 0; z < MAX_MAPSIZE; ++z)
            {
                GameObject mapTile = Instantiate(MapDataManager.MapDic[MAP_ID.PLAIN]) as GameObject;
                mapTile.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                mapTile.transform.position = new Vector3(x, 0.0f, z);

                mapTile.AddComponent<MapTileNetworkScript>();
                mapTile.GetComponent<MapTileNetworkScript>().dungeonPrefab = dPrefab;

                if (dungeonIndex.Contains(MAX_MAPSIZE * x + z))
                {
                    Debug.Log("Dungeon Setted");
                    mapTile.GetComponent<MapTileNetworkScript>().dData = tData[i++];
                    mapTile.GetComponent<MapTileNetworkScript>().SetDungeon();
                }
            }
        }
    }


    GameObject SelectObject(Vector2 touchPosition)
    {
        Ray ray = mainCam.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, 0));

        List<newRaycastResult> hitList = new List<newRaycastResult>();

        hitList.Clear();
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            hitList.Add(new newRaycastResult(hit));
        }

        if (hitList.Capacity == 0)
            return null;

        hitList.Sort();

        // layer 10 is Block Object;
        if (hits[0].transform.gameObject.layer == 10)
            return hits[0].transform.gameObject;

        return null;
    }
}
