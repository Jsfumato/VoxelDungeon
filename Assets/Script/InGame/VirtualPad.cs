using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class VirtualPad : MonoBehaviour
{
    public Image inner = null;
 //   public Camera mainCam = null;

    Vector3 originPos = Vector3.zero;
    InGameScript GameManager = null;

    void Start()
    {
        GameManager = GetComponentInParent<InGameScript>();
        originPos = inner.transform.position;
    }

    public void OnDragStart()
    {
        Touch touch = Input.GetTouch(0);
        if(inner != null)
            inner.rectTransform.position = touch.position;

        Vector3 dir = new Vector3(touch.position.x - originPos.x, 0.0f, touch.position.y - originPos.y).normalized;
        dir = Quaternion.AngleAxis(-45, Vector3.up) * dir;
        Debug.Log(originPos + " : " + touch.position + " : " + dir);
        GameManager.MovePlayer(dir, 0.1f);

        float touchAreaRadius = Vector3.Distance(originPos, new Vector3(touch.position.x, touch.position.y, originPos.z));
    }

    public void OnDragEnd()
    {
        if (inner == null)
            return;

        inner.rectTransform.position = originPos;
    }
}