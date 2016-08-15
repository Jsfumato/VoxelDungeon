using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

class RayInfo
{
    public RayInfo(Vector3 speed, int dir)
    {
        this.speed = speed;
        this.dir = dir;
    }

    public Vector3 speed;
    public int dir;

    public void SetDir(int newDir)
    {
        dir = newDir;
    }
}

public class MySceneScript : MonoBehaviour
{
    public Button searchBtn = null;
    public Button editBtn = null;
    public Button unitBtn = null;

    public GameObject godRay = null;

    List<GameObject> rayList = new List<GameObject>();
    List<RayInfo> rayInfoList = new List<RayInfo>();

    void Start ()
    {
        searchBtn.onClick.AddListener(
            () => { SceneManager.LoadScene("SearchScene"); }
            );
        editBtn.onClick.AddListener(
            () => { SceneManager.LoadScene("EditScene"); }
            );
        unitBtn.onClick.AddListener(
            () => { SceneManager.LoadScene("UnitManagerScene"); }
            );

        rayList.Add(Instantiate(godRay) as GameObject);
        rayList.Add(Instantiate(godRay) as GameObject);
        rayList.Add(Instantiate(godRay) as GameObject);

        foreach (GameObject ray in rayList)
        {
            ray.transform.position = new Vector3(0.5f, -1.0f, 0.5f);
        }

        rayInfoList.Add(new RayInfo(new Vector3(0.15f, 0.0f, 0.15f), 1));
        rayInfoList.Add(new RayInfo(new Vector3(0.1f, 0.0f, 0.0f), 1));
        rayInfoList.Add(new RayInfo(new Vector3(0.0f, 0.0f, 0.1f), 1));
    }

    void Update ()
    {
        for(int i = 0; i< rayList.Count; ++ i)
        {
            if (rayList[i].transform.position.x > 1.0f || rayList[i].transform.position.z > 1.0f)
                rayInfoList[i].SetDir(-1);

            if (rayList[i].transform.position.x < -1.0f || rayList[i].transform.position.z < -1.0f)
                rayInfoList[i].SetDir(1);

            rayList[i].transform.position += rayInfoList[i].speed * Time.deltaTime * rayInfoList[i].dir;
        }
    }
}
