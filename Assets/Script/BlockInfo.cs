using UnityEngine;
using System.Collections;

public class BlockInfo
{
    public int posX;
    public int posY;
    public int posZ;
    public BLOCK_TYPE type;
    public float rotation = 0.0f;

    public BlockInfo(int posX, int posY, int posZ, BLOCK_TYPE type)
    {
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;
        this.type = type;
    }
}