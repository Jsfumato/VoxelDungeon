using UnityEngine;
using System.Collections;

public class StartGateScript : MonoBehaviour
{
    public GameObject playerCharacter = null;

	void Start ()
    {
        if (playerCharacter == null)
            return;

        GameObject inst = Instantiate(playerCharacter) as GameObject;

        Vector3 pos = transform.position;
        pos.y += 2.0f;

        inst.transform.position = pos;
	}
}
