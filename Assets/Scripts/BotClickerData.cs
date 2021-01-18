using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class BotClickerData : MonoBehaviour, ISelectHandler, IPointerClickHandler, IDeselectHandler
{
    public static HashSet<BotClickerData> allMySelectables = new HashSet<BotClickerData>();
    public static HashSet<BotClickerData> currentlySelected = new HashSet<BotClickerData>();

    static float newStoppingDistance;
    public int botType;
    public Material unselectedMaterial, selectedMaterial;
    Bot botClass;
    BotNav botNav;
    Renderer myRenderer;
    static TargetMarkerManager targetMarkerManager;
    public Vector3 orderedPosition;
    public static List<Vector3> orderPosList = new List<Vector3>();
    bool removeThis;
    private void Awake()
    {
        orderedPosition = transform.position;
        targetMarkerManager = GameObject.Find("TargetMarkerManager").GetComponent<TargetMarkerManager>();
        myRenderer = gameObject.GetComponent<Renderer>();
        botClass = gameObject.GetComponent<Bot>();
        botNav = gameObject.GetComponent<BotNav>();
        allMySelectables.Add(this);
    }
    private void Update()
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            {
                DeselectAll(eventData);
            }
            OnSelect(eventData);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("Onselect");
        if (currentlySelected.Contains(this))
        {
            if (!DragBoxHandler.dragging)
            {
                OnDeselect(eventData);
            }   
        }
        else
        {
            currentlySelected.Add(this);
            myRenderer.material = selectedMaterial;
            OrderedVectors();
            removeThis = false;
        }
    }
    public void OnDeselect(BaseEventData eventData)
    {
        myRenderer.material = unselectedMaterial;
        OrderedVectors();
        removeThis = true;
    }
    public static void DeselectAll(BaseEventData eventData)
    {
        foreach (BotClickerData bot in currentlySelected)
        {
            bot.OnDeselect(eventData);
        }
        currentlySelected.Clear();
        OrderedVectors();
    }
    public void MoveToPosition2(Vector3 vec)
    {
        newStoppingDistance = 0;
        orderedPosition = vec;
        botClass.directOrder = true;
        botNav.SetStoppingDistance(newStoppingDistance);
        botNav.SetNavDestination((vec));
    }
    public static void OrderedVectors()
    {
        orderPosList.Clear();
        foreach(BotClickerData bot in currentlySelected)
        {
            if (bot.orderedPosition != null)
            {
                if (!orderPosList.Contains(bot.orderedPosition))
                {
                    orderPosList.Add(bot.orderedPosition);
                }
            }
        }
        if (orderPosList.Count > 0)
        {
            foreach(BotClickerData bot in currentlySelected)
            {
                targetMarkerManager.CreateMarkers(bot.unselectedMaterial);
                break;
            }
        }
        else
        {
            targetMarkerManager.DestroyMarkers();
        }
    }

    public void RemoveWhenDead()
    {
        if (currentlySelected.Contains(this))
        {
            currentlySelected.Remove(this);
        }
        allMySelectables.Remove(this);
    }
    private void OnDestroy()
    {
        orderPosList.Remove(this.orderedPosition);
    }
    private void LateUpdate()
    {
        if (removeThis == true)
        {
            currentlySelected.Remove(this);
            removeThis = false;
        }
    }
}
