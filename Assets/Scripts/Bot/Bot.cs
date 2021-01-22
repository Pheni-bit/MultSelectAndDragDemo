using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Linq;

public abstract class Bot : MonoBehaviour, IDamagable, IAttackable, ISelectHandler, IPointerClickHandler, IDeselectHandler
{
    BotProxy botProxy;
    GameObject enemyTarget;
    GameObject parentCohort;
    public NavMeshAgent navMeshAgent;
    Renderer myRenderer;
    public Material unselectedMaterial, selectedMaterial;
    static TargetMarkerManager targetMarkerManager;
    public static HashSet<Bot> allMySelectableBots = new HashSet<Bot>();
    public static HashSet<Bot> currentlySelectedBots = new HashSet<Bot>();
    public static List<Vector3> orderPosList = new List<Vector3>();
    public Vector3 orderedPosition;
    public bool directOrder, removeThis;
    public bool enemysInRange, canAttack = true,  returnToOrders, isDead;
    private float pathEndThreshold = 1.5f;

    // BOT PROPERTIES   {
    public int team;
    public int botType;
    public float attackDamage = 1;
    public float health = 10;
    public float attackCoolDown = 2;
    //                  }
    
    // BOT INTERFACE PROPERTIES     {
    public float Health { get; set; }
    public float AttackCoolDown { get; set; }
    public float AttackDamage { get; set; }
    //                              }

