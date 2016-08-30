using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlateLinkingScript : MonoBehaviour
{
    public GameObject link1;
    public GameObject link2;
    public GameObject link3;

    public bool set = false;
    bool curSetting = false;

    public float maxTime = 3.0f;
    float curTime = 0.0f;

    Vector3 originPos;
    List<GameObject> links = new List<GameObject>();

    void Start()
    {
        originPos = GetComponent<Transform>().position;
        SetTriggerOff();

        links.Add(link1);
        links.Add(link2);
        links.Add(link3);
    }

    void Update()
    {
        if (curSetting == true)
            curTime -= Time.deltaTime;

        if (curTime < 0)
        {
            SetTriggerOff();
        }
    }

    public void SetTriggerOn()
    {
        curTime = maxTime;
        curSetting = true;

        Vector3 pos = transform.position;
        pos.y -= 0.05f;
        transform.position = pos;

        foreach (GameObject link in links)
        {
            if (link == null)
                continue;

            link.GetComponent<LinkedObjectScript>().SetTriggerOn();
        }
    }

    public void SetTriggerOff()
    {
        curTime = 0.0f;
        curSetting = false;

        transform.position = originPos;

        foreach (GameObject link in links)
        {
            if (link == null)
                continue;

            link.GetComponent<LinkedObjectScript>().SetTriggerOff();
        }
    }
}
