using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CONNECT_STATE : short
{
    DISCONNECTED = 0,
    CONNECTING = 1,
    CONNECTED = 2,
}

public enum SCENE_STATE : short
{
    LOGIN_SCENE = 0,
    LOGIN_SCENE_FAILED = 1,
    // LOGIN_SCENE_SUCCESS = 2,

    CREATE_USER_FAILED = 2,
    CREATE_USER_SUCCESS = 3,

    MY_SCENE = 4,

    EDIT_SCENE = 5,

    SEARCH_SCENE_UNLOADED = 15,
    SEARCH_SCENE_LOADED = 16,
}

public struct MapTileData
{
    public MapTileData(string uID, string dID, string dName, string dInfo, int roomNum)
    {
        this.uID = uID;
        this.dID = dID;
        this.dName = dName;
        this.dInfo = dInfo;
        this.roomNum = roomNum;
    }

    public string uID;
    public string dID;
    public string dName;
    public string dInfo;
    public int roomNum;
}

public class PlayerDataManager : MonoBehaviour
{
    string dTile = "dTile";

	void Start ()
    {
        DontDestroyOnLoad(gameObject);
	}
	
	void Update ()
    {
	
	}

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SavePlayerKey(string key)
    {
        PlayerPrefs.SetString("uID", key);
        Debug.Log("uID : " + GetPlayerKey());
    }

    public string GetPlayerKey()
    {
        return PlayerPrefs.GetString("uID");
    }

    public void SetConnectState(CONNECT_STATE cState)
    {
        PlayerPrefs.SetInt("connectState", (int)cState);
    }

    public CONNECT_STATE GetConnectState()
    {
        return (CONNECT_STATE)PlayerPrefs.GetInt("connectState");
    }

    public void SetPlayerState(SCENE_STATE pState)
    {
        PlayerPrefs.SetInt("curState", (int)pState);
        Debug.Log("curState : " + GetPlayerState());
    }

    public SCENE_STATE GetPlayerState()
    {
        return (SCENE_STATE)PlayerPrefs.GetInt("curState");
    }


    public void SaveDungeonKey(string key)
    {
        PlayerPrefs.SetString("dID", key);
        Debug.Log("dID : " + GetPlayerKey());
    }

    public string GetDungeonID()
    {
        return PlayerPrefs.GetString("dID");
    }







    // Search Scene
    public void SaveTotalDTileNum(int num)
    {
        PlayerPrefs.SetInt("dTileNum", num);
    }

    public int GetTotalDTileNum()
    {
        return PlayerPrefs.GetInt("dTileNum");
    }

    public void SaveSearchSceneMapData(int index, string uID, string dID, string dName, string dInfo, int roomNum)
    {
        PlayerPrefs.SetString(dTile + index + "_uID", uID);
        PlayerPrefs.SetString(dTile + index + "_dID", dID);
        PlayerPrefs.SetString(dTile + index + "_dName", dName);
        PlayerPrefs.SetString(dTile + index + "_dInfo", dInfo);
        PlayerPrefs.SetInt(dTile + index + "_roomNum", roomNum);
    }

    public List<MapTileData> GetMapTileDataList()
    {
        int totalTileNum = GetTotalDTileNum();
        List<MapTileData> tileDataList = new List<MapTileData>();

        for (int i = 0; i < totalTileNum; ++i)
        {
            tileDataList.Add(
                new MapTileData(
                    PlayerPrefs.GetString(dTile + i + "_uID"),
                    PlayerPrefs.GetString(dTile + i + "_dID"),
                    PlayerPrefs.GetString(dTile + i + "_dName"),
                    PlayerPrefs.GetString(dTile + i + "_dInfo"),
                    PlayerPrefs.GetInt(dTile + i + "_roomNum")
                    )
                );
        }

        return tileDataList;
    }
}
