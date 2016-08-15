using UnityEngine;
using System.Collections;

public class TrapBladeScript : MonoBehaviour
{
    public GameObject Player = null;
    private InGameScript GameManager = null;

    private int curTurn = 0;

    void Start()
    {
        GameManager = Player.GetComponent<InGameScript>();
    }

	void Update ()
    {
        //RotateByTurn(GameManager.turn);
        if(curTurn != GameManager.turn)
            StartCoroutine(RotateCoroutine(GameManager.turn));
    }

    void RotateByTurn(int turn)
    {
        if (turn == curTurn)
            return;

        transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
        curTurn = turn;
    }

    // http://linecode.tistory.com/9
    IEnumerator RotateCoroutine(int turn)
    {
        //Debug.Log(turn + ", " + curTurn);
        transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));

        //Vector3 targetDir = transform.rotation.eulerAngles;
        //targetDir.y += 90.0f;

        //Vector3.RotateTowards(transform.rotation.eulerAngles,
        //    targetDir,
        //    1.0f,
        //    0.0f);
        curTurn = turn;
        yield return null;
    }
}
