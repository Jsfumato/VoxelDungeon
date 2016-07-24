using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SimpleJSON;

public class InGameScript : MonoBehaviour {

    public GameObject LoadBtn = null;
    public GameObject ButtonPrefab = null;
    public GameObject ListPanel = null;

    private List<BlockInfo> sparseBlockInfo = new List<BlockInfo>();

    void Start ()
    {
        LoadBtn.GetComponent<Button>().onClick.AddListener(LoadJSONData);
    }
	
	void Update ()
    {
	
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
}
