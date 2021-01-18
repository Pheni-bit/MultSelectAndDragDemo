using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCaptureProxy : MonoBehaviour
{
    GameObject myParent;
    private void Awake()
    {
        myParent = gameObject.transform.parent.gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            myParent.GetComponent<Tower>().inRangeOfcapture.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            myParent.GetComponent<Tower>().inRangeOfcapture.Remove(other.gameObject);
        }
    }
}
