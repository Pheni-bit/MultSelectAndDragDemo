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
    public GameObject buildingSelected;

    public List<GameObject> priorCohorts = new List<GameObject>();
    public TerrainGrid terrainGrid;

    void Start()
    {
        charactersHolder = GameObject.Find("Characters");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.BuildingUIOff();
        Debug.Log("Getting Called");
        if (Input.GetMouseButtonUp(0))
        {
            Bot.DeselectAll(eventData);
            if (building && terrainGrid.gameObject.activeSelf == false)
            {
                Debug.Log("left clicking building");
                //buildingSelected
                UIManager.Instance.BuildingUI(buildingSelected);
            }
            else if (terrainGrid.gameObject.activeSelf == true)
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
            
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (building) // are we hovering over a building when pressing 
            {
                Debug.Log("right clicking building");
                if (!buildingSelected.GetComponent<Building>().built) // is building built 
                {
                    if (UIManager.Selected) // are bots selected
                    {
                        bool tempBool = false;
                        foreach(Bot bot in Bot.currentlySelectedBots)
                        {
                            if (bot.botType == 0)
                            {
                                tempBool = true;
                                break;
                            }
                        }
                        if (tempBool)
                        {
                            List<Bot> tempWorkerBotList = new List<Bot>();
                            worldPosition = buildingSelected.transform.position;
                            foreach (Bot bot in Bot.currentlySelectedBots) // add worker bots to seperate list
                            {
                                if (bot.botType == 0)
                                    tempWorkerBotList.Add(bot);
                            }
                            Bot.DeselectAll(eventData); // deselect all from hashSet
                            foreach(Bot bot in tempWorkerBotList) // reselect bots from temp list
                            {
                                bot.OnSelect(eventData);
                            }
                            AnyCohorts();
                            DetectAndCreateCohort();
                            RemovePriorEmptyCohort();
                            UIManager.Instance.GetSelectedCohort(); //for ui i think
                        }
                    }
                }
            }
            else if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Terrain")))
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
    public void UpdateCohort() // called from the ui manager when changes to the formation are made
    {
        int childCount = 0;
        foreach(Bot bot in Bot.currentlySelectedBots)
        {
            childCount = bot.transform.parent.childCount;
            break;
        }
        if (Bot.currentlySelectedBots.Count != childCount)
        {
            foreach (Bot bot in Bot.currentlySelectedBots)
            {
                worldPosition = bot.transform.position;
                break;
            }
        }
        else
        {
            foreach(Bot bot in Bot.currentlySelectedBots)
            {
                worldPosition = bot.transform.parent.position;
                break;
            }
        }
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
        foreach (Bot bot in Bot.currentlySelectedBots)
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
        if (building)
        {
            foreach (Bot bot in Bot.currentlySelectedBots)
            {
                bot.gameObject.GetComponent<BotWorker>().OrderedToBuild(buildingSelected);
            }
        }
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
}