    private void Awake()
    {
        botProxy = transform.GetComponentInChildren<BotProxy>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        myRenderer = GetComponent<Renderer>();
        targetMarkerManager = GameObject.Find("TargetMarkerManager").GetComponent<TargetMarkerManager>();
        canAttack = true;
        // set intefaces {
        AttackCoolDown = attackCoolDown;
        Health = health;
        AttackDamage = attackDamage;
        //               }

        // attach a new parent cohort to created bots {
        parentCohort = NewCohort(gameObject.transform.position, gameObject.transform.rotation.eulerAngles);
        transform.parent = parentCohort.transform;
        //                                            }
        orderedPosition = transform.position;
        allMySelectableBots.Add(this); 
    }
    public virtual void Update()
    {
        if (isDead)
            return;
        if (transform.parent.gameObject != parentCohort) 
        {
            parentCohort = transform.parent.CompareTag("Cohort") ? transform.parent.gameObject : null;
        }
        if (parentCohort != null)
        {
            ManageOrders();
        }
        Debug.DrawRay(gameObject.transform.position, navMeshAgent.transform.forward * 1, Color.blue);
    }
    private void LateUpdate()
    {
        if (removeThis == true)
        {
            currentlySelectedBots.Remove(this);
            removeThis = false;
        }
    }
    public virtual void ManageOrders()
    {
        switch (parentCohort.GetComponent<Cohort>().aggression)
        {
            case "Defend":
                if (directOrder)
                {
                    returnToOrders = true;
                    directOrder = CompletedOrder();
                }
                else if (enemysInRange)
                {
                    returnToOrders = true;
                    if (botProxy.EnemyProxies.Count != 0)
                    {
                        enemyTarget = botProxy.EnemyProxies[0];
                        if (enemyTarget != null)
                        {
                            if (Vector3.Distance(orderedPosition, enemyTarget.transform.position) <= botProxy.colliderRadius + 1)
                            {
                                Attack();
                            }
                            else if (returnToOrders)
                            {
                                SetNavDestination(orderedPosition);
                            }
                        }
                    }
                }
                else if (returnToOrders)
                {
                    SetNavDestination(orderedPosition);
                    returnToOrders = false;
                }
                break;
            case "Guerrilla":
                if (enemysInRange)
                {
                    returnToOrders = true;
                    if (botProxy.EnemyProxies.Count != 0)
                    {
                        enemyTarget = botProxy.EnemyProxies[0];
                        if (enemyTarget != null)
                        {
                            if (Vector3.Distance(orderedPosition, enemyTarget.transform.position) <= botProxy.colliderRadius + 1)
                            {
                                Attack();
                            }
                            else if (returnToOrders)
                            {
                                SetNavDestination(orderedPosition);
                            }
                        }
                    }
                }
                else if (returnToOrders)
                {
                    SetNavDestination(orderedPosition);
                    returnToOrders = false;
                }
                break;
            case "Aggressive":
                if (enemysInRange)
                {
                    returnToOrders = true;
                    if (botProxy.EnemyProxies.Count != 0)
                    {
                        enemyTarget = botProxy.EnemyProxies[0];
                        if (enemyTarget != null)
                        {
                            if (Vector3.Distance(orderedPosition, enemyTarget.transform.position) <= botProxy.colliderRadius + 1)
                            {
                                Attack();
                            }
                            else if (returnToOrders)
                            {
                                SetNavDestination(orderedPosition);
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public virtual void Attack()
    {
        SetStoppingDistance(1.4f);
        if (enemyTarget)
        {
            Ray attackRay = new Ray(transform.position, navMeshAgent.transform.forward);
            SetNavDestination(enemyTarget.transform.position);
            SetNavRotation(enemyTarget);
            if (Physics.Raycast(attackRay, out RaycastHit hit, 1.5f, ~gameObject.layer))
            {
                if (hit.transform.gameObject == enemyTarget)
                {
                    if (canAttack)
                    {
                        canAttack = false;
                        StartCoroutine(AttackCoolDownRoutine(enemyTarget));
                    }
                }
            }
        }
    }

    public virtual void Damage(float damage)
    {
        //Debug.Log("were damaging");
        Health -= damage;
        directOrder = false;
        if (Health <= 0)
        {
            isDead = true;
            StopAllCoroutines();
            this.gameObject.GetComponent<Collider>().enabled = false;
            Destroy(this.gameObject);
        }
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
        if (currentlySelectedBots.Contains(this))
        {
            if (!DragBoxHandler.dragging)
            {
                OnDeselect(eventData);
            }
        }
        else
        {
            currentlySelectedBots.Add(this);
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
        foreach (Bot bot in currentlySelectedBots)
        {
            bot.OnDeselect(eventData);
        }
        currentlySelectedBots.Clear();
        OrderedVectors();
    }
    public void MoveToPosition(Vector3 vec)
    {
        orderedPosition = vec;
        if (!TerrainManager.building)
            directOrder = true;
        else
            directOrder = false;
        SetNavDestination((vec));
    }
    public static void OrderedVectors()
    {
        orderPosList.Clear();
        foreach (Bot bot in currentlySelectedBots)
        {
            if (bot.orderedPosition != null)
            {
                if (!orderPosList.Contains(bot.orderedPosition))
                {
                    orderPosList.Add(bot.orderedPosition);
                }
            }
        }

        // below needs work
        if (orderPosList.Count > 0)
        {
            foreach (Bot bot in currentlySelectedBots)
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
    private void OnDestroy()
    {
        allMySelectableBots.Remove(this); // remove from hashset
        if (currentlySelectedBots.Contains(this)) //if in hashSet, remove from selected hashSet;
        {
            currentlySelectedBots.Remove(this);
        }
        orderPosList.Remove(this.orderedPosition);
        if (transform.parent.CompareTag("Cohort"))
        {
            if (transform.parent.transform.childCount <= 1)
            {
                Destroy(transform.parent.gameObject, 0.25f);
            }
        }
    }


    public bool CompletedOrder()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + pathEndThreshold;
    }
    public void SetStoppingDistance(float distance)
    {
        navMeshAgent.stoppingDistance = distance;
    }
    public void SetNavDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }
    public void SetNavRotation(GameObject enemy)
    {
        navMeshAgent.transform.LookAt(enemy.transform.position);
        navMeshAgent.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
    }
    IEnumerator AttackCoolDownRoutine(GameObject hitObj)
    {
        transform.Translate(Vector3.forward * 0.2f);
        yield return new WaitForSeconds(0.1f);
        if (hitObj != null)
        {
            hitObj.GetComponent<IDamagable>()
                  .Damage(AttackDamage);
        }
        transform.Translate(Vector3.forward * -0.2f);
        yield return new WaitForSeconds(1.0f);
        canAttack = true;
    }

    GameObject NewCohort(Vector3 pos, Vector3 rot)
    {
        GameObject cohort = new GameObject();
        cohort.transform.position = pos;
        cohort.transform.rotation = Quaternion.Euler(rot);
        cohort.name = "Cohort";
        cohort.tag = "Cohort";
        cohort.transform.parent = gameObject.transform.parent;
        cohort.AddComponent<Cohort>();
        return cohort;
    }
}
