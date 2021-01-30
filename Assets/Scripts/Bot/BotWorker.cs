using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotWorker : Bot
{
    GameObject targetOrderedToBuild;
    Vector3 posAssignedToBuild;
    bool getBuilding;
    public bool toggledToBuild = true;
    public bool canAddBuildPoints;
    int buildPointsAddedPerTick = 1;
    float buildingTick = 1;
    float tempTime;
    public override void Awake()
    {
        base.Awake();
        toggledToBuild = true;
    }
    public void OrderedToBuild(GameObject buildTarget)
    {
        targetOrderedToBuild = buildTarget;
        getBuilding = true;
    }
    public override void ManageOrders() // called in base update
    {

        base.ManageOrders();
        //if (!directOrder)
        //{
            if (getBuilding)
            {
                if (targetOrderedToBuild)
                {
                    if (!targetOrderedToBuild.GetComponent<Building>().built)
                    {
                        GoToBuildingTarget();
                    }
                    else
                    {
                        posAssignedToBuild = Vector3.zero;
                        getBuilding = false;
                    }
                }
                else
                {
                    posAssignedToBuild = Vector3.zero;
                    getBuilding = false;
                }

            }
            else if (toggledToBuild && BuildingManager.BuildingsNeedingBuilt.Count > 0)
            {
                //Debug.Log("Getting here");
                getBuilding = true;
                List<GameObject> tempList =  BuildingManager.BuildingsNeedingBuilt;
                tempList.Sort(SortBuildingsNeededBuiltDistances);
                for(int i = 0; i < tempList.Count; i++)
                {
                    Vector3 tempVec = tempList[i].GetComponent<Building>().ReturnAvailableBuildSpot(gameObject);
                    if (tempVec != Vector3.zero)
                    {
                        targetOrderedToBuild = tempList[i];
                        posAssignedToBuild = tempVec;
                        Debug.Log(targetOrderedToBuild);
                        break;
                    }

                }
                
            }
            else
            {
                posAssignedToBuild = Vector3.zero;
            }
        //}
        //else
            //posAssignedToBuild = Vector3.zero;
    }
    int SortBuildingsNeededBuiltDistances(GameObject x, GameObject y)
    {
        float distanceToX = Vector3.Distance(transform.position, x.transform.position);
        float distanceToY = Vector3.Distance(transform.position, y.transform.position);
        return distanceToX.CompareTo(distanceToY);
    }
    void GoToBuildingTarget()
    {
        //Debug.Log("GETTING CALLED TO BUILD");
        if (posAssignedToBuild == Vector3.zero)
            posAssignedToBuild = targetOrderedToBuild.GetComponent<Building>().ReturnAvailableBuildSpot(gameObject);
        if (posAssignedToBuild != Vector3.zero)
        {
            navMeshAgent.SetDestination(posAssignedToBuild);
            SetStoppingDistance(0);
            SetNavRotation(targetOrderedToBuild);
            if (CompletedOrder())
            {
                if (canAddBuildPoints)
                {
                    if (Time.time > buildingTick + tempTime)
                    {
                        tempTime = Time.time;
                        StartCoroutine(AddBuildPointsCoolDownRoutine());
                    }
                }
            }
        }
        else
        {

        }
    }
    IEnumerator AddBuildPointsCoolDownRoutine()
    {
        transform.Translate(Vector3.forward * 0.2f);
        yield return new WaitForSeconds(0.1f);
        if (targetOrderedToBuild != null)
        {
            targetOrderedToBuild.GetComponent<Building>()
                  .AddBuildingPoints(buildPointsAddedPerTick);
        }
        transform.Translate(Vector3.forward * -0.2f);
        yield return new WaitForSeconds(0.9f);
    }
}
