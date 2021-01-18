using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    public string name;
    int maxLevel;
    int level = 1;
    public int sizeX;
    public int sizeY;
    public Material transparentMaterial;
    public Material teamMaterial;
    public int requiredBuildingPoints;
    public int[] requiredResources;
    public int[] resourceAdds;
    public int[] resourceMults;
    int subtractResourceTrigger = 1;
    int buildPoints;
    bool built;
    public bool mouseOverBuilding;
    public TerrainManager terrainManager;
    private void Start()
    {
        Debug.Log(maxLevel);
        terrainManager = GameObject.Find("Terrain").GetComponent<TerrainManager>();
    }
    private void OnMouseEnter()
    {
        terrainManager.IfBuilding(true);
    }
    private void OnMouseExit()
    {
        terrainManager.IfBuilding(false);
    }
    public void Init()
    {
        gameObject.layer = 13;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = 13;
        }
        BuildingManager.BuildingsNeedingBuilt.Add(gameObject);
        TakeRequiredMaterials(requiredResources);
        AddOrSubResourceDifferences();
    }
    public void TakeRequiredMaterials(int[] resourceArr)
    {
        for (int i = 0; i < resourceArr.Length; i++)
        {
            ResourceManager.ResourceInstance.current[i] -= resourceArr[i];
        }
    }
    public void AddBuildingPoints(int points)
    {
        buildPoints += points;
        if (requiredBuildingPoints <= buildPoints)
        {
            built = true;
            BuildingHasBeenBuilt();
            AddOrSubResourceDifferences();
        }
    }
    public void BuildingHasBeenBuilt()
    {
        Collider collider = gameObject.GetComponent<Collider>();
        Renderer renderer = gameObject.GetComponent<Renderer>();
        NavMeshObstacle navMeshObstacle = gameObject.GetComponent<NavMeshObstacle>();
        if (collider != null)
            collider.enabled = true;
        if (navMeshObstacle != null)
            navMeshObstacle.enabled = true;
        if (renderer != null)
            renderer.material = teamMaterial;
        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer renderer1 = transform.GetChild(i).GetComponent<Renderer>();
            Collider collider1 = transform.GetChild(i).GetComponent<Collider>();
            if (renderer1 != null)
                renderer1.material = teamMaterial;
            if (collider1 != null)
                collider1.isTrigger = false;
        }
    }
    public void AddOrSubResourceDifferences()
    {
        for (int i = 0; i < resourceAdds.Length; i++)
        {
            ResourceManager.addedResources[i] += resourceAdds[i] * subtractResourceTrigger;
            ResourceManager.addedMultipliers[i] += resourceMults[i] * subtractResourceTrigger;
        }
    }
    public bool HasTheRequiredResources()
    {
        for (int i = 0; i < requiredResources.Length; i++)
        {
            if (requiredResources[i] > ResourceManager.ResourceInstance.current[i])
            {
                return false;
            }
        }
        return true;
    }
    private void OnDestroy()
    {
        if (built)
        {
            subtractResourceTrigger = -1;
            AddOrSubResourceDifferences();
        }
    }
}
