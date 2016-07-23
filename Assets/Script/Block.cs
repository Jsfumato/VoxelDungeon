using UnityEngine;
using System.Collections;

public enum BLOCK_TYPE
{
    NONE = 0,
    BASE = 1,
    INDICATOR = 2,
    STONE = 3,

    /* deco */
    SKULL = 200,
    PILLAR_BOTTOM = 201,
    PILLAR = 202,
    PILLAR_TOP = 203,

    /* base block */
    ATTACHABLE = 255
}

public class Block : MonoBehaviour
{
    public bool isBreakable = true;
    public BLOCK_TYPE bType = BLOCK_TYPE.NONE;
    public float rotationY = 0.0f;

    public bool attachable_top = true;
    public bool attachable_bottom = true;
    public bool attachable_front = true;
    public bool attachable_back = true;
    public bool attachable_right = true;
    public bool attachable_left = true;
}
