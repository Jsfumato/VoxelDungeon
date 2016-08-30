using UnityEngine;
using System.Collections;

public enum BLOCK_TYPE
{
    NONE = 0,
    BRICKS = 1,
    STONE = 2,

    PRESSURE_PLATE = 3,
    
    /* deco */
    CANDLE = 100,

    /* trap */
    TRAP_HOLE = 201,
    TRAP_BLADE = 202,

    /* base block */
    ATTACHABLE = 255
}

public class BlockScript : MonoBehaviour
{
    //public bool isBreakable = true;
    public BLOCK_TYPE bType = BLOCK_TYPE.NONE;
    public float rotationY = 0.0f;

    public bool attachable_top = true;
    public bool attachable_bottom = true;
    public bool attachable_front = true;
    public bool attachable_back = true;
    public bool attachable_right = true;
    public bool attachable_left = true;
}
