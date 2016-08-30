using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using SimpleJSON;
using System;

public class RoomData
{
    public RoomData(int roomNum, BasePos basePos)
    {
        roomNumber = (short)roomNum;
        bPos = basePos;
    }

    public short roomNumber;
    public List<BlockInfo> sparseBlockInfo = null;

    public BasePos bPos;
}

public struct BasePos : IEqualityComparer<BasePos>
{
    public BasePos(int xPos, int yPos, int zPos)
    {
        x = xPos;
        y = yPos;
        z = zPos;
    }
    public int x;
    public int y;
    public int z;

    public bool Equals(BasePos bPosA, BasePos bPosB)
    {
        if (bPosA.x != bPosB.x)
            return false;
        if (bPosA.y != bPosB.y)
            return false;
        if (bPosA.z != bPosB.z)
            return false;

        return true;
    }

    public bool Equals(BasePos bPos)
    {

        if (this.x != bPos.x)
            return false;
        if (this.y != bPos.y)
            return false;
        if (this.z != bPos.z)
            return false;

        return true;
    }

    public int GetHashCode(BasePos obj)
    {
        return 10000 * x + 100 * y + 1 * z;
    }

    public override string ToString()
    {
        return x + ", " + y + ", " + z;
    }
}

public class BlockDataManager : MonoBehaviour
{
    public const int MAX_WORLD_SIZE = 16;
    public const int MAX_ROOM_NUM = 25;

    public GameObject blockManagerObj;
    private BlockManager blockManager;

    private GameObject PlayerDataManagerObj = null;
    private PlayerDataManager PlayerDataManager = null;

    private GameObject NetManagerObj = null;
    private ClientAPI NetManager = null;

    public GameObject AddRoomObj;

    private Stack<BlockInfo> updateBlockInfo = null;
    private List<RoomData> roomList = null;
    private List<int> emptyRoomNum = null;
    private HashSet<BasePos> roomBaseSet = null;
    private BasePos basePos;

    private Dictionary<Vector3, int> roomNumDic = null;

    public List<GameObject> addObjList;

    private RoomData curRoom = null;

    private GameObject blockContainer = null;


