using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    public string buildingName;
    public int maxLevel;
    public int buildingLevel;
    public int sizeX;
    public int sizeY;
    public Material transparentMaterial;
    public Material teamMaterial;
    public int requiredBuildingPoints;
    public int[] requiredResources, resourceAdds, resourceMults;
    public int[] currentAdds;
    public int[] currentMults;
    public float[] levelUpgradeResourceMultiplier;
    int subtractResourceTrigger = 1;
    public int buildPoints;
    public bool built;
    public TerrainManager terrainManager;
    BuildingManager buildingManager;
    private void Awake()
    {
        //Debug.Log(levelUpgradeResourceMultiplier.Length + " lvlUpgradeResourceMult");
        //built = true;
        terrainManager = GameObject.Find("Terrain").GetComponent<TerrainManager>();
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();
        currentAdds = new int[4];
        currentMults = new int[4];
        levelUpgradeResourceMultiplier = new float[maxLevel];
        for (int i = 0; i < maxLevel; i++)
        {
            levelUpgradeResourceMultiplier[i] = 1.0f + (i / 4.0f);
        }
    }
    private void OnMouseEnter()
    {
        TerrainManager.building = true;
        terrainManager.buildingSelected = gameObject;
    }
    private void OnMouseExit()
    {
        TerrainManager.building = false;
        terrainManager.buildingSelected = gameObject;
    }
    public void Init()
    {
        gameObject.layer = 13;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = 13;
        }
        BuildingManager.BuildingsNeedingBuilt.Add(gameObject);
        TakeRequiredMaterials();
        buildingLevel = 1;
        AddOrSubResourceDifferences();
        ResourceManager.ResourceInstance.UpdateResourceUI();
        transform.parent = buildingManager.gameObject.transform;
    }
    public void TakeRequiredMaterials()
    {
        
        for (int i = 0; i < requiredResources.Length; i++)
        {
            //Debug.Log(levelUpgradeResourceMultiplier.Length + " levelmultLength");
            //Debug.Log(buildingLevel + " is building level");
            //Debug.Log(levelUpgradeResourceMultiplier[buildingLevel] + " combined");
            ResourceManager.ResourceInstance.current[i] -= (int)(requiredResources[i] * levelUpgradeResourceMultiplier[buildingLevel]);
        }
    }
    public void AddBuildingPoints(int points)
    {
        buildPoints += points;
        if (requiredBuildingPoints <= buildPoints)
            BuildingHasBeenBuilt();
    }
    public void BuildingHasBeenBuilt()
    {
        built = true;
        Collider collider = gameObject.GetComponent<Collider>();
        Renderer renderer = gameObject.GetComponent<Renderer>();
        NavMeshObstacle navMeshObstacle = gameObject.GetComponent<NavMeshObstacle>();
        if (collider != null)
            collider.enabled = true;
        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = true;
        }
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
        BuildingManager.BuildingsNeedingBuilt.Remove(gameObject);
        for (int i = 0; i < buildingLevel; i++) //incase the building is being built on buildingLevel 2 for start vs buildingLevel 1 / potential technology upgrade
        {
            AddOrSubResourceDifferences();
        }
    }
    public void AddOrSubResourceDifferences()
    {
        for (int i = 0; i < resourceAdds.Length; i++)
        {
            ResourceManager.addedResources[i] += resourceAdds[i] * subtractResourceTrigger;
            currentAdds[i] += resourceAdds[i] * subtractResourceTrigger;
            ResourceManager.addedMultipliers[i] += resourceMults[i] * subtractResourceTrigger;
            currentMults[i] += resourceMults[i] * subtractResourceTrigger;
        }
    }
    public bool HasTheRequiredResources()
    {
        if (buildingLevel == maxLevel)
        {
            return false;
        }
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
        TakeRequiredMaterials();
        buildingLevel++;
        AddOrSubResourceDifferences();
        UIManager.Instance.BuildingUI(this.gameObject);
        ResourceManager.ResourceInstance.UpdateResourceUI();
    }
}
