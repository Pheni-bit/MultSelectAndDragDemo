using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ITickable
{
    private static int lastSelectedCount;
    public Image[] bottomPanelImageArr;
    public Text[] bottomPanelTextArr;
    public GameObject[] bottomSelected;
    public GameObject[] selectedBuildingAddMultText;
    public GameObject formationPanel;
    public GameObject selectedBuildingPanel;
    GameObject selectedBuildingObj;
    private static UIManager _instance;
    public Text[] resourceTexts;
    [SerializeField]
    public static bool Selected;
    bool tempSelected;
    TerrainManager terrainManager;
    public GameObject selectedCohort;
    public Text errorText;
    public List<GameObject> selectedCohorts = new List<GameObject>();
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("uimanager is null");
            }
            return _instance;
        }
    }
    private void Start()
    {
        GameTick.InGameTick += Tick;
        //Debug.Log("scale factor = " + gameObject.GetComponent<Canvas>().scaleFactor);
        _instance = this;
        terrainManager = GameObject.Find("Terrain").GetComponent<TerrainManager>();
    }

    void Update()
    {
        SetSelected();
        OnSelectedChange();
        OnSelectedCountChange();
        IfBuildingPanelOpen();
    }
    void SetSelected()
    {
        if (Bot.currentlySelectedBots.Count > 0)
            Selected = true;
        else
        {
            Selected = false;
            selectedCohort = null;
        }
    }
    void OnSelectedChange()
    {
        if (tempSelected != Selected)
        {
            formationPanel.SetActive(Selected);
        }
        tempSelected = Selected;
    }
    void OnSelectedCountChange()
    {
        if (lastSelectedCount != Bot.currentlySelectedBots.Count)
        {
            if (Selected)
            {
                GetSelectedCohort();
            }
            CompareBot();
        }
        lastSelectedCount = Bot.currentlySelectedBots.Count;
    }
    void IfBuildingPanelOpen() // shows the running building points and progression bar for building the building
    {
        if (selectedBuildingPanel.gameObject.activeSelf == true)
        {
            Building buildingScript = selectedBuildingObj.GetComponent<Building>();
            if (selectedBuildingPanel.transform.GetChild(3).gameObject.activeSelf == true) // if building is not built
            {
                int points = buildingScript.buildPoints;
                int requirePoints = buildingScript.requiredBuildingPoints;
                GameObject buildingNotBuildPanel = selectedBuildingPanel.transform.GetChild(3).gameObject;
                RectTransform buildingPointsProgressionBar = buildingNotBuildPanel.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<RectTransform>();
                buildingPointsProgressionBar.anchorMax = new Vector2((float)points / requirePoints, 1);
            }
        }
    }
    public void UpgradeBuildingButton()
    {
        selectedBuildingObj.GetComponent<Building>().BuildingLevelUp();
    }
    public void GetSelectedCohort()
    {
        selectedCohorts.Clear();
        foreach (Bot bot in Bot.currentlySelectedBots)
        {
            if (bot.transform.parent.CompareTag("Cohort"))
            {
                if (!selectedCohorts.Contains(bot.transform.parent.gameObject))
                    selectedCohorts.Add(bot.transform.parent.gameObject);
            }
        }
        if (selectedCohorts.Count == 1)
        {
            selectedCohort = selectedCohorts[0];
        }
        else
        {
            selectedCohort = null;
        }
    }
    void CompareBot()
    {
        int bot0 = 0;
        int bot1 = 0;
        Color color = Color.white;
        foreach (Bot bot in Bot.currentlySelectedBots)
        {
            if (bot.botType == 0)
            {
                bot0++;
            }
            if (bot.botType == 1)
            {
                bot1++;
            }
        }
        //Debug.Log(bot0 + " " + bot1);
        foreach (Bot bot in Bot.currentlySelectedBots)
        {
            color = bot.GetComponent<Bot>().unselectedMaterial.color;
            break;
        }
        if (bot0 > 0)
        {
            UpdateBottomPanel(bot0, 0, color);
        }
        else
        {
            HideTheBottomSelected(0);
        }
        if (bot1 > 0)
        {
            UpdateBottomPanel(bot1, 1, color);
        }
        else
        {
            HideTheBottomSelected(1);
        }
    }
    public void UpdateBottomPanel(int numberOfSelected, int index, Color color)
    {
        bottomSelected[index].SetActive(true);
        bottomPanelImageArr[index].color = color;
        bottomPanelTextArr[index].text = numberOfSelected.ToString();
    }
    public void HideTheBottomSelected(int index)
    {
        bottomSelected[index].SetActive(false);
    }
    public void UpdateCurrentCohortAggression(string str)
    {
        if (Selected && selectedCohort != null)
        {
            selectedCohort.GetComponent<Cohort>().aggression = str;
            terrainManager.UpdateCohort();
        }
        
    }
    public void UpdateCurrentCohortStance(string str)
    {
        if (Selected && selectedCohort != null)
        {
            selectedCohort.GetComponent<Cohort>().stance = str;
            terrainManager.UpdateCohort();
        }
        
    }
    public void UpdateCohortDensity(string str)
    {
        if (Selected && selectedCohort != null)
        {
            selectedCohort.GetComponent<Cohort>().density = str;
            terrainManager.UpdateCohort();
        }
        
    }
    public void UpdateAndRemoveErrorText(string str)
    {
        errorText.text = str;
        StartCoroutine(SlowlyRemoveErrorText());
    }
    
    IEnumerator SlowlyRemoveErrorText()
    {
        yield return new WaitForSeconds(4.0f);
        errorText.text = "";
    }
    public void SetScaleFactor()
    {

        gameObject.GetComponent<Canvas>().scaleFactor = 1;
    }
    public void UpdateResourceText(Vector2 resource) // resource.x = type of resource and text index; resource.y = int/text
    {
        resourceTexts[(int)resource.x].text = ((int)resource.y).ToString();
    }
    public void BuildingUI(GameObject building)
    {
        selectedBuildingObj = building;
        selectedBuildingPanel.SetActive(true);
        Building buildingScript = building.GetComponent<Building>();
        selectedBuildingPanel.transform.GetChild(0).GetComponent<Text>().text = buildingScript.buildingName.ToString();
        selectedBuildingPanel.transform.GetChild(1).GetComponent<Text>().text = buildingScript.buildingLevel.ToString();
        GameObject buildingBuiltPanel = selectedBuildingPanel.transform.GetChild(2).gameObject;
        GameObject buildingNotBuildPanel = selectedBuildingPanel.transform.GetChild(3).gameObject;
        if (buildingScript.built == true)
        {
            buildingBuiltPanel.SetActive(true);
            buildingNotBuildPanel.SetActive(false);
            buildingBuiltPanel.transform.GetChild(0).GetComponent<Button>().interactable = buildingScript.HasTheRequiredResources();
            for(int i = 0; i < selectedBuildingAddMultText.Length; i++)
            {
                selectedBuildingAddMultText[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = buildingScript.currentAdds[i].ToString();
                selectedBuildingAddMultText[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = buildingScript.currentMults[i].ToString();
            }
        }
        else
        {
            buildingNotBuildPanel.SetActive(true);
            buildingBuiltPanel.SetActive(false);
            int points = buildingScript.buildPoints;
            int requirePoints = buildingScript.requiredBuildingPoints;
            buildingNotBuildPanel.transform.GetChild(0).GetComponent<Text>().text = points.ToString();
            buildingNotBuildPanel.transform.GetChild(1).GetComponent<Text>().text = requirePoints.ToString();
            RectTransform buildingPointsProgressionBar = buildingNotBuildPanel.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<RectTransform>();
            buildingPointsProgressionBar.anchorMax = new Vector2((float)points / requirePoints, 1);
        }
    }
    public void BuildingUIOff()
    {
        selectedBuildingPanel.SetActive(false);
    }
    public void Tick()
    {
        if (selectedBuildingPanel.gameObject.activeSelf == true)
        {
            Building buildingScript = selectedBuildingObj.GetComponent<Building>();
            if (selectedBuildingPanel.transform.GetChild(3).gameObject.activeSelf == true) // if building is not built
            {

            }
            else
            {
                GameObject buildingBuiltPanel = selectedBuildingPanel.transform.GetChild(2).gameObject;
                buildingBuiltPanel.transform.GetChild(0).GetComponent<Button>().interactable = buildingScript.HasTheRequiredResources();
            }
        }
    }
    private void OnDisable()
    {
        GameTick.InGameTick -= Tick;
    }
}
