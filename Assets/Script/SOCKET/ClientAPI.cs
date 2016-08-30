using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class ClientAPI : MonoBehaviour {

    // GameObj
    public GameObject pData = null;
    private PlayerDataManager pDataManager = null;

    // Variables
    const int MAX_BYTE_SIZE = 1024 * 4;

    public string ipAddress = "127.0.0.1";
    public int port = 8777;

    private Socket m_Socket;

    private int sendDataLength;
    private int recvDataLength;

    private byte[] sendByte;
    static private byte[] recvByte = new byte[MAX_BYTE_SIZE];
    private string ReceiveString;

    public bool isConnected = false;

    public bool ping = false;
    private bool prevBool = false;

    List<SendPacket> SendPacketQueue = new List<SendPacket>();
    List<RecvPacket> RecvPacketQueue = new List<RecvPacket>();

    struct SendPacket
    {
        public byte[]  buffer;
        public int length;
    }

    struct RecvPacket
    {
        public byte[] buffer;
        public int length;
    }
    
    void Awake()
	{
        bool result = InitAndConnect();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        pDataManager = pData.GetComponent<PlayerDataManager>();

        StartCoroutine(SendCoroutine());
        //StartCoroutine(RecvCoroutine());
        Receive(m_Socket);
        StartCoroutine(ProcessCoroutine());
    }

    void Update()
    {
        if (ping != prevBool)
        {
            TestPing();
            prevBool = ping;
        }
    }

    void OnApplicationQuit()
    {
        m_Socket.Close();
        m_Socket = null;
    }

    IEnumerator SendCoroutine()
    {
        while (true)
        {
            if (SendPacketQueue.Count <= 0)
            {

            }
            else
            {
                SendPacket pkt = SendPacketQueue[0];
                int sendSize = m_Socket.Send(pkt.buffer);
                Debug.LogError("Send Size : " + sendSize + "/" + pkt.length);
                SendPacketQueue.RemoveAt(0);
            }
            yield return null;
        }
    }

    IEnumerator ProcessCoroutine()
    {
        while (true)
        {
            if (RecvPacketQueue.Count <= 0)
            {

            }
            else
            {
                RecvPacket rPkt = RecvPacketQueue[0];
                PacketProcess(rPkt);
                RecvPacketQueue.RemoveAt(0);
            }
            yield return null;
        }
    }

    IEnumerator ConnectCoroutine()
    {
        pDataManager.SetConnectState(CONNECT_STATE.CONNECTING);

        //while (NetworkManager.InitAndConnect() == false)
        //{
        //    if (NetworkManager.isConnected == true)
        //    {
        //        pDataManager.SetPlayerState(PLAYER_STATE.CONNECTED);
        //        yield break;
        //    }

        //    NetworkManager.InitAndConnect();
        //    yield return new WaitForSeconds(5);
        //}

        while (isConnected == false)
        {
            if (pDataManager.GetConnectState() == CONNECT_STATE.CONNECTED)
                yield break;

            InitAndConnect();
            yield return new WaitForSeconds(5);
        }
    }

    void AddPacketToRecvQueue(byte[] bytes, int length)
    {
        RecvPacket pkt = new RecvPacket();
        pkt.buffer = new byte[length];

        Buffer.BlockCopy(bytes, 0, pkt.buffer, 0, length);
        pkt.length = length;

        RecvPacketQueue.Add(pkt);

        Buffer.BlockCopy(bytes, length, bytes, 0, MAX_BYTE_SIZE - length);
    }


    public void SendData(byte[] packetBytes)
    {
        SendPacket pkt;
        pkt.length = packetBytes.Length;
        pkt.buffer = new byte[pkt.length];
        
        Buffer.BlockCopy(packetBytes, 0, pkt.buffer, 0, pkt.length);

        SendPacketQueue.Add(pkt);
        Debug.LogError("SendPacketQueue Size : " + SendPacketQueue.Count);
    }

    void TestPing()
    {
        StringBuilder sb = new StringBuilder(); // String Builder Create
        sb.Append("Test Ping... 제발 잘 되려무나...");

        try
        {
            byte[] result = PacketInfo.MakeReqPingPacket();
            m_Socket.Send(result);
        }
        catch (SocketException err)
        {
            Debug.Log("Socket send or receive error! : " + err.ToString());
        }
    }


    void PacketProcess(RecvPacket rPkt)
    {
        PacketHeader pHeader = new PacketHeader();
        Buffer.BlockCopy(rPkt.buffer, 0, pHeader.pID, 0, 2);
        Buffer.BlockCopy(rPkt.buffer, 2, pHeader.bodySize, 0, 2);

        PACKETID pID = (PACKETID)BitConverter.ToInt16(pHeader.pID, 0);

        Debug.LogWarning("header ID : " + BitConverter.ToInt16(pHeader.pID, 0) + " | pID : " + pID);

        switch (pID)
        {
            case PACKETID.RES_LOGIN:
                {
                    Debug.Log("RES_LOGIN");
                    string key = Encoding.Default.GetString(rPkt.buffer, 4, 10);
                    pDataManager.SavePlayerKey(key);
                    pDataManager.SetPlayerState(SCENE_STATE.MY_SCENE);
                }
                break;

            case PACKETID.RES_GET_DUNGEON_LIST:
                {
                    byte[] dataNum = new byte[2];
                    Buffer.BlockCopy(rPkt.buffer, 4, dataNum, 0, 2);
                    short num = BitConverter.ToInt16(dataNum, 0);

                    Debug.LogWarning("recved Tile Num : " + num);
                    Debug.LogWarning("bytes : " + dataNum[0] + dataNum[1]);
                    pDataManager.SaveTotalDTileNum(num);

                    byte[] uID = new byte[10];
                    byte[] dID = new byte[10];
                    byte[] dName = new byte[10];
                    byte[] dInfo = new byte[30];
                    byte[] roomNum = new byte[2];

                    for (int i = 0; i < num; ++i)
                    {
                        Buffer.BlockCopy(rPkt.buffer, 6 + 62 * i, uID, 0, 10);
                        Buffer.BlockCopy(rPkt.buffer, 16 + 62 * i, dID, 0, 10);
                        Buffer.BlockCopy(rPkt.buffer, 26 + 62 * i, dName, 0, 10);
                        Buffer.BlockCopy(rPkt.buffer, 36 + 62 * i, dInfo, 0, 30);
                        Buffer.BlockCopy(rPkt.buffer, 66 + 62 * i, roomNum, 0, 2);

                        pDataManager.SaveSearchSceneMapData(
                            i,
                            Encoding.Default.GetString(uID),
                            Encoding.Default.GetString(dID),
                            Encoding.Default.GetString(dName),
                            Encoding.Default.GetString(dInfo),
                            BitConverter.ToInt16(roomNum, 0)
                            );
                    }

                    pDataManager.SetPlayerState(SCENE_STATE.SEARCH_SCENE_LOADED);
                }
                break;

            default:
                break;
        }
    }


    public bool InitAndConnect()
    {
        isConnected = true;
        Debug.Log("Connect 시도");
        m_Socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
            );

        m_Socket.SetSocketOption(
            SocketOptionLevel.Socket,
            SocketOptionName.SendTimeout, 10000
            );

        m_Socket.SetSocketOption(
            SocketOptionLevel.Socket,
            SocketOptionName.ReceiveTimeout, 10000
            );

        try
        {
            IPAddress ipAddr = System.Net.IPAddress.Parse(ipAddress);
            IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, port);
            m_Socket.Connect(ipEndPoint);
            //m_Socket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallback), null);
        }
        catch (SocketException socketExcept)
        {
            Debug.LogError("Socket connect ERROR! : " + socketExcept.ToString());
            isConnected = false;
        }

        return isConnected;
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        m_Socket.EndConnect(ar);
        // pDataManager.SetPlayerState(PLAYER_STATE.CONNECTED);
        isConnected = true;
    }

    private void Receive(Socket client)
    {
        Debug.Log("Receive Async");
        try
        {
            // Begin receiving the data from the remote device
            client.BeginReceive(recvByte, 0, MAX_BYTE_SIZE, 0, new AsyncCallback(ReceiveCallback), null);
        }
        catch (SocketException se)
        {
            Console.WriteLine("Receive SocketException Error : {0} ", se.Message.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Receive Exception Error : {0} ", ex.Message.ToString());
        }
    }



    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Read data from the remote device
            int bytesRead = m_Socket.EndReceive(ar);

            if (bytesRead > 0)
            {
                AddPacketToRecvQueue(recvByte, bytesRead);
            }

            Receive(m_Socket);
        }
        catch (SocketException se)
        {
            Console.WriteLine("ReceiveCallback SocketException Error : {0} ", se.Message.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("ReceiveCallback Exception Error : {0} ", ex.Message.ToString());
        }
    }
}
