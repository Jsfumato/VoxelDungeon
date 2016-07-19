using UnityEngine;
using System.Collections;

public enum BLOCK_TYPE
{
    NONE = 0,
    BASE = 1,
    INDICATOR = 2,

    /* deco */
    SKULL = 200,

    /* base block */
    ATTACHABLE = 255
}

public class Block
{
    public Block(BLOCK_TYPE type)
    {
        //this.prefab = prefab;
        this.bType = type;

        if (type == BLOCK_TYPE.BASE)
            isBreakable = false;
    }

    public void SetInfo(BLOCK_TYPE type)
    {
        //this.prefab = prefab;
        this.bType = type;

        if (type == BLOCK_TYPE.BASE)
            isBreakable = false;
    }

    private bool isBreakable = true;
    //private GameObject prefab = null;
    public BLOCK_TYPE bType = BLOCK_TYPE.NONE;
    Vector2 lookVector = Vector2.zero;
    private GameObject block;
}