using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BotProxy : MonoBehaviour
{
    GameObject thisParent;
    Bot parentBot;
    public List<GameObject> EnemyProxies = new List<GameObject>();
    public List<float> enemyDistances = new List<float>();
    public float colliderRadius;
    SphereCollider sphereCollider;

    private void Awake()
    {
        colliderRadius = 8;
        thisParent = gameObject.transform.parent.gameObject;
        parentBot = thisParent.GetComponent<Bot>();
        sphereCollider = this.gameObject.GetComponent<SphereCollider>();
        sphereCollider.radius = colliderRadius;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("were getting here1");
        if (other.gameObject.transform.parent)
        {
            if (other.gameObject.transform.parent.gameObject.CompareTag("Bot"))
            {
                //Debug.Log("were getting here2");
                if (other.gameObject.transform.parent.GetComponent<Bot>().team != parentBot.team)
                {
                    EnemyProxies.Add(other.gameObject.transform.parent.gameObject);
                    parentBot.enemysInRange = true;
                    //parentBot.GetComponent<Bot>().Attack(other.gameObject.transform.parent.gameObject);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.parent)
        {
            if (other.gameObject.transform.parent.gameObject.CompareTag("Bot"))
            {
                if (EnemyProxies.Contains(other.gameObject.transform.parent.gameObject))
                {
                    EnemyProxies.Remove(other.gameObject.transform.parent.gameObject);
                    if (EnemyProxies.Count == 0)
                    {
                        parentBot.enemysInRange = false;
                    }
                }
            }
        }  
    }
    private void FixedUpdate()
    {
        
        if (EnemyProxies.Count != 0)
        {
            for (int i  = 0; i < EnemyProxies.Count; i++)
            {
                GameObject enemyGO = EnemyProxies[i];
                if (enemyGO == null) 
                {
                    EnemyProxies.RemoveAt(i);
                }
            }
            EnemyProxies.Sort(SortEnemyDistance); 
        }
        else
        {
                parentBot.enemysInRange = false;    
        }
    }
    int SortEnemyDistance(GameObject x, GameObject y)
    {
            float distanceToX = Vector3.Distance(thisParent.transform.position, x.transform.position);
            float distanceToY = Vector3.Distance(thisParent.transform.position, y.transform.position);
            return distanceToX.CompareTo(distanceToY);
    }
    public void SetRadius(float radius)
    {
        sphereCollider.radius = radius;
    }

}
