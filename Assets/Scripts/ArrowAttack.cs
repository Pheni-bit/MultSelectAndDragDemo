using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttack : MonoBehaviour
{
    Arrow thisArrow;

    private void Awake()
    {
        thisArrow = transform.GetComponentInParent<Arrow>();
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (thisArrow.hit == false)
        {
            if (other.gameObject.CompareTag("Floor"))
            {
                thisArrow.targetAquired = false;
                thisArrow.HitTerrain();
            }
            if (other.gameObject.CompareTag("Bot"))
            {
                thisArrow.targetAquired = false;
                thisArrow.HitBot(other.gameObject);
            }
        }
    }
}
