using UnityEngine;
using System.Collections;

public class CharacterRenderScript : MonoBehaviour {

    public GameObject part = null;
    public GameObject item = null;

    private GameObject obj = null;
    private GameObject itemObj = null;

    void Start ()
    {
        if (part != null)
        {
            obj = Instantiate(part) as GameObject;
            obj.transform.position = GetComponent<Transform>().position;
            obj.transform.rotation = GetComponent<Transform>().rotation;
        }
        if (item != null)
        {
            itemObj = Instantiate(item) as GameObject;
            itemObj.transform.position = GetComponentInChildren<Transform>().position;
            itemObj.transform.rotation = GetComponentInChildren<Transform>().rotation;
        }
    }

    void Update()
    {
        if (obj != null)
        {
            obj.transform.position = GetComponent<Transform>().position;
            obj.transform.rotation = GetComponent<Transform>().rotation;
        }

        if (itemObj != null)
        {
            itemObj.transform.position = GetComponentInChildren<Transform>().position;
            itemObj.transform.rotation = GetComponentInChildren<Transform>().rotation;
        }
    }
}
