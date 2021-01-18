using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohort : MonoBehaviour
{
    [SerializeField]
    public string aggression;
    [SerializeField]
    public string stance;
    [SerializeField]
    public string density;
    int gridWidth;
    int gridHeight;
    [SerializeField]
    List<Vector3> placementVectors = new List<Vector3>();
    public void Init()
    {
        CreateGridWidthHeight(GetBotStance());
        AssignGridPointsToVectorList(GetBotDensity());
        AssignBotsToFormationPosition(GetBotAggression());
    }
    public void CreateGridWidthHeight(int stanceType)
    {
        gridWidth = 0;
        gridHeight = 0;
        int currentlySelectedCount = BotClickerData.currentlySelected.Count;
        switch (stanceType)
        {
            case 0: //column
                for (int n = 1; n < 20; n++)
                {
                    if ((n + 1) * (n * 3) > currentlySelectedCount)
                    {
                        gridWidth = n;
                        break;
                    }
                }
                gridHeight = currentlySelectedCount % gridWidth == 0 ?
                    currentlySelectedCount / gridWidth :
                    ((currentlySelectedCount - (currentlySelectedCount % gridWidth)) / gridWidth) + 1;
                break;
            case 1: //line
                for (int n = 1; n < 20; n++)
                {
                    if ((n + 1) * (n * 3) > currentlySelectedCount)
                    {
                        gridHeight = n;
                        break;
                    }
                }
                gridWidth = currentlySelectedCount % gridHeight == 0 ?
                    currentlySelectedCount / gridHeight :
                    ((currentlySelectedCount - (currentlySelectedCount % gridHeight)) / gridHeight) + 1;
                break;
            case 2: //square
                for (int n = 1; n < 20; n++)
                {
                    if ((n + 1) * (n + 1) > currentlySelectedCount)
                    {
                        gridHeight = n;
                        gridWidth = currentlySelectedCount % n == 0 ? 
                            currentlySelectedCount / n :
                            ((currentlySelectedCount - (currentlySelectedCount % n)) / n) + 1;
                        break;
                    }
                }
                break;
        }
    }
    void AssignGridPointsToVectorList(float density)
    {
        placementVectors.Clear();
        Vector3 startPos = new Vector3((gridWidth / 2) * - density, 0, (gridHeight / 2) * density);
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 aPos = new Vector3(startPos.x + (x * density), startPos.y, startPos.z - (y * density));
                placementVectors.Add(aPos);
            }
        }
    }
    void AssignBotsToFormationPosition(float botAggression)
    {
        int i = 0;
        foreach (BotClickerData bot in BotClickerData.currentlySelected)
        {
            bot.transform.parent = transform;
            bot.MoveToPosition2(transform.TransformPoint(placementVectors[i]));
            bot.transform.GetComponentInChildren<BotProxy>().SetRadius(botAggression);
            i++;
        }
        BotClickerData.OrderedVectors();
    }
    public int GetBotAggression()
    {
        switch (aggression)
        {
            case "Defend":
                return 2;
            case "Guerrilla":
                return 5;
            case "Aggressive":
                return 10;
            default:
                return 5;
                //set proxy colliders to act on each
        }
    }
    public int GetBotStance()
    {
        switch (stance)
        {
            case "Column":
                return 0;
            case "Line":
                return 1;
            case "Square":
                return 2;
            default:
                return 2;
                //set cohort placement grid stance
        }
    }
    public float GetBotDensity()
    {
        switch (density)
        {
            case "Tight":
                return 1.2f;
            case "Normal":
                return 2;
            case "Loose":
                return 4;
            default:
                return 2;
        }
        // set cohort placement grid distance;
    }
}
