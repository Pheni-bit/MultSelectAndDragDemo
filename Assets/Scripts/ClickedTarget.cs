using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickedTarget : MonoBehaviour
{
    GameObject enemyAttached;
    public void ChangeFlagMaterial(Material mat)
    {

    }
    public void EnableAndPosition(Vector3 pos)
    {
        enemyAttached = null;
        gameObject.SetActive(true);
        gameObject.transform.position = pos;
    }
    public void Hide()
    {
        enemyAttached = null;
        gameObject.SetActive(false);
    }
    public void EnemySelected(GameObject enemy)
    {
        enemyAttached = enemy;
    }
    private void Update()
    {
        if (enemyAttached)
        {
            gameObject.transform.position = enemyAttached.transform.position;
        }
    }

}
