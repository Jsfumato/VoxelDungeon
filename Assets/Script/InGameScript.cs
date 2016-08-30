using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SimpleJSON;

public enum SELECT_RESULT
{
    NONE = 0,
    BLOCK,
    PLAYERABLE,
    ENEMY
}

public class InGameScript : MonoBehaviour {

    public GameObject LoadBtn = null;
    public GameObject ButtonPrefab = null;
    public GameObject ListPanel = null;

    public GameObject indicator = null;

    private GameObject playable = null;
    private GameObject selectedBlock = null;
    private GameObject enemyCharacter = null;

    public GameObject character = null;
    public GameObject startPortal = null;

    public int turn;

    private List<BlockInfo> sparseBlockInfo = new List<BlockInfo>();
    public Camera playerCamera = null;

    private List<GameObject> tmpIndicator = new List<GameObject>();

    private GameObject pObj = null;

    private Vector3 charToCam;

    void Start ()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        //LoadBtn.GetComponent<Button>().onClick.AddListener(LoadJSONData);
        pObj = Instantiate(character) as GameObject;
        pObj.transform.parent = transform;

        charToCam = playerCamera.transform.position - pObj.transform.position;

        Vector3 pos = startPortal.transform.position;
        pos.y += 2.0f;
        pObj.transform.position = pos;
    }
	
	void Update ()
    {
        if (Input.touchCount != 1)
            return;

        if (Input.GetTouch(0).phase != TouchPhase.Began)
            return;

        var result = SelectObject(Input.GetTouch(0).position);

        if (playable != null)
        {
            Collider[] colliders;
            Transform pos = playable.transform;
            if ((colliders = Physics.OverlapSphere(pos.position, 0.5f)).Length > 1)
            {
                foreach(Collider collider in colliders)
                {
                    if (collider.gameObject.layer != 10)
                        continue;

                    Vector3 blockPos = collider.gameObject.transform.position;
                    Vector3 playablePos = playable.transform.position;
                    playablePos.y -= 0.5f;

                    if (blockPos == playablePos)
                        continue;

                    blockPos.y += 0.5f;
                    GameObject obj = Instantiate(indicator) as GameObject;
                    obj.transform.position = blockPos;
                    tmpIndicator.Add(obj);
                }
            }

            var targetBlock = SelectObject(Input.GetTouch(0).position);
            if(targetBlock != SELECT_RESULT.BLOCK)
                return;

            MovePlayable(selectedBlock.transform.position);
        }


	}

    void LoadJSONData()
    {
        Debug.Log(Application.persistentDataPath);
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.json");

        int i = 7;
        foreach (var file in info)
        {
            MakeLoadList(file.Name, ListPanel, i++);
        }
    }

    void MakeLoadList(string fileName, GameObject ListPanel, int position)
    {
        GameObject btn = Instantiate(ButtonPrefab) as GameObject;
        btn.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.9f - (float)(position) / 10);
        btn.GetComponent<RectTransform>().anchorMax = new Vector2(1.0f, 1.0f - (float)(position) / 10);

        btn.transform.SetParent(ListPanel.transform, false);
        btn.GetComponentInChildren<Text>().text = fileName;

        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("load : " + fileName);
            string result = File.ReadAllText(Application.persistentDataPath + "/" + fileName);

            Debug.LogWarning(result);
            LoadMapFromJSON(JSON.Parse(result));
        });
    }

    void LoadMapFromJSON(JSONNode nodes)
    {
        for(int i = 0; i< nodes.Count; ++i)
        {
            List<JSONNode> nodeInfo = nodes[i].Childs.ToList();
            BLOCK_TYPE bType = (BLOCK_TYPE)System.Enum.Parse(typeof(BLOCK_TYPE), nodeInfo[3]);
            sparseBlockInfo.Add(new BlockInfo(
                nodeInfo[0].AsInt,
                nodeInfo[1].AsInt,
                nodeInfo[2].AsInt,
                bType,
                nodeInfo[4].AsFloat
                ));
        }
    }






    public void MovePlayer(Vector3 dir, float speed)
    {
        pObj.transform.rotation = Quaternion.LookRotation(dir);
        pObj.transform.position += dir * Time.deltaTime;
        playerCamera.transform.position = pObj.transform.position + charToCam;
    }




    SELECT_RESULT SelectObject(Vector2 touchPosition)
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, 0));

        List<newRaycastResult> hitList = new List<newRaycastResult>();

        hitList.Clear();
        RaycastHit[] hits = Physics.RaycastAll(ray);
        List<RaycastHit> newHits = new List<RaycastHit>();
        
        foreach (RaycastHit hit in hits)
        {
            hitList.Add(new newRaycastResult(hit));
        }

        hitList.Sort();
        newHits.AddRange(hits);
        
        if (hitList.Capacity == 0)
        {
            playable = null;
            return SELECT_RESULT.NONE;
        }

        while(newHits[0].transform.gameObject.layer == 13)
        {
            newHits.RemoveAt(0);
        }

        RaycastHit target = newHits[0];

        // layer 12 is Enemy
        if (target.transform.gameObject.layer == 12)
        {
            enemyCharacter = target.transform.gameObject;
            return SELECT_RESULT.ENEMY;
        }

        // layer 11 is Playerable Character
        if (target.transform.gameObject.layer == 11)
        {
            playable = target.transform.gameObject;
            return SELECT_RESULT.PLAYERABLE;
        }

        // layer 10 is Block Object;
        if (target.transform.gameObject.layer == 10)
        {
            Vector3 pos = target.transform.position;
            selectedBlock = target.transform.gameObject;
            return SELECT_RESULT.BLOCK;
        }

        playable = null;
        selectedBlock = null;
        return SELECT_RESULT.NONE;
    }

    void MovePlayable(Vector3 Toward)
    {
        if (playable == null)
            return;

        Vector3 target = Toward;
        target.y += 0.5f;

        Vector3 pos = playable.transform.position;

        if (target.x + target.y + target.z > pos.x + pos.y + pos.z + 1.0f)
        {
            selectedBlock = null;
            return;
        }
        if (target.x + target.y + target.z < pos.x + pos.y + pos.z - 1.0f)
        {
            selectedBlock = null;
            return;
        }

        if (target == playable.transform.position)
            return;

        playable.transform.position = target;
        turn++;
        playable = null;
        while (tmpIndicator.Count > 0)
        {
            Destroy(tmpIndicator[0]);
            tmpIndicator.RemoveAt(0);
        }
    }
}


// RatcastResult redefine
public class newRaycastResult : System.IComparable<newRaycastResult>
{
    public float distance;
    public Collider collider;
    public Vector2 textureCoord;
    public newRaycastResult(RaycastHit hit)
    {
        distance = hit.distance;
        collider = hit.collider;
        textureCoord = hit.textureCoord;
    }

    public int CompareTo(newRaycastResult other)
    {
        return distance.CompareTo(other.distance);
    }
}