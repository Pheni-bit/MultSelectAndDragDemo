using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotWorker : Bot
{
    GameObject targetOrderedToBuild;

    public void OrderedToBuild(GameObject buildTarget)
    {
        targetOrderedToBuild = buildTarget;
    }
}