    // Use this for initialization
    void Start()
    {
        // Init ManagerScript
        blockManager = blockManagerObj.GetComponent<BlockManager>();
        NetManagerObj = GameObject.Find("NetworkManager");
        NetManager = NetManagerObj.GetComponent<ClientAPI>();
        PlayerDataManagerObj = GameObject.Find("PlayerDataManager");
        PlayerDataManager = PlayerDataManagerObj.GetComponent<PlayerDataManager>();

        roomList = new List<RoomData>();
        updateBlockInfo = new Stack<BlockInfo>();
        emptyRoomNum = new List<int>();
        roomBaseSet = new HashSet<BasePos>();
        addObjList = new List<GameObject>();
        roomNumDic = new Dictionary<Vector3, int>();

        for (int i = 0; i < MAX_ROOM_NUM; ++i)
            emptyRoomNum.Add(i);

        blockContainer = new GameObject();
        blockContainer.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void UpdateRender()
    {
        while (updateBlockInfo.Count != 0)
        {
            BlockInfo block = updateBlockInfo.Pop();
            Vector3 position = new Vector3(block.posX, block.posY, block.posZ);

            GameObject typePrefab = blockManager.blockPrefabDic[block.type];
            GameObject instPrefab = Instantiate(typePrefab) as GameObject;

            instPrefab.AddComponent<BoxCollider>();
            instPrefab.transform.rotation = Quaternion.identity * Quaternion.Euler(Vector3.up * block.rotation);
            instPrefab.transform.position = position;
            instPrefab.transform.SetParent(blockContainer.transform, false);
        }
    }

    public void SaveEditData(string dName, string dInfo)
    {
        if (updateBlockInfo.Count != 0)
            return;

        string uID = PlayerDataManager.GetPlayerKey();
        byte[] packet = PacketInfo.MakeReqSaveMapNameInfoBody(uID, dName, dInfo, (short)(roomList.Count));
        NetManager.SendData(packet);

        List<byte[]> byteList = new List<byte[]>();

        foreach(var roomData in roomList)
        {
            for (int i = 0; i < roomData.sparseBlockInfo.Count; i++)
            {
                byteList.Add(roomData.sparseBlockInfo[i].SaveAsByteArray());
            }

            var bPkt = PacketInfo.MakeReqSaveMapDataBody(
                    PlayerDataManager.GetPlayerKey(),
                    //PlayerDataManager.GetDungeonID(),
                    "d000000000",
                    roomData.roomNumber,
                    byteList
                    );

            NetManager.SendData(bPkt);
            byteList.Clear();
        }
    }

    public void SetBaseBlock(BasePos basePos)
    {
        if (emptyRoomNum.Count <= 0)
            return;

        BLOCK_TYPE baseBlock = BLOCK_TYPE.STONE;
        GameObject blockPrefab = blockManager.blockPrefabDic[baseBlock];

        int curRoomNum = emptyRoomNum[0];

        RoomData curRoom = new RoomData(curRoomNum, basePos);
        curRoom.sparseBlockInfo = new List<BlockInfo>();
        roomList.Add(curRoom);

        emptyRoomNum.RemoveAt(0);

        for (int i = 0; i < MAX_WORLD_SIZE; ++i)
        {
            for (int j = 0; j < MAX_WORLD_SIZE; ++j)
            {
                curRoom.sparseBlockInfo.Add(new BlockInfo(basePos.x + i, basePos.y + 0, basePos.z + j, baseBlock, 0.0f));
                updateBlockInfo.Push(new BlockInfo(basePos.x + i, basePos.y + 0, basePos.z + j, baseBlock, 0.0f));
                roomNumDic.Add(new Vector3(basePos.x + i, 0, basePos.z + j), curRoomNum);
            }
        }

        return;
    }

    public void SetBlock(Vector3 selectedPos, BLOCK_TYPE bType, Quaternion rotation)
    {
        Vector3 vec = selectedPos;
        vec.y = 0.0f;
        
        int curRoomNum = roomNumDic[vec];
        curRoom = roomList.Find( r => r.roomNumber == curRoomNum );
        Debug.LogError(curRoomNum + ", " + curRoom.roomNumber);

        Vector3 rot = rotation.eulerAngles;
        var rotate = rot.y;

        curRoom.sparseBlockInfo.Add(new BlockInfo((int)selectedPos.x, (int)selectedPos.y, (int)selectedPos.z, bType, rotate));
        updateBlockInfo.Push(new BlockInfo((int)selectedPos.x, (int)selectedPos.y, (int)selectedPos.z, bType, rotate));
    }

    public void SetAddBaseObj()
    {
        while (addObjList.Count > 0)
        {
            Destroy(addObjList[0]);
            addObjList.RemoveAt(0);
        }

        foreach (RoomData rData in roomList)
        {
            BasePos rBase = rData.bPos;
            int offset = MAX_WORLD_SIZE / 2;

            roomBaseSet.Add(new BasePos(rBase.x - offset, rBase.y, rBase.z));
            roomBaseSet.Add(new BasePos(rBase.x + 3*offset, rBase.y, rBase.z));
            roomBaseSet.Add(new BasePos(rBase.x + offset, rBase.y, rBase.z - 2*offset));
            roomBaseSet.Add(new BasePos(rBase.x + offset, rBase.y, rBase.z + 2*offset));
        }

        foreach (RoomData rData in roomList)
        {
            BasePos rBase = rData.bPos;
            rBase.x += MAX_WORLD_SIZE / 2;
            roomBaseSet.RemoveWhere((cPos) => { return cPos.Equals(rBase); });
        }

        foreach (BasePos bPos in roomBaseSet)
        {
            GameObject obj = Instantiate(AddRoomObj) as GameObject;
            addObjList.Add(obj);
            obj.transform.position = new Vector3(bPos.x - 0.5f, bPos.y + 0.5f, bPos.z + 7.5f);
        }
    }

    private bool IsEqual(BasePos comparePos, int x, int y, int z)
    {
        if (comparePos.x != x)
            return false;
        if (comparePos.y != y)
            return false;
        if (comparePos.z != z)
            return false;

        return true;
    }
}