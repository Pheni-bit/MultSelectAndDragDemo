using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    public string name;
    public int maxLevel;
    public int buildingLevel;
    public int sizeX;
    public int sizeY;
    public Material transparentMaterial;
    public Material teamMaterial;
    public int requiredBuildingPoints;
    public int[] requiredResources, resourceAdds, resourceMults, currentAdds, currentMults;
    public float[] levelUpgradeResourceMultiplier;
    int subtractResourceTrigger = 1;
    public int buildPoints;
    public bool built;
    public bool mouseOverBuilding;
    public TerrainManager terrainManager;
    private void Start()
    {
        levelUpgradeResourceMultiplier = new float[maxLevel];
        for (int i = 0; i < maxLevel; i++)
        {
            levelUpgradeResourceMultiplier[i] = 1 + (i / 4);
        }
        built = true;
        Debug.Log(maxLevel);
        terrainManager = GameObject.Find("Terrain").GetComponent<TerrainManager>();
    }
    private void OnMouseEnter()
    {
        terrainManager.building = true;
        terrainManager.buildingSelected = this.gameObject;
    }
    private void OnMouseExit()
    {
        terrainManager.building = false;
        terrainManager.buildingSelected = this.gameObject;
    }
    public void Init()
    {
        buildingLevel = 1;
        gameObject.layer = 13;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = 13;
        }
        BuildingManager.BuildingsNeedingBuilt.Add(gameObject);
        TakeRequiredMaterials();
        AddOrSubResourceDifferences();
        ResourceManager.ResourceInstance.UpdateResourceUI();
    }
    public void TakeRequiredMaterials()
    {
        for (int i = 0; i < requiredResources.Length; i++)
        {
            ResourceManager.ResourceInstance.current[i] -= (int)(requiredResources[i] * levelUpgradeResourceMultiplier[buildingLevel]);
        }
    }
    public void AddBuildingPoints(int points)
    {
        buildPoints += points;
        if (requiredBuildingPoints <= buildPoints)
        {
            built = true;
            BuildingHasBeenBuilt();
            for (int i  = 0; i < buildingLevel; i++) //incase the building is being built on buildingLevel 2 for start vs buildingLevel 1 / potential technology upgrade
            {
                AddOrSubResourceDifferences();
            }
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
            if (requiredResources[i] * levelUpgradeResourceMultiplier[buildingLevel] > ResourceManager.ResourceInstance.current[i])
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
            for (int i = 0; i < buildingLevel; i++) 
            {
                AddOrSubResourceDifferences();
            }
        }
    }
    public void BuildingLevelUp()
    {
        buildingLevel++;
        TakeRequiredMaterials();
        AddOrSubResourceDifferences();
        UIManager.Instance.BuildingUI(this.gameObject);
        ResourceManager.ResourceInstance.UpdateResourceUI();
    }
}
