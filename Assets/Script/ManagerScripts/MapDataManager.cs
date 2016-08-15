using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MAP_ID
{
    PLAIN = 0,

    MAX
}

public class MapDataManager : MonoBehaviour {

    public GameObject plain = null;

    static public Dictionary<MAP_ID, GameObject> MapDic
        = new Dictionary<MAP_ID, GameObject>();

    void Awake ()
    {
        MapDic.Add(MAP_ID.PLAIN, plain);
	
	}
	
	void Update () {
	
	}
}
