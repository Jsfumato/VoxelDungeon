using UnityEngine;
using System.Collections;

public class LinkedObjectScript : MonoBehaviour
{
    public bool set = false;
    bool curSetting = false;

    public float maxTime = 3.0f;
    float curTime = 0.0f;

    void Start()
    {
        SetTriggerOff();
    }

    void Update()
    {
        if (curSetting == true)
            curTime -= Time.deltaTime;

        if (curTime < 0)
            SetTriggerOff();
    }

    virtual public void SetTriggerOn()
    {
        curTime = maxTime;
        curSetting = true;
    }

    virtual public void SetTriggerOff()
    {
        curTime = 0.0f;
        curSetting = false;
    }
}
