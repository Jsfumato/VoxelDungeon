using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.IO;
using System.Collections.Generic;

class PacketInfo
{
    public const int PACKET_HEADER_SIZE = 4;
    public const int MAX_BUFFER_SIZE = 1024;

    public static PacketHeader MakePacketHeader(PACKETID pID, int byteLength)
    {
        PacketHeader pHeader = new PacketHeader();
        
        pHeader.pID = BitConverter.GetBytes((short)pID);
        pHeader.bodySize = BitConverter.GetBytes((short)byteLength);

        return pHeader;
    }

    public static byte[] MakeReqPingPacket()
    {
        PacketHeader pHeader = MakePacketHeader(PACKETID.REQ_PING, 0);
        
        byte[] ret = new byte[PACKET_HEADER_SIZE];
        Buffer.BlockCopy(pHeader.pID, 0, ret, 0, 2);
        Buffer.BlockCopy(pHeader.bodySize, 0, ret, 2, 2);

        return ret;
    }

    public static byte[] MakeReqLoginPacket(string Email, string pw)
    {
        var emailLength = Encoding.Default.GetByteCount(Email);
        var emailByte = Encoding.Default.GetBytes(Email);

        var pwLength = Encoding.Default.GetByteCount(pw);
        var pwByte = Encoding.Default.GetBytes(pw);

        PacketHeader pHeader = MakePacketHeader(PACKETID.REQ_LOGIN, emailLength + pwLength + 4);

        byte[] ret = new byte[PACKET_HEADER_SIZE + emailLength + pwLength + 4];
        Buffer.BlockCopy(pHeader.pID, 0, ret, 0, 2);
        Buffer.BlockCopy(pHeader.bodySize, 0, ret, 2, 2);

        var emailLengthByte = BitConverter.GetBytes(emailLength);
        var pwLengthByte = BitConverter.GetBytes(pwLength);

        Buffer.BlockCopy(emailLengthByte, 0, ret, 4, 2);
        Buffer.BlockCopy(pwLengthByte, 0, ret, 6, 2);

        Buffer.BlockCopy(emailByte, 0, ret, PACKET_HEADER_SIZE + 4, emailLength);
        Buffer.BlockCopy(pwByte, 0, ret, PACKET_HEADER_SIZE + 4 + emailLength, pwLength);

        return ret;
    }

    public static byte[] MakeReqRegisterBody(string Email, string pw)
    {
        var emailLength = Encoding.Default.GetByteCount(Email);
        var emailByte = Encoding.Default.GetBytes(Email);

        var pwLength = Encoding.Default.GetByteCount(pw);
        var pwByte = Encoding.Default.GetBytes(pw);

        PacketHeader pHeader = MakePacketHeader(PACKETID.REQ_REGISTER_USER, emailLength + pwLength + 4);

        byte[] ret = new byte[PACKET_HEADER_SIZE + emailLength + pwLength + 4];
        Buffer.BlockCopy(pHeader.pID, 0, ret, 0, 2);
        Buffer.BlockCopy(pHeader.bodySize, 0, ret, 2, 2);

        var emailLengthByte = BitConverter.GetBytes(emailLength);
        var pwLengthByte = BitConverter.GetBytes(pwLength);

        Buffer.BlockCopy(emailLengthByte, 0, ret, 4, 2);
        Buffer.BlockCopy(pwLengthByte, 0, ret, 6, 2);

        Buffer.BlockCopy(emailByte, 0, ret, PACKET_HEADER_SIZE + 4, emailLength);
        Buffer.BlockCopy(pwByte, 0, ret, PACKET_HEADER_SIZE + 4 + emailLength, pwLength);

        return ret;
    }

    public static byte[] MakeReqGetMapDataPacket(string uID, string dID, int roomNum)
    {
        var uIDLength = Encoding.Default.GetByteCount(uID);
        var uIDByte = Encoding.Default.GetBytes(uID);

        var dIDLength = Encoding.Default.GetByteCount(dID);
        var dIDByte = Encoding.Default.GetBytes(dID);

        var rNum = BitConverter.GetBytes((short)roomNum);

        PacketHeader pHeader = MakePacketHeader(PACKETID.REQ_GET_MAPDATA, 8 + 8 + 4);

        byte[] ret = new byte[PACKET_HEADER_SIZE + 8 + 8 + 4];
        Buffer.BlockCopy(pHeader.pID, 0, ret, 0, 2);
        Buffer.BlockCopy(pHeader.bodySize, 0, ret, 2, 2);

        Buffer.BlockCopy(uIDByte, 0, ret, PACKET_HEADER_SIZE, Mathf.Min(8, uIDLength));
        Buffer.BlockCopy(dIDByte, 0, ret, PACKET_HEADER_SIZE + 8, Mathf.Min(8, dIDLength));
        Buffer.BlockCopy(rNum, 0, ret, PACKET_HEADER_SIZE + 16, 4);

        return ret;
    }

