using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarkerManager : MonoBehaviour
{
    public GameObject clickedTargetPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMarkers(Material mat)
    {
        DestroyMarkers();
        for (int i = 0; i < BotClickerData.orderPosList.Count; i++)
        {
            GameObject marker = Instantiate(clickedTargetPrefab, BotClickerData.orderPosList[i] + new Vector3(0, 0.1f, 0), Quaternion.Euler(new Vector3(90,0,0)));
            marker.transform.parent = this.transform;
            marker.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = mat;
        }
    }
    public void DestroyMarkers()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }
}
