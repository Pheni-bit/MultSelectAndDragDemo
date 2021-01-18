using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLight : MonoBehaviour
{
    public float startingTime, nightMinutes, dayMinutes;
    Light light;
    float totalDaySeconds, totalNightSeconds, totalTime, priorTimetime, runningTime, rotX, dayFraction, nightFraction;
    float baseLightIntesity = 0.5f;
    int amountOfDaysPast;
    // Start is called before the first frame update
    void Start()
    {
        light = gameObject.GetComponent<Light>();
        gameObject.transform.eulerAngles = (new Vector3(rotX, 10, 0));
        light.intensity = 0.0f;
        totalDaySeconds = 60 * dayMinutes;
        totalNightSeconds = 60 * nightMinutes;
        totalTime = totalNightSeconds + totalDaySeconds;
        dayFraction = 180 / totalDaySeconds;
        nightFraction = 180 / totalNightSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        ManageSunRunningTime();
        ManageSunRotation();
        ManageSunIntensity();
    }
    void ManageSunRunningTime()
    {
        if (amountOfDaysPast == 0)
            runningTime = Time.time + startingTime;
        else
            runningTime = Time.time - priorTimetime;
        //Debug.Log(runningTime);
        if (runningTime >= totalTime)
        {
            runningTime = 0;
            priorTimetime = Time.time;
            amountOfDaysPast++;
        }
    }
    void ManageSunRotation()
    {
        if (runningTime < totalDaySeconds)
        {   //its daytime
            rotX = dayFraction * runningTime;
        }
        else
        {   //its nighttime
            rotX = nightFraction * (runningTime - totalDaySeconds);
            rotX += 180;
        }
        gameObject.transform.eulerAngles = new Vector3(rotX, 10, 0);
    }
    void ManageSunIntensity()
    {
        if (rotX > 170)
        {
            if (light.intensity > 0)
            {
                float tempFloat = 180 - rotX;
                float tempFloat2 = baseLightIntesity / 10;
                light.intensity = tempFloat * tempFloat2;
            }
            else
            {
                light.intensity = 0;
            }
        }
        else if (rotX > 0)
        {
            if (light.intensity < baseLightIntesity)
            {
                float tempFloat = baseLightIntesity / 20;
                light.intensity = rotX * tempFloat;
            }
            else
            {
                light.intensity = baseLightIntesity;
            }
        }
    }
}
