using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
public class Tower : MonoBehaviour, IAttackable, IPointerClickHandler
{
    // Start is called before the first frame update
    [SerializeField]
    public List<GameObject> inRangeOfcapture = new List<GameObject>();
    [SerializeField]
    public List<GameObject> inRangeToShoot = new List<GameObject>();
    [SerializeField]
    public List<GameObject> enemyInRangeToShoot = new List<GameObject>();
    bool inRangeOfcaptureBool;
    [SerializeField]
    int convertPoints;
    int convertPointsMax = 100;
    float timeSinceTick;
    float timeSinceAttack;
    float convertTick;
    [SerializeField]
    GameObject shootingTop;
    public GameObject arrowPrefab;
    [SerializeField]
    private int assignedTeam;
    private bool captured;
    bool canAttack;
    float attackCoolDown;
    Color neutralColor;
    public float AttackCoolDown { get ; set ; }
    public float AttackDamage { get ; set ; }
    public int AttackType { get; set; }

    private void Awake()
    {
        attackCoolDown = 2.5f;
        shootingTop = transform.GetChild(3).gameObject;
        convertTick = 1;
        convertPoints = convertPointsMax;
        canAttack = true;
        neutralColor = shootingTop.GetComponent<Renderer>().material.color;
    }

    private void FixedUpdate()
    {
        if (inRangeOfcapture.Count > 0)
        {
            for (int i = 0; i < inRangeOfcapture.Count; i++)
            {
                if (inRangeOfcapture[i].GetComponent<Bot>().isDead == true)
                {
                    inRangeOfcapture.RemoveAt(i);
                }
            }
            int team1 = 0;
            int team2 = 0;
            foreach(GameObject capturee in inRangeOfcapture)
            {
                if (capturee.GetComponent<Bot>().team == 1)
                {
                    team1++;
                }
                else if (capturee.GetComponent<Bot>().team == 2)
                {
                    team2++;
                }
            }
            if (team1 > team2)
            {
                CaptureProcess(1, team1);
            }
            else if (team2 > team1)
            {
                CaptureProcess(2, team2);
            }
        }
        else
        {
            CaptureProcess(assignedTeam, 1);
        }
        if (inRangeToShoot.Count > 0)
        {
            for (int i = 0; i < inRangeToShoot.Count; i++)
            {
                GameObject go = inRangeToShoot[i];
                if (inRangeToShoot[i] == null)
                {
                    inRangeToShoot.RemoveAt(i);
                }
                else if (go.GetComponent<Bot>().isDead == true)
                {
                    inRangeToShoot.Remove(go);
                }
            }
            SortEnemysFromFriends();
            enemyInRangeToShoot.Sort(SortEnemyDistance);
            if (enemyInRangeToShoot.Count > 0)
            {
                if (captured)
                {
                    if (Time.time > timeSinceAttack + attackCoolDown)
                    {
                        Attack();
                    }
                }
            }
        }
    }
    private void CaptureProcess(int team, int quantity)
    {
        if (Time.time > timeSinceTick + convertTick)
        {
            if (team != assignedTeam)
            {
                convertPoints -= quantity;
                if (convertPoints < 0)
                {
                    convertPoints = 0;
                }
                if (convertPoints == 0)
                {
                    captured = false;
                    assignedTeam = team;
                    SetShootingTopColor(neutralColor);
                }
            }
            else
            {
                if (convertPoints != convertPointsMax)
                {
                    convertPoints += quantity;
                }
                if (convertPoints >= convertPointsMax)
                {
                    convertPoints = convertPointsMax;
                    if (assignedTeam != 0)
                    {
                        if (captured == false)
                        {
                            foreach (GameObject go in inRangeOfcapture)
                            {
                                if (go.GetComponent<Bot>().team == assignedTeam)
                                {
                                    SetShootingTopColor(go.GetComponent<BotClickerData>().unselectedMaterial.color);
                                    break;
                                }
                            }
                        }
                        captured = true;
                    }
                }
            }
            timeSinceTick = Time.time;
        }
    }

    
    int SortEnemyDistance(GameObject x, GameObject y)
    {

        float distanceToX = Vector3.Distance(transform.GetChild(1).gameObject.transform.position, x.transform.position);
        float distanceToY = Vector3.Distance(transform.GetChild(1).gameObject.transform.position, y.transform.position);
        return distanceToX.CompareTo(distanceToY);
    }

    public void Attack()
    {
        GameObject ar = Instantiate(arrowPrefab, shootingTop.transform.position, Quaternion.identity);
        ar.GetComponent<Arrow>().GiveArrowTarget(enemyInRangeToShoot[0], assignedTeam);
        timeSinceAttack = Time.time;
    }
    private void SortEnemysFromFriends()
    {
        enemyInRangeToShoot.Clear();
        foreach (GameObject go in inRangeToShoot)
        {
            if (go.GetComponent<Bot>().team != assignedTeam)
            {
                enemyInRangeToShoot.Add(go);
            }
        }
    }
    private void SetShootingTopColor(Color color)
    {
        shootingTop.GetComponent<Renderer>().material.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData);
    }
}
