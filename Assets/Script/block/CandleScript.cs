using UnityEngine;
using System.Collections;

public class CandleScript : MonoBehaviour
{
    public Light candleLight = null;
    public ParticleSystem fireParticleSys = null;

    public bool set = false;
    bool curSetting = false;

    public float maxTime = 3.0f;
    float curTime = 0.0f;

    void Start()
    {
        SetLightOff();
    }

	void Update()
    {
        Debug.Log(gameObject.name + " : " + curTime);
        if (curSetting == true)
            curTime -= Time.deltaTime;

        if (curTime < 0)
        {
            SetLightOff();
        }
    }

    public void SetLightOn()
    {
        curTime = maxTime;
        curSetting = true;

        candleLight.gameObject.SetActive(true);
        fireParticleSys.gameObject.SetActive(true);
    }

    public void SetLightOff()
    {
        curTime = 0.0f;
        curSetting = false;

        candleLight.gameObject.SetActive(false);
        fireParticleSys.gameObject.SetActive(false);
    }
}
