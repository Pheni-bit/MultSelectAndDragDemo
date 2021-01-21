using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragBoxHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField]
    Image selectionBox;
    Vector2 startPos;
    Vector2 endPos;
    public Vector3 worldStartPos;
    Vector3 worldEndPos;
    Rect selectionRect;
    Ray ray;
    public static bool dragging;
    public GameObject arrowPointerPrefab;
    GameObject tempArrow;
    public Vector3 tempArrowRotation;
    Canvas canvas;
    private void Awake()
    {
        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            {
                Bot.DeselectAll(new BaseEventData(EventSystem.current));
            }
            dragging = true;
            selectionBox.gameObject.SetActive(true);
            canvas = UIManager.Instance.gameObject.GetComponent<Canvas>();
            startPos = eventData.position;
            Debug.Log(eventData.position + " eventData.position StartDrag");
            Debug.Log(Input.mousePosition + "input.mouse.pos StartDrag");
            Debug.Log(canvas.pixelRect.width);
            Debug.Log(canvas.pixelRect.height);
            selectionRect = new Rect();
        }
        else if (Input.GetMouseButton(1))
        {
            if (Bot.currentlySelectedBots.Count != 0)
            {
                startPos = eventData.position;
                ray = Camera.main.ScreenPointToRay(eventData.position);
                if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Terrain")))
                {
                    dragging = true;
                    worldStartPos = hitData.point;
                    tempArrow = Instantiate(arrowPointerPrefab, worldStartPos, Quaternion.identity);
                }
            }
        }
    }
    private void LateUpdate()
    {
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            
            if (eventData.position.x < startPos.x)
            {
                selectionRect.xMin = eventData.position.x;
                selectionRect.xMax = startPos.x;
            }
            else
            {
                selectionRect.xMin = startPos.x;
                selectionRect.xMax = eventData.position.x;
            }
            if (eventData.position.y < startPos.y)
            {
                selectionRect.yMin = eventData.position.y;
                selectionRect.yMax = startPos.y;
            }
            else
            {
                selectionRect.yMin = startPos.y;
                selectionRect.yMax = eventData.position.y;
            }
            selectionBox.rectTransform.offsetMin = selectionRect.min / canvas.scaleFactor ;
            selectionBox.rectTransform.offsetMax = selectionRect.max / canvas.scaleFactor;
        }
        else if (Input.GetMouseButton(1))
        {
            if (Bot.currentlySelectedBots.Count != 0)
            {
                endPos = eventData.position;
                ray = Camera.main.ScreenPointToRay(eventData.position);
                if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Terrain")))
                {
                    worldEndPos = hitData.point; 
                    tempArrow.transform.LookAt(worldEndPos);
                    tempArrowRotation = tempArrow.transform.rotation.eulerAngles;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.position);
        if (Input.GetMouseButtonUp(0))
        {
            selectionBox.gameObject.SetActive(false);
            foreach (Bot bot in Bot.allMySelectableBots)
            {
                if (selectionRect.Contains(Camera.main.WorldToScreenPoint(bot.transform.position)))
                {
                    bot.OnSelect(eventData);
                }
            }
        }
        Destroy(tempArrow);
        StartCoroutine( DraggingEnded());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        float myDistance = 0;
        foreach(RaycastResult result in results)
        {
            if (result.gameObject == gameObject)
            {
                myDistance = result.distance;
                break;
            }
        }
        GameObject nextObj = null;
        float maxDistance = Mathf.Infinity;

        foreach (RaycastResult result in results)
        {
            if (result.distance > myDistance && result.distance < maxDistance)
            {
                nextObj = result.gameObject;
                maxDistance = result.distance;
            }
        }
        if (nextObj)
        {
            ExecuteEvents.Execute<IPointerClickHandler>(nextObj, eventData, (x, y) => { x.OnPointerClick((PointerEventData)y); });
        }
    }
   IEnumerator DraggingEnded()
    {
        yield return new WaitForEndOfFrame();
        dragging = false;
    }

}
