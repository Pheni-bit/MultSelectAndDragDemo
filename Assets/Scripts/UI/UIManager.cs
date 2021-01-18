using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static int lastSelectedCount;
    public Image[] bottomPanelImageArr;
    public Text[] bottomPanelTextArr;
    public GameObject[] bottomSelected;
    public GameObject formationPanel;
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
        Debug.Log("scale factor = " + gameObject.GetComponent<Canvas>().scaleFactor);
        _instance = this;
        terrainManager = GameObject.Find("Terrain").GetComponent<TerrainManager>();
    }

    void Update()
    {
        SetSelected();
        OnSelectedChange();
        OnSelectedCountChange();
    }
    void SetSelected()
    {
        if (BotClickerData.currentlySelected.Count > 0)
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
        if (lastSelectedCount != BotClickerData.currentlySelected.Count)
        {
            if (Selected)
            {
                GetSelectedCohort();
            }
            CompareBot();
        }
        lastSelectedCount = BotClickerData.currentlySelected.Count;
    }
    public void GetSelectedCohort()
    {
        selectedCohorts.Clear();
        foreach (BotClickerData bot in BotClickerData.currentlySelected)
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
        foreach (BotClickerData bot in BotClickerData.currentlySelected)
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
        foreach (BotClickerData bot in BotClickerData.currentlySelected)
        {
            color = bot.GetComponent<BotClickerData>().unselectedMaterial.color;
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
    public void UpdateCohortAggression(string str)
    {
        if (Selected && selectedCohort != null)
        {
            selectedCohort.GetComponent<Cohort>().aggression = str;
            terrainManager.UpdateCohort();
        }
        
    }
    public void UpdateCohortStance(string str)
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
}
