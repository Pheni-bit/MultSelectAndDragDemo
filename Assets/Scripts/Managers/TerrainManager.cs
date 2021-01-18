using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TerrainManager : MonoBehaviour, IPointerClickHandler
{
    public LayerMask layerMask;
    Vector3 worldPosition;
    public Vector3 cohortRotation;
    Ray ray;
    public DragBoxHandler dragBoxHandler;
    GameObject charactersHolder;
    Cohort createdCohort;
    GameObject cohort;
    public bool building;
    public List<GameObject> priorCohorts = new List<GameObject>();
    public TerrainGrid terrainGrid;

    void Start()
    {
        charactersHolder = GameObject.Find("Characters");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Getting Called");
        if (Input.GetMouseButtonUp(0))
        {
            BotClickerData.DeselectAll(eventData);
            //if (building)
            //{
                if (terrainGrid.gameObject.activeSelf == true)
                {
                    if (terrainGrid.AttemptToPlaceObject() == "true")
                    {
                        terrainGrid.PlaceObject();
                    }
                    else
                    {
                        UIManager.Instance.UpdateAndRemoveErrorText(terrainGrid.AttemptToPlaceObject());
                    }

                }
            //}
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (building)
            {

            }
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Terrain")))
            {
                if (UIManager.Selected)
                {
                    worldPosition = SetPos(hitData);
                    AnyCohorts();
                    DetectAndCreateCohort();
                    RemovePriorEmptyCohort();
                    UIManager.Instance.GetSelectedCohort();
                }
            }
            
        }
    }
    public void UpdateCohort(/*Vector3 newWorldPos*/)
    {
        int childCount = 0;
        foreach(BotClickerData bot in BotClickerData.currentlySelected)
        {
            childCount = bot.transform.parent.childCount;
            break;
        }
        if (BotClickerData.currentlySelected.Count != childCount)
        {
            foreach (BotClickerData bot in BotClickerData.currentlySelected)
            {
                worldPosition = bot.transform.position;
                break;
            }
        }
        else
        {
            foreach(BotClickerData bot in BotClickerData.currentlySelected)
            {
                worldPosition = bot.transform.parent.position;
                break;
            }
        }
        //worldPosition = newWorldPos;
        AnyCohorts();
        DetectAndCreateCohort();
        RemovePriorEmptyCohort();
        UIManager.Instance.GetSelectedCohort();
    }
    Vector3 SetPos(RaycastHit hit)
    {
        if (DragBoxHandler.dragging)
            return dragBoxHandler.worldStartPos;
        else
            return hit.point;
    }
    public void AnyCohorts()
    {
        priorCohorts.Clear();
        foreach (BotClickerData bot in BotClickerData.currentlySelected)
        {
            if (bot.transform.parent.CompareTag("Cohort"))
            {
                if (!priorCohorts.Contains(bot.transform.parent.gameObject))
                priorCohorts.Add(bot.transform.parent.gameObject);
            }
        }
    }
    void DetectAndCreateCohort()
    {
        cohortRotation = dragBoxHandler.tempArrowRotation;
        cohort = NewCohort(worldPosition, cohortRotation);
        cohort.GetComponent<Cohort>().Init();
    }
    void RemovePriorEmptyCohort()
    {
        if (priorCohorts.Count != 0)
        {
            for (int i = 0; i <  priorCohorts.Count; i++)
            {
                if (priorCohorts[i].transform.childCount == 0)
                {
                    Destroy(priorCohorts[i]);
                }
            }
        }
    }
    GameObject NewCohort(Vector3 pos, Vector3 rot)
    {
        GameObject cohort = new GameObject();
        cohort.transform.position = pos;
        cohort.transform.rotation = Quaternion.Euler(rot);
        cohort.name = "Cohort";
        cohort.tag = "Cohort";
        cohort.transform.parent = charactersHolder.transform;
        cohort.AddComponent<Cohort>();
        if (priorCohorts.Count != 0)
        {
            Cohort thisCohortSwitch = cohort.GetComponent<Cohort>();
            Cohort priorCohortScript = priorCohorts[0].GetComponent<Cohort>();
            thisCohortSwitch.aggression = priorCohortScript.aggression;
            thisCohortSwitch.density = priorCohortScript.density;
            thisCohortSwitch.stance = priorCohortScript.stance;
        }
        return cohort;
    }
    public void IfBuilding(bool build)
    {
        building = build;
    }
}
