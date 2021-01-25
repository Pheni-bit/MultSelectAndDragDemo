using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSpawnSpotChecker : MonoBehaviour
{
    public GameObject parentBuildingObj;
    public void Init()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f, ~2 << 8, QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            Debug.Log(colliders[i].gameObject.name);
            if (colliders[i].gameObject.CompareTag("Building"))
            {
                parentBuildingObj.GetComponent<Building>().spawnBuildSpots.Remove(gameObject);
                colliders[i].gameObject.GetComponent<Building>().ReInitSpotCheckers();
                Debug.DebugBreak();
                Destroy(gameObject);
            }
            else if (colliders[i].gameObject.CompareTag("Floor"))
            {
                Debug.DebugBreak();
                Debug.Log(transform.position.y);
                Destroy(gameObject);
            }
        }
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * 0.4f, Color.green);
    }
    
    private void OnDestroy()
    {
        Building parentBuilding = parentBuildingObj.GetComponent<Building>();
        if (parentBuilding.spawnBuildSpots.Contains(gameObject))
        {
            parentBuilding.spawnBuildSpots.Remove(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("kinda got em");
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Bot"))
        {
            Debug.Log("GOt'em");
            if (other.GetComponent<Bot>().botType == 0)
            {
                other.GetComponent<BotWorker>().canAddBuildPoints = true;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            if (other.GetComponent<Bot>().botType == 0)
            {
                other.GetComponent<BotWorker>().canAddBuildPoints = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            if (other.GetComponent<Bot>().botType == 0)
            {
                other.GetComponent<BotWorker>().canAddBuildPoints = false;
            }
        }
    }
}
