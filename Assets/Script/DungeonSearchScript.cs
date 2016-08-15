using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public class DungeonSearchScript : MonoBehaviour
{
    [Range(0, 6)]
    public int MAX_MAPSIZE = 4;
    public Camera mainCam = null;
    public GameObject godRay = null;

    // UI
    public Button enterDungeonBtn = null;
    public Button closeBtn = null;
    public Image bgPopupImage = null;

    private GameObject selectedTile = null;

	void Start ()
    {
        InitMapBlock();
        enterDungeonBtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
        bgPopupImage.gameObject.SetActive(false);

        enterDungeonBtn.onClick.AddListener(()=>SceneManager.LoadScene("GameScene"));
        closeBtn.onClick.AddListener(DisSelectMapTile);
    }
	
	void Update ()
    {
        SelectMapTile();

        if(selectedTile != null)
        {
            enterDungeonBtn.gameObject.SetActive(true);
            closeBtn.gameObject.SetActive(true);
            bgPopupImage.gameObject.SetActive(true);
        }
        if (selectedTile == null)
        {
            enterDungeonBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(false);
            bgPopupImage.gameObject.SetActive(false);
        }
    }


    void SelectMapTile()
    {
        if (Input.touchCount == 0)
            return;

        if (Input.touches[0].phase != TouchPhase.Began)
            return;

        if (selectedTile != null)
            return;

        GameObject selected = SelectObject(Input.touches[0].position);
        if (selected == null)
            return;

        Debug.Log(selected);
        //selectedTile = Instantiate(selected) as GameObject;
        selectedTile = Instantiate(MapDataManager.MapDic[selected.GetComponent<TileInfo>().tileID]) as GameObject;
        //selectedTile.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);

        Vector3 pos = mainCam.transform.position;
        pos.x -= (3 * (pos.x / pos.y) +1);
        pos.z -= (3 * (pos.z / pos.y)+1);
        pos.y -= 3;

        selectedTile.transform.position = pos;
    }

    void DisSelectMapTile()
    {
        //if (Input.touchCount == 0)
        //    return;

        //if (Input.touches[0].phase != TouchPhase.Began)
        //    return;

        //if (selectedTile == null)
        //    return;

        //GameObject selected = SelectObject(Input.touches[0].position);

        //if (selected == selectedTile)
        //    return;
         
        Destroy(selectedTile);
        selectedTile = null;
        return;
    }

    void InitMapBlock()
    {
        Vector3 pos = mainCam.transform.position;
        pos.x += ((float)MAX_MAPSIZE / 2 - 0.5f);
        pos.z += ((float)MAX_MAPSIZE / 2 - 0.5f);

        mainCam.transform.position = pos;

        for (int x = 0; x< MAX_MAPSIZE;++x)
        {
            for (int z = 0; z < MAX_MAPSIZE; ++z)
            {
                GameObject mapTile = Instantiate(MapDataManager.MapDic[MAP_ID.PLAIN]) as GameObject;
                mapTile.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                mapTile.transform.position = new Vector3(x, 0.0f, z);
            }
        }
    }


    GameObject SelectObject(Vector2 touchPosition)
    {
        Ray ray = mainCam.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, 0));

        List<newRaycastResult> hitList = new List<newRaycastResult>();

        hitList.Clear();
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            hitList.Add(new newRaycastResult(hit));
        }

        if (hitList.Capacity == 0)
            return null;

        hitList.Sort();

        // layer 10 is Block Object;
        if (hits[0].transform.gameObject.layer == 10)
            return hits[0].transform.gameObject;

        return null;
    }
}
