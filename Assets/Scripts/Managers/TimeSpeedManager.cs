using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeSpeedManager : MonoBehaviour
{
    Toggle[] timeToggleArray = new Toggle[4];
    Toggle pauseToggle;
    Toggle playToggle;
    Toggle forwardToggle;
    Toggle fastForwardToggle;
    [SerializeField]
    Toggle LastToggleOn;
    
    ToggleGroup timeToggleGroup;
    private void Awake()
    {

        pauseToggle = gameObject.transform.GetChild(0).GetComponent<Toggle>();
        timeToggleArray[0] = pauseToggle;
        playToggle = gameObject.transform.GetChild(1).GetComponent<Toggle>();
        timeToggleArray[1] = playToggle;
        forwardToggle = gameObject.transform.GetChild(2).GetComponent<Toggle>();
        timeToggleArray[2] = forwardToggle;
        fastForwardToggle = gameObject.transform.GetChild(3).GetComponent<Toggle>();
        timeToggleArray[3] = fastForwardToggle;
        timeToggleGroup = gameObject.GetComponent<ToggleGroup>();
        LastToggleOn = playToggle;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pauseToggle.isOn == true)
            {
                LastToggleOn.isOn = true;
                
            }
            else
            {
                foreach(Toggle tog in timeToggleGroup.ActiveToggles())
                {
                    if (tog.isOn == true)
                    {
                        LastToggleOn = tog;
                        //Debug.Log(LastToggleOn + "firstone");
                        break;
                    }
                }
                /*foreach (Toggle tog in timeToggleArray)
                {
                    if (tog.isOn == true)
                    {
                        LastToggleOn = tog;
                        Debug.Log(LastToggleOn + "secondone");
                    }
                }*/
                pauseToggle.isOn = true;
            }
        }   
    }
    public void CheckToggles()
    {
        if (pauseToggle.isOn)
        {
            Time.timeScale = 0;
        }
        else if (playToggle.isOn)
        {
            Time.timeScale = 1;
            LastToggleOn = playToggle;
        }
        else if (forwardToggle.isOn)
        {
            Time.timeScale = 2;
            LastToggleOn = forwardToggle;
        }
        else if (fastForwardToggle.isOn)
        {
            Time.timeScale = 4;
            LastToggleOn = fastForwardToggle;
        }
    }
}
