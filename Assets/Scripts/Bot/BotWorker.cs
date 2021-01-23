using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotWorker : Bot
{
    GameObject targetOrderedToBuild;
    bool getBuilding, canAddBuildPoints = true;
    public bool toggledToBuild;
    public bool closeEnough;
    int buildPointsAddedPerTick = 1;
    public void OrderedToBuild(GameObject buildTarget)
    {
        targetOrderedToBuild = buildTarget;
        getBuilding = true;
    }
    public override void ManageOrders() // called in base update
    {

        base.ManageOrders();
        if (!directOrder)
        {
            if (getBuilding)
            {
                if (!targetOrderedToBuild.GetComponent<Building>().built)
                {
                    GoToBuildingTarget();
                }
                else
                {

                    closeEnough = false;
                    getBuilding = false;

                }
            }
            else if (toggledToBuild && BuildingManager.BuildingsNeedingBuilt.Count > 0)
            {
                getBuilding = true;
                List<GameObject> tempList = BuildingManager.BuildingsNeedingBuilt;
                tempList.Sort(SortBuildingsNeededBuiltDistances);
                targetOrderedToBuild = tempList[0];
            }
        }
        else
            closeEnough = false;
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
        Ray buildRay = new Ray(transform.position, navMeshAgent.transform.forward);
        if (!closeEnough)
            SetNavDestination(targetOrderedToBuild.transform.position);
        else
            SetNavDestination(transform.position);
        SetNavRotation(targetOrderedToBuild);
        if (Physics.Raycast(buildRay, out RaycastHit hit, 1.5f, ~gameObject.layer))
        {
            if (hit.transform.gameObject == targetOrderedToBuild)
            {
                closeEnough = true;
                if (canAddBuildPoints)
                {
                    canAddBuildPoints = false;
                    StartCoroutine(AddBuildPointsCoolDownRoutine());
                }
            }
            else
                closeEnough = false;
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
        canAddBuildPoints = true;
    }
}
