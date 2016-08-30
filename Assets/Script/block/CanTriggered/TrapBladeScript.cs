using UnityEngine;
using System.Collections;

public class TrapBladeScript : LinkedObjectScript
{
    public GameObject blade = null;
    private GameObject instBlade = null;

    void Start()
    {
        instBlade = Instantiate(blade) as GameObject;

        SetTriggerOff();
    }

	void Update ()
    {
        
    }

    public override void SetTriggerOn()
    {
        base.SetTriggerOn();

        instBlade.transform.position = transform.position;
    }

    public override void SetTriggerOff()
    {
        base.SetTriggerOff();

        Vector3 newPos = transform.position;
        newPos.y -= 0.5f;
        instBlade.transform.position = newPos;
    }
}
