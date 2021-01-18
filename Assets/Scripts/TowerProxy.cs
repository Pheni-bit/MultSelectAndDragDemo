using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProxy : MonoBehaviour
{
    GameObject myParent;
    Tower tower;
    private void Awake()
    {
        myParent = gameObject.transform.parent.gameObject;
        tower = myParent.GetComponent<Tower>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            //Debug.Log("DetectingBotToShoot");
            tower.inRangeToShoot.Add(other.gameObject);
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            tower.inRangeToShoot.Remove(other.gameObject);
        }
    }
}
