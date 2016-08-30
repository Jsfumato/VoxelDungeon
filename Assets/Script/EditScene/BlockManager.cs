using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class BlockInfo
{
    public short posX;
    public short posY;
    public short posZ;
    public BLOCK_TYPE type;
    public float rotation = 0.0f;

    public BlockInfo(int posX, int posY, int posZ, BLOCK_TYPE type, float rotation)
    {
        this.posX = (short)posX;
        this.posY = (short)posY;
        this.posZ = (short)posZ;
        this.type = type;
        this.rotation = rotation;
    }

    public JSONNode SaveToJSON()
    {
        JSONNode node = new JSONClass();
        node["posX"] = posX.ToString();
        node["posY"] = posY.ToString();
        node["posZ"] = posZ.ToString();
        node["type"] = type.ToString();
        node["rotation"] = rotation.ToString();
        return node;
    }

    public byte[] SaveAsByteArray()
    {
        byte[] node = new byte[12];

        var byteX = System.BitConverter.GetBytes(posX);
        var byteY = System.BitConverter.GetBytes(posY);
        var byteZ = System.BitConverter.GetBytes(posZ);
        var byteType = System.BitConverter.GetBytes((short)type);
        var byteRotation = System.BitConverter.GetBytes(rotation);

        //System.Array.Reverse(byteX);
        //System.Array.Reverse(byteY);
        //System.Array.Reverse(byteZ);
        //System.Array.Reverse(byteType);
        //System.Array.Reverse(byteRotation);

        System.Buffer.BlockCopy(byteX, 0, node, 0, 2);
        System.Buffer.BlockCopy(byteY, 0, node, 2, 2);
        System.Buffer.BlockCopy(byteZ, 0, node, 4, 2);
        System.Buffer.BlockCopy(byteType, 0, node, 6, 2);
        System.Buffer.BlockCopy(byteRotation, 0, node, 8, 4);

        //Debug.Log(System.Text.Encoding.Default.GetString(node));
        //Debug.LogError(System.Text.Encoding.UTF8.GetString(node));

        return node;
    }
}

public class BlockManager : MonoBehaviour
{
    public GameObject btnPrefab;
    public GameObject selectedBlock;

    private Object[] blockPrefabs;
    private Object[] decoPrefabs;
    private Object[] triggerPrefabs;
    private Object[] trapPrefabs;

    public Dictionary<BLOCK_TYPE, GameObject> blockPrefabDic = null;

    void Awake()
    {
        blockPrefabDic = new Dictionary<BLOCK_TYPE, GameObject>();
    }

    void Start()
    {
        // need "resources" folder
        // Loads list of prefabs in "Resources/Prefab" path


        blockPrefabs = Resources.LoadAll("Prefab/block", typeof(GameObject));
        decoPrefabs = Resources.LoadAll("Prefab/deco", typeof(GameObject));
        triggerPrefabs = Resources.LoadAll("Prefab/trigger", typeof(GameObject));
        trapPrefabs = Resources.LoadAll("Prefab/trap", typeof(GameObject));

        AddBlockPrefabToDictionary(blockPrefabs);
        AddBlockPrefabToDictionary(decoPrefabs);
        AddBlockPrefabToDictionary(triggerPrefabs);
        AddBlockPrefabToDictionary(trapPrefabs);

        selectedBlock = blockPrefabDic[BLOCK_TYPE.STONE];
    }

    // public
    public void AppendBlockToList(GameObject panel)
    {
        Debug.Log(blockPrefabs.Length);
        AppendBlockToList(blockPrefabs, panel);
    }

    public void AppendDecoBlockToList(GameObject panel)
    {
        AppendBlockToList(decoPrefabs, panel);
    }

    public void AppendTriggerBlockToList(GameObject panel)
    {
        AppendBlockToList(triggerPrefabs, panel);
    }

    public void AppendTraprBlockToList(GameObject panel)
    {
        AppendBlockToList(trapPrefabs, panel);
    }




    void AddBlockPrefabToDictionary(Object[] prefabList)
    {
        Debug.Log("AddBlockPrefabToDictionary Called");
        foreach (GameObject prefab in prefabList)
        {
            // Add to Dictionary
            BLOCK_TYPE bType = prefab.GetComponent<BlockScript>().bType;
            blockPrefabDic.Add(bType, prefab);
            Debug.LogWarning("Append blockPrefabDic : " + bType + " , " + prefab.name);
        }
    }


    // private
    void AppendBlockToList(Object[] prefabList, GameObject blockPanel)
    {
        int position = 0;

        foreach (GameObject prefab in prefabList)
        {
            // GUI
            Debug.Log("Append Block Prefab : " + prefab.name);

            GameObject btn = Instantiate(btnPrefab) as GameObject;
            btn.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.9f - (float)(position) / 10);
            btn.GetComponent<RectTransform>().anchorMax = new Vector2(1.0f, 1.0f - (float)(position) / 10);

            btn.transform.SetParent(blockPanel.transform, false);
            btn.GetComponentInChildren<Text>().text = prefab.name;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Block : " + prefab.name);
                selectedBlock = prefab;
            });
            position++;
        }
    }
}
