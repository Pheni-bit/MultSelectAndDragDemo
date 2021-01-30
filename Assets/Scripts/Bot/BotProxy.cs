using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BotProxy : MonoBehaviour
{
    GameObject thisParent;
    Bot parentBot;
    int team;
    Cohort grandParentCohort;
    public List<float> enemyDistances = new List<float>();
    public float colliderRadius;
    SphereCollider sphereCollider;

    private void Awake()
    {
        colliderRadius = 8;
        thisParent = gameObject.transform.parent.gameObject;
        parentBot = thisParent.GetComponent<Bot>();
        team = parentBot.team;
        sphereCollider = this.gameObject.GetComponent<SphereCollider>();
        sphereCollider.radius = colliderRadius;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            //Debug.Log("were getting here2");
            if (other.gameObject.GetComponent<Bot>().team != team)
            {
                Cohort cohort = thisParent.transform.parent.GetComponent<Cohort>();
                if (!cohort.enemyProxies.Contains(other.gameObject)) //.transform.parent.gameObject))
                {
                    cohort.enemyProxies.Add(other.gameObject); //.transform.parent.gameObject);
                    Debug.Log(other.gameObject.name);
                }
                //parentBot.enemysInRange = true;
                //parentBot.GetComponent<Bot>().Attack(other.gameObject.transform.parent.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Bot"))
        {
            Cohort cohort = thisParent.transform.parent.GetComponent<Cohort>();
            Collider[] collider = Physics.OverlapSphere(other.gameObject.transform.position, 0.01f);
            for (int i = 0; i < collider.Length; i++)
            {
                if (collider[i].gameObject.CompareTag("Proxy"))
                {
                    if (collider[i].gameObject.transform.parent.transform.parent.gameObject == cohort.gameObject)
                    {
                        return;
                    }
                }
            }
            if (cohort.enemyProxies.Contains(other.gameObject))
            {
                cohort.enemyProxies.Remove(other.gameObject);
                if (cohort.enemyProxies.Count == 0)
                {
                    //parentBot.enemysInRange = false;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        

    }
    
    public void SetRadius(float radius)
    {
        sphereCollider.radius = radius;
    }

}
