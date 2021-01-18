using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotNav : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    private float pathEndThreshold = 1.5f;
    // Start is called before the first frame update
    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(gameObject.transform.position, navMeshAgent.transform.forward * 1, Color.blue);
        
    }

    public bool CompletedOrder()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + pathEndThreshold;
    }
    public void SetStoppingDistance(float distance)
    {
        navMeshAgent.stoppingDistance = distance;
    }
    public void SetNavDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }
    public void SetNavRotation(GameObject enemy)
    {
        navMeshAgent.transform.LookAt(enemy.transform.position);
        navMeshAgent.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
    }
}