    public static byte[] MakeReqSaveMapNameInfoBody(string uID, string dName, string dInfo, short totalRoomNum)
    {
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

        var uIDLength = utf8.GetByteCount(uID);
        var uIDByte = utf8.GetBytes(uID);

        var dNameLength = utf8.GetByteCount(dName);
        var dNameByte = utf8.GetBytes(dName);

        var dInfoLength = utf8.GetByteCount(dInfo);
        var dInfoByte = utf8.GetBytes(dInfo);

        var totalRoomNumber = BitConverter.GetBytes((short)totalRoomNum);

        PacketHeader pHeader = MakePacketHeader(PACKETID.REQ_SAVE_MAP_NAME_INFO, 10 + 10 + 30 + 2);

        byte[] ret = new byte[PACKET_HEADER_SIZE + 10 + 10 + 30 + 2];
        Buffer.BlockCopy(pHeader.pID, 0, ret, 0, 2);
        Buffer.BlockCopy(pHeader.bodySize, 0, ret, 2, 2);

        Buffer.BlockCopy(uIDByte, 0, ret, PACKET_HEADER_SIZE, Mathf.Min(10, uIDLength));
        Buffer.BlockCopy(dNameByte, 0, ret, PACKET_HEADER_SIZE + 10, Mathf.Min(10, dNameLength));
        Buffer.BlockCopy(dInfoByte, 0, ret, PACKET_HEADER_SIZE + 20, Mathf.Min(30, dInfoLength));
        Buffer.BlockCopy(totalRoomNumber, 0, ret, PACKET_HEADER_SIZE + 50, 2);

        return ret;
    }

    public static byte[] MakeReqSaveMapDataBody(string uID, string dID, short roomNum, List<byte[]> bDataList)
    {
        Debug.LogError("uID : " + uID + " | dID : " + dID + " | roomNum : " + roomNum + " | totalBlockNum : " + bDataList.Count);
        int totalBodyLength = 10 + 10 + 2 + 2 + bDataList.Count * 12;
        byte[] ret = new byte[totalBodyLength + PACKET_HEADER_SIZE];

        var uIDLength = System.Text.Encoding.Default.GetByteCount(uID);
        var uIDByte = System.Text.Encoding.Default.GetBytes(uID);

        var dIDLength = System.Text.Encoding.Default.GetByteCount(dID);
        var dIDByte = System.Text.Encoding.Default.GetBytes(dID);

        var roomNumber = BitConverter.GetBytes(roomNum);
        var totalBlockNumber = BitConverter.GetBytes((short)bDataList.Count);

        PacketHeader pHeader = MakePacketHeader(PACKETID.REQ_SAVE_MAP_DATA, totalBodyLength);

        // header
        Buffer.BlockCopy(pHeader.pID, 0, ret, 0, 2);
        Buffer.BlockCopy(pHeader.bodySize, 0, ret, 2, 2);

        // body
        Buffer.BlockCopy(roomNumber, 0, ret, 4, 2);
        Buffer.BlockCopy(totalBlockNumber, 0, ret, 6, 2);
        Buffer.BlockCopy(uIDByte, 0, ret, 8, Math.Min(10, uIDLength));
        Buffer.BlockCopy(dIDByte, 0, ret, 18, Math.Min(10, dIDLength));

        for (int i = 0; i < bDataList.Count; ++i)
            Buffer.BlockCopy(bDataList[i], 0, ret, PACKET_HEADER_SIZE + 24 + 12 * i, 12);

        return ret;
    }

    public static byte[] MakeReqGetDungeonListPacket(short num)
    {
        PacketHeader pHeader = MakePacketHeader(PACKETID.REQ_GET_DUNGEON_LIST, 2);
        byte[] numByte = BitConverter.GetBytes(num);

        byte[] ret = new byte[PACKET_HEADER_SIZE + 2];
        Buffer.BlockCopy(pHeader.pID, 0, ret, 0, 2);
        Buffer.BlockCopy(pHeader.bodySize, 0, ret, 2, 2);
        Buffer.BlockCopy(numByte, 0, ret, 4, 2);

        return ret;
    }
}

public enum PACKETID : short
{
    NONE = -1,

    REQ_PING = 0,
    RES_PING = 1,

    REQ_LOGIN = 2,
    RES_LOGIN = 3,

    REQ_REGISTER_USER = 4,
    RES_REGISTER_USER = 5,

    REQ_GET_MAPDATA = 10,
    RES_GET_MAPDATA = 11,

    REQ_SAVE_MAP_NAME_INFO = 20,
    RES_SAVE_MAP_NAME_INFO = 21,
    REQ_SAVE_MAP_DATA = 22,
    RES_SAVE_MAP_DATA = 23,

    REQ_GET_DUNGEON_LIST = 30,
    RES_GET_DUNGEON_LIST = 31,
};

public class PacketHeader
{
    public byte[] pID = new byte[2];
    public byte[] bodySize = new byte[2];
};

public class PacketBody {};

public class ReqLoginBody : PacketBody
{
    public byte[] email;    // 40 bytes
    public byte[] pw;       // fitted bytes
}

public class ReqGetMapDataBody : PacketBody
{
    public byte[] uID;      // 8 bytes
    public byte[] dID;      // 8 bytes
    public byte[] roomNum;  // 2 bytes
}

public class ReqSaveMapNameInfoBody : PacketBody
{
    public byte[] dName;      // 10 bytes
    public byte[] dInfo;      // 30 bytes
    public byte[] totalRoomNum;  // 2 bytes
}