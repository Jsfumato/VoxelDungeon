using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VirtualPad : MonoBehaviour
{
    public Image innerPad = null;
    public Image outerPad = null;
    public Camera cam = null;

    int movePadTouchID = -1;

    // Update is called once per frame
    //void Update()
    //{
    //    if (IsPress && touch_ID > -1)
    //    {
    //        // Touch 코드
    //        Vector3 touch = Input.GetTouch(touch_ID).position;
    //        Vector3 padPos = UICam.ScreenToWorldPoint(new Vector3(touch.x, touch.y, 0));
    //        // 최대 Touch 거리를 넘어섰을 경우 최대거리로 제한
    //        if ((padPos - transform.position).magnitude > distanceMax)
    //        {
    //            padPos = transform.position + (padPos - transform.position).normalized * distanceMax;
    //        }
    //        InnerPad.position = padPos;
    //    }

    //    // 방향벡터 얻기
    //    Vector3 Temp = (InnerPad.position - transform.position).normalized;
    //    dir.x = Temp.x;
    //    dir.y = 0.0f;
    //    dir.z = Temp.y;

    //    // 거리 계산
    //    distance = (InnerPad.position - transform.position).magnitude;
    //    //Debug.Log(InnerPad.position);

    //    // 캐릭터 이동 회전 호출
    //    CharMove.playerMove(dir, distance / distanceMax);
    //}

    //void OnPress(bool _IsPress)
    //{
    //    // 패드가 눌렸을 경우 터치의 충돌여부 판단과 충돌된 터치번호를 얻음
    //    IsPress = _IsPress;
    //    if (IsPress)
    //    {
    //        for (int i = 0; i < Input.touchCount; ++i)
    //        {
    //            Ray ray = cam.ScreenPointToRay(Input.GetTouch(i).position);
    //            RaycastHit2D rayhit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
    //            if (rayhit.collider == this.GetComponent<collider2d>())
    //            {
    //                touch_ID = i;
    //            }
    //        }
    //    }
    //}

    //void OnRelease()
    //{
    //    IsPress = false;
    //    touch_ID = -1;
    //}
}
