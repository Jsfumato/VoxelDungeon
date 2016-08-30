using UnityEngine;
using System.Collections;

public class CandleScript : MonoBehaviour
{
    public Light candleLight = null;
    public ParticleSystem fireParticleSys = null;

    public bool alwaysLightOn = false;
    bool curSetting = false;

    public float maxTime = 3.0f;
    float curTime = 0.0f;

    void Start()
    {
        SetLightOff();
    }

	void Update()
    {
        if (alwaysLightOn == true)
        {
            SetLightOn();
        }
        else
        {
            if (curSetting == true)
                curTime -= Time.deltaTime;

            if (curTime < 0)
            {
                SetLightOff();
            }
        }
    }

    public void SetLightOn()
    {
        curTime = maxTime;
        curSetting = true;

        candleLight.gameObject.SetActive(true);
        if(fireParticleSys.isPlaying != true)
            fireParticleSys.Play();
    }

    public void SetLightOff()
    {
        curTime = 0.0f;
        curSetting = false;

        candleLight.gameObject.SetActive(false);
        fireParticleSys.Stop();
    }
}
