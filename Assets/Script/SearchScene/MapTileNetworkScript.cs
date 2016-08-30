using UnityEngine;
using System.Collections;

public class MapTileNetworkScript : MonoBehaviour
{
    //public string dID = null;
    public bool isDungeon = false;

    public MapTileData dData;
    public GameObject dungeonPrefab = null;

    public void SetDungeon()
    {
        isDungeon = true;

        dungeonPrefab = Instantiate(dungeonPrefab) as GameObject;
        Vector3 pos = GetComponent<Transform>().position;
        pos.y += 0.25f;

        dungeonPrefab.transform.position = pos;
        dungeonPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
