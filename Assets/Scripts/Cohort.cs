using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohort : MonoBehaviour
{
    [SerializeField]
    public string aggression = "Guerrilla";
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
        int currentlySelectedBotsCount = Bot.currentlySelectedBots.Count;
        switch (stanceType)
        {
            case 0: //column
                for (int n = 1; n < 20; n++)
                {
                    if ((n + 1) * (n * 3) > currentlySelectedBotsCount)
                    {
                        gridWidth = n;
                        break;
                    }
                }
                gridHeight = currentlySelectedBotsCount % gridWidth == 0 ?
                    currentlySelectedBotsCount / gridWidth :
                    ((currentlySelectedBotsCount - (currentlySelectedBotsCount % gridWidth)) / gridWidth) + 1;
                break;
            case 1: //line
                for (int n = 1; n < 20; n++)
                {
                    if ((n + 1) * (n * 3) > currentlySelectedBotsCount)
                    {
                        gridHeight = n;
                        break;
                    }
                }
                gridWidth = currentlySelectedBotsCount % gridHeight == 0 ?
                    currentlySelectedBotsCount / gridHeight :
                    ((currentlySelectedBotsCount - (currentlySelectedBotsCount % gridHeight)) / gridHeight) + 1;
                break;
            case 2: //square
                for (int n = 1; n < 20; n++)
                {
                    if ((n + 1) * (n + 1) > currentlySelectedBotsCount)
                    {
                        gridHeight = n;
                        gridWidth = currentlySelectedBotsCount % n == 0 ? 
                            currentlySelectedBotsCount / n :
                            ((currentlySelectedBotsCount - (currentlySelectedBotsCount % n)) / n) + 1;
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
        foreach (Bot bot in Bot.currentlySelectedBots)
        {
            bot.transform.parent = transform;
            bot.MoveToPosition(transform.TransformPoint(placementVectors[i]));
            bot.transform.GetComponentInChildren<BotProxy>().SetRadius(botAggression);
            i++;
        }
        Bot.OrderedVectors();
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
